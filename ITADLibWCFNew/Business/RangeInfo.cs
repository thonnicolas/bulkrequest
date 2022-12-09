using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF_v1.Business
{
    /// <summary>
    /// Range Information
    /// </summary>
    public class RangeInfo
    {
        public int Id {get; set;}
        public string RangeStart {get; set;}
        public string RangeEnd {get; set;}
        public int ElementTypeId {get; set; }
        public string ElementTypeName {get; set;}
        public int VersionId {get; set;}
        public string Version {get; set;}
        public int ElementId {get; set;}
        public string ElementName {get; set;}
        public string ElementIp {get; set;}
        public string ElementPort {get; set;}
        public string UserId {get; set; }
        public string Password {get; set;}
        public int BufferSize  {get; set;}
        public int SendreceiveTimeout {get; set;}
        public int CityId {get; set;}
        public string CityName {get; set;}
        public int Tplid  {get; set;}
        public int SubTemplate {get; set;}
        public int Tcsi { get; set; }
        public int Ocsi {get; set;}
        public int GprsCsi {get; set;}
        public int Ucsi {get; set;}
        public int UcsiNonBalanceTransfer {get; set;}
        public int Gtid {get; set;}
        public string Scp {get; set;}
        public string ScpDpc {get; set;}
        public string ScpGt {get; set;}
        public int SubClass {get; set;}
        public int SubscriberType {get; set;}
        public int Status {get; set;}
        public string LoginCommand {get; set;}
        public string LogoutCommand {get; set;}
        public string Encryption { get; set; }
        
    }
}
