using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Filters;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Models.ViewModels;
using System.Security.Claims;

namespace MotorcycleShopMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out var userId) ? userId : 0;
        }

        // Hiển thị trang thanh toán
        public async Task<IActionResult> Checkout([FromQuery] List<int>? selectedItemIds)
        {
            var userID = GetUserId();
            var cart=await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci=>ci.Motorcycle)
                .Include(c=>c.CartItems).ThenInclude(ci => ci.Part)
                .FirstOrDefaultAsync(c => c.UserId == userID);
            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            List<CartItem> itemsToCheckout;
            if(selectedItemIds!=null && selectedItemIds.Any())
            {
                itemsToCheckout = cart.CartItems
                    .Where(ci=>selectedItemIds.Contains(ci.CartItemId))
                    .ToList();
                if (!itemsToCheckout.Any())
                {
                    TempData["Error"] = "Không có mục nào hợp lệ được chọn để thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }
            }
            else
            {
                itemsToCheckout = cart.CartItems.ToList();
            }

            var model = new CheckoutViewModel
            {
                CartItems = itemsToCheckout,
                TotalAmount = itemsToCheckout.Sum(i => i.Quantity * ((i.Motorcycle != null ? i.Motorcycle.Price : 0m) + (i.Part != null ? i.Part.Price : 0m))),
                SelectedCartItemIds = itemsToCheckout.Select(ci => ci.CartItemId).ToList()
            };
            return View(model);
        }

        // Xử lý đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = GetUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Motorcycle)
                .Include(c => c.CartItems).ThenInclude(ci => ci.Part)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null || !cart.CartItems.Any()) return RedirectToAction("Index", "Home");

            var selectedIds = model.SelectedCartItemIds ?? new List<int>();
            var itemsToCreateOrderFrom = cart.CartItems.Where(c => selectedIds.Contains(c.CartItemId)).ToList();

            if (!itemsToCreateOrderFrom.Any())
            {
                TempData["Error"] = "Không có mục nào được chọn để tạo đơn.";
                return RedirectToAction("Index", "Cart");
            }

            //Tao Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = model.ShippingAddress,
                PaymentMethod = model.PaymentMethod,
                TotalAmount = itemsToCreateOrderFrom.Sum(i => i.Quantity * (i.Motorcycle?.Price ?? i.Part?.Price ?? 0)),
                Status = "pending",
                PaymentStatus = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Tạo các OrderItem chỉ cho những mục đã chọn
            foreach (var item in itemsToCreateOrderFrom)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    MotorcycleId = item.MotorcycleId,
                    PartId = item.PartId,
                    Quantity = item.Quantity,
                    Price = item.Motorcycle?.Price ?? item.Part?.Price ?? 0
                };
                _context.OrderItems.Add(orderItem);
            }

            //Xoa khoi Cart chi nhung CartItem da duoc tao don
            _context.CartItems.RemoveRange(itemsToCreateOrderFrom);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { id = order.OrderId });
        }

        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        // ==========================================
        // 1. LỊCH SỬ ĐƠN HÀNG (Dành cho Khách hàng)
        // ==========================================
        public async Task<IActionResult> History(string? tab = "all")
        {
            var userId = GetUserId();
            // Lấy danh sách đơn hàng của user đang đăng nhập
            var query = _context.Orders
                .Where(o => o.UserId == userId);

            switch (tab)
            {
                case "pending":
                    query = query.Where(o => o.Status == "pending");
                    break;

                case "completed":
                    query = query.Where(o => o.Status == "completed");
                    break;

                case "cancelled":
                    query = query.Where(o => o.Status == "cancelled" || o.Status == "canceled");
                    break;
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.CurrentTab = tab ?? "all";
            return View(orders);
        }

        //1. CHI TIẾT ĐƠN HÀNG(Dành cho Khách hàng)
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();

            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Motorcycle)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Part)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        // 2. QUẢN LÝ ĐƠN HÀNG (Dành cho Admin/Nhân viên)
        [RoleAuthorize("Vendor", "Admin")]
        public async Task<IActionResult> Manage()
        {
            // Lấy tất cả đơn hàng trong hệ thống, kèm theo thông tin User
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Action để Admin cập nhật trạng thái đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status, string paymentStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                order.PaymentStatus = paymentStatus;
                order.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            // Quay lại trang Manage sau khi cập nhật xong
            return RedirectToAction(nameof(Manage));
        }
    }
}