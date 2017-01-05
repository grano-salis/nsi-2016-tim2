using SSO.WCFService.DataContracts;
using SSO.WCFService.Exceptions;
using SSO.WCFService.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;

namespace SSO.WCFService.BusinessLogic
{

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Account : IAccount
    {
        private AccountServiceImplementation _mngr { get; set; }
        private IdentityServiceImplementation _identityMngr { get; set; }
        private SSOContext _db { get; set; }
        private WebOperationContext _ctx { get; set; }

        public Account()
        {
            _db = new SSOContext();
            _ctx = WebOperationContext.Current;
            _mngr = new AccountServiceImplementation(_db);
            _identityMngr = new IdentityServiceImplementation(_db);
        }

        public ActionResult ChangePassword()
        {
            try
            {
                // TODO get auth user
                User user = null;
                return _mngr.ChangePassword(user);
            }
            catch (SSOBaseException e)
            {
                _ctx.OutgoingResponse.StatusCode = e.StatusCode;
                return new ActionResult { Message = e.Message };
            }
            catch (Exception)
            {
                _ctx.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return new ActionResult { Message = "There has been an error while change password action." };
            }
        }

        public ActionResult Login(LoginRequest loginModel)
        {
            try
            {
                var claim = _mngr.Login(loginModel);
                HttpCookie cookie = new HttpCookie("sid", claim.Token);
                cookie.Domain = HttpContext.Current.Request.Url.Host;
                cookie.Path = "/";
                cookie.HttpOnly = true;
                // cookie expiration
                cookie.Expires = DateTime.Now.Add(new TimeSpan(24, 0, 0));
                HttpContext.Current.Response.SetCookie(cookie);

                return new ActionResult
                {
                    Message = "Successful login."
                };
            }
            catch (SSOBaseException e)
            {
                _ctx.OutgoingResponse.StatusCode = e.StatusCode;
                return new ActionResult { Message = e.Message };
            }
            catch (Exception e)
            {
                _ctx.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return new ActionResult { Message = "There has been an error while login action." };
            }
        }

        public ActionResult Register(DataContracts.RegisterRequest registerModel)
        {
            try
            {
                return _mngr.Register(registerModel);
            }
            catch (SSOBaseException e)
            {
                _ctx.OutgoingResponse.StatusCode = e.StatusCode;
                return new ActionResult
                {
                    Message = e.Message
                };
            }

            catch (Exception)
            {
                _ctx.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return new ActionResult { Message = "There has been an error while register action." };
            }
        }

        public AuthResponse Auth()
        {
            try
            {
                var cookie = HttpContext.Current.Request.Cookies["sid"];
                if(cookie == null)
                {
                    throw new WrongOrExpiredToken();
                }

                string token = HttpContext.Current.Request.Cookies["sid"].Value;

                if (String.IsNullOrWhiteSpace(token))
                {
                    throw new WrongOrExpiredToken();
                }

                return _identityMngr.Auth(token);
            }
            catch (WrongOrExpiredToken e)
            {
                // unset cookie
                var current = HttpContext.Current.Request.Cookies["sid"];
                if(current != null)
                {
                    HttpContext.Current.Response.Cookies.Remove("sid");
                    current.Value = null;
                    current.Expires = DateTime.Now.AddDays(-10);
                    current.HttpOnly = true;
                    HttpContext.Current.Response.SetCookie(current);
                }

                var myf = new MyFault { Details = e.Message };
                throw new WebFaultException<MyFault>(myf, e.StatusCode);
            }
            catch (SSOBaseException e)
            {
                var myf = new MyFault { Details = e.Message };
                throw new WebFaultException<MyFault>(myf, e.StatusCode);
            }
            catch (Exception e)
            {
                var myf = new MyFault { Details = "There has been an error in authorization process." };
                throw new WebFaultException<MyFault>(myf, HttpStatusCode.InternalServerError);
            }

        }


        public ActionResult Logout()
        {
            try
            { 

                var cookie = HttpContext.Current.Request.Cookies["sid"];
                if (cookie == null)
                {
                    return new ActionResult()
                    {
                        Message = "Logout succesful"
                    };
                }

                string token = HttpContext.Current.Request.Cookies["sid"].Value;
                if (String.IsNullOrWhiteSpace(token))
                {
                    throw new WrongOrExpiredToken();
                }

                _identityMngr.Logout(token);

                
                if (cookie != null)
                {
                    HttpContext.Current.Response.Cookies.Remove("sid");
                    cookie.Value = null;
                    cookie.Expires = DateTime.Now.AddDays(-10);
                    cookie.HttpOnly = true;
                    HttpContext.Current.Response.SetCookie(cookie);
                }

                return new ActionResult()
                {
                    Message = "Logout succesful"
                };
            }
            catch (WrongOrExpiredToken e)
            {
                // unset cookie
                var current = HttpContext.Current.Request.Cookies["sid"];
                if (current != null)
                {
                    HttpContext.Current.Response.Cookies.Remove("sid");
                    current.Value = null;
                    current.Expires = DateTime.Now.AddDays(-10);
                    current.HttpOnly = true;
                    HttpContext.Current.Response.SetCookie(current);
                }

                var myf = new MyFault { Details = e.Message };
                throw new WebFaultException<MyFault>(myf, e.StatusCode);
            }
            catch (SSOBaseException e)
            {
                var myf = new MyFault { Details = e.Message };
                throw new WebFaultException<MyFault>(myf, e.StatusCode);
            }
            catch (Exception e)
            {
                var myf = new MyFault { Details = "There has been an error in authorization process." };
                throw new WebFaultException<MyFault>(myf, HttpStatusCode.InternalServerError);
            }

        }

        public void GetOptions()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE, OPTIONS");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
            HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");

            HttpContext.Current.Response.End();
        }
    }
}