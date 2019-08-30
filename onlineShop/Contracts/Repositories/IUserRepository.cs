using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using onlineShop.Models;

namespace onlineShop.Repositories
{
    public interface IUserRepository
    {
        List<IdentityRole> GetAllRoles();
        AppUser GetUserById(string userId);
        AppUser GetUserInclById(string userId);
        AppUser GetUserWithAddressById(string userId);
        int SaveChanges();
        bool UserRoleExists(string userId);
        void AddCustomerAddress(CustomerAddress address);
        CustomerAddress GetCustomerAddress(string userId);
    }
}