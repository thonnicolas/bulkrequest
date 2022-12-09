using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.Utilities
{
    public class SystemErrorCodes
    {
        public const int Succeed = 0;  //No error

        #region Client Error

        public const int Client_LogInError = 100;  //Client Login Error                          
        public const int Client_UnauthaurizeIP = 101;  //Unauthaurize IP                             
        public const int Client_No_Permission = 102;  //No Permission to execute command            
        public const int Client_ConnectionTimeOut = 103;  //Client Connection Time Out                  
        public const int Client_Close_NotProperly = 104;  //Client Close connection not properly        
        public const int Client_Unknow_Command = 105;  //Unknown Command                             
        public const int Client_Invalid_Parameter = 106;  //Invalid Parameter                            
        public const int Client_Invalid_MSISDNorIMSI = 107;  //Invalid MSISDN or IMSI                      
        public const int Client_ConnectionLose = 108;  //Client Connection lose      
        public const int Client_MAX_Connection_Reached = 109;  //Client Connection lose      
        public const int Client_Has_No_Command_Permission = 110;    //Client has no permission to execute command
        public const int Client_Invalid_PINSerial = 111; //Invalid PIN or Serial number (Not numeric, length is correct)

        #endregion Client Error

        #region Server error

        public const int Server_Database_Connection_Failed = 200;  //Database connection failed                  
        public const int Server_LoginTo_GSM_Failed = 201;  //LogIn to GSM failed                         
        public const int Server_TransportError_or_ConnectionLose = 202;  //Transportation Error or Connection Lose     
        public const int Server_GSM_Connection_Timeout = 203;  //GSM connection timeout                      
        public const int Server_Subscriber_Not_Define_In_System_Range = 204;  //Subscriber not define in system range       
        public const int Server_Subscriber_Not_Define_In_GSM_Element = 205;  //Subscriber not define in GSM Element        
        public const int Server_Invalid_GSM_Command = 206;  //Invalid GSM Command                         
        public const int Server_Unable_Connect_To_GSM_Element = 207;  //Unable connect to GSM Element               
        public const int Server_Execute_SQL_Insert_failed = 208;  //Insert data ({0}) to database failed        
        public const int Server_Execute_SQL_Query_Failed = 209;  //Execute query({0}) from database failed     
        public const int Server_Execute_SQL_Update_Failed = 210;  //Update record failed ({0})                  
        public const int Server_Execute_SQL_Delete_Failed = 211;  //Delete record failed ({0})                  
        public const int Server_Wait_For_GSM_Element_Response_Timedout = 212;  //Element failed to write to Send table on time
        public const int Server_Invalid_LoginKey = 213; //Element loginkey from server to element
        public const int Server_ElementID_Not_Found = 214; // Element id doest not found.
        public const int Server_Request_Timeout_Due_To_Maximum_Request_Is_Exceeded_To_Element_Node = 219;
        public const int Server_Maximum_Request_Is_Exceeded_To_Element_Node = 218;

        #endregion Server error

        #region Succeed

        public const int Server_Dabatase_connection_succeed = 300;  //Connect to database succeed                 
        public const int Server_Connect_to_GSM_succeed = 301;  //Connect to GSM ({0}) succeed                
        public const int Server_Login_To_GSM_succeed = 302;  //Login to GSM ({0}) succeed                  
        public const int Server_Submit_GSMCommand_succeed = 303;  //Submit GSM command {0} succeed              
        public const int Server_Client_Login_succeed = 304;  //Client login with userid : {0} succeed      
        public const int Server_CommandResult_to_Client_succeed = 305;  //Send command result of {0} to client succeed
        //public const int Server__succeed= 306;  //Client {0} request to disconnect            
        public const int Server_Client_diconnect_succeed = 307;  //Client {0} disconnect succeed               
        public const int Server_Execute_SQL_Insert_succeed = 308;  //Insert data to database succeed             
        public const int Server_Execute_SQL_Query_succeed = 309;  //Query data from database succeed            
        public const int Server_Execute_SQL_Update_succeed = 310;  //Update record into database succeed         
        public const int Server_Execute_SQL_Delete_succeed = 311;  //Delete record into database succeed         
        public const int Server_Client_IP_Authenticated_succeed = 312;  //The Client IP {0} is authenticated 

        #endregion Succeed

        #region System Message

        public const int Server_System_Error = 900;       //Application System Error            
        public const int Server_Execute_SQL_Insert = 901; //Insert data Into database , Sequen :
        public const int Server_Execute_SQL_Delete = 902; //delete data , Sequen : {0}          
        public const int Server_Execute_SQL_Update = 903; //Update data , Sequen : {0}          
        public const int Server_Client_Send_Request = 904;//Client send requested command : {0} 
        public const int Server_Inquiry_GSM_Element = 905;//Inquiry GSM Element
        public const int Server_Get_GSM_Command = 906; //Get GSM Command
        public const int Server_MSISDN_IMSI_Region = 907; //This subscriber in {0} region
        public const int Server_GSM_Element_Info = 908; //GSM Element Info {0}


        #endregion System Message

        #region GSM System Error

        public const int GSM_Element_Error = 888; //GSM Element Info {0}    

        #endregion



        public const int Unknown_Error = 999;  //Unknown Error                                                                   
    }
}
