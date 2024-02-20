namespace BlazinBlog.Models;

public record PagedResult<TResult>(TResult[] Records, int TotalCount);
