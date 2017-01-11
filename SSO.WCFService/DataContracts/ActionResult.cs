using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SSO.WCFService.DataContracts
{
    [DataContract]
    public class ActionResult
    {
        [DataMember]
        public string Message { get; set; }

    }
}