using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using onlineShop.Models;
using onlineShop.Repositories;

namespace onlineShop.Areas.Identity.Pages.Account.Manage
{
    public class ChangeDeliveryAddressModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;

        public ChangeDeliveryAddressModel(
            UserManager<AppUser> userManager,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            public string Street { get; set; }
            public string Building { get; set; }
            public string Appartment { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }

        public IActionResult OnGet()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return NotFound("Unable to retrieve user.");

            var address = _userRepository.GetCustomerAddress(userId);

            if (!(address == null))
            {
                Input = new InputModel
                {
                    Street = address.Street,
                    Appartment = address.Appartment,
                    Building = address.Building,
                    City = address.City,
                    Country = address.Country,
                    PostalCode = address.PostalCode
                };
            }
            else
            {
                Input = new InputModel();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                return NotFound("Unable to retrieve user.");

            var address = _userRepository.GetCustomerAddress(userId);

            if (!(address == null))
            {
                address.Street = Input.Street;
                address.Appartment = Input.Appartment;
                address.Building = Input.Building;
                address.City = Input.City;
                address.Country = Input.Country;
                address.PostalCode = Input.PostalCode;
            }
            else
            {
                var newAddress = new CustomerAddress
                {
                    Street = Input.Street,
                    Appartment = Input.Appartment,
                    Building = Input.Building,
                    City = Input.City,
                    Country = Input.Country,
                    PostalCode = Input.PostalCode,
                    CustomerId = userId
                };

                _userRepository.AddCustomerAddress(newAddress);
            }

            _userRepository.SaveChanges();

            StatusMessage = "Default delivery address has been updated.";

            return Page();
        }
    }
}
