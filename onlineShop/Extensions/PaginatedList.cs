using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Extensions
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get => this.PageIndex > 1; }
        public bool HasNextPage { get => this.PageIndex < this.TotalPages; }
        public bool IsFirstPage { get => this.PageIndex == 1; }
        public bool IsLastPage { get => this.PageIndex == this.TotalPages; }
        public int ItemsTotal { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, int itemsTotal)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            ItemsTotal = itemsTotal;

            this.AddRange(items);
        }

        public static PaginatedList<T> CreateFromList(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize, count);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize, count);
        }
    }
}
