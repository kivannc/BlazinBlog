using BlazinBlog.Data;
using BlazinBlog.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazinBlog.Services;

internal static class AdminAccount
{
    public const string Name = "Kivanc";
    public const string Email = "erturkkivanc@gmail.com";
    public const string Role = "Admin";
    public const string Password = "P@ssw0rd!";
}
public interface ISeedService
{
    Task SeedDataAsync();
}

public class SeedService : ISeedService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedService(
        ApplicationDbContext context,
        IUserStore<ApplicationUser> userStore,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userStore = userStore;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    private async Task MigrateDatabaseAsync()
    {
#if DEBUG
        if ((await _context.Database.GetPendingMigrationsAsync()).Any())
        {
            await _context.Database.MigrateAsync();
        }
#endif
    }
    public async Task SeedDataAsync()
    {
        await MigrateDatabaseAsync();
        //Seed AdminRole
        if (await _roleManager.FindByNameAsync(AdminAccount.Role) is null)
        {
            // Create an Admin Role.
            var adminRole = new IdentityRole(AdminAccount.Role);
            var result = await _roleManager.CreateAsync(adminRole);
            if (!result.Succeeded)
            {
                var errorString = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                throw new Exception(errorString);
            }
        }

        //Seed AdminUser
        var adminUser = await _userManager.FindByEmailAsync(AdminAccount.Email);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                Name = AdminAccount.Name,
                UserName = AdminAccount.Email,
                Email = AdminAccount.Email
            };
            var result = await _userManager.CreateAsync(adminUser, AdminAccount.Password);
            if (!result.Succeeded)
            {
                var errorString = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                throw new Exception(errorString);
            }
            result = await _userManager.AddToRoleAsync(adminUser, AdminAccount.Role);
            if (!result.Succeeded)
            {
                var errorString = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                throw new Exception(errorString);
            }
        }


        //Seed Categories

        if (!await _context.Categories.AsNoTracking().AnyAsync())
        {
            //No categories in database create some categories
            await _context.Categories.AddRangeAsync(Category.GetSeedCategories());
            await _context.SaveChangesAsync();
        }
    }
}