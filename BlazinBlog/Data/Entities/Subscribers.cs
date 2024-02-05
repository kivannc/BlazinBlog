using System.ComponentModel.DataAnnotations;

namespace BlazinBlog.Data.Entities;

public class Subscribers
{
    public int Id { get; set; }
    [EmailAddress, Required,MaxLength(100)]
    public string Email { get; set; }
    [Required,MaxLength(25)]
    public string Name { get; set; }
    public DateTime SubscribedOn { get; set; }
}