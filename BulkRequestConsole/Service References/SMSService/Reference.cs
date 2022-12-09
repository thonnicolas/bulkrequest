﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BulkRequestConsole.SMSService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SMSService.mdw_SMSSoap")]
    public interface mdw_SMSSoap {
        
        // CODEGEN: Generating message contract since element name MSISDN from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendSMS", ReplyAction="*")]
        BulkRequestConsole.SMSService.SendSMSResponse SendSMS(BulkRequestConsole.SMSService.SendSMSRequest request);
        
        // CODEGEN: Generating message contract since element name sender from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendDirectSMS", ReplyAction="*")]
        BulkRequestConsole.SMSService.SendDirectSMSResponse SendDirectSMS(BulkRequestConsole.SMSService.SendDirectSMSRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SendSMSRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SendSMS", Namespace="http://tempuri.org/", Order=0)]
        public BulkRequestConsole.SMSService.SendSMSRequestBody Body;
        
        public SendSMSRequest() {
        }
        
        public SendSMSRequest(BulkRequestConsole.SMSService.SendSMSRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SendSMSRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string MSISDN;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string sender;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string msg;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string UserName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string Password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string loggedUser;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=6)]
        public int lang;
        
        public SendSMSRequestBody() {
        }
        
        public SendSMSRequestBody(string MSISDN, string sender, string msg, string UserName, string Password, string loggedUser, int lang) {
            this.MSISDN = MSISDN;
            this.sender = sender;
            this.msg = msg;
            this.UserName = UserName;
            this.Password = Password;
            this.loggedUser = loggedUser;
            this.lang = lang;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SendSMSResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SendSMSResponse", Namespace="http://tempuri.org/", Order=0)]
        public BulkRequestConsole.SMSService.SendSMSResponseBody Body;
        
        public SendSMSResponse() {
        }
        
        public SendSMSResponse(BulkRequestConsole.SMSService.SendSMSResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SendSMSResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string SendSMSResult;
        
        public SendSMSResponseBody() {
        }
        
        public SendSMSResponseBody(string SendSMSResult) {
            this.SendSMSResult = SendSMSResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SendDirectSMSRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SendDirectSMS", Namespace="http://tempuri.org/", Order=0)]
        public BulkRequestConsole.SMSService.SendDirectSMSRequestBody Body;
        
        public SendDirectSMSRequest() {
        }
        
        public SendDirectSMSRequest(BulkRequestConsole.SMSService.SendDirectSMSRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SendDirectSMSRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string sender;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string receiver;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string msg;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string user;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string loggedUser;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string pass;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string lang;
        
        public SendDirectSMSRequestBody() {
        }
        
        public SendDirectSMSRequestBody(string sender, string receiver, string msg, string user, string loggedUser, string pass, string lang) {
            this.sender = sender;
            this.receiver = receiver;
            this.msg = msg;
            this.user = user;
            this.loggedUser = loggedUser;
            this.pass = pass;
            this.lang = lang;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SendDirectSMSResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SendDirectSMSResponse", Namespace="http://tempuri.org/", Order=0)]
        public BulkRequestConsole.SMSService.SendDirectSMSResponseBody Body;
        
        public SendDirectSMSResponse() {
        }
        
        public SendDirectSMSResponse(BulkRequestConsole.SMSService.SendDirectSMSResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SendDirectSMSResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string SendDirectSMSResult;
        
        public SendDirectSMSResponseBody() {
        }
        
        public SendDirectSMSResponseBody(string SendDirectSMSResult) {
            this.SendDirectSMSResult = SendDirectSMSResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface mdw_SMSSoapChannel : BulkRequestConsole.SMSService.mdw_SMSSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class mdw_SMSSoapClient : System.ServiceModel.ClientBase<BulkRequestConsole.SMSService.mdw_SMSSoap>, BulkRequestConsole.SMSService.mdw_SMSSoap {
        
        public mdw_SMSSoapClient() {
        }
        
        public mdw_SMSSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public mdw_SMSSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public mdw_SMSSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public mdw_SMSSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BulkRequestConsole.SMSService.SendSMSResponse BulkRequestConsole.SMSService.mdw_SMSSoap.SendSMS(BulkRequestConsole.SMSService.SendSMSRequest request) {
            return base.Channel.SendSMS(request);
        }
        
        public string SendSMS(string MSISDN, string sender, string msg, string UserName, string Password, string loggedUser, int lang) {
            BulkRequestConsole.SMSService.SendSMSRequest inValue = new BulkRequestConsole.SMSService.SendSMSRequest();
            inValue.Body = new BulkRequestConsole.SMSService.SendSMSRequestBody();
            inValue.Body.MSISDN = MSISDN;
            inValue.Body.sender = sender;
            inValue.Body.msg = msg;
            inValue.Body.UserName = UserName;
            inValue.Body.Password = Password;
            inValue.Body.loggedUser = loggedUser;
            inValue.Body.lang = lang;
            BulkRequestConsole.SMSService.SendSMSResponse retVal = ((BulkRequestConsole.SMSService.mdw_SMSSoap)(this)).SendSMS(inValue);
            return retVal.Body.SendSMSResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BulkRequestConsole.SMSService.SendDirectSMSResponse BulkRequestConsole.SMSService.mdw_SMSSoap.SendDirectSMS(BulkRequestConsole.SMSService.SendDirectSMSRequest request) {
            return base.Channel.SendDirectSMS(request);
        }
        
        public string SendDirectSMS(string sender, string receiver, string msg, string user, string loggedUser, string pass, string lang) {
            BulkRequestConsole.SMSService.SendDirectSMSRequest inValue = new BulkRequestConsole.SMSService.SendDirectSMSRequest();
            inValue.Body = new BulkRequestConsole.SMSService.SendDirectSMSRequestBody();
            inValue.Body.sender = sender;
            inValue.Body.receiver = receiver;
            inValue.Body.msg = msg;
            inValue.Body.user = user;
            inValue.Body.loggedUser = loggedUser;
            inValue.Body.pass = pass;
            inValue.Body.lang = lang;
            BulkRequestConsole.SMSService.SendDirectSMSResponse retVal = ((BulkRequestConsole.SMSService.mdw_SMSSoap)(this)).SendDirectSMS(inValue);
            return retVal.Body.SendDirectSMSResult;
        }
    }
}
