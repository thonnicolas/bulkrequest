using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Asiacell.ITADLibWCF_v1.Service;
using Asiacell.ITADLibWCF_v1.Business;
using System.Xml;

namespace Asiacell.ITADLibWCF_v1.Service
{
    public class ElementClient : IClient
    {
        /// <summary>
        /// Private properties
        /// </summary>
        private ElementLogin _elementLogin;
        private IElementService _service;
        private ChannelFactory<IElementService> _channelFactory;

        /// <summary>
        /// Element Client Constructor
        /// </summary>
        /// <param name="elementLogin"></param>
        public ElementClient(ElementLogin elementLogin)
        {
            Initialize(elementLogin);
        }


         /// <summary>
         /// Element Client Constructor
         /// </summary>
         /// <param name="elementLogin"></param>
        public ElementClient(ElementLogin elementLogin, int sendReceiveTimeoutMinute = 10, int sendReceiveTimeoutSecond = 0)
        {
            Initialize(elementLogin, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
        }

        private void Initialize(ElementLogin elementLogin, int sendReceiveTimeoutMinute = 10, int sendReceiveTimeoutSecond = 0)
        {
            this._elementLogin = elementLogin;
            // dynamically configure based on passed service url
            var binding = new BasicHttpBinding();

            XmlDictionaryReaderQuotas myReaderQuotas = new XmlDictionaryReaderQuotas();
            myReaderQuotas.MaxStringContentLength = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas = myReaderQuotas;

            var endpoint = new EndpointAddress(this._elementLogin.Url);
            binding.OpenTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
            binding.CloseTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
            binding.SendTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
            binding.ReceiveTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);

            //proxy.Endpoint.Binding = customBinding;
            var customBinding = new CustomBinding(binding);
            var transportElement = customBinding.Elements.Find<HttpTransportBindingElement>();
            transportElement.KeepAliveEnabled = false;

            this._channelFactory = new ChannelFactory<IElementService>(customBinding, endpoint);
            try { this._channelFactory.Open(); }
            catch { }
            // create and make channel call
            this._service = _channelFactory.CreateChannel();
        }

        /// <summary>
        /// Element Authentication
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public RequestObject Authenticate(string loginKey, RequestObject requestObject)
        {            
            return this._service.Authenticate(loginKey, requestObject);
        }


        public RequestObject Execute(string LoginKey, RequestObject ReqObject)
        {
            return this._service.Execute(LoginKey, ReqObject);
        }

        public string CheckService()
        {
            return this._service.CheckService();

        }
        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="loginObject"></param>
        /// <returns></returns>
        public bool Refresh()
        {
            return this._service.Refresh();
        }

        public int GetId
        {
            get
            {
                return _elementLogin.Id;
            }            
        }


        public void Dispose()
        {
            try
            {
                _channelFactory.Close();
            }
            catch { }
        }
    }
}
