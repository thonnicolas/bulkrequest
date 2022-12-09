using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
namespace Asiacell.ITADLibWCF_v1.Business
{
    [DataContract]
    public class RequestObject
    {
        private List<NameValuePair> _commandParameterValues = new List<NameValuePair>();
        private DateTime _Requested_Date = DateTime.Now;
        private string _UserName = string.Empty;
        // Element Id is equal to ElementTypeID
        private int _Element_Id = -1; 
        private string _Element_Name = string.Empty;
        private string _Element_Host_Name = string.Empty;
        private string _Command_Name = string.Empty;
        private int _CommandID = -1;
        private int _Error_Code = -1;
        private string _Description = string.Empty;
        private string _Element_Node_Name = string.Empty;
        private string _Server_Node_Name = string.Empty;
        private String _Result = String.Empty;
        private string _refID = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string UserName { get { return _UserName; } set { _UserName = value; } }
        [DataMember]
        public int Element_Id { get { return _Element_Id; } set { _Element_Id = value; } }
        [DataMember]
        public string Element_Name { get { return _Element_Name; } set { _Element_Name = value; } }
        [DataMember]
        public string Element_Host_Name { get { return _Element_Host_Name; } set { _Element_Host_Name = value; } }
        [DataMember]
        public string Command_Name { get { return _Command_Name; } set { _Command_Name = value; } }
        [DataMember]
        public int CommandID { get { return _CommandID; } set { _CommandID = value; } }
        [DataMember]
        public List<NameValuePair> CommandParamaterValues { get { return _commandParameterValues;} set { _commandParameterValues = value; }}
        [DataMember]
        public DateTime Requested_Date { get { return _Requested_Date; } set { _Requested_Date = value; } }
        [DataMember]
        public DateTime Respond_Date { get; set; }
        [DataMember]
        public int Error_Code { get { return _Error_Code; } set { _Error_Code = value; } }
        [DataMember]
        public string Description { get { return _Description; } set { _Description = value; } }
        [DataMember]
        public string Element_Node_Name { get { return _Element_Node_Name; } set { _Element_Node_Name = value; } }
        [DataMember]
        public string Server_Node_Name { get { return _Server_Node_Name; } set { _Server_Node_Name = value; } }         
        [DataMember]
        public String Result { get { return _Result; } set { _Result = value; } }

        public JObject ResultToJsonObject()
        {
            return JObject.Parse(this.Result);
        }

        public String ToJson()
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }       
    }
}
