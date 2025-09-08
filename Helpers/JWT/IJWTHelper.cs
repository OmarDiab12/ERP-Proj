using ERP.Models;
using System.Security.Claims;

namespace ERP.Helpers.JWT
{
    public interface IJWTHelper
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        int? GetUserIdFromClaims(ClaimsPrincipal user);
    }
}
