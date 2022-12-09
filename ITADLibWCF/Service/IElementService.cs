using Asiacell.ITADLibWCF.Business;
using Asiacell.ITADLibWCF.Utilities;
using System.ServiceModel;
using System;

namespace Asiacell.ITADLibWCF.Service
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