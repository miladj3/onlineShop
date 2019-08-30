using onlineShop.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace onlineShop.Extensions
{
    public static class OrderDynamic
    {
        // Source: https://stackoverflow.com/questions/7265186/how-do-i-specify-the-linq-orderby-argument-dynamically/7265354#7265354

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> q, string SortField, OrderDirection orderDirection)
        {
            if (String.IsNullOrEmpty(SortField))
                return q;

            bool Ascending = true;

            if (!(orderDirection == OrderDirection.Asc))
                Ascending = false;

            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = (bool) Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
    }
}
