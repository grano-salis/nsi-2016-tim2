using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.WCFService.BusinessLogic
{
    public class AccountManagementImpl
    {
        private SSOContext _db;

        public AccountManagementImpl(SSOContext _db)
        {
            this._db = _db;
        }

        public bool AddRole(string roleId, string userId)
        {
            throw new NotImplementedException();
        }

        public bool BanUser(string userId)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string newPassword, string userId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRole(string roleId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}