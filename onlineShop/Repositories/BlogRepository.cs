using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System.Linq;

namespace onlineShop.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Blog GetBlogById(int id)
        {
            return _context.Blogs.Include(b => b.LastModifiedBy).FirstOrDefault(b => b.Id == id);
        }

        public IQueryable<Blog> FetchAllBlogs()
        {
            return _context.Blogs.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .OrderByDescending(x => x.IsPinned);
        }

        public void AddBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
        }

        public void RemoveBlog(Blog blog)
        {
            _context.Blogs.Remove(blog);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
