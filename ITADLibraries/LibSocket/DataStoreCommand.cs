using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Oracle.DataAccess.Client;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class DataStoreCommand
    {
        //private object Locked = new object();

        //Queue request
        private BlockingCollection<CommandProperties> RQueue = null;
        //Queue Send
        private SocketClientInfo AllClient = null;

        private OracleCommand oraCommand = null;
        private DBConnectionPool db = null;
        private LoggerEntities logger = null;

        public DataStoreCommand(int QSize, SocketClientInfo AllClient, DBConnectionPool db, LoggerEntities logger)
        {
            RQueue = new BlockingCollection<CommandProperties>(QSize);
            this.db = db;

            this.logger = logger;
        }

        public DataStoreCommand(SocketClientInfo AllClient, DBConnectionPool db, LoggerEntities logger)
        {
            RQueue = new BlockingCollection<CommandProperties>();
            this.AllClient = AllClient;
            this.db = db;
            this.logger = logger;
        }


        /// <summary>
        /// Enter Command request
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public void EnterReceiverData(string CommandID, string UserName, string Command)
        {
            //RQueue.Add(new CommandProperties(UserName, CommandID, Command));

            CommandProperties command = new CommandProperties(UserName, CommandID, Command);
            Task.Factory.StartNew(cmd => AddCommand((CommandProperties)cmd) , command);
        }

        public void EnterReceiverData(CommandProperties command)
        {
            //RQueue.Add(command);
            CommandProperties GetCommand = command;
            Task.Factory.StartNew(cmd => AddCommand((CommandProperties)cmd), GetCommand);
        }

        private void AddCommand(CommandProperties command)
        {
            int SystemCommandID=0;
            int ElementID=0;
            if (IsCorrectCommand(command, ref ElementID, ref SystemCommandID))
            {
                command.SystemCommandID = SystemCommandID;
                command.ElementID = ElementID;

                RQueue.Add(command);
            }         
 
        }

        /// <summary>
        /// Get Command request (FIFO)
        /// </summary>
        /// <returns></returns>
        public CommandProperties GetReceiverData()
        {
            CommandProperties getcommand = RQueue.Take();
            //AllClient.GetSocketClient(getcommand.UserName).bufferData.RemoveCommand(getcommand.CommandID);
            return getcommand;
        }

        /// <summary>
        /// Check System Command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool IsCorrectCommand(CommandProperties command, ref int ElementID, ref int SystemCommandID)
        {
            bool isCorrect = false;

            try
            {
                string SQL = "select systemcommandid, command_name, command_param, elementtypeid from tbl_system_command where command_name=:command_name";

                OracleParameter param = new OracleParameter("command_name", OracleDbType.Varchar2);
                param.Value = command.GetCommandName;

                using (OracleDataReader reader = db.GetConnection().GetDataReader(SQL, param))
                {
                    SystemCommandID = 0;
                    ElementID = 0;
                    if (reader.Read())
                    {
                        string[] getCommandParam = reader["command_param"].ToString().Split(SocketCommandConstant.SeperaterParam.ToCharArray());

                        ElementID = Functions.ToNumber(reader["elementtypeid"]);
                        SystemCommandID = Functions.ToNumber(reader["systemcommandid"]);

                        if (command.GetValue.Count == getCommandParam.Length)
                        {
                            isCorrect = true;
                        }
                        else
                        {

                            command.Result = "Incorrect Parameter";
                            command.ErroCode = SocketERRORCode.Incorrect_Parameter;
                            logger.AddtoLog(command.CommandID, command.Result + " : " + command.Command, LoggerLevel.Info);
                            AllClient.GetSocketClient(command.UserName).EnterToRespondResult(command);
                        }

                    }
                    else
                    {
                        command.Result = "Invalid Command";
                        command.ErroCode = SocketERRORCode.InvalidCommand;
                        logger.AddtoLog(command.CommandID, command.Result + " : " + command.Command, LoggerLevel.Info);
                        AllClient.GetSocketClient(command.UserName).EnterToRespondResult(command);
                    }
                }

            }
            catch (DBConnectionException Ex)
            {
                
                oraCommand = db.GetConnection().GetCommand();

                command.Result = "Database Connection Error !";
                command.ErroCode = SocketERRORCode.DatabaseConnection_Error;

                logger.AddtoLog(command.CommandID, command.Result + " : " + Ex.Message, LoggerLevel.Fatal );
                
                AllClient.GetSocketClient(command.UserName).EnterToRespondResult(command);
            }
            catch (Exception ex)
            {
                command.Result = "Internal Command";
                command.ErroCode = SocketERRORCode.Internal_Error;                
                AllClient.GetSocketClient(command.UserName).EnterToRespondResult(command);
            }

            return isCorrect;
        }
    }
}
