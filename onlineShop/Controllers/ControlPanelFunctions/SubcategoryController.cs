using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Filters;
using onlineShop.Models;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public partial class SubcategoryController : Controller
    {

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateProductDescriptionFields(ProductDescriptionFieldsViewModel vm)
        {
            var subcat = _subcategoryRepository.GetSubcategoryById(vm.SubcategoryId);

            foreach (var field in vm.Fields)
            {
                var fieldInDb = _subcategoryRepository.GetProductDescFieldById(field.Id);

                fieldInDb.Name = field.Name;
                fieldInDb.DisplayOrderId = field.DisplayOrderId;
                fieldInDb.DisplayInItemPreview = field.DisplayInItemPreview;
            }

            foreach (var fieldToAdd in vm.FieldsToAdd)
            {
                if (!String.IsNullOrEmpty(fieldToAdd.Name) && !String.IsNullOrWhiteSpace(fieldToAdd.Name))
                {
                    var newField = new ProductDescriptionField
                    {
                        Name = fieldToAdd.Name,
                        DisplayOrderId = fieldToAdd.DisplayOrderId,
                        DisplayInItemPreview = fieldToAdd.DisplayInItemPreview,
                    };

                    subcat.DescriptionFields.Add(fieldToAdd);
                }
            }

            _subcategoryRepository.SaveChanges();

            return RedirectToAction("EditProductDescriptionFields", "Subcategory", new { subcategoryId = vm.SubcategoryId });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/ControlPanel/Subcategories/{subcategoryId}/ProductDescriptionFields/")]
        public IActionResult EditProductDescriptionFields(int subcategoryId, [FromQuery] int newFields)
        {
            var descFields = _subcategoryRepository.GetProductDescFieldsBySubcat(subcategoryId);

            PrepareNavData(subcategoryId, "CPanelProductDescriptionFields");

            var vm = new ProductDescriptionFieldsViewModel
            {
                SubcategoryId = subcategoryId,
                Fields = descFields,
                CategoryName = _subcategoryRepository.GetSubcategoryById(subcategoryId).Name
            };

            int i = 0;
            var maxOrderId = descFields.Max(df => df.DisplayOrderId);

            while (i < newFields)
            {
                vm.FieldsToAdd.Add(new ProductDescriptionField() { DisplayOrderId = (maxOrderId + 1 + i) });
                i++;
            }

            return View(vm);
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin")]
        [HttpPost("/ControlPanel/ProductDescriptionFields/{fieldId}/Delete")]
        public IActionResult DeleteProductDescriptionField(int fieldId)
        {
            var fieldToDel = _subcategoryRepository.GetProductDescFieldById(fieldId);
            var subcatId = fieldToDel.SubcategoryId;

            _subcategoryRepository.RemoveProductDescField(fieldToDel);
            _subcategoryRepository.SaveChanges();

            return RedirectToAction("EditProductDescriptionFields", "Subcategory", new { subcategoryId = subcatId });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Subcategories/{id}")]
        public IActionResult AdminView(int id)
        {
            var subcategory = _subcategoryRepository.GetSubcategoryInclById(id);

            PrepareNavData(subcategory.Id, "CPanelSubcategoryView");

            return View("AdminView", new SubcategoryViewModel { Id = subcategory.Id, Name = subcategory.Name, Description = subcategory.Description, Picture = subcategory.Picture, Products = subcategory.Products });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Subcategories/Create")]
        public IActionResult Create(int categoryId)
        {
            // prepare breadcrumb navigation data
            var cat = _categoryRepository.GetCategoryById(categoryId);
            var dep = _departmentRepository.GetDepartmentById(cat.DepartmentId);

            _breadcrumbNavBuilder.CreateForNode("CPanelSubcategoryAdd",
                new
                {
                    categoryId = cat.Id,
                    categoryName = cat.Name,
                    departmentId = dep.Id,
                    departmentName = dep.Name
                }
                , this);

            return View("Edit", new SubcategoryViewModel { CategoryId = categoryId });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Subcategories/{id}/Edit/")]
        public IActionResult Edit(int id)
        {
            PrepareNavData(id, "CPanelSubcategoryEdit");
            return View("Edit", FetchDataToModify(id));
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] SubcategoryViewModel vm)
        {
            Subcategory subcat;
            string uploaderErrorMessage = "";

            // Validate all inputs
            if (vm.PictureToUpload != null)
            {
                if (!_uploader.ValidateImageSingle(vm.PictureToUpload, ref uploaderErrorMessage))
                    ModelState.AddModelError("", uploaderErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                if (vm.Id == 0)
                {
                    // prepare breadcrumb navigation data
                    var cat = _categoryRepository.GetCategoryById(vm.CategoryId);
                    var dep = _departmentRepository.GetDepartmentById(cat.DepartmentId);

                    _breadcrumbNavBuilder.CreateForNode("CPanelSubcategoryAdd",
                        new
                        {
                            categoryId = cat.Id,
                            categoryName = cat.Name,
                            departmentId = dep.Id,
                            departmentName = dep.Name
                        }
                        , this);

                    return View("Edit", new SubcategoryViewModel { CategoryId = vm.CategoryId });
                }
                else
                {
                    PrepareNavData(vm.Id, "CPanelSubcategoryEdit");
                    return View("Edit", FetchDataToModify(vm.CategoryId));
                }
            }

            // Create new subcategory
            if (vm.Id == 0)
            {
                subcat = new Subcategory
                {
                    Name = vm.Name,
                    Description = vm.Description,
                    CategoryId = vm.CategoryId
                };

                _subcategoryRepository.AddSubcategory(subcat);
            }

            // Edit existing subcategory
            else
            {
                subcat = _subcategoryRepository.GetSubcategoryById(vm.Id);

                subcat.Name = vm.Name;
                subcat.Description = vm.Description;
            }

            // Upload and set picture
            if (vm.PictureToUpload != null)
            {
                var imgUploaded = await _uploader.UploadSingle(vm.PictureToUpload);
                subcat.Picture = new FilePath { Path = imgUploaded };
            }

            _auditTrailService.RetrieveAndLogChanges();

            _subcategoryRepository.SaveChanges();

            return RedirectToAction("AdminView", new { id = subcat.Id });
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin")]
        [HttpPost("/ControlPanel/Subcategories/{id}/Delete")]
        public IActionResult Delete(int id)
        {
            var subcat = _subcategoryRepository.GetSubcategoryById(id);

            if (_subcategoryRepository.SubcategoryHasProducts(subcat.Id))
                return BadRequest("Action blocked: subcategory contains some products.");

            _subcategoryRepository.RemoveSubcategory(subcat);

            _auditTrailService.RetrieveAndLogChanges();

            _subcategoryRepository.SaveChanges();

            return Ok(new { redirectUrl = Url.Action("AdminView", "Category", new { id = subcat.CategoryId}) });
        }

        private SubcategoryViewModel FetchDataToModify(int id)
        {
            var subcategory = _subcategoryRepository.GetSubcategoryInclById(id);

            return new SubcategoryViewModel
            {
                Id = subcategory.Id,
                CategoryId = subcategory.CategoryId,
                Name = subcategory.Name,
                Description = subcategory.Description,
                Picture = subcategory.Picture
            };
        }
    }
}
