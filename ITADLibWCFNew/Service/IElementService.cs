using Asiacell.ITADLibWCF_v1.Business;
using Asiacell.ITADLibWCF_v1.Utilities;
using System.ServiceModel;
using System;

namespace Asiacell.ITADLibWCF_v1.Service
{
    [ServiceContract(Namespace = ServiceConstants.Namespace)]
    public interface IElementService
    {
        [OperationContract]
        RequestObject Authenticate(string loginKey, RequestObject requestObject);

        [OperationContract]
        RequestObject Execute(string LoginKey, RequestObject ReqObject);

        [OperationContract]
        String CheckService();

        [OperationContract]
        bool Refresh();
    }

}