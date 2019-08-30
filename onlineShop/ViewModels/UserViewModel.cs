using onlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace onlineShop.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Roles = new List<UserRoleViewModel>();
            Orders = new List<Order>();
            ChangeHistory = new List<ChangeLog>();
        }

        public string UserId  { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Registered On")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy H:mm}", ApplyFormatInEditMode = true)]
        public DateTime RegisteredOn { get; set; }
        public ICollection<Order> Orders { get; set; }

        [Required]
        [Display(Name = "Restricted Account")]
        public bool IsBlocked { get; set; }

        // Additional data
        public List<UserRoleViewModel> Roles { get; set; }
        public bool AllowRoleChange { get; set; }
        public bool IsCustomer { get { return this.Roles.Where(r => r.IsAssigned == true).Count() == 0; } }

        public List<ChangeLog> ChangeHistory { get; set; }
    }
}
