using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BlazinBlog
{
    public static partial class Extensions
    {
        public static string GetUserName(this ClaimsPrincipal principal) => principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

        public static string GetUserId(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public static string ToSlug(this string text) {
            text = SlugRegex().Replace(text.ToLowerInvariant(), "-");

            return text
                .Replace("--", "-").Trim('-');
    }

        [GeneratedRegex(@"[^a-z0-9\-]")]
        private static partial Regex SlugRegex();
    }
}
