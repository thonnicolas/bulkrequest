using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Asiacell.ITADLibWCF_v1.Utilities
{
    public static class WCFUtilities
    {

        /// <summary>
        /// Get requested client ip
        /// </summary>
        /// <returns></returns>
        public static string GetRequestedClientIP()
        {
            var ip = "";
            try
            {
                var props = OperationContext.Current.IncomingMessageProperties;
                var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpointProperty != null)
                {
                    ip = endpointProperty.Address;
                }
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("WCFUtilities").Error("Get RequestedClientIP : " + e.Message);
            }
            return ip;

        }
    }
}
