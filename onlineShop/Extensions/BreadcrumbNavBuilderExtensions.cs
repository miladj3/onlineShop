using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using onlineShop.Extensions.BreadcrumbNavBuilderData;
using System;
using System.Collections.Generic;

namespace onlineShop.Extensions
{
    public static class BreadcrumbNavBuilderExtensions
    {
        public static IHtmlContent BreadcrumbNavRender(this IHtmlHelper helper)
        {
            string markup = "<ol class='breadcrumb'>";

            try
            {
                var deserializedModel = JsonConvert.DeserializeObject<List<BreadcrumbItem>>((string)helper.TempData["BNBData"]);

                foreach (var item in deserializedModel)
                {
                    var displayStyle = item.IsActive ? "active" : "";
                    markup += $"<li class='breadcrumb-item {displayStyle}'><a href='{item.ItemUrl}'>{item.ItemDisplayName}</a></li>";
                }

                markup += "</ol>";

                return new HtmlString(markup);
            }
            catch (Exception ex)
            {
                return new HtmlString("");
            }
        }
    }
}
