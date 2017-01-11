using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SSO.WCFService.Exceptions
{
    public class SSOBaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public SSOBaseException(HttpStatusCode code)
        {
            StatusCode = code;
        }

        public SSOBaseException(string message, HttpStatusCode code) : base(message)
        {
            StatusCode = code;
        }

        public SSOBaseException(string message, Exception innerException, HttpStatusCode code) : base(message, innerException)
        {
            StatusCode = code;
        }
    }
}
