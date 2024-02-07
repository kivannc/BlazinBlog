using System.Security.Claims;

namespace BlazinBlog
{
    public static class Extensions
    {
        public static string GetUserName(this ClaimsPrincipal principal) => principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

        public static string GetUserId(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

    }
}
