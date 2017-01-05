using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SSO.WCFService.Exceptions
{
    public class WrongCredentialsException : SSOBaseException
    {
       
        public WrongCredentialsException(): base("Wrong login credentials.", HttpStatusCode.BadRequest)
        {
        }
        
        public WrongCredentialsException(Exception innerException) : base("Wrong login credentials.", innerException, HttpStatusCode.BadRequest)
        {
        }
    }
}
