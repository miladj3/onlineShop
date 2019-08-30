using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using onlineShop.Repositories;

namespace onlineShop.Areas.Identity.Pages.Account.Manage
{
    public partial class OrderListModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public OrderListModel(
            UserManager<AppUser> userManager,
            IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        public List<Order> OrderList { get; set; }

        public IActionResult OnGet()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User not found.");

            OrderList = _orderRepository.GetAllUserOrdersIncl(userId);

            return Page();
        }
    }
}
