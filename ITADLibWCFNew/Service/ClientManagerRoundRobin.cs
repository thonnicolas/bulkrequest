using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries_v1.LibLogger;
using Asiacell.ITADLibraries_v1.Utilities;

namespace Asiacell.ITADLibWCF_v1.Service
{
    public class ClientManagerRoundRobin   
    {
        public ConcurrentDictionary<int, ConcurrentDictionary<int, IClient>> _clientList;
        public ConcurrentDictionary<int, RoundRobin<IClient>> _roundRobin;
        private LoggerEntities _logger;

        /// <summary>
        /// WCF Client Manager constructor
        /// </summary>
        public ClientManagerRoundRobin(LoggerEntities logger)
        {
            _logger = logger;
            _clientList = new ConcurrentDictionary<int, ConcurrentDictionary<int, IClient>>();
            _roundRobin = new ConcurrentDictionary<int, RoundRobin<IClient>>();
        }

        /// <summary>
        /// Add service element to collection
        /// </summary>
        /// <param name="element"></param>
        public void Add(int id, ConcurrentDictionary<int, IClient> c)
        {
            try
            {
                var roundRobin = new RoundRobin<IClient>();
                this._roundRobin.AddOrUpdate(id, roundRobin, (key, old) => roundRobin);            
                this._clientList.AddOrUpdate(id, c, (key, old) => c);
            }
            catch (Exception e)
            {
                _logger.AddtoLog("Failed to add element into collection", LoggerLevel.Error);
            }
        }

        /// <summary>
        /// Get client by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(int id)
        {
            ConcurrentDictionary<int, IClient> cc;
            IClient t;

            this._clientList.TryGetValue(id, out cc);            
            //get round robin
            
            t = this._roundRobin[id].Get(cc);
            try{
                return (T)t;
            }catch(Exception e){
                _logger.AddtoLog("Cannot get service client", LoggerLevel.Error);
                throw new Exception("Cannot get client element");
            }
        }   
    }
}
