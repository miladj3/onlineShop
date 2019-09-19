using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Extensions;
using onlineShop.Filters;
using onlineShop.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public partial class BlogController : Controller
    {
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Blogs/Create")]
        public IActionResult Create()
        {
            var blog = new Blog { PreviewSize = 500, DisplayDate = true, IsPinned = false };

            _breadcrumbNavBuilder.CreateForNode("CPanelBlogAdd", new { }, this);
            return View("Edit", blog);
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Save(Blog blogModel)
        {
            // validate model
            if (!ModelState.IsValid)
            {
                if (blogModel.Id == 0)
                {
                    _breadcrumbNavBuilder.CreateForNode("CPanelBlogAdd", new { }, this);
                    return View("Edit", blogModel);
                }
                else
                {
                    var blogInDb = _blogRepository.GetBlogById(blogModel.Id);
                    _breadcrumbNavBuilder.CreateForNode("CPanelBlogEdit", new { blogId = blogModel.Id, blogName = blogInDb.Title.SetLengthLimit(40) }, this);
                    return View("Edit", blogModel);
                }
            }

            Blog blog;
            var admin = await _userManager.GetUserAsync(User);
            var sanitizer = new HtmlSanitizer();

            if (blogModel.Id == 0) // create new blog
            {
                blog = new Blog
                {
                    AddedBy = admin.UserName,
                    AddedOn = DateTime.UtcNow,
                    Markup = WebUtility.HtmlDecode(sanitizer.Sanitize(blogModel.Markup)),
                    Title = blogModel.Title,
                    IsPublished = blogModel.IsPublished,
                    PreviewSize = blogModel.PreviewSize,
                    DisplayDate = blogModel.DisplayDate,
                    IsPinned = blogModel.IsPinned
                };

                _blogRepository.AddBlog(blog);
            }
            else // amend existing blog
            {
                blog = _blogRepository.GetBlogById(blogModel.Id);

                blog.Markup = WebUtility.HtmlDecode(sanitizer.Sanitize(blogModel.Markup));
                blog.LastModifiedById = admin.Id;
                blog.LastModifiedOn = DateTime.UtcNow;
                blog.IsPublished = blogModel.IsPublished;
                blog.Title = blogModel.Title;
                blog.PreviewSize = blogModel.PreviewSize;

                blog.DisplayDate = blogModel.DisplayDate;
                blog.IsPinned = blogModel.IsPinned;
            }

            _blogRepository.SaveChanges();

            return RedirectToAction("AdminView", new { id = blog.Id });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Blogs/")]
        public IActionResult ManageBlogs()
        {
            var blogs = _blogRepository.FetchAllBlogs().ToList();

            _breadcrumbNavBuilder.CreateForNode("CPanelBlogsView", new { }, this);
            return View(blogs);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Blogs/{id}/Edit/")]
        public IActionResult Edit(int id)
        {
            var blog = _blogRepository.GetBlogById(id);

            _breadcrumbNavBuilder.CreateForNode("CPanelBlogEdit", new { blogId = blog.Id, blogName = blog.Title.SetLengthLimit(40) }, this);
            return View("Edit", blog);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Blogs/{id}/View/")]
        public IActionResult AdminView(int id)
        {
            var blog = _blogRepository.GetBlogById(id);

            _breadcrumbNavBuilder.CreateForNode("CPanelBlogView", new { blogId = blog.Id, blogName = blog.Title.SetLengthLimit(40) }, this);
            return View("AdminView", blog);
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost("/ControlPanel/Blogs/{id}/Delete/")]
        public IActionResult Delete(int id)
        {
            var blog = _blogRepository.GetBlogById(id);

            _blogRepository.RemoveBlog(blog);
            _blogRepository.SaveChanges();

            return Ok(new { redirectUrl = Url.Action("ManageBlogs", "Blog") });
        }
    }
}
