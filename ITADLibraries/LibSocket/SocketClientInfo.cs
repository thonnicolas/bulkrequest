using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Asiacell.ITADLibraries.LibSocket
{
    public class SocketClientInfo
    {
        private ConcurrentDictionary<string, StateObject> socketClient = new ConcurrentDictionary<string, StateObject>();

        public SocketClientInfo()
        {

        }

        /// <summary>
        /// Add Socket client info
        /// </summary>
        /// <param name="stateobject"></param>
        public void AddClient(StateObject stateobject)
        {
            if (!socketClient.TryAdd(stateobject.UserName, stateobject))
                throw new Exception("Can't add client object");
        }


        public StateObject RemoveClient(StateObject stateobject)
        {
            StateObject state = null;
            try
            {
                socketClient.TryRemove(stateobject.UserName, out state);
            }
            catch { }

            return state;
        }

        public StateObject GetSocketClient(string UserName) { try { return socketClient[UserName]; } catch { } return null; }

        public ICollection<StateObject> ListClient()
        { return socketClient.Values; }


    }
}
