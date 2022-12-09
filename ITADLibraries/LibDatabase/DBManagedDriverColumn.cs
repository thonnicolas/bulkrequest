using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;


namespace Asiacell.ITADLibraries.LibDatabase
{

    /// <summary>
    /// Oracle Managed Driver version 12_1_0
    /// </summary>
    public class DBManagedDriverColumn
    {
        public readonly string Name;
        public readonly OracleDbType DataType;
        public readonly object Value = null;

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="DataType"></param>
        public DBManagedDriverColumn(string ColumnName, OracleDbType DataType)
        {
            Name = ColumnName;
            this.DataType = DataType;
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="column"></param>
        /// <param name="Value"></param>
        public DBManagedDriverColumn(DBManagedDriverColumn column, object Value)
        {
            this.Name = column.Name;
            this.DataType = column.DataType;
            this.Value = Value;
        }
    }
}
