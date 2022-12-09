using System.Data.Common;
using System.Runtime.Serialization;
using Oracle.ManagedDataAccess.Client;


namespace Asiacell.ITADLibraries.LibDatabase
{

    /// <summary>
    /// Oracle Managed Driver version 12_1_0
    /// </summary>
    public sealed class DBManagedDriverConnectionException: DbException
    {
            public DBManagedDriverConnectionException() : base() { }
            public DBManagedDriverConnectionException(string message) : base(message) { }
            public DBManagedDriverConnectionException(string message, System.Exception inner) : base(message, inner) { }
            public DBManagedDriverConnectionException(OracleException ex) : base(ex.Message, ex) { }

        
    }
}
