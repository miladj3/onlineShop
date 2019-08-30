using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace onlineShop.Repositories
{
    public class OrderRepository : IOrderRepository
    {

        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Order> GetAllOrdersIncl()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.DeliveryDetails)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public List<Order> GetAllUserOrdersIncl(string userId)
        {
            return _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.Customer)
                .Include(o => o.DeliveryDetails)
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.Id == id);
        }

        public Order GetOrderInclById(int id)
        {
            _context.DeliveryDetails
                .Include(d => d.CourierData)
                .Include(d => d.LockerData)
                .Include(d => d.StoreData)
                .Where(o => o.OrderId == id).Load();

            return _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ChangeHistory)
                .ThenInclude(ocl => ocl.ChangeLog).ThenInclude(cl => cl.Employee)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
