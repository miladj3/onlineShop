using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using onlineShop.DTOs;
using onlineShop.Extensions;
using onlineShop.Models;
using onlineShop.Repositories;

namespace onlineShop.Areas.Identity.Pages.Account.Manage
{
    public partial class OrderDetailsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public OrderDetailsModel(
            UserManager<AppUser> userManager,
            IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        [BindProperty]
        public OrderDTO OrderDTO { get; set; }

        public IActionResult OnGetAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User not found.");

            var order = _orderRepository.GetOrderInclById(id);

            //validate user
            if (order == null || !String.Equals(order.CustomerId, userId))
                return BadRequest("Order with such ID for current user doesn't exist.");

            OrderDTO = new OrderDTO();
            Object2ObjectMappings.OrderItemListToOrderItemDtoList(order.Items, OrderDTO.Items);
            Object2ObjectMappings.OrderToOrderDetailsDto(order, OrderDTO.DeliveryDetails);
            Object2ObjectMappings.OrderMainDataToOrderDto(order, OrderDTO);

            return Page();
        }
    }
}
