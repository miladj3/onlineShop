using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.DTOs;
using onlineShop.Extensions;
using onlineShop.Filters;
using onlineShop.Models;
using System;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public partial class OrderController : Controller
    {
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Orders/")]
        public IActionResult ManageOrders()
        {
            var orderList = _orderRepository.GetAllOrdersIncl();

            _breadcrumbNavBuilder.CreateForNode("CPanelOrdersView", new { }, this);

            return View(orderList);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Orders/View/{id}")]
        public IActionResult AdminView(int id)
        {
            var order = _orderRepository.GetOrderInclById(id);

            // map to dto and return view
            var orderDto = new OrderDTO();
            Object2ObjectMappings.OrderItemListToOrderItemDtoList(order.Items, orderDto.Items);
            Object2ObjectMappings.OrderToOrderDetailsDto(order, orderDto.DeliveryDetails);
            Object2ObjectMappings.OrderMainDataToOrderDto(order, orderDto);

            _breadcrumbNavBuilder.CreateForNode("CPanelOrderView", new { orderId = order.Id, orderName = ("Order #" + order.Id).ToString() }, this);

            return View("AdminView", orderDto);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Orders/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            // retrieve order and related data
            var order = _orderRepository.GetOrderInclById(id);

            var deliveryDetailsDTO = new OrderDetailsDTO();

            // get relevant delivery and payment data
            Object2ObjectMappings.OrderToOrderDetailsDto(order, deliveryDetailsDTO);

            // append to order DTO
            var orderDTO = new OrderDTO()
            {
                DeliveryDetails = deliveryDetailsDTO,
                Id = order.Id,
                NoUserAccount = (String.IsNullOrEmpty(order.CustomerId))
            };

            _breadcrumbNavBuilder.CreateForNode("CPanelOrderEdit", new { orderId = order.Id, orderName = ("Order #" + order.Id).ToString() }, this);

            return View(orderDTO);
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Save(OrderDTO orderDto)
        {
            var order = _orderRepository.GetOrderInclById(orderDto.Id);

            if (order == null)
                return BadRequest("Order not found.");

            // Ensure that email is provided for order created by guest user
            if (String.IsNullOrEmpty(order.CustomerId) && String.IsNullOrEmpty(orderDto.DeliveryDetails.Email))
            {
                ModelState.AddModelError("", "Email is not provided.");
            }

            if (!ModelState.IsValid)
            {
                _breadcrumbNavBuilder.CreateForNode("CPanelOrderEdit", new { orderId = order.Id, orderName = ("Order #" + order.Id).ToString() }, this);
                return View("Edit", orderDto);
            }

            var admin = await _userManager.GetUserAsync(User);

            // Preserve original user comment
            var tempComment = order.DeliveryDetails.Comment;

            // Set delivery & payment details
            Object2ObjectMappings.OrderDetailsDtoToOrder(orderDto.DeliveryDetails, order);

            // Recalculate delivery fee
            order.DeliveryFee = DeliveryFeeCalculator.Calculate(order.DeliveryDetails.DeliveryMethodType);
            order.DeliveryDetails.Comment = tempComment;

            // Retireve changes and log
            var changeLogs = _auditTrailService.RetrieveAndLogChanges();

            foreach (var changeLog in changeLogs)
                order.ChangeHistory.Add(new OrderChangeLog { ChangeLog = changeLog, Order = order });

            // Update last edit timestamp
            order.LastModifiedById = admin.Id;
            order.LastModifiedOn = DateTime.UtcNow;

            _orderRepository.SaveChanges();

            return RedirectToAction("AdminView", new { id = orderDto.Id });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(Order order)
        {
            var orderInDb = _orderRepository.GetOrderInclById(order.Id);

            var admin = _userManager.GetUserAsync(User);

            // determine if order gets cancelled
            var wasCancelled = (orderInDb.Status == OrderStatus.Cancelled || orderInDb.Status == OrderStatus.CancelledByCustomer);
            var isCancelled = (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.CancelledByCustomer);

            // continue if order is found and status actually has been changed
            if (orderInDb != null && orderInDb.Status != order.Status)
            {
                orderInDb.Status = order.Status;

                // Retireve changes and log
                var changeLogs = _auditTrailService.RetrieveAndLogChanges();

                foreach (var changeLog in changeLogs)
                    orderInDb.ChangeHistory.Add(new OrderChangeLog { ChangeLog = changeLog, Order = order });

                // update entity state change info
                orderInDb.LastModifiedOn = DateTime.UtcNow;
                orderInDb.LastModifiedById = (await admin).Id;

                if (orderInDb.Status == OrderStatus.Completed)
                    orderInDb.CompletedOn = DateTime.UtcNow;

                // return items to inventory if order gets cancelled
                if (isCancelled)
                {
                    //var orderItems = _context.OrderItems.Include(i => i.Product).Where(i => i.OrderId == orderInDb.Id).ToList();

                    foreach (var orderItem in orderInDb.Items)
                    {
                        var productInDb = _productRepository.GetProductById(orderItem.ProductId);
                        productInDb.NumberInStock += orderItem.Quantity;
                    }
                }

                // adjust inventory if order is moved FROM status Cancelled/CancelledByUser
                if (wasCancelled & !isCancelled)
                {
                    foreach (var orderItem in orderInDb.Items)
                    {
                        var productInDb = _productRepository.GetProductById(orderItem.ProductId);
                        productInDb.NumberInStock -= orderItem.Quantity;
                    }
                }

                // inform customer about status change
                TempData["StatusUpdate"] = true;
                var markupString = await _viewMarkupExtractor.MarkupToString(this.ControllerContext, "OrderEmailConfirmation", orderInDb, this.TempData);
                var recipient = (orderInDb.Customer) == null ? orderInDb.DeliveryDetails.EmailAddress : orderInDb.Customer.Email;
                await _mailSender.SendEmailAsync(recipient, "Order Status Update", markupString);

                _orderRepository.SaveChanges();
            }

            return RedirectToAction("AdminView", new { id = order.Id });
        }
    }
}