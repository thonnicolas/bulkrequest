using System.Runtime.Serialization;
using Asiacell.ITADLibraries_v1.Utilities;

namespace Asiacell.ITADLibWCF_v1.Business
{
    [DataContract]
    public class ServerLoginObject
    {
        private string _refId = Functions.GetPID;

        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Command { get; set; }
        [DataMember]
        public string RefID { get { return _refId; } set { _refId = value == null ? Functions.GetPID : value; } }

        public string ResetRefID()
        {
            return this.RefID = Functions.GetPID;
        }
    }
}
