using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using Asiacell.ITADLibWCF_v1.Service;
using Asiacell.ITADLibWCF_v1.Business;
using System.Xml;
using Asiacell.ITADLibraries_v1.Utilities;
namespace Asiacell.ITADLibWCF_v1.Service
{
    public class ServerClient :  IClient, IDisposable
    {
        /// <summary>
        /// Private properties
        /// </summary>
        private string _url;
        private int _id = 0;
        private IServerService _service;
        private ChannelFactory<IServerService> _channelFactory;
 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementLogin"></param>
        public ServerClient(int id, string url)
        {
            Initialize(id, url);                                      
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementLogin"></param>
        public ServerClient(int id, string url, int sendReceiveTimeoutMinute, int sendReceiveTimeoutSecond)
        {
            Initialize(id, url, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
        }

        private void Initialize(int id, string url, int sendReceiveTimeoutMinute = 10, int sendReceiveTimeoutSecond = 0)
        {
            this._url = url;
            this._id = id;
            // dynamically configure based on passed service url
            var binding = new BasicHttpBinding();


            XmlDictionaryReaderQuotas myReaderQuotas = new XmlDictionaryReaderQuotas();
            myReaderQuotas.MaxStringContentLength = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas = myReaderQuotas;
            binding.OpenTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
            binding.CloseTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
            binding.SendTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);
            binding.ReceiveTimeout = new TimeSpan(0, sendReceiveTimeoutMinute, sendReceiveTimeoutSecond);

            var endpoint = new EndpointAddress(this._url);

            //proxy.Endpoint.Binding = customBinding;
            var customBinding = new CustomBinding(binding);
            var transportElement = customBinding.Elements.Find<HttpTransportBindingElement>();
            transportElement.KeepAliveEnabled = false;

            this._channelFactory = new ChannelFactory<IServerService>(customBinding, endpoint);
            try { this._channelFactory.Open(); }
            catch { }
            // create and make channel call
            this._service = _channelFactory.CreateChannel();
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="loginObject"></param>
        /// <returns></returns>
        public string Run(ServerLoginObject loginObject)
        {            
            return this._service.Run(loginObject);
        }

        /// <summary>
        /// Authenticate method
        /// </summary>
        /// <param name="loginObject"></param>
        /// <returns></returns>
        public RequestObject Authenticate(ServerLoginObject loginObject)
        {            
            return this._service.Authenticate(loginObject);
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
            get { return _id; }
        }

        public void Dispose()
        {
            try
            {
                _channelFactory.Close();                          
            }
            catch { }
        }

        int IClient.GetId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
