using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;

namespace MotorcycleShopMVC.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parts
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var parts = _context.Parts
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsQueryable();

            int pageSize = 10;
            var paginatedList = await PaginatedList<Part>.CreateAsync(parts.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedList);
        }

        // GET: Parts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PartId == id);

            if (part == null) return NotFound();

            return View(part);
        }
    }
}