using System;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibraries.LibDatabase
{
    public class SQLStatements
    {
        /// <summary>
        /// Insert SQL
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public static string InsertSQL(string TableName, Dictionary<string, OracleDbType> Columns)
        {
            string SQL = string.Empty;
            string Value = string.Empty;

            foreach (KeyValuePair<string, OracleDbType> pair in Columns)
            {
                if (string.IsNullOrEmpty(SQL))
                {
                    SQL = "INSERT INTO " + TableName + "(" + pair.Key;
                    if (pair.Value == OracleDbType.Date)
                        Value = " VALUES(to_date(:" + pair.Key + ",'" + Functions.dateFormateOracle + "')";
                    else
                        Value = " VALUES(:" + pair.Key;
                }
                else
                {
                    SQL += "," + pair.Key;
                    if (pair.Value == OracleDbType.Date)
                        Value += ",to_date(:" + pair.Key + ",'" + Functions.dateFormateOracle + "')";
                    else
                        Value += ",:" + pair.Key;
                }
            }
            SQL += ")" + Value + ")";

            return SQL;
        }

        /// <summary>
        /// Deelete SQL
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string DeleteSQL(string TableName, string QueryString)
        {
            string SQL = "DELETE FROM " + TableName + " WHERE " + QueryString;
            return SQL;
        }

        /// <summary>
        /// Update SQL
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="columns"></param>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string UpdateSQL(string TableName, Column[] columns, string QueryString)
        {
            string SQL = string.Empty;
            string Value = string.Empty;

            foreach (Column column in columns)
            {
                if (string.IsNullOrEmpty(SQL))
                {
                    SQL = "UPDATE " + TableName + " SET " + column.Name + "='" + column.Name + "'";
                }
                else
                {
                    SQL += ", " + column.Name + "='" + column.Name + "'";
                }
            }

            SQL += " WHERE " + QueryString;

            return SQL;
        }



        /// <summary>
        /// Select Statement
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Columns"></param>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        public static string SelectSQL(string TableName, Dictionary<string, OracleDbType> Columns, string QueryString)
        {
            string SQL = SelectSQL(TableName, Columns);

            if (!String.IsNullOrWhiteSpace(QueryString))
                SQL += " WHERE " + QueryString;

            return SQL;

        }


        public static string SelectSQL(string TableName, Dictionary<string, OracleDbType> Columns, Column[] InquiryColum)
        {

            string SQL = SelectSQL(TableName, Columns);

            if (InquiryColum != null && InquiryColum.Length > 0)
            {
                for (int i = 0; i < InquiryColum.Length; i++)
                {
                    if (i == 0)
                        SQL += " WHERE " + InquiryColum[i].Name + "=:" + InquiryColum[i].Name;
                    else
                        SQL += " AND " + InquiryColum[i].Name + "=:" + InquiryColum[i].Name;
                }
            }

            return SQL;

        }


        public static string SelectSQL(string TableName, Dictionary<string, OracleDbType> Columns)
        {
            string SQL = string.Empty;

            foreach (KeyValuePair<string, OracleDbType> pair in Columns)
            {
                if (string.IsNullOrEmpty(SQL))
                {
                    if (pair.Value == OracleDbType.Date)
                        SQL = "SELECT to_char(" + pair.Key + ",'" + Functions.dateFormateOracle + "') " + pair.Key;
                    else
                        SQL = "SELECT " + pair.Key;
                }
                else
                {
                    if (pair.Value == OracleDbType.Date)
                        SQL += ", to_char(" + pair.Key + ",'" + Functions.dateFormateOracle + "') " + pair.Key;
                    else
                        SQL += "," + pair.Key;
                }
            }
            SQL += " FROM " + TableName;

            return SQL;

        }

    }
}
