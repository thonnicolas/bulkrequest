using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibWCF.Business;
using Asiacell.ITADLibWCF.Service;
using System.Threading;
using Asiacell.ITADLibraries.Utilities;
using Newtonsoft.Json.Linq;
using BulkRequestConsole.Classes;
using BulkRequestConsole.Utilities;

namespace BulkRequestConsole.Classes
{
    public class BasicSubscriberInformation
    {
        public int Lanauage { get; set; }
        public int ServiceClass { get; set; }
        public long BalanceAmount { get; set; }

        public long VolumneAmount { get; set; }
    }
    /// <summary>
    /// Batch Processsing
    /// </summary>
    public class BatchProcessingLogic : IBulkReqester
    {
        LoggerEntities logger;
        DBConnection db;

        /// <summary>
        /// Batch process constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="db"></param>
        public BatchProcessingLogic(LoggerEntities logger, DBConnection db)
        {
            this.db = db;
            this.logger = logger;
        }

        public bool SetRunningStatus(BulkElement obj)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void ExecuteLogic(string PID, ref RequestObject requestObj, ref ServerLoginObject serverLogin, ref BulkElement bulk, ServerClient serverClient)
        {
            #region Sample code
            /* Sample code
            serverLogin.Command = bulk.command1;

            bool isTest = true;

            if (isTest == false)
            {
                requestObj = serverClient.Authenticate(serverLogin);
            }
            else
            {
                Thread.Sleep(100); // for test
                requestObj.Error_Code = 0; // for test
            }

            if (requestObj.Id == null)
            {
                requestObj.Id = Asiacell.ITADLibraries.Utilities.Functions.GetPID;
            }
            if (requestObj.Error_Code != 0)
            {
                requestObj.Id = Asiacell.ITADLibraries.Utilities.Functions.GetPID;
            }

            logger.AddtoLog(requestObj.Id, "Respond from server with RefID: " + serverLogin.RefID + ", Command: " + serverLogin.Command + ", Error_code: " + requestObj.Error_Code + ", Descriptin: " + requestObj.Description, LoggerLevel.Info);
            
            if (requestObj.Error_Code == 0)
            {
                double point = 0;

                if (isTest == false)
                {
                    JsonObject jParser = new JsonObject();
                    var a = jParser.GetJsonQueryObject(JObject.Parse(requestObj.Result), "dedicatedAccountInformation").ElementWhere("dedicatedAccountID", "20");
                    if (a != null)
                    {
                        var cur_point = jParser.GetJsonQueryObject(JObject.Parse(a.ToString()), "dedicatedAccountValue1").GetValue();
                        if (cur_point == null)
                            point = 0;
                        else
                            point = double.Parse(cur_point);
                    }
                    else
                        point = 0;


                    point = ((point / 1000) + Convert.ToDouble(bulk.value1)) * 1000;
                }


                bulk.command2 = String.Format(bulk.command2, point);

                //airBulkElement.original_balance = 300.0; // for test

                serverLogin.Command = bulk.command2;

                if (isTest == false)
                {
                    requestObj = serverClient.Authenticate(serverLogin); // production
                }
                else
                {
                    Thread.Sleep(100); // for test
                    requestObj.Error_Code = 0; // for test
                }

                logger.AddtoLog(requestObj.Id, "Respond from server with RefID: " + serverLogin.RefID + ", Command: " + serverLogin.Command, LoggerLevel.Info);
             }   
             */
            #endregion
            requestObj = serverClient.Authenticate(serverLogin);
            
            bulk.transaction_id = requestObj.Id;

            bulk.error_code = requestObj.Error_Code;
            bulk.result = requestObj.Result;
            bulk.user_id = Functions.ToString(requestObj.UserName);
            logger.AddtoLog(requestObj.Id, "Respond from server with RefID: " + serverLogin.RefID + ", Command: " + serverLogin.Command + ", Error_code: " + requestObj.Error_Code + ", Descriptin: " + requestObj.Description + ", Descriptin: " + requestObj.Result, LoggerLevel.Info);
            
        }


        public bool SetCompletedStatus(BulkElement obj)
        {
            return false;
        }


        public BasicSubscriberInformation GetBasicSubscriberBasicInformation(string PID, string MSIDN, 
            ref RequestObject requestObj, ServerLoginObject serverLogin, ServerClient serverClient)
        {
            ServerLoginObject serverLoginForBasicInformation = new ServerLoginObject();
            serverLoginForBasicInformation.UserName = serverLogin.UserName;
            serverLoginForBasicInformation.Password = serverLogin.Password;
            serverLoginForBasicInformation.Command = String.Format("{0} {1}", GatewayCommands.GETBALANDDATE, MSIDN); 
            requestObj = serverClient.Authenticate(serverLoginForBasicInformation);
          
           
            logger.AddtoLog(requestObj.Id, "Respond from server with RefID: " + serverLogin.RefID + ", Command: " + serverLogin.Command + ", Error_code: " + requestObj.Error_Code + ", Descriptin: " + requestObj.Description + ", Descriptin: " + requestObj.Result, LoggerLevel.Info);
           

            if (requestObj.Error_Code != 0)
            {
                GlobalObject.logger.AddtoLog(PID,  String.Format("Fail to basic information inquiry, error code: {0}, message: {1}, GW TransactionID: {2}", requestObj.Error_Code, requestObj.Description, requestObj.Id),
                    Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
                GlobalObject.logger.AddtoLog(PID, requestObj.Description, Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
                
                return null;
            }
            else
            {
                GlobalObject.logger.AddtoLog(PID, String.Format("GW TrasactionID: {0}",requestObj.Id), Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
                GlobalObject.logger.AddtoLog(PID, String.Format("{0}, GW TransactionID: {1}",requestObj.Result, requestObj.Id), Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
            }

            JObject obj = JObject.Parse(requestObj.Result);

            int serviceClass = Functions.ToNumber(obj.SelectToken("serviceClassCurrent"));
            int language = Functions.ToNumber(obj.SelectToken("languageIDCurrent"));
            long amount = Functions.ToLong(obj.SelectToken("accountValue1"));

            BasicSubscriberInformation basic = new BasicSubscriberInformation()
            {
                Lanauage = language,
                ServiceClass = serviceClass,
                BalanceAmount = amount
            };
            return basic;
        }

 
    }   
}
