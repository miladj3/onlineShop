using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace onlineShop.Extensions
{
    // breadcrumb navigation container
    public class BreadcrumbNavItem
    {
        public string ItemDisplayName { get; set; }
        public string ItemUrl { get; set; }
        public bool IsActive { get; set; }
    }

    // html extensions to create breadcrumbs from object passed with tempdata
    public static class BreadcrumbNavBuilderExtensions
    {
        public static IHtmlContent BreadcrumbNavRender(this IHtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.Append("<ol class='breadcrumb'>");

            try
            {
                // deserialize collection of breadcrumb items
                var deserializedModel = JsonConvert.DeserializeObject<List<BreadcrumbNavItem>>((string)helper.TempData["BNBData"]);

                // append to list
                foreach (var item in deserializedModel)
                {
                    var displayStyle = item.IsActive ? "active" : "";
                    sb.Append($"<li class='breadcrumb-item {displayStyle}'><a href='{item.ItemUrl}'>{item.ItemDisplayName}</a></li>");
                }

                sb.Append("</ol>");

                return new HtmlString(sb.ToString());
            }
            catch
            {
                return new HtmlString("");
            }
        }
    }
}
