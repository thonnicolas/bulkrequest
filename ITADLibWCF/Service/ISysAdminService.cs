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
    public interface ISysAdminService
    {

        [OperationContract]
        bool Refresh(ServerLoginObject loginObject);

        [OperationContract]
        bool ReloadClient(ServerLoginObject loginObject);

        [OperationContract]
        bool RefreshElement(ServerLoginObject loginObject);

        [OperationContract]
        bool RefreshByElementType(ServerLoginObject loginObject, int elementTypeId);
    }
}