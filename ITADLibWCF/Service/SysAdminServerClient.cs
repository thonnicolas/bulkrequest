using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using Asiacell.ITADLibWCF.Service;
using Asiacell.ITADLibWCF.Business;
using System.Xml;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibWCF.Service
{
    public class SysAdminServerClient : IClient, IDisposable
    {
        /// <summary>
        /// Private properties
        /// </summary>
        private string _url;
        private int _id = 0;
        private ISysAdminService _service;
        private ChannelFactory<ISysAdminService> _channelFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementLogin"></param>
        public SysAdminServerClient(int id, string url)
        {
            Initialize(id, url);                                      
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementLogin"></param>
        public SysAdminServerClient(int id, string url, int sendReceiveTimeoutMinute, int sendReceiveTimeoutSecond)
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
           
            if (url.ToLower().Contains("https://"))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            }
            binding.UseDefaultWebProxy = false;
            var endpoint = new EndpointAddress(this._url);

            //proxy.Endpoint.Binding = customBinding;
            var customBinding = new CustomBinding(binding);
            
            var transportElement = customBinding.Elements.Find<HttpTransportBindingElement>();
            transportElement.KeepAliveEnabled = false;

            this._channelFactory = new ChannelFactory<ISysAdminService>(customBinding, endpoint);
            try { this._channelFactory.Open(); }
            catch { }
            // create and make channel call
            this._service = _channelFactory.CreateChannel();
        }
         
        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="loginObject"></param>
        /// <returns></returns>
        public bool Refresh(ServerLoginObject loginObject)
        {
            return this._service.Refresh(loginObject);
        }

        public bool RefreshElement(ServerLoginObject loginObject)
        {
            return this._service.RefreshElement(loginObject);
        }

        public bool ReloadElement(ServerLoginObject loginObject)
        {
            return this._service.ReloadClient(loginObject);
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
    }
}
