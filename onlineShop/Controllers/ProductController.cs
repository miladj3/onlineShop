using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Extensions;
using onlineShop.Filters;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileUploader _uploader;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly IEmailSender _emailSender;
        private readonly IProductRepository _productRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAuditTrailService _auditTrailService;

        public ProductController(
            UserManager<AppUser> userManager,
            IFileUploader uploader,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            IEmailSender emailSender,
            ISubcategoryRepository subcategoryRepository,
            ICategoryRepository categoryRepository,
            IDepartmentRepository departmentRepository,
            IProductRepository productRepository,
            IAuditTrailService auditTrailService)
        {
            _userManager = userManager;
            _uploader = uploader;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _emailSender = emailSender;
            _subcategoryRepository = subcategoryRepository;
            _categoryRepository = categoryRepository;
            _departmentRepository = departmentRepository;
            _productRepository = productRepository;
            _auditTrailService = auditTrailService;
        }

        [HttpGet("/Catalog/Product/{id}/")]
        public IActionResult Display(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = _productRepository.GetProductInclById(id);

            // map data to viewmodel
            var vm = new ProductViewModel();
            Object2ObjectMappings.ProductToProductViewModel(product, vm);

            vm.NoUserAccount = String.IsNullOrEmpty(userId);
            vm.IsRatedByUser = _productRepository.ProductIsRatedByUserId(product.Id, userId);

            // check if availability is watched by existing user
            if (!product.IsAvailable && !vm.NoUserAccount)
                vm.IsWatchedByUser = _productRepository.ProductIsWatchedByUserId(product.Id, userId); 

            PrepareNavData(id, "ProductDisplay");

            return View(vm);
        }

        [HttpGet("/Catalog/Product/{id}/Comments/{pages}")]
        public IActionResult GetComments(int id, int pages)
        {
            var comments = _productRepository.GetProductCommentsByProductId(id);

            return ViewComponent("ProductComments", new { allComments = comments, pages = pages });
        }

        [HttpGet("/Catalog/Product/{id}/RatingSummary/")]
        public IActionResult GetRatingSummary(int id)
        {
            var comments = _productRepository.GetProductCommentsByProductId(id);

            var vm = new ProductViewModel()
            {
                Comments = comments
            };

            return PartialView("_RatingSummary", vm);
        }

        // ------------------------------------- ADMIN PANEL AREA: PRODUCT -------------------------------------------

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Products/{id}")]
        public IActionResult AdminView(int id)
        {
            var product = _productRepository.GetProductInclById(id);

            // map data to viewmodel
            var vm = new ProductViewModel();
            Object2ObjectMappings.ProductToProductViewModel(product, vm);

            PrepareNavData(id, "CPanelProductView");

            return View("AdminView", vm);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Products/Create")]
        public IActionResult Create(int subcategoryId)
        {
            return View("Edit", FetchDataToCreate(subcategoryId));
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Products/{id}/Edit")]
        public IActionResult Edit(int id)
        {
            PrepareNavData(id, "CPanelProductEdit");
            return View("Edit", FetchDataToModify(id));
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost("/ControlPanel/Products/Save/")]
        public async Task<IActionResult> Save([FromForm] ProductViewModel vm)
        {
            Product product;
            string uploaderErrorMessage = "";
            bool isAvailableAgain = false;

            // Validate all inputs
            if (!_uploader.ValidateImageMulti(vm.PicturesToUpload, ref uploaderErrorMessage))
            {
                ModelState.AddModelError("", uploaderErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                if (vm.Id == 0)
                {
                    return View("Edit", FetchDataToCreate(vm.SubcategoryId));
                }
                else
                {
                    return View("Edit", FetchDataToModify(vm.Id));
                }
            }

            var admin = await _userManager.GetUserAsync(User);

            // New product
            if (vm.Id == 0)
            {
                product = new Product() { SubcategoryId = vm.SubcategoryId };

                // Copy main prop values
                Object2ObjectMappings.ProductViewModelToProduct(vm, product);

                product.AddedOn = DateTime.UtcNow;
                product.AddedBy = admin.Email;

                _productRepository.AddProduct(product);
            }
            // Edit existing product
            else
            {
                product = _productRepository.GetProductInclById(vm.Id);

                // check if product wasnt avaiable & not is available again
                if (!product.IsAvailable && vm.IsAvailable)
                    isAvailableAgain = true;

                // Copy main prop values
                Object2ObjectMappings.ProductViewModelToProduct(vm, product);

                // Log modifications
                var changeLogs = _auditTrailService.RetrieveAndLogChanges();

                foreach (var changeLog in changeLogs)
                    product.ChangeHistory.Add(new ProductChangeLog { ChangeLog = changeLog, Product = product });

                //_productRepository.ArchieveChangeLogs(product, admin);

                product.LastModifiedOn = DateTime.UtcNow;
                product.LastModifiedById = admin.Id;
            }

            // Reset existing pictures if user requested
            if (vm.RemoveExistingImages)
            {
                product.Pictures.Clear();
            }

            // Add images
            var imgUploaded = await _uploader.UploadMulti(vm.PicturesToUpload);
            product.Pictures.AddRange(imgUploaded.Select(pic => new FilePath { Path = pic }).ToList());

            if (_productRepository.SaveChanges() > 0 && isAvailableAgain)
                await NotifyAllUsers(product);

            return RedirectToAction("AdminView", new { id = product.Id });
        }

        private async Task NotifyAllUsers(Product product)
        {
            var notifications = _productRepository.GetProductNotifications(product.Id);

            foreach (var notification in notifications)
            {
                string recipientAddress;

                if (notification.Customer == null)
                {
                    recipientAddress = notification.Email;
                }
                else
                {
                    recipientAddress = notification.Customer.Email;
                }

                var productLink = Url.Action("Display", "Product", new { id = product.Id }, protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(recipientAddress, "Product from your wishlist is now available!",
                     $"<a href='{productLink}'>{product.Name}</a> is now available!");
            }

            _productRepository.RemoveProductNotifications(notifications);
            _productRepository.SaveChanges();
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin")]
        [HttpPost("/ControlPanel/Products/{id}/Delete")]
        public IActionResult Delete(int id)
        {
            var prod = _productRepository.GetProductWithItemsById(id);
            _productRepository.RemoveProduct(prod);

            _auditTrailService.RetrieveAndLogChanges();
            _productRepository.SaveChanges();

            return Ok( new { redirectUrl = Url.Action("AdminView", "Subcategory", new { id = prod.SubcategoryId }) });
        }

        private ProductViewModel FetchDataToCreate(int subcategoryId)
        {
            var vm = new ProductViewModel { SubcategoryId = subcategoryId, IsActive = true };

            // Fill all description fields for particular category
            var requiredFields = _subcategoryRepository.GetProductDescFieldsBySubcat(subcategoryId);

            foreach (var reqField in requiredFields)
            {
                vm.ProductDescriptionItems.Add(new ProductDescriptionItem { Field = reqField, FieldId = reqField.Id });
            }

            // prepare breadcrumb navigation data
            var subcat = _subcategoryRepository.GetSubcategoryById(subcategoryId);
            var cat = _categoryRepository.GetCategoryById(subcat.CategoryId);
            var dep = _departmentRepository.GetDepartmentById(cat.DepartmentId);

            _breadcrumbNavBuilder.CreateForNode("CPanelProductAdd",
                new
                {
                    subcategoryId = subcat.Id,
                    subcategoryName = subcat.Name,
                    categoryId = cat.Id,
                    categoryName = cat.Name,
                    departmentId = dep.Id,
                    departmentName = dep.Name
                }
                , this);

            return vm;
        }

        private ProductViewModel FetchDataToModify(int id)
        {
            var product = _productRepository.GetProductInclById(id);

            // Find description fields not initialized yet (for example, some fields for category A have been set up AFTER product X within category A has been created)
            var populatedFields = _productRepository.GetProductDescItemsByProductId(product.Id);
            var requiredFields = _subcategoryRepository.GetProductDescFieldsBySubcat(product.SubcategoryId);
            var unpopulatedFields = requiredFields.Where(f => !populatedFields.Select(p => p.Field.Id).Contains(f.Id)).ToList();

            foreach (var field in unpopulatedFields)
            {
                product.ProductDescriptionItems.Add(new ProductDescriptionItem { Field = field });
            }

            // Persist changes for easier change tracking via existing extension
            if (unpopulatedFields.Count > 0)
                _productRepository.SaveChanges();

            var vm = new ProductViewModel();

            Object2ObjectMappings.ProductToProductViewModel(product, vm);

            PrepareNavData(id, "CPanelProductEdit");

            return vm;
        }

        private void PrepareNavData(int productId, string nodeName)
        {
            var prod = _productRepository.GetProductWithItemsById(productId);
            var subcat = _subcategoryRepository.GetSubcategoryById(prod.SubcategoryId);
            var cat = _categoryRepository.GetCategoryById(subcat.CategoryId);
            var dep = _departmentRepository.GetDepartmentById(cat.DepartmentId);

            _breadcrumbNavBuilder.CreateForNode(nodeName,
                new
                {
                    productId = prod.Id,
                    productName = prod.Name.SetLengthLimit(50),
                    subcategoryId = subcat.Id,
                    subcategoryName = subcat.Name,
                    categoryId = cat.Id,
                    categoryName = cat.Name,
                    departmentId = dep.Id,
                    departmentName = dep.Name
                }
                , this);
        }
    }
}
