using System.Collections.Generic;
using onlineShop.Models;

namespace onlineShop.Repositories
{
    public interface IOrderRepository
    {
        void AddOrder(Order order);
        List<Order> GetAllOrdersIncl();
        Order GetOrderById(int id);
        List<Order> GetAllUserOrdersIncl(string userId);
        Order GetOrderInclById(int id);
        int SaveChanges();
    }
}