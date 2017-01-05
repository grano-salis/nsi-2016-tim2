using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SSO.WCFService.DataContracts
{
    [DataContract]
    public class MyFault
    {
        [DataMember]
        public string Details
        {
            get;
            set;
        }
    }
}