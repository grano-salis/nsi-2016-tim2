using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SSO.WCFService.Exceptions
{
    public class WrongOrExpiredToken : SSOBaseException
    {
        public WrongOrExpiredToken(): base("Wrong or expired token", HttpStatusCode.BadRequest )
        {
        }

        public WrongOrExpiredToken(Exception innerException) : base("Wrong or expired token", innerException, HttpStatusCode.BadRequest)
        {
        }
    }
}
