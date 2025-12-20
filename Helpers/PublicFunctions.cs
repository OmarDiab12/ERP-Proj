using System.Security.Claims;

namespace ERP.Helpers
{
    public static class PublicFunctions
    {

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var val = user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? user.FindFirstValue("UserId")
                      ?? user.FindFirstValue("sub");
            return int.TryParse(val, out var id) ? id : (int?)null;
        }
    }
}
