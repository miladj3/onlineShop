using onlineShop.Extensions;
using onlineShop.Models;

namespace onlineShop.ViewModels
{
    public class SearchResultsViewModel
    {
        public ProductQuery ProductQuery { get; set; }
        public ProductListDisplayMode DisplayMode { get; set; }

        // optional, for subcategory view only
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }

        // price slider range
        public double priceLimitMin { get; set; }
        public double priceLimitMax { get; set; }

        public PaginatedList<Product> ProductList { get; set; }

        public static object GenerateNavLinksWithFilters(int pages, string subCatId, ProductQuery productQuery)
        {
            string MaxPrice = null;
            string MinPrice = null;
            string OnlyAvailable = null;
            string OrderBy;
            string Asc;
            string SearchString = null;

            if (productQuery.MaxPrice.HasValue)
                MaxPrice = productQuery.MaxPrice.ToString();

            if (productQuery.MinPrice.HasValue)
                MinPrice = productQuery.MinPrice.ToString();

            if (productQuery.OnlyAvailable)
                OnlyAvailable = productQuery.OnlyAvailable.ToString();

            OrderBy = productQuery.OrderBy.ToString();
            Asc = productQuery.OrderDirection.ToString();

            if (!string.IsNullOrEmpty(productQuery.SearchString))
                SearchString = productQuery.SearchString.ToString();

            return new
            {
                id = subCatId,
                pages = pages.ToString(),
                maxPrice = (string)MaxPrice,
                minPrice = (string)MinPrice,
                onlyAvailable = (string)OnlyAvailable,
                orderBy = (string)OrderBy,
                orderDirection = (string)Asc,
                searchString = (string)SearchString
            };
        }
    }
}
