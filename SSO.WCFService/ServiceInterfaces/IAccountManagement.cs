using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SSO.WCFService.ServiceInterfaces
{
    [ServiceContract]
    interface IAccountManagement
    {
        //these operations are authorized and only user with ADMIN role can execute them
        //TODO change inputs - make DATA contracts instead
        [OperationContract]
        Boolean AddRole(string roleId, string userId); //add new role to a user, TODO: change input and output parameters 

        [OperationContract]
        Boolean RemoveRole(string roleId, string userId); //remove role from a user, TODO: change input and output parameters

        [OperationContract]
        Boolean BanUser(string userId); //TODO: change input and output parameters

        [OperationContract]
        Boolean ChangePassword(string newPassword, string userId); //TODO: change input and output parameters

    }
}
