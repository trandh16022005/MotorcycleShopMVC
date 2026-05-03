using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Filters;

namespace MotorcycleShopMVC.Controllers
{
    public class MotorcycleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MotorcycleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Motorcycle
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Motorcycles.Include(m => m.Brand).Include(m => m.Type);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Motorcycle/Details/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motorcycle = await _context.Motorcycles
                .Include(m => m.Brand)
                .Include(m => m.Type)
                .FirstOrDefaultAsync(m => m.MotorcycleId == id);
            if (motorcycle == null)
            {
                return NotFound();
            }

            return View(motorcycle);
        }

        // GET: Motorcycle/Create
        [RoleAuthorize("Admin")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeId");
            return View();
        }

        // POST: Motorcycle/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Create([Bind("MotorcycleId,ModelName,BrandId,TypeId,EngineCapacity,YearFrom,YearTo,Price,Color,WarrantyPolicy,ImagePath,Description,StockQty,CreatedAt,UpdatedAt")] Motorcycle motorcycle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(motorcycle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", motorcycle.BrandId);
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeId", motorcycle.TypeId);
            return View(motorcycle);
        }

        // GET: Motorcycle/Edit/5
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", motorcycle.BrandId);
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeId", motorcycle.TypeId);
            return View(motorcycle);
        }

        // POST: Motorcycle/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("MotorcycleId,ModelName,BrandId,TypeId,EngineCapacity,YearFrom,YearTo,Price,Color,WarrantyPolicy,ImagePath,Description,StockQty,CreatedAt,UpdatedAt")] Motorcycle motorcycle)
        {
            if (id != motorcycle.MotorcycleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(motorcycle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MotorcycleExists(motorcycle.MotorcycleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId", motorcycle.BrandId);
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeId", motorcycle.TypeId);
            return View(motorcycle);
        }

        // GET: Motorcycle/Delete/5
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var motorcycle = await _context.Motorcycles
                .Include(m => m.Brand)
                .Include(m => m.Type)
                .FirstOrDefaultAsync(m => m.MotorcycleId == id);
            if (motorcycle == null)
            {
                return NotFound();
            }

            return View(motorcycle);
        }

        // POST: Motorcycle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle != null)
            {
                _context.Motorcycles.Remove(motorcycle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MotorcycleExists(int id)
        {
            return _context.Motorcycles.Any(e => e.MotorcycleId == id);
        }
    }
}
