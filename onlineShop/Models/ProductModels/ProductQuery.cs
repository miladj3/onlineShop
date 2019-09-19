using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace onlineShop.Models
{
    public enum OrderBy
    {
        Date = 0,
        Price = 1,
        Name = 2
    }

    public enum OrderDirection
    {
        Desc = 0,
        Asc = 1
    }

    public class ProductQuery
    {
        [Display(Name = "Min. Price")]
        public double? MinPrice { get; set; }
        [Display(Name = "Max. Price")]
        public double? MaxPrice { get; set; }
        [Display(Name = "Only Available")]
        public bool OnlyAvailable { get; set; }
        [Display(Name = "Name Contains")]
        public string SearchString { get; set; }
        [Display(Name = "Order By")]
        public OrderBy OrderBy { get; set; }
        [Display(Name = "Order Direction")]
        public OrderDirection OrderDirection { get; set; }

        public string GetCriteria
        {
            get
            {
                switch (this.OrderBy)
                {
                    case OrderBy.Price:
                        return "SalePrice";

                    case OrderBy.Name:
                        return "Name";

                    case OrderBy.Date:
                        return "AddedOn";

                    default:
                        return "";
                }
            }
        }

        public IEnumerable<Expression<Func<Product, bool>>> Filters
        {
            get
            {
                var filters = new List<Expression<Func<Product, bool>>>();

                if (this.MinPrice.HasValue)
                    filters.Add(p => p.SalePrice >= this.MinPrice);

                if (this.MaxPrice.HasValue)
                    filters.Add(p => p.SalePrice <= this.MaxPrice);

                if (this.OnlyAvailable)
                    filters.Add(p => p.IsAvailable);

                if (!String.IsNullOrEmpty(this.SearchString))
                    filters.Add(p => p.Name.IndexOf(this.SearchString, StringComparison.OrdinalIgnoreCase) != -1);

                return filters;
            }
        }
    }
}
