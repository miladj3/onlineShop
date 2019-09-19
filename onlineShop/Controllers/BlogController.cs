using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using onlineShop.Extensions;
using onlineShop.Models;
using onlineShop.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public partial class BlogController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly IBlogRepository _blogRepository;

        private readonly int blogsPerPage = 5;

        public BlogController(
            UserManager<AppUser> userManager,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            IBlogRepository blogRepository)
        {
            _userManager = userManager;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _blogRepository = blogRepository;
        }

        [HttpGet("/Blogs/{id}/")]
        public IActionResult Display(int id)
        {
            var blog = _blogRepository.GetBlogById(id);

            _breadcrumbNavBuilder.CreateForNode("BlogDisplay", new { blogId = id, blogTitle = blog.Title.SetLengthLimit(40) }, this);
            return View(blog);
        }

        [HttpGet("/Blogs/")]
        public async Task<IActionResult> Index(int? pages)
        {
            pages = (pages > 0) ? pages : 1;

            var blogs = _blogRepository.FetchAllBlogs()
                .Where(x => x.IsPublished)
                .OrderByDescending(x => x.Id)
                .OrderByDescending(x => x.IsPinned);

            var blogsPaginated = await PaginatedList<Blog>.CreateAsync(blogs, (int)pages, blogsPerPage);

            _breadcrumbNavBuilder.CreateForNode("AllBlogs", new { }, this);
            return View(blogsPaginated);
        }

    }
}
