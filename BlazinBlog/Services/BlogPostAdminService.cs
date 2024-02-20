using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazinBlog.Data;
using BlazinBlog.Data.Entities;
using BlazinBlog.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlazinBlog.Services;

public interface IBlogPostAdminService
{
    Task<PagedResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize);
    Task<BlogPost> GetBlogPostByIdAsync(int id);
    Task<BlogPost> SaveBlogPost(BlogPost blogPost, string userId);
}
public class BlogPostAdminService : IBlogPostAdminService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public BlogPostAdminService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        using var context = _contextFactory.CreateDbContext();
        return await query.Invoke(context);
    }
    public Task<BlogPost> GetBlogPostByIdAsync(int id)
    {
        return ExecuteOnContext(context =>
            context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(c => c.Id == id));
    }

    public Task<PagedResult<BlogPost>> GetBlogPostsAsync(int startIndex, int pageSize)
    {
        return ExecuteOnContext(async context =>
        {
            var total = await context.BlogPosts.CountAsync();
            var blogPosts = await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Skip(startIndex)
                .Take(pageSize)
                .ToArrayAsync();
            return new PagedResult<BlogPost>(blogPosts, total);
        });
    }

    private async Task<string> GenerateSlugAsync(BlogPost blogPost)
    {
        return await ExecuteOnContext(async context =>
        {
            var originalSlug = blogPost.Title.ToSlug();
            string slug = originalSlug;
            int count = 1;
            while (await context.BlogPosts
                .AsNoTracking()
                .AnyAsync(c => c.Slug == slug))
            {
                slug = $"{originalSlug}-{count}";
                count++;
            }
            return slug;
        });
    }

    public Task<BlogPost> SaveBlogPost(BlogPost blogPost, string userId)
    {
        return ExecuteOnContext(async context =>
        {
            if (blogPost.Id != 0)
            {
                var dbBlogPost = await context.BlogPosts
                                            .AsNoTracking()
                                            .AnyAsync(c => c.Title == blogPost.Title && c.Id != blogPost.Id);

                if (dbBlogPost) throw new InvalidOperationException("Blog post with same title already exists.");

                var dbBlog = await context.BlogPosts.FindAsync(blogPost.Id);
                dbBlog.Title = blogPost.Title;
                dbBlog.Image = blogPost.Image;
                dbBlog.Introduction = blogPost.Introduction;
                dbBlog.Content = blogPost.Content;
                dbBlog.CategoryId = blogPost.CategoryId;
                dbBlog.IsPublished = blogPost.IsPublished;
                dbBlog.IsFeatured = blogPost.IsFeatured;

                if (dbBlog.IsPublished && dbBlog.PublishedAt is null)
                {
                    dbBlog.PublishedAt = DateTime.Now;
                }
                else if (!dbBlog.IsPublished)
                {
                    dbBlog.PublishedAt = null;
                }
            }
            else
            {
                var dbBlogPost = await context.BlogPosts
                                            .AsNoTracking()
                                            .AnyAsync(c => c.Title == blogPost.Title);

                if (dbBlogPost) throw new InvalidOperationException("Blog post with same title already exists.");

                blogPost.Slug = await GenerateSlugAsync(blogPost);
                blogPost.UserId = userId;
                blogPost.CreatedAt = DateTime.UtcNow;
                if (blogPost.IsPublished)
                {
                    blogPost.PublishedAt = DateTime.UtcNow;
                }
                await context.BlogPosts.AddAsync(blogPost);
            }
            await context.SaveChangesAsync();
            return blogPost;
        });
    }
}
