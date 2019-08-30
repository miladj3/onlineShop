using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using onlineShop.Contracts;
using onlineShop.DTOs;
using onlineShop.Extensions;
using onlineShop.Models;
using onlineShop.Repositories;

namespace onlineShop.Controllers.API
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IViewMarkupExtractor _viewMarkupExtractor;
        private readonly ILogger<OrderController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailSender _mailSender;
        private readonly ICartManager _cartManager;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderController(
              UserManager<AppUser> userManager,
             IMemoryCache memoryCache,
             IViewMarkupExtractor viewMarkupExtractor,
             ILogger<OrderController> logger,
             IEmailSender mailSender,
             ICartManager cartManager,
             IProductRepository productRepository,
             IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _memoryCache = memoryCache;
            _viewMarkupExtractor = viewMarkupExtractor;
            _logger = logger;
            _mailSender = mailSender;
            _cartManager = cartManager;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        [HttpPost("/API/Checkout/Summary")]
        public async Task<IActionResult> CheckoutSummary([FromForm] OrderDetailsDTO deliveryDetailsDTO)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // check if account is restricted (read-only)
                if (user.IsBlocked)
                {
                    return BadRequest("Your request can't be processed as you account has been restricted.");
                }
            }
            else
            {
                if (String.IsNullOrEmpty(deliveryDetailsDTO.Email))
                {
                    ModelState.AddModelError("", "Email is not provided.");
                }
            }

            if (!ModelState.IsValid)
            {
                var msg = String.Join(" \r\n ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(msg);
            }

            try
            {
                var cart = _cartManager.GetCart();

                // redirect to cart if it's empty
                if (_cartManager.GetCart() == null)
                    return BadRequest("Cart is empty. Please try again.");

                if (_cartManager.GetCart().ItemCount == 0)
                    return BadRequest("Cart is empty. Please try again.");

                // create order draft (dto)
                var tempOrder = new OrderDTO()
                {
                    DeliveryDetails = deliveryDetailsDTO,
                    Items = cart.Items.Select(c => new OrderItemDTO
                    {
                        Product = new ProductDTO
                        {
                            Name = c.Product.Name,
                            Price = c.Product.SalePrice,
                            Id = c.Product.Id
                        },
                        Quantity = c.Quantity
                    })
                    .ToList(),
                    NoUserAccount = (user == null),
                    DeliveryFee = DeliveryFeeCalculator.Calculate(deliveryDetailsDTO.DeliveryMethodType)
                };

                // write temp order object to cache and assign confirmation key to be sent to client
                var orderConfirmationKey = WriteToCache(tempOrder);

                // generate markup to be show in order confirmation
                var viewMarkup = await _viewMarkupExtractor.MarkupToString(this.ControllerContext, "OrderPreview", tempOrder, this.TempData);

                return new JsonResult(new { summaryMarkup = viewMarkup, confirmationKey = orderConfirmationKey });
            }
            catch (Exception ex)
            {
                var userId = _userManager.GetUserId(User);

                _logger.LogError("Order validation failed (user ID: " + userId + "). " + ex.ToString());
                return BadRequest("Unexpected error has occured. Please try again later.  \r\n If problem persists, please contact administrator.");
            }
        }

        [HttpPost("/API/Checkout/Confirm/{confirmationKey}")]
        public async Task<IActionResult> CheckoutConfirm([FromRoute] string confirmationKey)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                // validate confirmation key
                if (String.IsNullOrEmpty(confirmationKey))
                    return BadRequest("Missing confirmation key.");

                // try to retrieve temp order from cache
                var tempOrder = ReadFromCache(confirmationKey);

                if (tempOrder == null)
                    return BadRequest("Temp order not found.");

                // create order
                var order = new Order(tempOrder.DeliveryDetails.DeliveryMethodType)
                {
                    CreatedOn = DateTime.UtcNow,
                    CustomerId = userId,
                    Status = OrderStatus.Created
                };

                // append all items
                foreach (var tempItem in tempOrder.Items)
                {
                    var productInDb = _productRepository.GetProductById(tempItem.Product.Id);

                    if (productInDb != null 
                        && productInDb.NumberInStock >= tempItem.Quantity 
                        && productInDb.IsActive 
                        && productInDb.IsAvailable)
                    {
                        order.Items.Add(new OrderItem
                        {
                            Product = productInDb,
                            ProductId = productInDb.Id,
                            PurchasePrice = productInDb.SalePrice,
                            Quantity = tempItem.Quantity
                        });
                    }
                }

                // check if email has been provided for guest user
                if (String.IsNullOrEmpty(userId))
                {
                    if (String.IsNullOrEmpty(tempOrder.DeliveryDetails.Email))
                        return BadRequest("No email address provided for guest user's order.");

                    order.DeliveryDetails.EmailAddress = tempOrder.DeliveryDetails.Email;
                }

                // set basic delivery details
                order.DeliveryFee = DeliveryFeeCalculator.Calculate(tempOrder.DeliveryDetails.DeliveryMethodType);

                // set delivery-specific details
                Object2ObjectMappings.OrderDetailsDtoToOrder(tempOrder.DeliveryDetails, order);

                // check consistency or order amount (prevents user from accidentally ordering items for which the price has just changed)
                if (order.OrderAmountTotal != tempOrder.OrderAmountTotal)
                    return BadRequest("Price of some items you are trying to order might have changed.");

                // persist changes 
                _orderRepository.AddOrder(order);

                if (_orderRepository.SaveChanges() > 0)
                {
                    _cartManager.ResetCart();
                    _memoryCache.Remove(confirmationKey);

                    await SendOrderConfirmation(order);
                }

                // return order id
                return Ok(new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                var userId = _userManager.GetUserId(User);

                _logger.LogError("Order validation failed (user ID: " + userId + "). " + ex.ToString());
                return BadRequest("Unexpected error has occured. Please try again later.");
            }
        }

        private async Task SendOrderConfirmation(Order order)
        {
            var user = _userManager.GetUserAsync(User);

            var markupString = await _viewMarkupExtractor.MarkupToString(this.ControllerContext, "OrderEmailConfirmation", order, this.TempData);
            var recipient = (await user) == null ? order.DeliveryDetails.EmailAddress : (await user).Email;
            await _mailSender.SendEmailAsync(recipient, "Order Confirmation", markupString);
        }

        private string WriteToCache(OrderDTO tempOrder)
        {
            var objKey = Guid.NewGuid().ToString().Replace("-", "");
            var objSerialized = JsonConvert.SerializeObject(tempOrder);
            var dateTimeExpire = DateTime.UtcNow.AddMinutes(20);

            _memoryCache.Set(objKey, objSerialized, dateTimeExpire);

            return objKey;
        }

        private OrderDTO ReadFromCache(string tempOrderKey)
        {
            var tempOrderSerialized = (string)_memoryCache.Get(tempOrderKey);
            var tempOrder = JsonConvert.DeserializeObject<OrderDTO>(tempOrderSerialized);

            return tempOrder;
        }
    }
}
