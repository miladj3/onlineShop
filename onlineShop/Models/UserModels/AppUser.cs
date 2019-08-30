using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace onlineShop.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Orders = new List<Order>();
            ChangeHistory = new List<UserChangeLog>();
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime RegisteredOn { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Cart Cart { get; set; }
        public CustomerAddress CustomerAddress { get; set; }

        public bool IsBlocked { get; set; } = false;
        public List<UserChangeLog> ChangeHistory { get; set; }
    }
}
