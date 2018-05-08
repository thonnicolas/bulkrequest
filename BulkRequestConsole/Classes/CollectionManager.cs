using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibWCF.Business;
using System.Collections.Concurrent;
using Oracle.DataAccess.Client;
using System.Data;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.LibLogger;
using Microsoft.AspNet.SignalR.Client;
using BulkRequestConsole.Utilities;

namespace BulkRequestConsole.Classes
{
    /// <summary>
    /// Bulk Element
    /// </summary>
    public class BulkElement
    {  
        public string id { get; set; }
        public string rowid { get; set; }
        public string command1 { get; set; }
        public string command2 { get; set; }
        public string command3 { get; set; }
        public string command4 { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }
        public string value4 { get; set; }
        public string status { get; set; }
        public string application_id { get; set; }
        public string ref_id { get; set; }
        public int run_count { get; set; }
        public string transaction_id { get; set; }
        public int error_code { get; set; }
        public string result { get; set; }
        public string user_id { get; set; }
        public DateTime respond_date { get; set; }
        public DateTime request_date { get; set; }        
        public string login_account { get; set; }
        public string sms { get; set; }
        public string sms_ar { get; set; }
        public string sms_ku { get; set; }
        public string sms_en { get; set; }
        public int is_send_sms { get; set; }
        public int lang { get; set; }
        public string msisdn { get; set; }
        public int systemcommandid { get; set; }
        public string convert_params { get; set; }
        public int is_reset { get; set; }
        public string reset_value_path { get; set; } 
    }

    /// <summary>
    /// Bulk Execute Map
    /// </summary>
    public class BulkExecuteMap
    {
        public string rowid { get; set; }
        public string ref_id { get; set; }
        public string is_valid { get; set; }
        public string is_loaded { get; set; }
        public string is_ready { get; set; }
        public string userid { get; set; }     
   

    }

    /// <summary>
    /// Request Collection Manager
    /// </summary>
    public class CollectionManager
    {
        
        WorkerPool []worker = null;

        private int numRequest = 0;
      
        //string sampleLoadTestCommandFilePath = String.Empty;

        string GWUser;
        string GWPassword;
        private DBConnection db;
        private DBConnection db2;
        private string migrate_table;
         
        string ApplicationID;
        string smsMessage = "";
        int lang = 3;
        int isSendSMS = 0;
        LoggerEntities logger;
        string lastBulkMapperRefID = "";
        private string smsMessageAR;
        private string smsMessageKU;
        private string smsMessageEN;
        private long totalRecord;
        private string convertParams;
        private int systemCommandId;
        private string resetValuePath;
        private int isReset;
        private string loginAccount;
       
        /// <summary>
        /// Collection Manager
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="applicationID"></param>
        /// <param name="numRequest"></param>
        /// <param name="worker"></param>
        /// <param name="sampleLoadTestCommandFilePath"></param>
        /// <param name="GWUser"></param>
        /// <param name="GWPassword"></param>
        /// <param name="db"></param>
        /// <param name="db2"></param>
        /// <param name="migrate_table"></param>
        public CollectionManager(LoggerEntities logger, string applicationID, int numRequest, WorkerPool[] worker, string sampleLoadTestCommandFilePath, string GWUser, string GWPassword, DBConnection db, DBConnection db2, string migrate_table)
        {              
            this.worker = worker;
            //this.sampleLoadTestCommandFilePath = sampleLoadTestCommandFilePath;
            this.numRequest = numRequest;           
            this.GWPassword = GWPassword;
            this.GWUser = GWUser;
            this.db = db;
            this.db2 = db2;
            this.migrate_table = migrate_table;
            this.ApplicationID = applicationID;
            this.logger = logger;
        }

