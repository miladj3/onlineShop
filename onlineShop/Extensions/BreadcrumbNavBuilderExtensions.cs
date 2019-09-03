using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using onlineShop.Extensions.BreadcrumbNavBuilderData;
using System;
using System.Collections.Generic;
using System.Text;

namespace onlineShop.Extensions
{
    public static class BreadcrumbNavBuilderExtensions
    {
        public static IHtmlContent BreadcrumbNavRender(this IHtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.Append("<ol class='breadcrumb'>");

            try
            {
                var deserializedModel = JsonConvert.DeserializeObject<List<BreadcrumbItem>>((string)helper.TempData["BNBData"]);

                foreach (var item in deserializedModel)
                {
                    var displayStyle = item.IsActive ? "active" : "";
                    sb.Append($"<li class='breadcrumb-item {displayStyle}'><a href='{item.ItemUrl}'>{item.ItemDisplayName}</a></li>");
                }

                sb.Append("</ol>");

                return new HtmlString(sb.ToString());
            }
            catch (Exception ex)
            {
                return new HtmlString("");
            }
        }
    }
}
