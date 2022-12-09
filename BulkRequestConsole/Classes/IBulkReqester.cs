using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibWCF.Business;
using Asiacell.ITADLibWCF.Service;
using System.Threading;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibraries.Utilities;
using Newtonsoft.Json.Linq;

namespace BulkRequestConsole.Classes
{
    public interface IBulkReqester
    {
        bool SetRunningStatus(BulkElement obj);

        BasicSubscriberInformation GetBasicSubscriberBasicInformation(string PID, string MSIDN, ref RequestObject requestObj, ServerLoginObject serverLogin,
              ServerClient serverClient);

        void ExecuteLogic(string PID, ref RequestObject requestObject, ref ServerLoginObject serverLogin, ref BulkElement bulk, ServerClient serverClient);
        bool SetCompletedStatus(BulkElement obj);
    }       
}
