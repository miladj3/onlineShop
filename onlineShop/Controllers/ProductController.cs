using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Extensions;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public partial class ProductController : Controller
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

        private const int commentsPerPage = 5;

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
        public async Task<IActionResult> GetComments(int id, int pages)
        {
            var publishedComments = _productRepository.FetchProductCommentsPublishedByProductId(id);
            var publishedCommentsPaginated = await PaginatedList<ProductComment>.CreateAsync(publishedComments, pages, commentsPerPage);

            return PartialView("_PublishedComments", publishedCommentsPaginated);
        }

        [HttpGet("/Catalog/Product/{id}/RatingSummary/")]
        public IActionResult GetRatingSummary(int id)
        {
            var vm = new ProductViewModel()
            {
                Comments = _productRepository.FetchProductCommentsPublishedByProductId(id).ToList()
            };

            return PartialView("_RatingSummary", vm);
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
