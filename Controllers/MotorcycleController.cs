using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MotorcycleShopMVC.Controllers
{
    public class MotorcycleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MotorcycleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CHỈ CÓ ACTION NÀY SỬ DỤNG PAGINATEDLIST
        public async Task<IActionResult> Index(
            string searchString,
            int? brandId,
            int? typeId,
            int? yearFrom,
            string priceRange,
            string engineRange,
            int? pageNumber)
        {
            // 1. Chuẩn bị dữ liệu cho các dropdown bộ lọc
            ViewData["BrandId"] = new SelectList(await _context.Brands.ToListAsync(), "BrandId", "BrandName", brandId);
            ViewData["TypeId"] = new SelectList(await _context.VehicleTypes.ToListAsync(), "TypeId", "TypeName", typeId);
            ViewData["YearFrom"] = new SelectList(await _context.Motorcycles.Select(m => m.YearFrom).Distinct().OrderByDescending(y => y).ToListAsync());

            var priceRanges = new List<SelectListItem>
            {
                new SelectListItem { Value = "0-20000000", Text = "Dưới 20 triệu" },
                new SelectListItem { Value = "20000000-50000000", Text = "Từ 20 - 50 triệu" },
                new SelectListItem { Value = "50000000-100000000", Text = "Từ 50 - 100 triệu" },
                new SelectListItem { Value = "100000000-9999999999", Text = "Trên 100 triệu" }
            };
            ViewData["PriceRanges"] = new SelectList(priceRanges, "Value", "Text", priceRange);

            var engineRanges = new List<SelectListItem>
            {
                new SelectListItem { Value = "0-100", Text = "Dưới 100cc" },
                new SelectListItem { Value = "100-175", Text = "100cc - 175cc" },
                new SelectListItem { Value = "175-9999", Text = "Trên 175cc" }
            };
            ViewData["EngineRanges"] = new SelectList(engineRanges, "Value", "Text", engineRange);

            // Lưu lại các giá trị lọc để hiển thị lại trên View
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentBrandId"] = brandId;
            ViewData["CurrentTypeId"] = typeId;
            ViewData["CurrentYearFrom"] = yearFrom;
            ViewData["CurrentPriceRange"] = priceRange;
            ViewData["CurrentEngineRange"] = engineRange;

            // 2. Bắt đầu truy vấn
            var motorcycles = _context.Motorcycles.Include(m => m.Brand).Include(m => m.Type).AsQueryable();

            // 3. Áp dụng các bộ lọc từ người dùng
            if (!string.IsNullOrEmpty(searchString))
                motorcycles = motorcycles.Where(s => s.ModelName.Contains(searchString));
            if (brandId.HasValue)
                motorcycles = motorcycles.Where(m => m.BrandId == brandId.Value);
            if (typeId.HasValue)
                motorcycles = motorcycles.Where(m => m.TypeId == typeId.Value);
            if (yearFrom.HasValue)
                motorcycles = motorcycles.Where(m => m.YearFrom == yearFrom.Value);
            if (!string.IsNullOrEmpty(priceRange))
            {
                var prices = priceRange.Split('-').Select(decimal.Parse).ToList();
                motorcycles = motorcycles.Where(m => m.Price >= prices[0] && m.Price <= prices[1]);
            }
            if (!string.IsNullOrEmpty(engineRange))
            {
                var capacities = engineRange.Split('-').Select(int.Parse).ToList();
                motorcycles = motorcycles.Where(m => m.EngineCapacity >= capacities[0] && m.EngineCapacity < capacities[1]);
            }

            // 4. Phân trang và trả về đúng kiểu PaginatedList
            int pageSize = 9; // 9 sản phẩm mỗi trang (tạo thành lưới 3x3)
            var paginatedList = await PaginatedList<Motorcycle>.CreateAsync(motorcycles.AsNoTracking().OrderByDescending(m => m.CreatedAt), pageNumber ?? 1, pageSize);

            return View(paginatedList);
        }

        // CÁC ACTION KHÁC GIỮ NGUYÊN
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var motorcycle = await _context.Motorcycles
                .Include(m => m.Brand)
                .Include(m => m.Type)
                .FirstOrDefaultAsync(m => m.MotorcycleId == id);

            if (motorcycle == null) return NotFound();

            // Chỉ định rõ ràng trả về View tên là "Detail"
            return View(motorcycle);
        }

        [RoleAuthorize("Admin")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Create([Bind("MotorcycleId,ModelName,BrandId,TypeId,EngineCapacity,YearFrom,YearTo,Price,Color,WarrantyPolicy,ImagePath,Description,StockQty")] Motorcycle motorcycle)
        {
            if (ModelState.IsValid)
            {
                motorcycle.CreatedAt = DateTime.Now;
                motorcycle.UpdatedAt = DateTime.Now;
                _context.Add(motorcycle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", motorcycle.BrandId);
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeName", motorcycle.TypeId);
            return View(motorcycle);
        }

        // ... các action Edit, Delete, và MotorcycleExists giữ nguyên như file bạn đã cung cấp ...
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle == null) return NotFound();
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", motorcycle.BrandId);
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeName", motorcycle.TypeId);
            return View(motorcycle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("MotorcycleId,ModelName,BrandId,TypeId,EngineCapacity,YearFrom,YearTo,Price,Color,WarrantyPolicy,ImagePath,Description,StockQty,CreatedAt")] Motorcycle motorcycle)
        {
            if (id != motorcycle.MotorcycleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    motorcycle.UpdatedAt = DateTime.Now;
                    _context.Update(motorcycle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MotorcycleExists(motorcycle.MotorcycleId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", motorcycle.BrandId);
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeName", motorcycle.TypeId);
            return View(motorcycle);
        }

        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var motorcycle = await _context.Motorcycles.Include(m => m.Brand).Include(m => m.Type).FirstOrDefaultAsync(m => m.MotorcycleId == id);
            if (motorcycle == null) return NotFound();
            return View(motorcycle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle != null) _context.Motorcycles.Remove(motorcycle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MotorcycleExists(int id)
        {
            return _context.Motorcycles.Any(e => e.MotorcycleId == id);
        }
    }
}