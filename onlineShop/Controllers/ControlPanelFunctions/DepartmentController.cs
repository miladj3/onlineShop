using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Filters;
using onlineShop.Models;
using onlineShop.ViewModels;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public partial class DepartmentController : Controller
    {
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Departments/{id}")]
        public IActionResult AdminView(int id)
        {
            var department = _departmentRepository.GetDepartmentInclById(id);

            PrepareNavData(id, "CPanelDepartmentView");

            return View("AdminView", new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                Picture = department.Picture,
                Categories = department.Categories
            });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Departments/Create")]
        public IActionResult Create()
        {
            _breadcrumbNavBuilder.CreateForNode("CPanelDepartmentAdd", new { }, this);
            return View("Edit", new DepartmentViewModel());
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Departments/{id}/Edit/")]
        public IActionResult Edit(int id)
        {
            return View("Edit", FetchDataToModify(id));
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] DepartmentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                if (vm.Id == 0)
                {
                    _breadcrumbNavBuilder.CreateForNode("CPanelDepartmentAdd", new { }, this);
                    return View("Edit", vm);
                }
                else
                {
                    PrepareNavData(vm.Id, "DepartmentDisplay");
                    return View("Edit", FetchDataToModify(vm.Id));
                }
            }

            Department dep;

            // new department
            if (vm.Id == 0)
            {
                dep = new Department
                {
                    Name = vm.Name,
                    Description = vm.Description
                };

                _departmentRepository.AddDepartment(dep);
            }
            // edit existing department
            else
            {
                dep = _departmentRepository.GetDepartmentById(vm.Id);

                dep.Name = vm.Name;
                dep.Description = vm.Description;
            }

            // Upload and set new pictures
            string errorMessage = "";

            if (vm.PictureToUpload != null)
            {
                if (_uploader.ValidateImageSingle(vm.PictureToUpload, ref errorMessage))
                {
                    var imgUploaded = await _uploader.UploadSingle(vm.PictureToUpload);
                    dep.Picture = new FilePath { Path = imgUploaded };
                }
                else
                {
                    // Return to the same view if file uploader thrown warning
                    ModelState.AddModelError("", errorMessage);
                    return View("Edit", FetchDataToModify(vm.Id));
                }
            }

            _auditTrailService.RetrieveAndLogChanges();

            _departmentRepository.SaveChanges();
            return RedirectToAction("AdminView", new { id = dep.Id });
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin")]
        [HttpPost("/ControlPanel/Departments/{id}/Delete")]
        public IActionResult Delete(int id)
        {
            var dep = _departmentRepository.GetDepartmentById(id);

            if (_departmentRepository.DepartmentHasProducts(id))
                return BadRequest("Action blocked: department contains some products.");

            _departmentRepository.RemoveDepartment(dep);

            _auditTrailService.RetrieveAndLogChanges();

            _departmentRepository.SaveChanges();

            return Ok(new { redirectUrl = Url.Action("ManageCatalog", "Catalog") });
        }

        private DepartmentViewModel FetchDataToModify(int id)
        {
            var department = _departmentRepository.GetDepartmentById(id);

            PrepareNavData(id, "CPanelDepartmentEdit");

            return new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                Picture = department.Picture
            };
        }
    }
}