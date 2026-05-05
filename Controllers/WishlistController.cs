using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Filters;
using MotorcycleShopMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorcycleShopMVC.Controllers
{
    [SessionAuthorize] // Yêu cầu người dùng phải đăng nhập
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper để lấy UserId từ Session
        private int GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            int.TryParse(userIdStr, out var userId);
            return userId;
        }

        // GET: /Wishlist
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Wishlist" });
            }

            // Lấy tất cả sản phẩm trong wishlist của user
            var wishlistItems = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Motorcycle)
                    .ThenInclude(m => m.Brand) // Include Brand cho Motorcycle
                .Include(w => w.Part)
                    .ThenInclude(p => p.Brand) // Include Brand cho Part
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();

            return View(wishlistItems);
        }

        // POST: /Wishlist/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int? motorcycleId, int? partId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized("Vui lòng đăng nhập để thêm vào danh sách yêu thích.");

            if (motorcycleId == null && partId == null) return BadRequest("Không có sản phẩm để thêm.");

            // Kiểm tra xem sản phẩm đã có trong wishlist chưa
            var alreadyExists = await _context.Wishlists.AnyAsync(w =>
                w.UserId == userId &&
                (w.MotorcycleId == motorcycleId || w.PartId == partId));

            if (alreadyExists)
            {
                TempData["WishlistMessage"] = "Sản phẩm đã có trong danh sách yêu thích của bạn.";
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                MotorcycleId = motorcycleId,
                PartId = partId,
                AddedAt = DateTime.Now
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["WishlistMessage"] = "Đã thêm sản phẩm vào danh sách yêu thích!";

            // Quay lại trang trước đó
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        // POST: /Wishlist/Remove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.WishlistId == id && w.UserId == userId);

            if (wishlistItem == null) return NotFound();

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["WishlistMessage"] = "Đã xóa sản phẩm khỏi danh sách yêu thích.";

            // Nếu đang ở trang wishlist thì load lại, nếu không thì quay lại trang trước
            if (Request.Headers["Referer"].ToString().Contains("/Wishlist"))
            {
                return RedirectToAction(nameof(Index));
            }

            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
    }
}