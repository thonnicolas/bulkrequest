using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Asiacell.ITADLibWCF.Business;

/*
 *This class is use API SoapHttpClientProtocol instead of WCF BasicHTTP 
 *to optimize version contorl system.
 *
*/

namespace Asiacell.ITADLibWCF.SoapHttpClients
{

    // 
    // This source code was auto-generated by wsdl, Version=4.0.30319.1.
    // 
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "BasicHttpBinding_IServerService", Namespace = "http://www.asiacell.com/")]
    public partial class SoapClient : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback AuthenticateOperationCompleted;

        /// <remarks/>
        public SoapClient(string url)
        {
            this.Url = url; // "http://192.168.164.142:83/Execute.svc";
        }
        public SoapClient(int id, string url, int timeoutmin, int timeoutsecond)
        {
            this.Url = url; // "http://192.168.164.142:83/Execute.svc";
            this.Timeout = 1000 * (timeoutmin * 60 + timeoutsecond);
        }

        /// <remarks/>
        public event AuthenticateCompletedEventHandler AuthenticateCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.asiacell.com/IServerService/Authenticate", RequestNamespace = "http://www.asiacell.com/", ResponseNamespace = "http://www.asiacell.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public RequestObject Authenticate([System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] ServerLoginObject loginObject)
        {
            object[] results = this.Invoke("Authenticate", new object[] {
                    loginObject});
            return ((RequestObject)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginAuthenticate(ServerLoginObject loginObject, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("Authenticate", new object[] {
                    loginObject}, callback, asyncState);
        }

        /// <remarks/>
        public RequestObject EndAuthenticate(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((RequestObject)(results[0]));
        }

        /// <remarks/>
        public void AuthenticateAsync(ServerLoginObject loginObject)
        {
            this.AuthenticateAsync(loginObject, null);
        }

        /// <remarks/>
        public void AuthenticateAsync(ServerLoginObject loginObject, object userState)
        {
            if ((this.AuthenticateOperationCompleted == null))
            {
                this.AuthenticateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAuthenticateOperationCompleted);
            }
            this.InvokeAsync("Authenticate", new object[] {
                    loginObject}, this.AuthenticateOperationCompleted, userState);
        }

        private void OnAuthenticateOperationCompleted(object arg)
        {
            if ((this.AuthenticateCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AuthenticateCompleted(this, new AuthenticateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }
    
    /*Not use as already existing in Asiacell.ITADLibWCF.Business
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Asiacell.ITADLibWCF.SoapHttpClients")]
    public partial class ServerLoginObject
    {

        private string commandField;

        private string passwordField;

        private string refIDField;

        private string userNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Command
        {
            get
            {
                return this.commandField;
            }
            set
            {
                this.commandField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string RefID
        {
            get
            {
                return this.refIDField;
            }
            set
            {
                this.refIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string UserName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Asiacell.ITADLibWCF.SoapHttpClients")]
    public partial class NameValuePair
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
    

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Asiacell.ITADLibWCF.SoapHttpClients")]
    public partial class RequestObject
    {

        private int commandIDField;

        private bool commandIDFieldSpecified;

        private NameValuePair[] commandParamaterValuesField;

        private string command_NameField;

        private string descriptionField;

        private string element_Host_NameField;

        private int element_IdField;

        private bool element_IdFieldSpecified;

        private string element_NameField;

        private string element_Node_NameField;

        private int error_CodeField;

        private bool error_CodeFieldSpecified;

        private string idField;

        private System.DateTime requested_DateField;

        private bool requested_DateFieldSpecified;

        private System.DateTime respond_DateField;

        private bool respond_DateFieldSpecified;

        private string resultField;

        private string server_Node_NameField;

        private string userNameField;

        /// <remarks/>
        public int CommandID
        {
            get
            {
                return this.commandIDField;
            }
            set
            {
                this.commandIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CommandIDSpecified
        {
            get
            {
                return this.commandIDFieldSpecified;
            }
            set
            {
                this.commandIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable = true)]
        public NameValuePair[] CommandParamaterValues
        {
            get
            {
                return this.commandParamaterValuesField;
            }
            set
            {
                this.commandParamaterValuesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Command_Name
        {
            get
            {
                return this.command_NameField;
            }
            set
            {
                this.command_NameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Element_Host_Name
        {
            get
            {
                return this.element_Host_NameField;
            }
            set
            {
                this.element_Host_NameField = value;
            }
        }

        /// <remarks/>
        public int Element_Id
        {
            get
            {
                return this.element_IdField;
            }
            set
            {
                this.element_IdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Element_IdSpecified
        {
            get
            {
                return this.element_IdFieldSpecified;
            }
            set
            {
                this.element_IdFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Element_Name
        {
            get
            {
                return this.element_NameField;
            }
            set
            {
                this.element_NameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Element_Node_Name
        {
            get
            {
                return this.element_Node_NameField;
            }
            set
            {
                this.element_Node_NameField = value;
            }
        }

        /// <remarks/>
        public int Error_Code
        {
            get
            {
                return this.error_CodeField;
            }
            set
            {
                this.error_CodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Error_CodeSpecified
        {
            get
            {
                return this.error_CodeFieldSpecified;
            }
            set
            {
                this.error_CodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public System.DateTime Requested_Date
        {
            get
            {
                return this.requested_DateField;
            }
            set
            {
                this.requested_DateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Requested_DateSpecified
        {
            get
            {
                return this.requested_DateFieldSpecified;
            }
            set
            {
                this.requested_DateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public System.DateTime Respond_Date
        {
            get
            {
                return this.respond_DateField;
            }
            set
            {
                this.respond_DateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Respond_DateSpecified
        {
            get
            {
                return this.respond_DateFieldSpecified;
            }
            set
            {
                this.respond_DateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Server_Node_Name
        {
            get
            {
                return this.server_Node_NameField;
            }
            set
            {
                this.server_Node_NameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string UserName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }
    }
    */
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void AuthenticateCompletedEventHandler(object sender, AuthenticateCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AuthenticateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal AuthenticateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public RequestObject Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((RequestObject)(this.results[0]));
            }
        }
    }

}