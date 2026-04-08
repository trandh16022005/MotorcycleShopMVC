using Microsoft.AspNetCore.Mvc;
using MotorcycleShopMVC.Models;
using System.Linq;

namespace MotorcycleShopMVC.Controllers
{
    public class BrandsController : Controller
    {
        private readonly AppDbContext _context;

        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Brands
        public IActionResult Index()
        {
            return View(_context.Brands.ToList());
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Brands.Add(brand);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }
    }
}