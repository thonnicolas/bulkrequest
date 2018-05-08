using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibraries.LibLogger;
using BulkRequestConsole.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulkRequestConsole.Utilities
{
    public class GlobalObject
    {
        public static int bulkInsert = 10;
        public static volatile bool isStopped = false;                
        public static LoggerEntities logger;
        public static DBConnection db;
        public static CollectionManager collector;
        public static CancellationTokenSource canceller;
        public static CancellationToken token;
        public static Task taskJob = null;
        public static volatile bool interruptedExecution = false;

      //   /// <summary>
      //  /// Get Account Basic information
      //  /// </summary>
      //  /// <param name="newMSIDN"></param>
      //  /// <param name="amount"></param>
      //  /// <param name="volumeText"></param>
      //  /// <param name="suspendDate"></param>
      //  public static void GetSubscriberBasicInfo(string PID, string msisdn, out int amount, out string volumeText, 
      //      out string suspendDate, out string internetSuspendDate, ref bool success, ref string message,  Controller controller,List<KeyValuePair<string,string>> inquryResult)
      //  {
      //      int serviceClass;
      //      int language;
      //      string volumeBroadbandInternetText;
      //      string broadbandInternetSuspendDate;
      //      GetSubscribeBasicData(PID, msisdn, ref success, ref message, controller, out amount, out volumeText, out suspendDate,
      //          out internetSuspendDate, out serviceClass, out language,  out volumeBroadbandInternetText, out broadbandInternetSuspendDate,inquryResult);
      //  }

      //  /// <summary>
      //  /// Get Account Basic information
      //  /// </summary>
      //  /// <param name="newMSIDN"></param>
      //  /// <param name="amount"></param>
      //  /// <param name="volumeText"></param>
      //  /// <param name="suspendDate"></param>
      //  public static void GetSubscriberBasicInfo(string PID , string msisdn, out int amount, out string volumeText, 
      //      out string suspendDate, out string internetSuspendDate, out string volumeBroadbandInternetText, out string broadbandInternetSuspendDate, ref bool success, ref string message, Controller controller,List<KeyValuePair<string,string>> inquryResult)
      //  {
      //      int serviceClass;
      //      int language; 
      //      GetSubscribeBasicData(PID, msisdn, ref success, ref message, controller, out amount, out volumeText, out suspendDate,
      //          out internetSuspendDate, out serviceClass, out language,  out volumeBroadbandInternetText, out broadbandInternetSuspendDate, inquryResult);
      //  }
      //  /// <summary>
      //  /// Subscribe information include service class and language
      //  /// </summary>
      //  /// <param name="msisdn"></param>
      //  /// <param name="amount"></param>
      //  /// <param name="volumeText"></param>
      //  /// <param name="suspendDate"></param>
      //  /// <param name="internetSuspendDate"></param>
      //  /// <param name="serviceClass"></param>
      //  /// <param name="language"></param>
      //  /// <param name="success"></param>
      //  /// <param name="message"></param>
      //  /// <param name="controller"></param>
      //  public static void GetSubscriberBasicInfo(string PID, string msisdn, out int amount, out string volumeText, 
      //      out string suspendDate, out string internetSuspendDate, out int serviceClass, 
      //      out int language, ref bool success, ref string message, Controller controller,List<KeyValuePair<string,string>> inquryResult)
      //  {
      //      string volumeBroadbandInternetText;
      //      string broadbandInternetSuspendDate;
      //      GetSubscribeBasicData(PID, msisdn, ref success, ref message, controller, out amount, out volumeText, out suspendDate,
      //          out internetSuspendDate, out serviceClass, out language, out volumeBroadbandInternetText, out broadbandInternetSuspendDate, inquryResult);
      //  }

      //public static void GetSubscriberBasicInfo(string PID, string msisdn, out int amount, out string volumeText, 
      //      out string suspendDate, out string internetSuspendDate, out int serviceClass, 
      //      out int language,  out string volumeBroadbandInternetText,  out string broadbandInternetSuspendDate, 
      //      ref bool success, ref string message, Controller controller,List<KeyValuePair<string,string>> inquryResult)
      //  {
      //      GetSubscribeBasicData(PID, msisdn, ref success, ref message, controller, out amount, out volumeText, out suspendDate,
      //          out internetSuspendDate, out serviceClass, out language,  out volumeBroadbandInternetText, out broadbandInternetSuspendDate, inquryResult);
      //  }

      //  private static void GetSubscribeBasicData(string PID, string msisdn, ref bool success, ref string message, Controller controller, 
      //      out int amount, 
      //      out string volumeText, 
      //      out string suspendDate, 
      //      out string internetSuspendDate, 
      //      out int serviceClass,
      //      out int language,
      //      out string volumeBroadbandInternetText,
      //      out string broadbandInternetSuspendDate,
      //      List<KeyValuePair<string,string>> inquryResult)
      //  {
      //      string newMSIDN = String.Format("964{0}", msisdn.Substring(1));
      //      internetSuspendDate = String.Empty;
      //      amount = 0;
      //      volumeText = "";
      //      suspendDate = "";
      //      language = 0;
      //      serviceClass = 0;
      //      volumeBroadbandInternetText = "";
      //      broadbandInternetSuspendDate = "";


      //      GlobalObject.logger.AddtoLog(PID, String.Format("Subscriber {0} request for balance inquiry", newMSIDN), Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);

      //      var command = String.Format("{0} {1}", GatewayCommands.GETBALANDDATE, newMSIDN); 

      //      RequestObject request = Function.ExecuteGatewayCommand(PID, command);

      //      if (request.Error_Code != 0)
      //      {
      //          GlobalObject.logger.AddtoLog(PID,  String.Format("Fail to inquire balance for subscriber {0}, error code: {1}, message: {2}, GW TransactionID: {3}", newMSIDN, request.Error_Code, request.Description, request.Id),
      //              Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
      //          GlobalObject.logger.AddtoLog(PID, request.Description, Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
      //          success = false;
      //          message = request.Description;
      //          return;
      //      }
      //      else
      //      {
      //          GlobalObject.logger.AddtoLog(PID, String.Format("Successfully inquire balance for subscriber {0}, GW TrasactionID: {1}", newMSIDN,request.Id), Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
      //          GlobalObject.logger.AddtoLog(PID, String.Format("{0}, GW TransactionID: {1}",request.Result, request.Id), Asiacell.ITADLibraries.LibLogger.LoggerLevel.Info);
      //      }

      //      JObject obj = JObject.Parse(request.Result);

      //      amount = Functions.ToNumber(Functions.ToNumber(obj.SelectToken("accountValue1")) / 1000);
      //      volumeText = null;

      //      serviceClass = Functions.ToNumber(obj.SelectToken("serviceClassCurrent"));
      //      language = Functions.ToNumber(obj.SelectToken("languageIDCurrent"));

      //      if (serviceClass < 500) // pospaid
      //      {
      //          string formatValue = TextTranslation.ApplyLanguageForText("{0} IQD",controller);

      //          if(inquryResult != null)
      //              inquryResult.Add(new KeyValuePair<string, string>("Balance",String.Format(formatValue, amount)));
      //      }
      //      else // prepaid
      //      {
      //          bool postpaidSuccess = true;
      //          string postpaidMessage = String.Empty;
      //          decimal advanceBalance = 0;
      //          decimal billedAmount = 0;
      //          decimal unBilledAmount = 0;
      //          GetSubscribeBasicDataUsageForPostpaid(PID, msisdn, ref postpaidSuccess, ref postpaidMessage, controller, out advanceBalance, out billedAmount, out unBilledAmount);

      //          if(inquryResult != null){

      //              string formatValue = TextTranslation.ApplyLanguageForText("{0} IQD",controller);

      //              inquryResult.Add(new KeyValuePair<string, string>("Unpaid Bills Amount",String.Format(formatValue, billedAmount)));
                
      //              //inquryResult.Add(new KeyValuePair<string, string>("Unbilled Amount",String.Format("{0} IQD", unBilledAmount)));
      //              inquryResult.Add(new KeyValuePair<string, string>("Unbilled Amount",String.Format(formatValue, unBilledAmount)));
      //          }
      //      }

      //      var expiryDate = Functions.ToString(obj.SelectToken("supervisionExpiryDate"));

      //      suspendDate = GetLocalizeDateFormat(controller, expiryDate);
            
           
      //      try
      //      {
      //          var dedicateAccountInformations = obj.SelectToken("dedicatedAccountInformation");

      //          GlobalObject.logger.AddtoLog(PID, String.Format("GW Tran ID: {0}, {1}",request.Id, "Get volume "), LoggerLevel.Info);

      //          if (dedicateAccountInformations != null)
      //          {
                    
      //              bool foundVolume = false;

                    
      //              if (serviceClass < 500)
      //              {
      //                  // get internet
      //                  foundVolume = false;
      //                  GetDAWithVolumeAndSuspendDate(PID,request, dedicateAccountInformations, ref volumeText, ref internetSuspendDate,  ref foundVolume, GlobalObject.PrepaidSpeendooBundleDA, controller);

      //                  if (foundVolume == true && inquryResult != null)
      //                  {
      //                      //internetSuspendDate = GetLocalizeDateFormat(controller, internetSuspendDate);
      //                      inquryResult.Add(new KeyValuePair<string, string>("Internet", volumeText));
      //                      inquryResult.Add(new KeyValuePair<string, string>("Internet Suspend", internetSuspendDate));
      //                  }

      //                  foundVolume = false;
      //                  // get broadband internet
      //                  GetDAWithVolumeAndSuspendDate(PID,request, dedicateAccountInformations, ref volumeBroadbandInternetText, ref broadbandInternetSuspendDate,  ref foundVolume, GlobalObject.BroadbandInternetBundleDA, controller);
                        
      //                  if (foundVolume == true && inquryResult != null)
      //                  {
      //                      //broadbandInternetSuspendDate = GetLocalizeDateFormat(controller, broadbandInternetSuspendDate);

      //                      inquryResult.Add(new KeyValuePair<string, string>("Internet Broadband", volumeBroadbandInternetText));
      //                      inquryResult.Add(new KeyValuePair<string, string>("Internet Broadband Suspend", broadbandInternetSuspendDate));
      //                  }
      //              }
      //              else
      //              {
      //                  foundVolume = false;
      //                  GetDAWithVolumeAndSuspendDate(PID,request, dedicateAccountInformations, ref volumeText, ref internetSuspendDate,  ref foundVolume, GlobalObject.PostpaidSpeedooBundleDA, controller);
                        
      //                  if (foundVolume == true && inquryResult != null)
      //                  {
      //                      // internetSuspendDate = GetLocalizeDateFormat(controller, internetSuspendDate);
      //                      inquryResult.Add(new KeyValuePair<string, string>("Internet", volumeText));
      //                      inquryResult.Add(new KeyValuePair<string, string>("Internet Suspend", internetSuspendDate));
      //                  }
      //              }
      //          }

      //      }
      //      catch
      //      {
      //          GlobalObject.logger.AddtoLog(PID, String.Format("GW TransactionID: {0}, {1}",request.Id, String.Format("No volume for subscriber{0}", newMSIDN)), LoggerLevel.Info);
      //      }

      //      if(inquryResult != null)
      //          inquryResult.Add(new KeyValuePair<string, string>("Line Suspend",suspendDate));
            

      //  }

        public static string SignalRConnnectionURL { get; set; }

        public static string RechargeLogCommands { get; set; }
    }
}
