using SSO.WCFService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SSO.WCFService.ServiceInterfaces
{
    [ServiceContract]
    interface IAccount
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(MyFault))]
        ActionResult Login(LoginRequest loginModel);//Generate a new valid token for a user, TODO: change input and output parameters

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ActionResult Register(RegisterRequest registerModel);//Add new user, TODO: change input and output parameters

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(MyFault))]
        ActionResult ChangePassword(); //TODO: change input and output parameters

        [OperationContract] //used with parameter only when used by admin to get info about user with userId 
        [WebInvoke(Method = "GET",RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(MyFault))]
        AuthResponse Auth();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(MyFault))]
        ActionResult Logout(); // Logout and unset sid cookie


        [OperationContract]
        [WebInvoke(Method = "OPTIONS", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(MyFault))]
        void GetOptions();
    }

}
