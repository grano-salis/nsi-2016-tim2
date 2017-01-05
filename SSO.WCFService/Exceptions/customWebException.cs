using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace SSO.WCFService.Exceptions
{
    public class customWebException : WebFaultException
    {
        public String Message { get; set; }
        public customWebException(HttpStatusCode code, String msg): base(code)
        {
            Message = msg;
        }

    }
}