using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Extensions;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public partial class CatalogController : Controller
    {
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IProductRepository _productRepository;

        private readonly int productsPerPage = 9;

        public CatalogController(
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            IDepartmentRepository departmentRepository,
            IProductRepository productRepository)
        {
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _departmentRepository = departmentRepository;
            _productRepository = productRepository;
        }

        [HttpGet("/Catalog/SearchPreview/{query}/")]
        public IActionResult SearchPreview(string query)
        {
            var itemsFound = _productRepository.FetchAllProducts()
                .Where(p => p.Name.Contains(query) && p.IsActive);

            var resultList = itemsFound.Select(x => new { x.Id, Name = x.Name.SetLengthLimit(50), SubcategoryName = x.Subcategory.Name }).ToArray();

            return new JsonResult(new { resultCount = resultList.Count(), results = resultList });
        }

        [HttpPost]
        public IActionResult Search(ProductQuery productQuery)
        {
            return RedirectToAction("SearchResults", new
            {
                pages = 1,
                maxPrice = productQuery.MaxPrice.ToString(),
                minPrice = productQuery.MinPrice.ToString(),
                searchString = productQuery.SearchString,
                onlyAvailable = productQuery.OnlyAvailable.ToString(),
                orderBy = productQuery.OrderBy.ToString(),
                OrderDirection = productQuery.OrderDirection.ToString()
            });
        }

        [HttpPost]
        public IActionResult ResetFilters(ProductQuery productQuery)
        {
            return RedirectToAction("SearchResults", new
            {
                pages = 1,
                searchString = productQuery.SearchString
            });
        }

        [HttpGet("/Catalog/Search/")]
        public async Task<IActionResult> SearchResults(int pages, ProductQuery productQuery)
        {
            double priceLimitMax = 0;
            double priceLimitMin = 0;

            pages = (pages == 0) ? 1 : pages;

            IQueryable<Product> allProducts = _productRepository.FetchAllProductsIncl();

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

            var vm = new SearchResultsViewModel()
            {
                DisplayMode = ProductListDisplayMode.CatalogView,
                ProductList = await PaginatedList<Product>.CreateAsync(filteredProducts, pages, productsPerPage),
                ProductQuery = productQuery,
                priceLimitMax = priceLimitMax,
                priceLimitMin = priceLimitMin
            };

            ViewData["CurrentSearch"] = productQuery.SearchString;

            return View(vm);
        }
    }
}
