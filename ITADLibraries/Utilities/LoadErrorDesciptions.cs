using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using Asiacell.ITADLibraries.LibDatabase;
 

namespace Asiacell.ITADLibraries.Utilities
{
    public class LoadErrorDesciptions
    {
        Dictionary<int, string> ErroDescription = new Dictionary<int, string>();

        public LoadErrorDesciptions(DBConnection db)
        {
            LoadData(db);
        }

        private void LoadData(DBConnection db)
        {
            string SQL = "select Errorcode,Description from Tbl_Error_Code";
            using (OracleDataReader reader = db.GetDataReader(SQL))
            {
                while (reader.Read())
                {
                    ErroDescription.Add(Functions.ToNumber(reader["Errorcode"]), Functions.ToString(reader["Description"]));
                }
            }
        }


        public string GetDescription(int ErrorCode, params object[] Param)
        {
            try
            {

                if (Param != null && Param.Length > 0)
                    return String.Format(ErroDescription[ErrorCode], Param);
                else
                    return String.Format(ErroDescription[ErrorCode]);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
