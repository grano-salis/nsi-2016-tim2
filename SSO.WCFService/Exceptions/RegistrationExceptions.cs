using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SSO.WCFService.Exceptions
{
    public class UsernameExistsException : SSOBaseException
    {

        public UsernameExistsException(string username) : base(String.Format("Username '{0}' already exists.", username), HttpStatusCode.BadRequest)
        {
        }

        public UsernameExistsException(string username, Exception innerException) : base(String.Format("Username '{0}' already exists.", username), innerException, HttpStatusCode.BadRequest)
        {
        }
    }

    public class EmailExistsException : SSOBaseException
    {

        public EmailExistsException(string username) : base(String.Format("Email '{0}' already exists.", username), HttpStatusCode.BadRequest)
        {
        }

        public EmailExistsException(string username, Exception innerException) : base(String.Format("Email '{0}' already exists.", username), innerException, HttpStatusCode.BadRequest)
        {
        }
    }

    public class WeakPasswordException : SSOBaseException
    {
        static private string _errorMessage = "Password weak.";
        public WeakPasswordException() : base(_errorMessage, HttpStatusCode.BadRequest)
        {
        }

        public WeakPasswordException(Exception innerException) : base(_errorMessage, innerException, HttpStatusCode.BadRequest)
        {
        }
    }
}