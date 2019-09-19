using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.DTOs;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using System;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public partial class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly ICartManager _cartManager;
        private readonly IEmailSender _mailSender;
        private readonly IViewMarkupExtractor _viewMarkupExtractor;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditTrailService _auditTrailService;

        public OrderController(
            UserManager<AppUser> userManager,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            ICartManager cartManager,
            IEmailSender mailSender,
            IViewMarkupExtractor viewMarkupExtractor,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IAuditTrailService auditTrailService)
        {
            _userManager = userManager;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _cartManager = cartManager;
            _mailSender = mailSender;
            _viewMarkupExtractor = viewMarkupExtractor;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _auditTrailService = auditTrailService;
        }

        [HttpGet("/CheckoutOptions")]
        public async Task<IActionResult> CheckoutOptions()
        {
            var cart = _cartManager.GetCart();

            // redirect to cart if it's empty
            if (cart == null)
                return RedirectToAction("Index", "Cart");

            if (cart.ItemCount == 0)
                return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);

            // if user is not signed in redirect to the page with account checkout options
            if (user == null)
                return View("NoCustomerAccount");

            if (user.IsBlocked)
                return RedirectToPage("/AccountRestricted");

            // proceed to checkout if cart & login is OK
            return RedirectToAction("Checkout");
        }

        [HttpGet("/Checkout")]
        public IActionResult Checkout()
        {
            var cart = _cartManager.GetCart();

            // redirect to cart if it's empty
            if (cart == null)
                return RedirectToAction("Index", "Cart");

            if (cart.ItemCount == 0)
                return RedirectToAction("Index", "Cart");

            var dd = new OrderDetailsDTO();
            var userId = _userManager.GetUserId(User);

            // try to populate default delivery address (if available/set by user)
            if (!String.IsNullOrEmpty(userId))
            {
                var user = _userRepository.GetUserWithAddressById(userId);

                if (user != null)
                {
                    dd.Firstname = user.Firstname;
                    dd.Lastname = user.Lastname;
                    dd.PhoneNumber = user.PhoneNumber;

                    if (user.CustomerAddress != null)
                    {
                        dd.Appartment = user.CustomerAddress.Appartment;
                        dd.Building = user.CustomerAddress.Building;
                        dd.City = user.CustomerAddress.City;
                        dd.Country = user.CustomerAddress.City;
                        dd.PostalCode = user.CustomerAddress.PostalCode;
                        dd.Street = user.CustomerAddress.Street;
                    }
                }
            }

            return View(dd);
        }
       
    }
}