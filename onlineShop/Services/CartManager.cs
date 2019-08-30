using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using onlineShop.Contracts;
using onlineShop.Models;
using System.Linq;

namespace onlineShop.Services
{
    public class CartManager : ICartManager
    {
        private const string sessionKey = "shopCart";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Cart GetCart()
        {
            var sessionValue = _httpContextAccessor.HttpContext.Session.GetString(sessionKey);
            if (string.IsNullOrEmpty(sessionValue))
                return null;

            return JsonConvert.DeserializeObject<Cart>(sessionValue); ;
        }

        public void ResetCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove(sessionKey);
        }

        public void UpdateCartItem(Product product, int quantity, bool replaceExisting)
        {
            var cart = GetCart();

            // instantiate if doesn't exist yet
            if (cart == null)
                cart = new Cart();

            var cartItem = cart.Items.FirstOrDefault(i => i.Product.Id == product.Id);

            //No cart item with specified product
            if (cartItem == null)
            {
                cart.Items.Add(new CartItem { Product = product, Quantity = quantity });
            }
            //Cart item with such product already exists
            else
            {
                if (replaceExisting) // ignore current position in cart for given item and set quantity
                {
                    cartItem.Quantity = quantity;
                }
                else // append to current position in cart for given item
                {
                    cartItem.Quantity += quantity;
                }
            }

            SaveCart(cart);
        }

        public void RemoveCartItem(int productId)
        {
            var cart = GetCart();

            // instantiate if doesn't exist yet
            if (GetCart() != null)
            {
                var cartItem = cart.Items.FirstOrDefault(i => i.Product.Id == productId);

                if (cartItem != null)
                {
                    cart.Items.Remove(cartItem);
                    SaveCart(cart);
                }
            }
        }

        private void SaveCart(Cart cart)
        {
            var serializedCart = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Session.SetString(sessionKey, serializedCart);
        }
    }
}
