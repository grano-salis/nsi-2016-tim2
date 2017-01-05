using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AngularJSAuthentication.API.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }

    public static class Role
    {
        public const string Administrator = "Administrator";
        public const string Profesor = "Profesor";
        public const string Studentska = "Studentska";
    }



}