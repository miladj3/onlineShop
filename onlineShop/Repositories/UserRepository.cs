using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace onlineShop.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AppUser GetUserById(string userId)
        {
            return _context.Users
                    .FirstOrDefault(u => u.Id == userId);
        }

        public AppUser GetUserWithAddressById(string userId)
        {
            return _context.Users
                    .Include(u => u.CustomerAddress)
                    .FirstOrDefault(u => u.Id == userId);
        }

        public AppUser GetUserInclById(string userId)
        {
            return _context.Users
                .Include(u => u.Orders)
                .Include(u => u.ChangeHistory)
                .ThenInclude(ucl => ucl.ChangeLog)
                .ThenInclude(cl => cl.Employee)
                .FirstOrDefault(u => u.Id == userId);
        }

        public List<IdentityRole> GetAllRoles()
        {
            return _context.Roles.AsNoTracking().ToList();
        }

        public bool UserRoleExists(string userId)
        {
            return _context.UserRoles.Any(u => u.UserId == userId);
        }

        public CustomerAddress GetCustomerAddress(string userId)
        {
            return _context.CustomerAddresses.FirstOrDefault(a => a.CustomerId == userId);
        }

        public void AddCustomerAddress(CustomerAddress address)
        {
            _context.CustomerAddresses.Add(address);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
