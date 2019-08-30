using onlineShop.Models;

namespace onlineShop.Contracts
{
    public interface ICartManager
    {
        Cart GetCart();
        void RemoveCartItem(int productId);
        void ResetCart();
        void UpdateCartItem(Product product, int quantity, bool replaceExisting);
    }
}