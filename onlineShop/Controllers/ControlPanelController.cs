using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;

namespace onlineShop.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class ControlPanelController : Controller
    {
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;

        public ControlPanelController(IBreadcrumbNavBuilder breadcrumbNavBuilder)
        {
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
        }

        public IActionResult Index()
        {
            _breadcrumbNavBuilder.CreateForNode("ControlPanelIndex", new { }, this);
            return View();
        }

    }
}