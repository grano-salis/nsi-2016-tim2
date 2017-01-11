using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SSO.WCFService.DataContracts;
using System.Data.Entity;
using SSO.WCFService.Helpers;

namespace SSO.WCFService.BusinessLogic
{
    public class IdentityServiceImplementation
    {
        private SSOContext _db;
        private AuthProvider _authProvider;
        public IdentityServiceImplementation(SSOContext db)
        {
            _db = db;
            _authProvider = new AuthProvider(_db);
        }
        public AuthResponse Auth(string token)
        {
            var user = _authProvider.AuthenticateByToken(token);
            if (user == null)
            {
                throw new Exception("This shouldn happen.");
            }

            AuthResponse response = new AuthResponse()
            {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.Info.FirstName,
                LastName = user.Info.LastName,
                Email = user.Info.Email,
                Roles = user.Roles.ToList()
            };

            return response;
        }

        public void Logout(string token)
        {
            var claim = _db.Claims
                .SingleOrDefault(c => c.Valid.Equals("1") && c.Token.Equals(token));
            if (claim != null)
            {
                claim.Valid = "0";
                _db.SaveChanges();
            }
        }
    }
}