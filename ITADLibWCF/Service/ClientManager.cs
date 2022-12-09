using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Asiacell.ITADLibWCF.Business;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibWCF.Service
{
    public class ClientManager
    {
        private ConcurrentDictionary<int, IClient> _clientList;
        private LoggerEntities _logger;

        /// <summary>
        /// WCF Client Manager constructor
        /// </summary>
        public ClientManager(LoggerEntities logger)
        {
            _logger = logger;
            _clientList = new ConcurrentDictionary<int, IClient>();
        }

        /// <summary>
        /// Add service element to collection
        /// </summary>
        /// <param name="element"></param>
        public void Add(int id, IClient c)
        {
            try
            {               
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
            IClient t;
            this._clientList.TryGetValue(id, out t);            
            try{
                return (T)t;
            }catch(Exception e){
                throw new Exception("Cannot get client element");
            }
        }   
    }
}
