using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.LibDatabase;
using System.Collections.Concurrent;
using Asiacell.ITADLibWCF.Service;
using Newtonsoft.Json.Linq;
using System.Threading;
using Oracle.DataAccess.Client;
using System.Data;
using Asiacell.ITADLibWCF.Business;
using BulkRequestConsole.Utilities;

namespace BulkRequestConsole.Classes
{
    public class BulkRequestPool : TaskPool, IDisposable
    {
        public static volatile object sysObject = new object();
        private ServerClient serverClient = null;
        private string serviceUrl = null;
        LoggerEntities logger;
        //private IResultPushToDB pushToDB;
        string GWUser;
        DBConnection db;
        string GWPassword;
        string MigrateTable;
        IBulkReqester requester;

        public BulkRequestPool(BlockingCollection<object> IdleWorker, LoggerEntities logger,   string serviceUrl,  
            DBConnection db, string gWUser, string gWPassword, string migrateTable, IBulkRequesterFactory requesterFactory)
            : base(IdleWorker, logger)
        {             
            this.serverClient = new ServerClient(1, serviceUrl);            
            
            this.logger = logger;
            this.serviceUrl = serviceUrl;
            this.GWUser = gWUser;
            this.GWPassword = gWPassword;
            this.MigrateTable = migrateTable;
            this.requester = requesterFactory.Create();
            this.db = db;
        }      

        public override object RunWork(object command)
        {
            string PID = Functions.GetPID;
            return RequestExecute(PID,command);            
        }        
        
