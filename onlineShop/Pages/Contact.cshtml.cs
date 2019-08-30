using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using onlineShop.Models;

namespace onlineShop.Pages
{
    public class ContactModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public ContactModel(UserManager<AppUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [BindProperty]
        [Display(Name = "Your Question")]
        [MinLength(30)]
        [MaxLength(1500)]
        [Required]
        public string UserMessage { get; set; }

        [BindProperty]
        [Display(Name = "Email")]
        [MaxLength(75)]
        [Required]
        public string UserEmail { get; set; }

        public bool isSubmitted { get; set; }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                UserEmail = user.Email;
            }
        }

        public void OnGetSent(bool success)
        {
            isSubmitted = true;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var defaultAddress = _configuration.GetSection("Contact")["QueryInbox"].ToString();
                var userIP = HttpContext.Connection.RemoteIpAddress;

                await _emailSender.SendEmailAsync(defaultAddress, $"Question from: {UserEmail}", $"{UserMessage} (User IP: {userIP})");

                isSubmitted = true;
                UserEmail = "";
                UserMessage = "";

                return new RedirectToPageResult("Contact", "Sent");
            }
            else
            {
                isSubmitted = false;
                return null;
            }
        }
    }
}
