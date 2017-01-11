using AngularJSAuthentication.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularJSAuthentication.API.Providers
{
    public class AuthProvider
    {
        public UserInfo getAuth(string token)
        {
            try
            {
                SSO.IdentityClient client = new SSO.IdentityClient();
                SSO.AuthResponse ui = client.Auth(token);

                return new UserInfo
                {
                    Email = ui.Email,
                    FirstName = ui.FirstName,
                    LastName = ui.LastName,
                    Roles = ui.Roles.ToList(),
                    UserId = ui.UserId,
                    Username = ui.Username
                };
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException("Authorization failed.", e);
            }
        }

    }
}