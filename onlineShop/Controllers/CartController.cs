using Microsoft.AspNetCore.Mvc;
using onlineShop.Models;

namespace onlineShop.Controllers
{
    public class CartController : Controller
    {
        public CartController()
        {
        }

        [Route("/cart")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/cart/load/preview")]
        public IActionResult LoadCartPreview()
        {
            return ViewComponent("Cart", new { mode = CartDisplayMode.Preview });
        }

        [Route("/cart/load/full")]
        public IActionResult LoadCartFull()
        {
            return ViewComponent("Cart", new { mode = CartDisplayMode.Full });
        }

    }
}