        /// <summary>
        /// Server Request
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private object RequestExecute(string PID, object command)
        {            
            BulkElement bulk = (BulkElement)command;

            if (bulk.status != "prepared")
                return null;

            bool result = Update(PID, ref bulk, "running");

            if (result == false)
            {
                return null;
            }

            bulk.run_count += 1;

            requester.SetRunningStatus(bulk);

            ServerLoginObject serverLogin = new ServerLoginObject() { UserName = this.GWUser, Password = this.GWPassword };

            RequestObject requestObj = new RequestObject();

            RequestObject checkLanguageObject = new RequestObject();

            // check if 
            BasicSubscriberInformation basicInformation = FindMSISDNForResetCommandOrSendSMSCommandCondition(PID, bulk, serverLogin, ref checkLanguageObject);

            try
            {

                if (checkLanguageObject.Error_Code != 0 && (bulk.is_reset == 1 || bulk.is_send_sms == 1))
                {
                    Update(PID, ref bulk, "fatal");
                    MoveToLog(PID, bulk, checkLanguageObject);
                    logger.AddtoLog(PID, String.Format("Finished the request for execute on command: {0}, status: {1}, GW TranID: {2}, ErrorCode: {3}, Description: {4}", "GET Balance Info", "fatal", checkLanguageObject.Id, checkLanguageObject.Error_Code, checkLanguageObject.Description), LoggerLevel.Info);
                }
                else
                {
                    ApplyResetCommandIfWasEnable(bulk, basicInformation);

                    serverLogin.Command = bulk.command1;

                    // check if it is approval recharge
                    //AIR_APPROVED_EMPLOYEE_RECHARGE
                    bool okToGo = true;
                    if (bulk.command1 != null 
                        && (bulk.command1.IndexOf("AIR_APPROVED_EMPLOYEE_RECHARGE") > -1
                        //|| bulk.command1.IndexOf("GETACCOUNTDETAILS") > -1
                        ))
                    {
                        okToGo = false;
                        String sqlCheckHrms = @"select 1 from myhrms.tbl_recharges@to_addb where rownum = 1";
                          
                        ConcurrentDictionary<int, BulkExecuteMap> commandMapList = new ConcurrentDictionary<int, BulkExecuteMap>();
                        try
                        {
                            using (var commandHrms = this.db.GetCommand())
                            {
                                commandHrms.CommandText = sqlCheckHrms;
                                object output = commandHrms.ExecuteScalar();
                                okToGo = true;
                            }
                        }
                        catch (Exception e)
                        {
                            okToGo = false;
                            GlobalObject.interruptedExecution = true;
                            // asuume error
                            logger.AddtoLog(PID, String.Format("Failed to recharge employee line due to not able to connect with HRMS db: {0} aborted", serverLogin.Command), LoggerLevel.Fatal);
                        }

                    }

                    if (okToGo)
                    {
                        logger.AddtoLog(PID, String.Format("Start the request for execute of command: {0} ", serverLogin.Command), LoggerLevel.Info);

                        requester.ExecuteLogic(PID, ref requestObj, ref serverLogin, ref bulk, serverClient);

                        if (requestObj.Error_Code == 0)
                        {
                            Update(PID, ref bulk, "true");
                            requester.SetCompletedStatus(bulk);
                            MoveToLog(PID, bulk, requestObj);
                            SendToRechargeLogs(PID, bulk, requestObj);
                            SendSms(PID, bulk, requestObj, serverClient, basicInformation);
                            logger.AddtoLog(PID, String.Format("Finished the request for execute on command: {0}, status: {1}, GW TranID: {2}, ErrorCode: {3}, Description: {4}", serverLogin.Command, "success", requestObj.Id, requestObj.Error_Code, requestObj.Description), LoggerLevel.Info);
                        }
                        else
                        {
                            Update(PID, ref bulk, "fatal");
                            MoveToLog(PID, bulk, requestObj);
                            logger.AddtoLog(PID, String.Format("Finished the request for execute on command: {0}, status: {1}, GW TranID: {2}, ErrorCode: {3}, Description: {4}", serverLogin.Command, "fatal", requestObj.Id, requestObj.Error_Code, requestObj.Description), LoggerLevel.Info);
                        }
                    }
                    else
                    {
                        bulk.transaction_id = "no-tran-id";
                        bulk.error_code = -1;
                        bulk.result = "Process aborted due to could not connect to HRMS Schema";
                        bulk.user_id = Functions.ToString(serverLogin.UserName);
                        requestObj.Requested_Date = new DateTime();
                        requestObj.Respond_Date = new DateTime();
                        requestObj.Id = "no-tran-id";
                        requestObj.Description = "Process aborted due to could not connect to HRMS Schema";
                        Update(PID, ref bulk, "fatal");
                        MoveToLog(PID, bulk, requestObj);
                        logger.AddtoLog(PID, String.Format("Finished the request for execute on command: {0}, Description: Could not connected with HRMS, process aborted", serverLogin.Command, "fatal"), LoggerLevel.Fatal);
                    }
                }

            }
            catch (Exception e)
            {
                requestObj.Error_Code = -11;
                requestObj.Id = Asiacell.ITADLibraries.Utilities.Functions.GetPID;
                requestObj.Result = e.Message;
                requestObj.Description = bulk.rowid;
                logger.AddtoLog(PID, String.Format("{0}, {2}", "Error cannot execute bulk request Command",requestObj.Id, e.Message), e, LoggerLevel.Info);
            }
            return requestObj;             
        }

        private static void ApplyResetCommandIfWasEnable(BulkElement bulk, BasicSubscriberInformation basicInformation)
        {
            if (bulk.is_reset == 1)
            {
                if (basicInformation.BalanceAmount > 0)
                {
                    // transform command for reset command
                    bulk.command1 = String.Format("{0} -{1}", bulk.command1, basicInformation.BalanceAmount);
                }
                else
                {
                    bulk.command1 = String.Format("{0} {1}", bulk.command1, basicInformation.BalanceAmount);
                }
            }
        }

        private BasicSubscriberInformation FindMSISDNForResetCommandOrSendSMSCommandCondition(string PID, BulkElement bulk, ServerLoginObject serverLogin, 
               ref RequestObject checkLanguageObject)
        {
            BasicSubscriberInformation basicInformation = new BasicSubscriberInformation();

            if (bulk.is_reset == 1 || bulk.is_send_sms == 1)
            {


                var msisdns = Functions.SpliteBySplace(bulk.command1);
                foreach (var msisdn in msisdns)
                {
                    if (Functions.IsMSISDN(msisdn))
                    {
                        bulk.msisdn = msisdn;
                        break;
                    }
                }


                // check language
                basicInformation = requester.GetBasicSubscriberBasicInformation(PID, bulk.msisdn, ref checkLanguageObject, serverLogin, serverClient);

            }
            return basicInformation;
        }


