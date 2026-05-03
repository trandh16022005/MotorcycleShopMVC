using Microsoft.AspNetCore.Mvc;

namespace MotorcycleShopMVC.Controllers
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
