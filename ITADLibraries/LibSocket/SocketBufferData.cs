using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using System.Net.Sockets;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class SocketBufferData
    {
        private ConcurrentDictionary<string, CommandProperties> temp_bufferdata = null;
        private DataStoreCommand commandStore = null;
        private StringBuilder Temp_Result = null;
        private StateObject state = null;

        private LoggerEntities logger = null;

        public SocketBufferData(StateObject state, DataStoreCommand commandStore, LoggerEntities logger)
        {
            this.logger = logger;
            this.state = state;
            temp_bufferdata = new ConcurrentDictionary<string, CommandProperties>();
            Temp_Result = new StringBuilder();
            this.commandStore = commandStore;
        }

        /// <summary>
        /// Add buffer to temp data store
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        public void AddBuffer(string buffer)
        {

            string commandid = string.Empty;
            string command = string.Empty;
            string getBuffer = buffer;
            CommandProperties getCommand = null;

            string ProcessID = string.Empty;

            lock (temp_bufferdata)
            {
                if (Temp_Result.Length > 0)
                {
                    Temp_Result.Append(getBuffer);
                    getBuffer = Temp_Result.ToString();
                    Temp_Result.Clear();
                }

                if (getBuffer.IndexOf(SocketCommandConstant.EndCommand) == -1)
                {
                    Temp_Result.Append(getBuffer);
                    return;
                }

                if (InvalidCommand(getBuffer))
                {
                    ProcessID = Functions.GetPID;
                    Send(state, SocketCommands.GenerateResult(ProcessID, SocketERRORCode.UnknowCommand, "", SocketCommandConstant.CommandSubmited, state.UserName, "Invalid Commnad"));
                    //logger.Error("The Client <" + state.UserName + "> submited invalid command :" + getBuffer);
                    logger.AddtoLog(ProcessID, SocketERRORCode.UnknowCommand, "", getBuffer, DateTime.Now, DateTime.Now, "Invalid Commnad", LoggerLevel.Error, ref state);
                    return;
                }

                while (SocketCommands.IsExecutedCommand(ref getBuffer, ref commandid, ref command))
                {
                    try
                    {
                        //logger.Info("The Client <"+state.UserName +"> submite command id :" + commandid + ", command :" + command + ", On : " + DateTime.Now.ToString(Functions.dateFormate));
                        //state.SetToLog("The Client <" + state.UserName + "> submite command id :" + commandid + ", command :" + command + ", On : " + DateTime.Now.ToString(Functions.dateFormate));
                        //Check concurent queue session

                        ProcessID = Functions.GetPID;

                        if (temp_bufferdata.Count >= state.ConcrrentSession)
                        {
                            logger.AddtoLog(ProcessID, SocketERRORCode.The_Client_Queue_Reached_ToMax, commandid, command, DateTime.Now, DateTime.Now, "The queu memory reached to maximum", LoggerLevel.Error, ref state);
                            Send(state, SocketCommands.GenerateResult(ProcessID, SocketERRORCode.The_Client_Queue_Reached_ToMax, commandid, SocketCommandConstant.CommandSubmited, state.UserName, "The client queue memory reached to maximum"));
                        }

                        getCommand = new CommandProperties(state.UserName, commandid, command);
                        
                        
                        if (!temp_bufferdata.TryAdd(commandid, getCommand))
                        {
                            //logger.Error("This <" + state.UserName + "> ID [" + commandid + "] already exists " + DateTime.Now.ToString(Functions.dateFormate));
                            //state.SetToLog("This <" + state.UserName + "> ID [" + commandid + "] already exists" + DateTime.Now.ToString(Functions.dateFormate));
                            logger.AddtoLog(ProcessID, SocketERRORCode.The_Command_ID_Already_exists, commandid, command, DateTime.Now, DateTime.Now, "The command id already existes", LoggerLevel.Error, ref state);
                            Send(state, SocketCommands.GenerateResult(ProcessID , SocketERRORCode.The_Command_ID_Already_exists, commandid, SocketCommandConstant.CommandSubmited, state.UserName, "This ID already exists"));
                            //CommandProperties tmpCommand = null;
                            //temp_bufferdata.TryRemove(commandid, out getCommand);

                        }
                        else
                        {                            
                            logger.AddtoLog(getCommand.PID, SocketERRORCode.Successful, commandid, command, getCommand.Requested_time, DateTime.Now, "Client submit command", LoggerLevel.Info, ref state);
                            commandStore.EnterReceiverData(getCommand);
                            //temp_bufferdata.TryRemove(commandid, out getCommand);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.AddtoLog(Functions.GetPID, SocketERRORCode.Successful, commandid, command, DateTime.Now, DateTime.Now, LoggerLevel.Info, ex, ref state);
                    }
                }

                if (getBuffer.Length > 0) Temp_Result.Append(getBuffer);
            }

        }

        public CommandProperties RemoveCommand(string CommandID)
        {
            CommandProperties command = null;
            temp_bufferdata.TryRemove(CommandID, out command);
            return command;
        }

        public bool InvalidCommand(string data)
        {
            if (data.IndexOf(SocketCommandConstant.CommandSubmited) == -1 || data.IndexOf(SocketCommandConstant.EndCommand) < 17)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Send to client
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        private void Send(StateObject state, byte[] data)
        {
            // Begin sending the data to the remote device.
            state.ClientSocket.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), state);
        }

        /// <summary>
        /// Send Call
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.ClientSocket;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
        }

        public bool AcklegeRespond(ref DataStoreCommand store, string key)
        {

            CommandProperties getCommand = null;
            if (temp_bufferdata.TryRemove(key, out getCommand))
            {
                store.EnterReceiverData(getCommand);
                return true;
            }
            else
                return false;
        }

    }
}
