using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Models;
using onlineShop.Repositories;

namespace onlineShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartManager _cartManager;
        private readonly IProductRepository _productRepository;

        public CartController(
            ICartManager cartManager,
            IProductRepository productRepository)
        {
            _cartManager = cartManager;
            _productRepository = productRepository;
        }

        [Route("/cart/info")]
        public IActionResult GetCartInfo()
        {
            var cart = _cartManager.GetCart() ?? new Cart();
            return new JsonResult(new { cartAmount = cart.CartAmount, cartItemCount = cart.ItemCount });
        }

        [HttpPost]
        [Route("/api/cart/addItem/{productId}/{quantity}")]
        public IActionResult AddToCart([FromRoute] int productId, [FromRoute] int quantity)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
                return BadRequest("Product not found.");

            if (product.NumberInStock < quantity)
                return BadRequest("Not enough quantity in stock.");

            _cartManager.UpdateCartItem(product, quantity, false);

            return Ok();
        }

        [HttpPost]
        [Route("/api/cart/updateItem/{productId}/{quantity}")]
        public IActionResult UpdateQuantityForCartItem([FromRoute] int productId, [FromRoute] int quantity)
        {
            var product = _productRepository.GetProductById(productId);

            if (product != null)
                _cartManager.UpdateCartItem(product, quantity, true);

            return Ok();
        }

        [HttpPost]
        [Route("/api/cart/removeItem/{productId}")]
        public IActionResult RemoveItemFromCart([FromRoute] int productId)
        {
            _cartManager.RemoveCartItem(productId);
            return Ok();
        }

    }
}