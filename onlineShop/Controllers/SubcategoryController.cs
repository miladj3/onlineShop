using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Extensions;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;

namespace onlineShop.Controllers
{
    public partial class SubcategoryController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileUploader _uploader;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAuditTrailService _auditTrailService;

        private readonly int productsPerPage = 9;

        public SubcategoryController(
            UserManager<AppUser> userManager,
            IFileUploader uploader,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            ISubcategoryRepository catalogRepository,
            ICategoryRepository categoryRepository,
            IDepartmentRepository departmentRepository,
            IProductRepository productRepository,
            IAuditTrailService auditTrailService)
        {
            _userManager = userManager;
            _uploader = uploader;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _subcategoryRepository = catalogRepository;
            _categoryRepository = categoryRepository;
            _departmentRepository = departmentRepository;
            _productRepository = productRepository;
            _auditTrailService = auditTrailService;
        }

        [HttpPost("Catalog/Subcategory/{id}/Filter")]
        public IActionResult Search(int id, ProductQuery productQuery)
        {
            return RedirectToAction("Display", new
            {
                id,
                pages = 1,
                maxPrice = productQuery.MaxPrice.ToString(),
                minPrice = productQuery.MinPrice.ToString(),
                searchString = productQuery.SearchString,
                onlyAvailable = productQuery.OnlyAvailable.ToString(),
                orderBy = productQuery.OrderBy.ToString(),
                OrderDirection = productQuery.OrderDirection.ToString()
            });
        }

        [HttpPost("Catalog/Subcategory/{id}/ResetFilters")]
        public IActionResult ResetFilters(int id)
        {
            return RedirectToAction("Display", new { id = id });
        }

        [HttpGet("Catalog/Subcategory/{id}")]
        public async Task<IActionResult> Display(int id, [FromQuery]int pages, [FromQuery] ProductQuery productQuery)
        {
            double priceLimitMax = 0;
            double priceLimitMin = 0;

            pages = (pages == 0) ? 1 : pages;

            IQueryable<Product> allProducts = _productRepository.FetchProductsActiveInclBySubcatId(id);

            if (allProducts.Count() > 0)
            {
                priceLimitMax = allProducts.Max(p => p.SalePrice);
                priceLimitMin = allProducts.Min(p => p.SalePrice);
            }

            foreach (var filter in productQuery.Filters)
            {
                allProducts = allProducts.Where(filter);
            }

            var filteredProducts = allProducts.OrderByDynamic(productQuery.GetCriteria, productQuery.OrderDirection);

            // apply existing product description field ordering 
            filteredProducts.SelectMany(p => p.ProductDescriptionItems).OrderBy(pdi => pdi.Field.DisplayOrderId).ToList();

            var vm = new SearchResultsViewModel()
            {
                DisplayMode = ProductListDisplayMode.SubcategoryView,
                ProductList = await PaginatedList<Product>.CreateAsync(filteredProducts, pages, productsPerPage),
                ProductQuery = productQuery,
                SubcategoryId = id,
                SubcategoryName = _subcategoryRepository.GetSubcategoryById(id).Name,
                priceLimitMax = priceLimitMax,
                priceLimitMin = priceLimitMin
            };

            PrepareNavData(id, "SubcategoryDisplay");

            return View(vm);
        }

        private void PrepareNavData(int subcategoryId, string nodeName)
        {
            // prepare breadcrumb navigation data
            var subcat = _subcategoryRepository.GetSubcategoryById(subcategoryId);
            var cat = _categoryRepository.GetCategoryById(subcat.CategoryId);
            var dep = _departmentRepository.GetDepartmentById(cat.DepartmentId);

            _breadcrumbNavBuilder.CreateForNode(nodeName,
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
        }
    }
}
