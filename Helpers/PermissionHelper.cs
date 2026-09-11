using System.Security.Claims;

namespace Mhrm.Helpers
{
    public static class PermissionHelper
    {
        public static bool HasPermission(ClaimsPrincipal user, string permission)
        {
            return user.Claims.Any(c =>
                c.Type == "Permission" &&
                c.Value == permission);
        }
    }
}