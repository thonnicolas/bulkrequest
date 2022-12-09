using System;
using Oracle.DataAccess.Client;


namespace Asiacell.ITADLibraries.LibDatabase
{
    public class Column
    {
        public readonly string Name;
        public readonly OracleDbType DataType;
        public readonly object Value = null;

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="DataType"></param>
        public Column(string ColumnName, OracleDbType DataType)
        {
            Name = ColumnName;
            this.DataType = DataType;
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="column"></param>
        /// <param name="Value"></param>
        public Column(Column column, object Value)
        {
            this.Name = column.Name;
            this.DataType = column.DataType;
            this.Value = Value;
        }


        public Column(string ColumnName, object Value)
        {
            this.Name = ColumnName;
            this.Value = Value;
            this.DataType = OracleDbType.Object;
        }

    }
}
