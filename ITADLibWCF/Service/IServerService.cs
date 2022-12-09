using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Asiacell.ITADLibWCF.Business;

namespace Asiacell.ITADLibWCF.Service
{
    [ServiceContract]
    public interface IServerService
    {
        [OperationContract]
        [WebInvoke(Method="POST", RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json, UriTemplate="/Execute", BodyStyle=WebMessageBodyStyle.Wrapped)]
        RequestObject Authenticate(ServerLoginObject loginObject);
    }
} 