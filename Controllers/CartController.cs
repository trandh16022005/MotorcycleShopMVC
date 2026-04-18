using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MotorcycleShopMVC.Models;
using System.Security.Claims;

namespace MotorcycleShopMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // TODO: thay bằng user thật khi có login
        private int GetCurrentUserId()
        {
            if(User.Identity!=null && User.Identity.IsAuthenticated)
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(claim, out int uid))
                    return uid;
            }
            return 1;
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // Trang cart
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var cart = await GetOrCreateCartAsync(userId);

            var items = await _context.CartItems
                .Include(ci => ci.Motorcycle)
                .Include(ci => ci.Part)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();

            ViewBag.CartId = cart.CartId;
            return View(items);
        }

        // Add motorcycle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMotorcycle(int motorcycleId, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;
            int userId = GetCurrentUserId();
            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.MotorcycleId == motorcycleId);
            if (item == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    MotorcycleId = motorcycleId,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            //var count = await _context.CartItems
            //    .Where(ci => ci.CartId == cart.CartId)
            //    .SumAsync(ci => (int?)ci.Quantity) ?? 0;
            //return Json(new { success = true, count });
            return Redirect(Request.Headers["Referer"].ToString());
        }
        

        // API lấy tổng số lượng cho icon
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            int userId = GetCurrentUserId();

            var count = await _context.CartItems
                .Include(ci=>ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => (int?)ci.Quantity) ?? 0;

            return Json(count);
        }
    }
}
