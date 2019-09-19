using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace onlineShop.Controllers
{
    public partial class CatalogController : Controller
    {
               [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Catalog/")]
        public IActionResult ManageCatalog()
        {
            var departmentList = _departmentRepository.GetAllDepartments();

            _breadcrumbNavBuilder.CreateForNode("CPanelCatalogView", new { }, this);
            return View(departmentList);
        }
    }
}
