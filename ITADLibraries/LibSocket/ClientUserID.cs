using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibDatabase;
using Oracle.DataAccess.Client;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class ClientUserID
    {
        private Dictionary<string, ClientInfo> clients = new Dictionary<string, ClientInfo>();

        //public ClientUserID()
        //{
        //    for (int i = 0; i < 10; ++i)
        //    {
        //        ClientInfo info = new ClientInfo();

        //        info.Password = "password" + i;
        //        info.UserName = "user" + i;
        //        info.UserID = i;
        //        info.BufferSize = 1024;
        //        info.ConcurrentSession = 10;
        //        clients.Add(info.UserName, info);
        //    }
        //}

        public ClientUserID()
        { 
            
        }

        public ClientInfo GetClientInfo(DBConnection db,string UserName)
        {
            try
            {
                ClientInfo client = null;
                string sql = "select t.userid,t.user_name, t.password,t.max_pool_size  from TBL_USER t where t.user_name=:user_name and t.user_type=:user_type";

                OracleParameter[] param = new OracleParameter[2];
                param[0] = new OracleParameter("user_name", OracleDbType.Varchar2);
                param[0].Value = UserName;

                param[1] = new OracleParameter("user_type", OracleDbType.Int32);
                param[1].Value = 1; // 1 is only user for service only

                using (OracleDataReader reader = db.GetDataReader(sql, param))
                {
                    if (reader.Read())
                        client = new ClientInfo(Convert.ToInt32(reader["userid"]), 
                            Convert.ToString(reader["user_name"]), Convert.ToString(reader["password"]),
                            1024, Convert.ToUInt16(reader["max_pool_size"]));
                }

                return client;
            }
            catch (Exception ex){ return null; }
        }
    }
}