        public void SendToRechargeLogs(String PID, BulkElement obj, RequestObject rb)
        {

            if (GlobalObject.RechargeLogCommands.Contains(String.Format("[{0}]", rb.CommandID)) && (obj.is_reset == 1 || obj.is_reset == 0) )
            {

                String msisdn = "";

                try {
               
                    var converters = Functions.ToString(obj.convert_params).Split("|".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                    dynamic amount = null;

                    foreach (var ix in rb.CommandParamaterValues)
                    {
                        if (String.IsNullOrWhiteSpace(obj.convert_params) == false)
                        {
                            string findParameter = ix.Name;
                            amount = ParameterValueConvertionForRecharge(converters, ix, findParameter);
                        }
                        else
                        {
                            if ("%AMOUNT%" == ix.Name)
                            {
                                // convert to IQD
                                amount = Functions.ToLong(ix.Value) / 1000;
                            }
                            else if ("%VOLUME%" == ix.Name.Trim())
                            {
                                // convert to VOLUME to GB
                                amount = LocalFunctions.RoundUp(((Functions.ToDouble(ix.Value) / 1024) / 1024) / 1024);
                            }

                        }

                        if (ix.Name.Contains("%MSISDN%"))
                        {
                            if (String.IsNullOrWhiteSpace(obj.msisdn))
                            {
                                obj.msisdn = ix.Value;
                            }
                        }
                    }

                    if (amount != null)
                    {
                        msisdn = obj.msisdn;

                        OracleCommand command;
                        command = new OracleCommand();
                        command.Connection = db.GetDBconnection;

                        //command.CommandText = "myhrms.prc_individual_charging_log@to_entprod";
                        command.CommandText = "myhrms.prc_individual_charging_log@to_addb";
                        

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "msisdn";
                        parameter.Value = msisdn;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        var parameter2 = command.CreateParameter();
                        parameter2.ParameterName = "amount";
                        parameter2.Value = amount * 1000;
                        parameter2.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter2);

                        command.CommandType = CommandType.StoredProcedure;
                        command.Prepare();
                        command.ExecuteNonQuery();
                        logger.AddtoLog(PID, String.Format("Finished add msisdn {0} to recharge log table", msisdn), LoggerLevel.Info);
                    }
                    else
                    {
                        logger.AddtoLog(PID, String.Format("Could not able to get amount/volume for add to msisdn {0} to recharge log table", msisdn), LoggerLevel.Info);
                    }
                }
                catch (Exception e)
                {
                    logger.AddtoLog(PID, String.Format("Error could not add msisdn {0} to recharge log table", msisdn), LoggerLevel.Error);
                    logger.AddtoLog(PID, e, LoggerLevel.Error);
                }
             }

        }

