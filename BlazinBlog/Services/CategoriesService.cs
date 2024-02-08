using BlazinBlog.Data;
using BlazinBlog.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazinBlog.Services;

public interface ICategoriesService
{
    Task DeleteCategoryAsync(int id);
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryAsync(int id);
    Task<Category> GetCategoryBySlug(string slug);
    Task<Category> SaveCategoryAsync(Category category);
}

public class CategoriesService : ICategoriesService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CategoriesService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        using var context = _contextFactory.CreateDbContext();
        return await query.Invoke(context);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await ExecuteOnContext(context => context.Categories.AsNoTracking().ToListAsync());
    }

    public async Task<Category> GetCategoryAsync(int id)
    {
        var category = await ExecuteOnContext(context => context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id));
        if (category is null) throw new Exception("Category not found.");
        return category;
    }

    public async Task<Category> SaveCategoryAsync(Category category)
    {
        return await ExecuteOnContext(async context =>
        {
            if (category.Id != 0)
            {
                var dbCaregory = context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Name == category.Name && c.Id != category.Id);
                if (dbCaregory is not null) throw new InvalidOperationException("Category already exists.");
                category.Slug = category.Name.ToSlug();
                context.Categories.Update(category);

            }
            else
            {
                var dbCaregory = context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Name == category.Name);
                if (dbCaregory is not null) throw new InvalidOperationException("Category already exists.");
                category.Slug = category.Name.ToSlug();
                await context.Categories.AddAsync(category);
            }
            await context.SaveChangesAsync();
            return category;
        });
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await ExecuteOnContext(async context =>
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) throw new Exception("Category not found.");
            context.Categories.Remove(category);
            return await context.SaveChangesAsync();
        });
    }

    public async Task<Category> GetCategoryBySlug(string slug)
    {
        var category = await ExecuteOnContext(context => context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug));
        if (category is null) throw new Exception("Category not found.");
        return category;
    }
}

