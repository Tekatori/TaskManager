using System.Security.Claims;

namespace TaskManager.Models
{
    public static class CookieHelper
    {
        public static void SetCookie(HttpContext context, string key, string value, int? expireDays = null)
        {
            CookieOptions option = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            if (expireDays.HasValue)
                option.Expires = DateTimeOffset.Now.AddDays(expireDays.Value);
            else
                option.Expires = DateTimeOffset.Now.AddDays(1);

            context.Response.Cookies.Append(key, value, option);
        }

        public static string? GetCookie(HttpContext context, string key)
        {
            context.Request.Cookies.TryGetValue(key, out string? value);
            return value;
        }

        public static void RemoveCookie(HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }

   
        public static UserCookie? GetLoggedUser(ClaimsPrincipal user)
        {
            if (user.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            return new UserCookie
            {
                Id = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null,
                UserName = user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty
            };
        }
    }
    public class UserCookie
    {
        public string UserName { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
