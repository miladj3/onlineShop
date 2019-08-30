using Microsoft.AspNetCore.Mvc;
using onlineShop.Extensions;
using onlineShop.Models;
using System.Collections.Generic;

namespace onlineShop.ViewComponents
{
    public class ProductCommentsViewComponent : ViewComponent
    {
        public const int productCommentsPerPage = 5;

        public ProductCommentsViewComponent() 
        {
        }

        public IViewComponentResult Invoke(List<ProductComment> allComments, int pages)
        {
            var commentsFiletered = PaginatedList<ProductComment>.CreateFromList(allComments, pages, productCommentsPerPage);
            ViewBag.CommentsTotal = allComments.Count;
            return View("CommentsPaginated", commentsFiletered);
        }
    }
}
