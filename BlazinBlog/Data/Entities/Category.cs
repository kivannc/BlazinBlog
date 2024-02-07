using System.ComponentModel.DataAnnotations;

namespace BlazinBlog.Data.Entities;

public class Category
{
    public short Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(75)]
    public string Slug { get; set; }
    public bool ShowOnNavbar { get; set; }

    public static Category[] GetSeedCategories()
    {
        return [
            new Category { Name = "C#", Slug = "csharp", ShowOnNavbar = true },
            new Category { Name = "ASP.NET Core", Slug = "aspnet-core", ShowOnNavbar = true },
            new Category { Name = "Blazor", Slug = "blazor", ShowOnNavbar = true },
            new Category { Name = "Azure", Slug = "azure", ShowOnNavbar = false },
            new Category { Name = "Visual Studio", Slug = "visual-studio", ShowOnNavbar = false },
            new Category { Name = "Entity Framework", Slug = "entity-framework", ShowOnNavbar = true },
            new Category { Name = "SQL Server", Slug = "sql-server", ShowOnNavbar = false },
            new Category { Name = "Web Development", Slug = "web-development", ShowOnNavbar = true },
            new Category { Name = "JavaScript", Slug = "javascript", ShowOnNavbar = true },
            new Category { Name = "CSS", Slug = "css", ShowOnNavbar = false },
            new Category { Name = "HTML", Slug = "html", ShowOnNavbar = false },
            new Category { Name = "Bootstrap", Slug = "bootstrap", ShowOnNavbar = false },
            new Category { Name = "jQuery", Slug = "jquery", ShowOnNavbar = false },
        ];
    }

}