        /// <summary>
        /// Get Non Requested Records
        /// </summary>
        public void GetNonRequested()
        {

            if (String.IsNullOrWhiteSpace(this.lastBulkMapperRefID))
            {
                string sqlmap = @"select rowid, ref_id, file_path, is_valid, is_loaded, is_ready, userid,  sms, is_send_sms, lang, sms_ar, sms_ku, sms_en, 
                                    total_record, convert_params, systemcommandid, IS_RESET_COMMAND, reset_value_path
                                    from tbl_bulk_execute_map 
                                    where is_ready = 1 and is_loaded = 1 and is_executed = 0 and rownum <= 1 order by requested_date asc";

                ConcurrentDictionary<int, BulkExecuteMap> commandMapList = new ConcurrentDictionary<int, BulkExecuteMap>();
                try
                {
                    using (var command = this.db.GetCommand())
                    {
                        command.CommandText = sqlmap;
                        command.CommandType = CommandType.Text;

                        int i = 0;
                    
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string rowId = Functions.ToString(reader["rowid"]);
                            string ref_id = Functions.ToString(reader["ref_id"]);
                            try
                            {
                                this.smsMessage = Functions.ToString(reader["sms"]);
                                this.loginAccount = Functions.ToString(reader["userid"]);
                                this.isSendSMS = Functions.ToNumber(reader["is_send_sms"]);
                                this.smsMessageAR = Functions.ToString(reader["sms_ar"]);
                                this.smsMessageKU = Functions.ToString(reader["sms_ku"]);
                                this.smsMessageEN = Functions.ToString(reader["sms_en"]);
                                this.totalRecord = Functions.ToLong(reader["total_record"]);
                                this.convertParams = Functions.ToString(reader["convert_params"]);
                                this.systemCommandId = Functions.ToNumber(reader["systemcommandid"]);
                                this.resetValuePath = Functions.ToString(reader["reset_value_path"]);
                                this.isReset = Functions.ToNumber(reader["IS_RESET_COMMAND"]);
                                this.lang = Functions.ToNumber(reader["lang"]);

                            }
                            catch (Exception ex)
                            {
                                logger.AddtoLog(ex, Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                                logger.AddtoLog("Could not get SMS messaging options", Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                            }

                            this.lastBulkMapperRefID = ref_id;

                            i++;
                        }

                        try
                        {
                            reader.Dispose();
                        }
                        catch { }
                        try
                        {
                            command.Dispose();
                        }
                        catch { }
                    }
                }
                catch (Exception outerEx)
                {
                    logger.AddtoLog(outerEx, Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                    logger.AddtoLog("Could not get non requested map record", Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                           
                }
            }
            else
            {
                try
                {
                    RequestBulkRequestQueue();
                }
                catch (Exception externalEx)
                {
                    logger.AddtoLog(externalEx, Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                    logger.AddtoLog("Could not prepare for bulk request queue collection", Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                }
            }

        }

        /// <summary>
        /// Bulk Request Queue
        /// </summary>
        private void RequestBulkRequestQueue()
        {

            ConcurrentDictionary<int, BulkElement> commandList = new ConcurrentDictionary<int, BulkElement>();

            using (var command = this.db.GetCommand())
            {
                OracleDataReader reader = null;
                try
                {
                    OracleParameter isCompletedParam = command.CreateParameter();
                    isCompletedParam.ParameterName = ":iscompleted";
                    isCompletedParam.DbType = DbType.String;
                    isCompletedParam.Value = "false";
                    command.Parameters.Add(isCompletedParam);


                    OracleParameter applicationIdParam = command.CreateParameter();
                    applicationIdParam.ParameterName = ":application_id";
                    applicationIdParam.DbType = DbType.String;
                    applicationIdParam.Value = this.ApplicationID;
                    command.Parameters.Add(applicationIdParam);

                    //OracleParameter refIdParam = command.CreateParameter();
                    //refIdParam.ParameterName = ":ref_id";
                    //refIdParam.DbType = DbType.String;
                    //refIdParam.Value = this.lastBulkMapperRefID;
                    //command.Parameters.Add(refIdParam);

                    OracleParameter rowNumParam = command.CreateParameter();
                    rowNumParam.ParameterName = ":numRow";
                    rowNumParam.DbType = DbType.String;
                    rowNumParam.Value = numRequest;
                    command.Parameters.Add(rowNumParam);

                    string sql = "select id, rowid, command1, command2, command3, command4, value1, value2, value3, value4, status, application_id, " +
                                 " ref_id, run_count, user_id, login_account from " + this.migrate_table + " ";
                    sql += " where status = :iscompleted and application_id = :application_id and rownum <= :numRow and ref_id = '" + this.lastBulkMapperRefID +"'";
                    
                    command.CommandText = sql;

                    command.CommandType = CommandType.Text;
                    
                    int i = 0;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        string rowId = Functions.ToString(reader["rowid"]);
                        string id = Functions.ToString(reader["id"]);
                        string command1 = Functions.ToString(reader["command1"]);
                        string command2 = Functions.ToString(reader["command2"]);
                        string command3 = Functions.ToString(reader["command3"]);
                        string command4 = Functions.ToString(reader["command4"]);
                        string value1 = Functions.ToString(reader["value1"]);
                        string value2 = Functions.ToString(reader["value2"]);
                        string value3 = Functions.ToString(reader["value3"]);
                        string value4 = Functions.ToString(reader["value4"]);
                        string user_id = Functions.ToString(reader["user_id"]);
                        string login_account = Functions.ToString(reader["login_account"]);
                        string status = Functions.ToString(reader["status"]);
                        string application_id = Functions.ToString(reader["application_id"]);
                        string ref_id = Functions.ToString(reader["ref_id"]);
                        int run_round = Functions.ToNumber(reader["run_count"]);


                        commandList.TryAdd(i, new BulkElement
                        {
                            rowid = rowId,
                            command1 = command1,
                            command2 = command2,
                            command3 = command3,
                            command4 = command4,
                            value1 = value1,
                            value2 = value2,
                            value3 = value3,
                            value4 = value4,
                            status = status,
                            application_id = application_id,
                            ref_id = ref_id,
                            id = id,
                            run_count = run_round,
                            user_id = user_id,
                            login_account = login_account,
                            is_send_sms = this.isSendSMS,
                            lang = this.lang,
                            sms = this.smsMessage,
                            sms_ar = this.smsMessageAR,
                            sms_en = this.smsMessageEN,
                            sms_ku = this.smsMessageKU,
                            convert_params = this.convertParams,
                            systemcommandid = this.systemCommandId,
                            is_reset = this.isReset,
                            reset_value_path = this.resetValuePath
                            
                        });

                        i++;

                    }

                    
                }catch(Exception ex){
                    //ex.StackTrace();
                }

                try
                {
                    reader.Dispose();
                }
                catch { }
                try
                {
                    command.Dispose();
                }
                catch { }
            }

            for (int j = 0; j < commandList.Count; j++)
            {
                commandList[j].status = "prepared";

                string sql = "update " + this.migrate_table + " set status = 'prepared' where rowid = '" + commandList[j].rowid + "'";
                try
                {
                    using (var command = this.db2.GetCommand())
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    commandList[j].status = "failed";
                    throw new Exception("Error could not able to execute the command");
                }
            }

            for(int d = 0; d < worker.Length; d ++){
                for (int e = 0; e < commandList.Count; e++)
                {
                    worker[d].Execute(commandList[e]);
                }
            }
            
            // reset to get new map execution
            if (commandList.IsEmpty == true)
            {                               
                string sql = "update tbl_bulk_execute_map set is_executed = 1 where ref_id = '" + lastBulkMapperRefID + "'";
                try
                {
                    using (var command = this.db2.GetCommand())
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    string message = String.Format("Bulk Execution for RefID: {0} has completed!", lastBulkMapperRefID);

                    logger.AddtoLog(String.Format("Sending notification of completion RefID {0} ", lastBulkMapperRefID), LoggerLevel.Error);

                    if (GlobalObject.interruptedExecution == true)
                    {
                        message = String.Format("Process aborted due to could not connect to HRMS Schema, RefID: {0}", lastBulkMapperRefID); ;
                        NotifyCompletion(message);
                        GlobalObject.interruptedExecution = false;
                    }
                    else
                    {
                        NotifyCompletion(message);
                    }
                    
                    logger.AddtoLog(String.Format("Sent notification of completion RefID {0} ", lastBulkMapperRefID), LoggerLevel.Error);
                }
                catch (Exception e)
                {
                    logger.AddtoLog(e, LoggerLevel.Error);
                    logger.AddtoLog(String.Format("Error could not update bulk reference mapping id {0} to executed status", lastBulkMapperRefID), LoggerLevel.Error);
                    throw new Exception("Error could reset bulk reference id mapping");
                 
                }
                this.lastBulkMapperRefID = String.Empty; // reset the bulk referenceid
            }
        }

        private void NotifyCompletion(string message)
        {
            var entpoint = GlobalObject.SignalRConnnectionURL.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var socket in entpoint)
            {
                var hubConnection = new HubConnection(socket.Trim());
                try
                {
                    IHubProxy bulkNotificationProxy = hubConnection.CreateHubProxy("NotificationManagerHub");
                    //stockTickerHubProxy.On<Stock>("UpdateStockPrice", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));
                    hubConnection.Start().Wait();
                    //List<object> paramlist1 = new List<object>();
                    //paramlist1.Add(this.loginAccount);
                    //bulkNotificationProxy.Invoke("Hello", paramlist1.ToArray());
                    List<object> paramlist = new List<object>();
                  
                    logger.AddtoLog(String.Format("Notified to ", lastBulkMapperRefID), LoggerLevel.Error);
                  
                    paramlist.Add(this.loginAccount);
                    paramlist.Add(message);
                    bulkNotificationProxy.Invoke("BulkProcessCompletionNotification", paramlist.ToArray());
                }
                finally
                {
                    try
                    {
                        hubConnection.Dispose();
                    }
                    catch { 
                    }
                }
            }
             
        }        

        /// <summary>
        /// Dispose resource
        /// </summary>
        public void Dispose()
        {
            foreach (var x in worker)
            {
                try
                {
                    x.Dispose();
                }
                catch { }
            }            
        }
    }
}
