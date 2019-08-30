using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace onlineShop.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";
        public static string ChangePassword => "ChangePassword";
        public static string ChangeDeliveryAddress => "ChangeDeliveryAddress";
        public static List<string> OrderList => new List<string> { "OrderDetails", "OrderList" };


        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        public static string ChangeDeliveryAddressNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangeDeliveryAddress);
        public static string OrderListNavClass(ViewContext viewContext) => PageNavClassExt(viewContext, OrderList);


        public static string PageNavClassExt(ViewContext viewContext, List<string> pages)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return pages.Contains(activePage) ? "active" : null;
        }

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}