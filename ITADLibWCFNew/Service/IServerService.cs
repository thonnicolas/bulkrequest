using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Asiacell.ITADLibWCF_v1.Business;

namespace Asiacell.ITADLibWCF_v1.Service
{
    [ServiceContract]
    public interface IServerService
    {
        [OperationContract]
        RequestObject Authenticate(ServerLoginObject loginObject);

        [OperationContract]
        string Run(ServerLoginObject loginObject);

        [OperationContract]
        bool Refresh();  

    }
}
/*
element, command, param
*/