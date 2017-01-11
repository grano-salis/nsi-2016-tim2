using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.WCFService.Helpers
{
    public class AuthProvider
    {
        private SSOContext _db;

        public AuthProvider(SSOContext db)
        {
            _db = db;
        }
        public UserInfoExtended AuthenticateByToken(string token)
        {
            var claim = _db.Claims
                .SingleOrDefault(c => c.Valid.Equals("1") && c.Token.Equals(token));
            if (claim == null)
            {
                // or return false
                throw new Exceptions.WrongOrExpiredToken();
            }

            // Check expiration
            var timeSpan = DateTime.Now.Subtract(claim.Created);
            if (timeSpan.TotalSeconds > 60 * 60 * 24) // 24h
            {
                claim.Valid = "0";
                _db.SaveChanges();
                throw new Exceptions.WrongOrExpiredToken();
            }


            UserInfoExtended user = _db.Users.Where(u => u.ID == claim.UserID).ToList()
                                    .Select(u => new UserInfoExtended{
                                        UserId = u.ID,
                                        Username = u.Username,
                                        Info = _db.UserInfoes.FirstOrDefault(i => i.UserID == u.ID),
                                        Roles = _db.ManageRoles.Include("Role")
                                                    .Where(mr => mr.UserID == u.ID)
                                                    .Select(mr => mr.Role.Name).ToList()
                                    }).FirstOrDefault();
            
            return user;
        }
        
    }
}