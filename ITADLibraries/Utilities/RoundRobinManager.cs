using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities
{
    
    public class RoundRobinManager<T> where T : class   
    {
        public ConcurrentDictionary<int, ConcurrentDictionary<int, T>> _clientList;
        public ConcurrentDictionary<int, RoundRobin<T>> _roundRobin;
        private LoggerEntities _logger;

        /// <summary>
        /// WCF Client Manager constructor
        /// </summary>
        public RoundRobinManager(LoggerEntities logger)
        {
            _logger = logger;
            _clientList = new ConcurrentDictionary<int, ConcurrentDictionary<int, T>>();
            _roundRobin = new ConcurrentDictionary<int, RoundRobin<T>>();
        }

        /// <summary>
        /// Add service element to collection
        /// </summary>
        /// <param name="element"></param>
        public void Add(int id, ConcurrentDictionary<int, T> c)
        {
            try
            {
                var roundRobin = new RoundRobin<T>();
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
        public T GetById(int id)
        {
            ConcurrentDictionary<int, T> cc;
            T t;

            this._clientList.TryGetValue(id, out cc);            
            //get round robin
            
            t = this._roundRobin[id].Get(cc);
            try{
                return (T)t;
            }catch(Exception e){
                _logger.AddtoLog("Cannot get round robin client", LoggerLevel.Error);
                throw new Exception("Cannot get round robin object");
            }
        }   
    }
}
