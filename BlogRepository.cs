using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DBRepository
{
    public interface IBlogRepository
    {
        public Task<Page<Post>> GetPosts(int index, int pageSize, string tag = null);
    }
    public class BlogRepository:BaseRepository, IBlogRepository
    {
        public BlogRepository(string connectionString, IRepositoryContextFactory contextFactory):base(connectionString, contextFactory)
        {
        }
        public async Task<Page<Post>> GetPosts(int index, int pageSize, string tag = null)
        {
            var result = new Page<Post>()
            {
                CurrentPage = index,
                PageSize = pageSize,
            };
            using (var context = ContextFactory.CreateDbContext(ConnectionString))
            {
                var query = context.Posts.AsQueryable();
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    query = query.Where(p => p.Tags.Any(t => t.TagName == tag));
                }
                result.TotalPages = await query.CountAsync();
                query = query.Include(p => p.Tags).Include(p => p.Comments).OrderByDescending(p => p.CreatedDate).Skip(index * pageSize).Take(pageSize);
                result.Records = await query.ToListAsync();
            }
            return result;
        }

    }
}
