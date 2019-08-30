using System.Collections.Generic;
using System.Linq;
using onlineShop.Models;
using onlineShop.Models.ProductModels;

namespace onlineShop.Repositories
{
    public interface IProductRepository
    {
        void AddProduct(Product product);
        void AddProductComment(ProductComment comment);
        void AddProductNotification(ProductAvailabilityNotification notification);
        IQueryable<Product> FetchAllProducts();
        IQueryable<Product> FetchAllProductsIncl();
        IQueryable<Product> FetchProductsActiveInclBySubcatId(int subcategoryId);
        ProductComment GetProductCommentById(int id);
        List<ProductComment> GetProductCommentsByProductId(int productId);
        List<ProductDescriptionItem> GetProductDescItemsByProductId(int productId);
        Product GetProductInclById(int id);
        Product GetProductById(int id);
        ProductAvailabilityNotification GetProductNotificationByUserId(int productId, string userId);
        List<ProductAvailabilityNotification> GetProductNotifications(int productId);
        Product GetProductWithItemsById(int id);
        bool ProductIsRatedByUserId(int productId, string userId);
        bool ProductIsWatchedByUserId(int productId, string userId);
        bool ProductNotifExistsForEmail(int productId, string email);
        bool ProductNotificationExistsForUserId(int productId, string userId);
        bool ProductWasOrderedByUser(int productId, string userId);
        void RemoveProduct(Product product);
        void RemoveProductComment(ProductComment comment);
        void RemoveProductNotification(ProductAvailabilityNotification notification);
        void RemoveProductNotifications(List<ProductAvailabilityNotification> notifications);
        int SaveChanges();
    }
}