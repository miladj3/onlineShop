using Extensions;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Repositories;
using onlineShop.ViewModels;
using System.Linq;

namespace onlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBlogRepository _blogRepository;

        private readonly int blogsPerPage = 5;

        public HomeController(
        IDepartmentRepository departmentRepository,
        IBlogRepository blogRepository)
        {
            _departmentRepository = departmentRepository;
            _blogRepository = blogRepository;
        }

        public IActionResult Index()
        {
            var blogs = _blogRepository.FetchAllBlogs()
                .Where(x => x.IsPublished)
                .Take(blogsPerPage)
                .ToList();

            foreach (var blog in blogs)
            {
                var originalMarkup = blog.Markup;
                blog.Markup = HtmlTruncation.TruncateHtml(originalMarkup, (blog.PreviewSize > 0) ? blog.PreviewSize : 500, "...");
            }

            var departments = _departmentRepository.GetAllDepartments();

            ViewBag.EnableMobileSidebar = true;

            return View(new MainPageViewModel { Blogs = blogs, Departments = departments });
        }
    }
}