        /// <summary>
        /// To Send SMS base on subscriber profile
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="obj"></param>
        /// <param name="rb"></param>
        /// <param name="serverClient"></param>
        public void SendSms(string PID, BulkElement obj, RequestObject rb, ServerClient serverClient, 
            BasicSubscriberInformation basicInformation)
        {
            OracleCommand commSms = null;
            try
            {
                // insert sms
                if (obj.is_send_sms == 1)
                {
                    

                    //Lang: 3-english, 8-unicode, smsc_id:1-suly, 2-Erbil, 3-Baghdad
                    string sql = @"insert into smppgw.tblmsg_send
                                        ( sendermisisdn, receivermsisdn, vas_service, message, rqtstatus, send_on, status, lang, smsc_id)
                                      values
                                        ('ASIACELL', :msisdn, 0, :message, 0, sysdate, 0, :lang, 1)";
                    commSms = new OracleCommand();
                    commSms.Connection = db.GetDBconnection;
                    commSms.CommandText = sql;
                    commSms.CommandType = CommandType.Text;

                    string smsMessage = obj.sms;

                    int lang = ApplySMSBaseOnlyProfileLanguage(obj, basicInformation, ref smsMessage);
                    
                    CreateParameter(":msisdn", OracleDbType.Varchar2, ref commSms, obj.msisdn);

                    var converters = Functions.ToString(obj.convert_params).Split("|".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var ix in rb.CommandParamaterValues)
                    {
                        if (String.IsNullOrWhiteSpace(obj.convert_params) == false)
                        {
                            string findParameter = ix.Name;

                            smsMessage = ParameterValueConvertion(smsMessage, converters, ix, findParameter);
                        }
                        else
                        {
                            if ("%AMOUNT%" == ix.Name)
                            {
                                // convert to IQD
                                long x = Functions.ToLong(ix.Value) / 1000;
                                smsMessage = smsMessage.Replace(ix.Name, Functions.ToString(x) + "");
                            }
                            else if ("%VOLUME%" == ix.Name.Trim())
                            {
                                // convert to VOLUME to GB
                                var x = LocalFunctions.RoundUp(((Functions.ToDouble(ix.Value) / 1024) / 1024) / 1024);
                                smsMessage = smsMessage.Replace(ix.Name, Functions.ToString(x) + "");
                            }
                            else
                            {
                                smsMessage = smsMessage.Replace(ix.Name, ix.Value);
                            }
                        }
                    }
                    
                    CreateParameter(":message", OracleDbType.NVarchar2, ref commSms, smsMessage);
                    CreateParameter(":lang", OracleDbType.Int32, ref commSms, lang);

                    commSms.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                logger.AddtoLog(PID,ex, Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
                logger.AddtoLog(PID, String.Format("TranID = {0}, {0}",rb.Id, "Could not send SMS"), Asiacell.ITADLibraries.LibLogger.LoggerLevel.Error);
            }
            finally
            {
                if (commSms != null)
                {
                    try { commSms.Dispose(); }
                    catch { }
                }
            }
        }

        private static string ParameterValueConvertion(string smsMessage, string[] converters, NameValuePair ix, string findParameter)
        {
            var convert = converters.FirstOrDefault(s => s.Contains(findParameter));

            if (convert != null)
            {
                var convertBody = convert.Split("=".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (convertBody.Count() > 1)
                {
                    dynamic parameterValue = GetValidFormattedValue(ix, convertBody);

                    // value
                    smsMessage = smsMessage.Replace(ix.Name, String.Format("{0}", parameterValue));   
                }
            }
            return smsMessage;
        }

        private static dynamic ParameterValueConvertionForRecharge(string[] converters, NameValuePair ix, string findParameter)
        {
            var convert = converters.FirstOrDefault(s => s.Contains(findParameter));

            if (convert != null)
            {
                var convertBody = convert.Split("=".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (convertBody.Count() > 1)
                {
                    dynamic parameterValue = GetValidFormattedValue(ix, convertBody);

                    return parameterValue;
                }

            }
            return null;
        }

        private static dynamic GetValidFormattedValue(NameValuePair ix, string[] convertBody)
        {
            var paramName = convertBody[0];

            var paramFormula = convertBody[1].Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var parameter = paramName.Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            dynamic parameterValue = null;


            if (parameter[1] == "L")
            {
                parameterValue = Functions.ToLong(ix.Value);
            }

            foreach (var f in paramFormula)
            {

                if (f.Contains("/")) // mean divide.
                {
                    var valueExtraction = f.Substring(1).Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    parameterValue = parameterValue / Functions.ToLong(valueExtraction[0]);
                }
                else
                {
                    if (f.Contains("ROUNDUP"))
                    {

                        parameterValue = LocalFunctions.RoundUp(parameterValue);
                    }
                }
            }
            return parameterValue;
        }

        private static int ApplySMSBaseOnlyProfileLanguage(BulkElement obj, BasicSubscriberInformation basicInformation, ref string smsMessage)
        {
            int lang = obj.lang;

            if (basicInformation.Lanauage == 1)
            {
                smsMessage = obj.sms_ar;
                lang = 8;
            }
            else if (basicInformation.Lanauage == 2)
            {
                smsMessage = obj.sms_ku;
                lang = 8;
            }
            else if(basicInformation.Lanauage == 3)
            {
                smsMessage = obj.sms_en;
                lang = 1;
            }
            return lang;
        }

        public void Clean(string PID, BulkElement obj, RequestObject r)
        {
            //lock (this)
            //{
                string sql =
@"delete tbl_bulk_execute where rowid = '" + obj.rowid + "'";
                int i = 0;
                OracleCommand command = null;
                try
                {
                    command = new OracleCommand();
                    command.Connection = db.GetDBconnection;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;                        
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    logger.AddtoLog(PID, String.Format("TranID = {0}",r.Id), e, LoggerLevel.Error);
                    logger.AddtoLog(PID,  String.Format("{0}, TranID = {0}","Could not transfer record from tbl_bulk_execute to tbl_bulk_execute_log", r.Id), LoggerLevel.Error);
                }
                finally
                {
                    if (command != null)
                    {
                        try { command.Dispose(); }
                        catch { }
                    }
                }


            //}
        }

        public void MoveToLog(string PID, BulkElement obj, RequestObject requestObject)
        {

                string sql =
                            @"insert into tbl_bulk_execute_log  (command1, value1, application_id, ref_id, status, run_count, id, result,
                                     error_code, user_id, transaction_id, login_account, description,msisdn)
                             values (:command1, :value1, :application_id, :ref_id, :status, :run_count, :id, :result, 
                                    :error_code, :user_id, :transaction_id, :login_account, :description,:msisdn)";
                
               // int i = 0;
                OracleCommand command = null;
                bool isSuccess = false;
                try
                {
                    command = new OracleCommand();
                    command.Connection = db.GetDBconnection;
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    
                        CreateParameter(":command1", OracleDbType.Varchar2, ref command, obj.command1);
                        //createParameter(":command2", OracleDbType.Varchar2, ref command, obj.command2);
                        //createParameter(":command3", OracleDbType.Varchar2, ref command, obj.command3);
                        //createParameter(":command4", OracleDbType.Varchar2, ref command, obj.command4);

                        CreateParameter(":value1", OracleDbType.Varchar2, ref command, obj.value1);
                        //createParameter(":value2", OracleDbType.Varchar2, ref command, obj.value2);
                        //createParameter(":value3", OracleDbType.Varchar2, ref command, obj.value3);
                        //createParameter(":value4", OracleDbType.Varchar2, ref command, obj.value4);

                        CreateParameter(":application_id", OracleDbType.Varchar2, ref command, obj.application_id);
                        CreateParameter(":ref_id", OracleDbType.Varchar2, ref command, obj.ref_id);
                        CreateParameter(":status", OracleDbType.Varchar2, ref command, obj.status);
                        CreateParameter(":run_count", OracleDbType.Int32, ref command, obj.run_count);
                        //createParameter(":request_date", OracleDbType.TimeStamp, ref command, r.Requested_Date);
                        CreateParameter(":id", OracleDbType.Varchar2, ref command, obj.id);

                        //if (obj.result != null && !obj.result.Trim().Equals("") && obj.result.Length > 4000)
                        //    CreateParameter(":result", OracleDbType.Clob, ref command, obj.result.Substring(0, 4000));
                        //else
                        CreateParameter(":result", OracleDbType.Clob, ref command, obj.result);

                        CreateParameter(":error_code", OracleDbType.Int32, ref command, obj.error_code);
                        CreateParameter(":user_id", OracleDbType.Varchar2, ref command, obj.user_id);
                        CreateParameter(":transaction_id", OracleDbType.Varchar2, ref command, obj.transaction_id);
                        CreateParameter(":login_account", OracleDbType.Varchar2, ref command, obj.login_account);
                        //createParameter(":respond_date", OracleDbType.TimeStamp, ref command, r.Respond_Date);
                        if (requestObject.Description != null && !requestObject.Description.Trim().Equals("") && requestObject.Description.Length > 250)
                            CreateParameter(":description", OracleDbType.Varchar2, ref command, requestObject.Description.Substring(0, 250));
                        else
                            CreateParameter(":description", OracleDbType.Varchar2, ref command, requestObject.Description);
                        CreateParameter(":msisdn", OracleDbType.Varchar2, ref command, obj.msisdn);  
                        command.ExecuteNonQuery();
                        isSuccess = true;
                  
                }
                catch (Exception e)
                {
                    
                    logger.AddtoLog(PID, e, LoggerLevel.Error);
                    logger.AddtoLog(PID, String.Format("S = {0} End = {1}, GW TranID = {2}", requestObject.Requested_Date.ToLongDateString(), requestObject.Respond_Date.ToLongDateString(),requestObject.Id), LoggerLevel.Error);
                    logger.AddtoLog(PID, String.Format("Could not transfer record from tbl_bulk_execute to tbl_bulk_execute_log. GW TranID = {2}",requestObject.Id), LoggerLevel.Error);
                }
                finally
                {
                    if (command != null)
                    {
                        try { command.Dispose(); }
                        catch { }
                    }
                }

                if (isSuccess == true)
                {
                    Clean(PID,obj, requestObject);
                }
            
            //}
        }

        private void CreateParameter(string name, OracleDbType dbType, ref OracleCommand command, object value)
        {
            OracleParameter statusParam = command.CreateParameter();
            statusParam.ParameterName = name;
            statusParam.OracleDbType = dbType;
            statusParam.Value = value;
            command.Parameters.Add(statusParam);            
        }
         

        public bool Update(string PID, ref BulkElement obj, string status)
        {
            string sql = "";

            sql = "update " + this.MigrateTable + " set status = :status, user_id = '" + obj.user_id + "', run_count = :run_count, transaction_id = :transaction_id, error_code = " + Functions.ToNumber(obj.error_code).ToString() + ", result = :result where rowid = '" + obj.rowid + "'";
 
            int i = 0;
            try
            {
                using (var command = this.db.GetCommand())
                {
                    try
                    {
                        OracleParameter statusParam = command.CreateParameter();
                        statusParam.ParameterName = ":status";
                        statusParam.DbType = DbType.String;
                        statusParam.Value = status;
                        command.Parameters.Add(statusParam);

                        OracleParameter runCountParam = command.CreateParameter();
                        runCountParam.ParameterName = ":run_count";
                        runCountParam.DbType = DbType.Int32;
                        runCountParam.Value = obj.run_count;
                        command.Parameters.Add(runCountParam);

                        //OracleParameter error_code = command.CreateParameter();
                        //error_code.ParameterName = ":error_code";
                        //error_code.DbType = DbType.Int32;
                        //error_code.Value = Functions.ToNumber(obj.error_code);
                        //command.Parameters.Add(error_code);

                        OracleParameter transaction_id = command.CreateParameter();
                        transaction_id.ParameterName = ":transaction_id";
                        transaction_id.DbType = DbType.String;
                        transaction_id.Value = obj.transaction_id;
                        command.Parameters.Add(transaction_id);

                        //OracleParameter user_id = command.CreateParameter();
                        //user_id.ParameterName = ":user_id";
                        //user_id.DbType = DbType.String;
                        //user_id.Value = obj.user_id;
                        //command.Parameters.Add(user_id);

                        OracleParameter result = command.CreateParameter();
                        result.ParameterName = ":result";
                        result.DbType = DbType.String;
                        result.Value = obj.result;
                        command.Parameters.Add(result);

                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        i = command.ExecuteNonQuery();

                        logger.AddtoLog(PID,"Updated status = " + status + " for rowid= " + obj.rowid, LoggerLevel.Info);
                    }
                    catch (Exception ex)
                    {
                        logger.AddtoLog(PID,ex, LoggerLevel.Error);
                        logger.AddtoLog(PID,"Error could not update request execution row with status = " + status + " for rowid= " + obj.rowid, LoggerLevel.Error);
                    }
                    finally
                    {
                        try
                        {
                            command.Dispose();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception e)
            {
                logger.AddtoLog(PID,e, LoggerLevel.Error);
                logger.AddtoLog(PID,"Error update rowid " + obj.rowid, LoggerLevel.Error);
            }

            if (i > 0)
            {
                obj.status = status;
                return true;
            }
            else
            {                
                return false;
            }            
        }                   

        public override void Dispose()
        {             
            try
            {
                serverClient.Dispose();
            }
            catch { }
            base.Dispose();
        }
    }
}
