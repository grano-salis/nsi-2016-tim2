using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SSO.WCFService.Helpers
{
    public class CustomIdentity : IIdentity
    {
        private int _id { get; set; }
        private string _username { get; set; }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public string AuthenticationType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
    }
}