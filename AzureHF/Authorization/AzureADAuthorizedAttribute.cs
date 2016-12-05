using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AzureHF
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AzureADAuthorizedAttribute : AuthorizeAttribute
    {
        public string Role { get; set; }

        public AzureADAuthorizedAttribute()
        {
            Roles += Role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            Claim roleClaim = ClaimsPrincipal.Current.Claims.FirstOrDefault(
                c => c.Type == "roles" &&
                c.Value.Equals(Role, StringComparison.CurrentCultureIgnoreCase));

            if (roleClaim != null)
                return true;

            return false;

        }

    }
}