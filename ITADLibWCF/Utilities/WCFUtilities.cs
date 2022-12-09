using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Asiacell.ITADLibWCF.Utilities
{
    public static class WCFUtilities
    {

        /// <summary>
        /// Get User IP address information
        /// </summary>
        /// <returns></returns>
        public static string GetRequestedClientIP()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            var ip = String.Empty;

            try
            {
                HttpRequestMessageProperty httprequest =
                   prop[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;

                var d = httprequest.Headers;
                
                ip = httprequest.Headers["X-Forwarded-For"];

                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    RemoteEndpointMessageProperty endpoint =
                     prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    if (endpoint != null)
                    {
                        ip = endpoint.Address;
                    }
                }
            }
            catch { ip = null; }

            return ip;
        }
    }
}
