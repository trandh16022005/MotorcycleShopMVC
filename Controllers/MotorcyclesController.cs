using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using System.IO;

namespace MotorcycleShopMVC.Controllers
{
    public class MotorcyclesController : Controller
    {
        private readonly AppDbContext _context;

        public MotorcyclesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Motorcycles
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Motorcycles.Include(m => m.Brand).Include(m => m.Type);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Motorcycles/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Motorcycles/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandId");
            ViewData["TypeId"] = new SelectList(_context.VehicleTypes, "TypeId", "TypeId");
            return View();
        }

        // POST: Motorcycles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Motorcycle motorcycle)
        {
            if (motorcycle.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(motorcycle.ImageFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await motorcycle.ImageFile.CopyToAsync(stream);
                }

                motorcycle.ImageUrl = "/images/" + fileName;
            }

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

        // GET: Motorcycles/Edit/5
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

        // POST: Motorcycles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Motorcycle motorcycle)
        {
            if (id != motorcycle.MotorcycleId)
            {
                return NotFound();
            }

            // Giữ ảnh cũ nếu không upload mới
            if (motorcycle.ImageFile == null)
            {
                var oldData = await _context.Motorcycles.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MotorcycleId == id);

                motorcycle.ImageUrl = oldData?.ImageUrl;
            }
            else
            {
                // Upload ảnh mới
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(motorcycle.ImageFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await motorcycle.ImageFile.CopyToAsync(stream);
                }

                motorcycle.ImageUrl = "/images/" + fileName;
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

        // GET: Motorcycles/Delete/5
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

        // POST: Motorcycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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