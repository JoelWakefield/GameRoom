using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Data
{
    public static class AuthHelper
    {
        public static string GetOid(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            var identity = (ClaimsIdentity)user.Identity;

            if (identity.Claims.Count() > 0)
                return identity.Claims
                    .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    .Value;
            else
                return null;
        }
    }
}
