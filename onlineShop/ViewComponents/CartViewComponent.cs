using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Models;

namespace onlineShop.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ICartManager _cartManager;

        public CartViewComponent(ICartManager cartManager)
        {
            _cartManager = cartManager;
        }

        public IViewComponentResult Invoke(CartDisplayMode mode)
        {
            var cart = _cartManager.GetCart() ?? new Cart();

            switch (mode)
            {
                case (CartDisplayMode.Preview):
                    return View("Preview", cart);

                case (CartDisplayMode.Full):
                    return View("Full", cart);

                default:
                    return null;
            }
        }
    }
}
