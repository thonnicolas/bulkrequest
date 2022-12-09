using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using System.Collections.Concurrent;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Asiacell.ITADLibraries.LibDatabase;
using System.Data;
using System.ComponentModel;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities_v1
{
    /// <summary>
    /// Transaction log to use for database logger
    /// Implement ITaskExecutor 
    /// </summary>
    public class DBManagedDriverDataTransaction
    {
        private DBManagedDriverConnection db;
        private int bulkBindCount;
        private LoggerEntities logger; 
        private bool useBulkCopy = false;
        private object sync  = new object();
        /// <summary>
        /// Name of the destination table on the server
        /// </summary>
        public string DestinationTableName
        {
            get;
            set;
        }


        /// <summary>
        /// Number of rows in each batch. 
        /// At the end of each batch, the rows in the batch are sent to the server.
        /// </summary>
        public int? BatchSize
        {
            get;
            set;
        }

        /// <summary>
        /// Filters the properties to be included
        /// </summary>
        public Func<PropertyDescriptor, bool> ExpressionFilter
        {
            get;
            set;
        }

        // constractor here
        public DBManagedDriverDataTransaction(String tableName, DBManagedDriverConnection db, int bulkBindCount, bool useBulkCopy, LoggerEntities logger)
        {
            this.logger = logger;
            // any code here
            this.db = db;
            this.bulkBindCount = bulkBindCount;
            this.DestinationTableName = tableName;
            this.useBulkCopy = useBulkCopy;
        }
        

        // constractor here
        public DBManagedDriverDataTransaction(String tableName, DBManagedDriverConnection db, int bulkBindCount)
        {
            this.logger = logger;
            // any code here
            this.db = db;
            this.bulkBindCount = bulkBindCount;
            this.DestinationTableName = tableName;
            this.useBulkCopy = false;
        }
        
        /// <summary>
        /// Copies all items in a collection to a destination table
        /// </summary>
        /// <param name="dataTable">The items that will be copied to the destination table</param>
        /// <param name="options">A combination of values from the System.Data.SqlClient.SqlBulkCopyOptions 
        /// enumeration that determines which data source rows are copied to the destination table. <see cref="SqlBulkCopyOptions"/></param>
        public virtual void WriteToServer<T>(IEnumerable<T> items) where T : class
        {
            //use 0 to replace with OracleBulkCopyOptions
            WriteToServer(items, 0);
        }

        /// <summary>
        /// Copies all items in a collection to a destination table
        /// </summary>
        /// <param name="dataTable">The items that will be copied to the destination table</param>
        /// <param name="options">A combination of values from the System.Data.SqlClient.SqlBulkCopyOptions 
        /// enumeration that determines which data source rows are copied to the destination table. <see cref="SqlBulkCopyOptions"/></param>
        public virtual void WriteToServer<T>(IEnumerable<T> items, Int16 options) where T : class
        {
            DataTable dataTable = (this.ExpressionFilter == null) ? items.ToDataTable() : items.ToDataTable(this.ExpressionFilter);

            WriteToServer(dataTable, options);
        }


        /// <summary>
        /// Copies all items in a collection to a destination table
        /// </summary>
        /// <param name="dataTable">The items that will be copied to the destination table</param>
        /// <param name="options">A combination of values from the System.Data.SqlClient.SqlBulkCopyOptions 
        /// enumeration that determines which data source rows are copied to the destination table. <see cref="SqlBulkCopyOptions"/></param>
        /// <param name="columnMappings">Returns a collection of System.Data.SqlClient.SqlBulkCopyColumnMapping items. 
        /// Column mappings define the relationships between columns in the data source and columns in the destination.</param>
        //public virtual void WriteToServer<T>(IEnumerable<T> items, Int16 options, KeyValuePair<string,Type> columnMappings) where T : class
        //{
        //    DataTable dataTable = (this.ExpressionFilter == null) ? items.ToDataTable() : items.ToDataTable(this.ExpressionFilter);

        //    WriteToServer(dataTable, options, columnMappings);
        //}


        /// <summary>
        /// Copies all rows in the supplied System.Data.DataTable to a destination table
        /// </summary>
        /// <param name="dataTable">A System.Data.DataTable whose rows will be copied to the destination table</param>
        private void WriteToServer(DataTable dataTable)
        {
            //use 0 to replace OracleBulkCopyOptions as oracle managed driver clinet 12.1.0 is not support.
            WriteToServer(dataTable, 0);
        }


        /// <summary>
        /// Copies all rows in the supplied System.Data.DataTable to a destination table
        /// </summary>
        /// <param name="dataTable">A System.Data.DataTable whose rows will be copied to the destination table</param>
        /// <param name="options">A combination of values from the System.Data.SqlClient.SqlBulkCopyOptions 
        /// enumeration that determines which data source rows are copied to the destination table. <see cref="SqlBulkCopyOptions"/></param>
        private void WriteToServer(DataTable dataTable, Int16 options)
        {
           
            var columnMappings = from x in dataTable.Columns.Cast<DataColumn>()
                                 select new KeyValuePair<string, Type> (x.ColumnName, x.DataType);

            //if(this.useBulkCopy == true)
            //    WriteToServer(dataTable, options, columnMappings);
            //else
            WriteToServerBulkBinding(dataTable, options, columnMappings);
        }

        /// <summary>
        /// Copies all rows in the supplied System.Data.DataTable to a destination table
        /// </summary>
        /// <param name="dataTable">A System.Data.DataTable whose rows will be copied to the destination table</param>
        /// <param name="options">A combination of values from the System.Data.SqlClient.SqlBulkCopyOptions 
        /// enumeration that determines which data source rows are copied to the destination table. <see cref="SqlBulkCopyOptions"/></param>
        /// <param name="columnMappings">Returns a collection of System.Data.SqlClient.SqlBulkCopyColumnMapping items. 
        /// Column mappings define the relationships between columns in the data source and columns in the destination.</param>
        //private void WriteToServer(DataTable dataTable, Int16 options, KeyValuePair<string,Type> columnMappings)
        //{
        //    // table name matching:
        //    // checks for DestinationTableName value
        //    // if null or empty, checks for dataTable.TableName
        //    string destinationTableName =
        //        (string.IsNullOrWhiteSpace(DestinationTableName) ? null : DestinationTableName)
        //        ?? (string.IsNullOrWhiteSpace(dataTable.TableName) ? null : dataTable.TableName);

        //    if (string.IsNullOrWhiteSpace(destinationTableName))
        //        throw new ArgumentException("destinationTableName cannot be null or empty");

        //    // create insert script             
        //    using (var bulkCopy = new OracleBulkCopy(this.db.GetDBconnection))
        //    {
        //        bulkCopy.DestinationTableName = destinationTableName;

        //        if (this.BatchSize.HasValue)
        //            bulkCopy.BatchSize = this.BatchSize.Value;

        //        foreach (var mapping in columnMappings)
        //        {
        //            bulkCopy.ColumnMappings.Add(mapping);
        //        }

        //        bulkCopy.WriteToServer(dataTable);               
        //    }
        //}

        private void WriteToServerBulkBinding(DataTable dataTable, Int16 options, IEnumerable<KeyValuePair<string,Type>> columnMappings)
        {
            
            // table name matching:
            // checks for DestinationTableName value
            // if null or empty, checks for dataTable.TableName
            string destinationTableName =
                (string.IsNullOrWhiteSpace(DestinationTableName) ? null : DestinationTableName)
                ?? (string.IsNullOrWhiteSpace(dataTable.TableName) ? null : dataTable.TableName);

            if (string.IsNullOrWhiteSpace(destinationTableName))
                throw new ArgumentException("destinationTableName cannot be null or empty");

            string pid = Functions.GetPID;
            logger.AddtoLog(pid, "Prepare bulk insert", LoggerLevel.Info);
            bool isSuccess = true;
            using (var command = db.GetCommand())
            {
               
                try
                {
                    string sql = "insert into " + destinationTableName;
                    sql += "(";
                    bool isFirst = true;
                    foreach (var mapping in columnMappings)
                    {
                        if (isFirst)
                            sql += mapping.Key;
                        else
                            sql += "," + mapping.Key;
                        isFirst = false;
                    }
                    sql += ") values (";

                    isFirst = true;
                    foreach (KeyValuePair<string, Type> mapping in columnMappings)
                    {
                        if (isFirst)
                            sql += ":" + mapping.Key;
                        else
                            sql += ", :" + mapping.Key;
                        isFirst = false;

                        // bind command                 
                    }
                    sql += ")";
                    command.BindByName = true;
                    command.ArrayBindCount = dataTable.Rows.Count;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;

                    foreach (KeyValuePair<string,Type> mapping in columnMappings)
                    {
                        OracleParameter x = GetParameterValues(dataTable, command, mapping);

                        command.Parameters.Add(x);
                    }

                    command.ExecuteNonQuery();
                    logger.AddtoLog(pid, "End bulk insert", LoggerLevel.Info);

                }
                catch (Exception e)
                {
                    isSuccess = false;
                    logger.AddtoLog(pid, e, LoggerLevel.Error);
                    logger.AddtoLog(pid, "Failed to execute bulk insert", LoggerLevel.Error);                 
                }

                try
                {
                    command.Dispose();
                }
                catch
                {
                    logger.AddtoLog(pid, "Failed to dispose command", LoggerLevel.Error);
                }
            }
            // throw execption if isSuccess == false;
            if (isSuccess == false)
                throw new Exception("Error to execute bulk insert");
        }
        /// <summary>
        /// Get Parameter Values array
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="command"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private OracleParameter GetParameterValues(DataTable dataTable, OracleCommand command, KeyValuePair<string, Type> mapping)
        {
            var result = new object();
            
            string paramName = ":" + mapping.Key;
            //var param = command.CreateParameter();           

            OracleParameter x = command.CreateParameter();
            x.ParameterName = ":" + mapping.Key;

            //Type type = dataTable.Columns[mapping.Key].DataType;
            Type type = mapping.Value;

            result = dataTable.Rows.Cast<DataRow>()
                     .Select(row => (row[mapping.Key]))
                     .ToArray();

            if (type.IsEquivalentTo(typeof(DateTime)))
            {
                x.DbType = DbType.DateTime;
                //result = dataTable.Rows.Cast<DataRow>()
                //     .Select(row => (row[mapping.SourceColumn]))
                //     .ToArray();
            }
            else if (type.IsEquivalentTo(typeof(int)) || type.IsEquivalentTo(typeof(Int32)))
                x.DbType = DbType.Int32;
            else if (type.IsEquivalentTo(typeof(int)) || type.IsEquivalentTo(typeof(Int64)))
                x.DbType = DbType.Int64;
            else if (type.IsEquivalentTo(typeof(int)) || type.IsEquivalentTo(typeof(Int64)))
                x.DbType = DbType.Int64;
            else if (type.IsEquivalentTo(typeof(decimal)) || type.IsEquivalentTo(typeof(Decimal)))
                x.DbType = DbType.Int64;
            else if (type.IsEquivalentTo(typeof(float)) ||  type.IsEquivalentTo(typeof(Double)) || type.IsEquivalentTo(typeof(double)))
                x.DbType = DbType.Double;
            else if (type.IsEquivalentTo(typeof(string)))
                x.DbType = DbType.String;

            x.Value = result;
            return x;
        }

    }
}
