using System;
using System.Data.Common;
using System.Runtime.Serialization;
using Oracle.DataAccess.Client;

namespace Asiacell.ITADLibraries.LibDatabase
{
    /// <summary>
    /// This exception fire only connection problem
    /// </summary>
    public sealed class DBConnectionException : DbException
    {
        public DBConnectionException() : base() { }
        public DBConnectionException(string message) : base(message) { }
        public DBConnectionException(string message, System.Exception inner) : base(message, inner) { }
        public DBConnectionException(OracleException ex) : base(ex.Message, ex) { }
        
    }
}
