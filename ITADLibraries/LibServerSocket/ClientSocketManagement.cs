using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net;

namespace Asiacell.ITADLibraries.LibServerSocket
{
    class ClientSocketManagement
    {

        private ConcurrentDictionary<long, ClientSocket> Client = null;        

        public ClientSocketManagement()
        {
            Client = new ConcurrentDictionary<long, ClientSocket>();
        }

        

    }
}
