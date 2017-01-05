using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.WCFService.Helpers
{
    public class UserInfoExtended
    {
        public int UserId;
        public string Username;
        public UserInfo Info;
        public List<string> Roles;
    }
}