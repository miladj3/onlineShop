using System.Linq;
using onlineShop.Models;

namespace onlineShop.Repositories
{
    public interface IBlogRepository
    {
        void AddBlog(Blog blog);
        IQueryable<Blog> FetchAllBlogs();
        Blog GetBlogById(int id);
        void RemoveBlog(Blog blog);
        int SaveChanges();
    }
}