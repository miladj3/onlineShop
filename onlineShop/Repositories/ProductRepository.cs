using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using onlineShop.Models.ProductModels;
using System.Collections.Generic;
using System.Linq;

namespace onlineShop.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Product GetProductById(int id)
        {
            return _context.Products
                .FirstOrDefault(p => p.Id == id);
        }

        public Product GetProductWithItemsById(int id)
        {
            return _context.Products
                .Include(p => p.Pictures)
                .Include(p => p.ProductDescriptionItems)
                .FirstOrDefault(p => p.Id == id);
        }

        public Product GetProductInclById(int id)
        {
            var product =  _context.Products
                .Include(p => p.Pictures)
                .Include(p => p.ProductDescriptionItems)
                .Include(p => p.LastModifiedBy)
                .Include(p => p.Subcategory)
                .ThenInclude(s => s.DescriptionFields)
                .FirstOrDefault(p => p.Id == id);

            _context.ProductComments
                .Where(pc => pc.ProductId == id)
                .OrderByDescending(pc => pc.Id)
                .Include(pc => pc.Customer).Load();

            return product;
        }

        public IQueryable<Product> FetchAllProducts()
        {
            return _context.Products
                .Include(p => p.Subcategory);
        }

        public IQueryable<Product> FetchAllProductsIncl()
        {
            return _context.Products
                        .Include(p => p.Pictures)
                        .Include(p => p.Comments)
                        .Include(p => p.ProductDescriptionItems)
                        .ThenInclude(d => d.Field)
                        .Where(p => p.IsActive == true);
        }

        public IQueryable<Product> FetchProductsActiveInclBySubcatId(int subcategoryId)
        {
            return _context.Products
                        .Include(p => p.Pictures)
                        .Include(p => p.Comments)
                        .Include(p => p.ProductDescriptionItems)
                        .ThenInclude(d => d.Field)
                        .Where(p => p.SubcategoryId == subcategoryId && p.IsActive == true);
        }

        public bool ProductIsRatedByUserId(int productId, string userId)
        {
            return _context.ProductComments
                .Any(c => c.ProductId == productId && c.CustomerId == userId && c.RatingValue > 0);
        }

        public bool ProductIsWatchedByUserId(int productId, string userId)
        {
            return _context.ProductAvalabilityNotifications
                .Any(n => n.CustomerId == userId && n.ProductId == productId);
        }

        public ProductComment GetProductCommentById(int id)
        {
            return _context.ProductComments.FirstOrDefault(c => c.Id == id);
        }

        public IQueryable<ProductComment> FetchProductCommentsPublishedByProductId(int productId)
        {
            return _context.ProductComments
                .Include(c => c.Customer)
                .Where(c => c.ProductId == productId && c.IsPublished)
                .OrderByDescending(c => c.DateAdded);
        }

        public IQueryable<ProductComment> FetchProductCommentsAllPending()
        {
            return _context.ProductComments
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .Where(c => !c.IsPublished)
                .OrderByDescending(c => c.DateAdded);
        }

        public void AddProductComment(ProductComment comment)
        {
            _context.ProductComments.Add(comment);
        }

        public void RemoveProductComment(ProductComment comment)
        {
            _context.ProductComments.Remove(comment);
        }

        public void RemoveProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public List<ProductAvailabilityNotification> GetProductNotifications(int productId)
        {
            return _context.ProductAvalabilityNotifications
                .Where(n => n.ProductId == productId)
                .Include(n => n.Customer).ToList();
        }

        public ProductAvailabilityNotification GetProductNotificationByUserId(int productId, string userId)
        {
            return _context.ProductAvalabilityNotifications
                .FirstOrDefault(n => n.CustomerId == userId && n.ProductId == productId);
        }

        public bool ProductNotificationExistsForUserId(int productId, string userId)
        {
            return _context.ProductAvalabilityNotifications.Any(n => n.CustomerId == userId && n.ProductId == productId);
        }

        public bool ProductNotifExistsForEmail(int productId, string email)
        {
            return _context.ProductAvalabilityNotifications.Any(n => n.Email == email && n.ProductId == productId);
        }

        public void AddProductNotification(ProductAvailabilityNotification notification)
        {
            _context.ProductAvalabilityNotifications.Add(notification);
        }

        public void RemoveProductNotification(ProductAvailabilityNotification notification)
        {
            _context.ProductAvalabilityNotifications.Remove(notification);
        }

        public void RemoveProductNotifications(List<ProductAvailabilityNotification> notifications)
        {
            _context.ProductAvalabilityNotifications.RemoveRange(notifications);
        }

        public List<ProductDescriptionItem> GetProductDescItemsByProductId(int productId)
        {
            return _context.ProductDescriptionItems
                .Where(f => f.ProductId == productId).ToList();
        }

        public bool ProductWasOrderedByUser(int productId, string userId)
        {
            return _context.Orders
                .Where(o => o.CustomerId == userId && o.Status == OrderStatus.Completed)
                .SelectMany(i => i.Items)
                .Any(i => i.ProductId == productId);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
