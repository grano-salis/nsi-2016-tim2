using SSO.WCFService.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SSO.WCFService.BusinessLogic
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AccountManagement" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AccountManagement.svc or AccountManagement.svc.cs at the Solution Explorer and start debugging.
    public class AccountManagement : IAccountManagement
    {
        private AccountManagementImpl _mngr { get; set; }
        private SSOContext _db { get; set; }
        private WebOperationContext _ctx { get; set; }

        public AccountManagement()
        {
            _db = new SSOContext();
            _ctx = WebOperationContext.Current;
            _mngr = new AccountManagementImpl(_db);
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
