using System;
using System.Threading;
using Asiacell.ITADLibraries.Utilities;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Sockets;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.LibSocketClient
{
   public class SocketClient : AsynSocketClient
    {
        private ManualResetEvent LoginDone = new ManualResetEvent(false);
        private ManualResetEvent LogOutDone = new ManualResetEvent(false);
        private ManualResetEvent InquiryIsDone = new ManualResetEvent(false);

        private object locked = new object();

        private System.Timers.Timer autoConnect = null;

        private volatile bool IsLogOut = false;
        private ClientProperties properties = null;

        private int Transaction_ID = 0;

        private bool IsLoginSuccess = false;

        private Task taskResult = null;

        public BlockingCollection<string> messagelog = null;
        public BlockingCollection<CommandProperty> CommandResult = null;

        private LoggerEntities logger = null;

        public SocketClient(ClientProperties prop, LoggerEntities logger)
            : base(prop.Server_IP, prop.Server_Port, logger)
        {
            this.logger = logger;
            messagelog = new BlockingCollection<string>();
            CommandResult = new BlockingCollection<CommandProperty>();
            this.properties = prop;
        }


        public override void OnError(int error, string Message)
        {
            throw new NotImplementedException();
        }

        private void OnTimedEvent(object Source, ElapsedEventArgs ag)
        {
            if (IsLogOut) return;
            InquiryLink();
        }

        private void InquiryLink()
        {
            lock (locked)
            {
                InquiryIsDone.Reset();
                if (IsLogOut) return;
                if (IsConnected())
                {
                    //int CmdID = GetTXTID();
                    //if (Send(CmdID, GenMMLCommand(INClientConstant.CMDHandShake, INCommandType.RegularCommand, CmdID)) == 0)
                    //{
                    //    logger.Info("Submit command (" + INClientConstant.CMDHandShake + ") to inquiry link succeed");
                    //}
                    //else
                    //{
                    //    logger.Error("Submit command (" + INClientConstant.CMDHandShake + ") to inquiry link failed");
                    //    logger.Error("Re-connect to IN");
                    //    Login();
                    //}
                    //InquiryIsDone.WaitOne();
                }
                else
                {

                    logger.AddtoLog(Functions.GetPID, "Re-connect to IN", LoggerLevel.Info);
                    Login();
                }
            }
        }

        public void AddToLog(String message)
        {
            Task.Factory.StartNew(delegate { messagelog.Add(message); }, message, TaskCreationOptions.PreferFairness);
        }

        public void RestartTimer()
        {
            if (autoConnect != null)
            {
                autoConnect.Stop();
                autoConnect.Start();
            }
        }

        public int GetCommandID()
        {
            if (Interlocked.CompareExchange(ref Transaction_ID, 1, int.MaxValue) < int.MaxValue)
                Interlocked.Increment(ref Transaction_ID);
            return Transaction_ID;
        }


        private void GetResult()
        {
            taskResult = Task.Factory.StartNew(() =>
            {
                while (!IsLogOut)
                {

                    CommandProperty command = ResultData.GetResult();

                    logger.AddtoLog("", "Command Counter : " + ResultData.CommandCount, LoggerLevel.Info);

                    if (!String.IsNullOrWhiteSpace(command.Command))
                    {
                        //Loging result
                        if (command.Command.IndexOf(ServerCommandConstant.LoginCommand) > -1)
                        {
                            if (command.ResultCode == ServerCommandConstant.Successful)
                            {
                                logger.AddtoLog("","Login to ITMDW success : " + DateTime.Now.ToString(Functions.dateFormate), LoggerLevel.Info);
                                IsLoginSuccess = true;

                                if (properties.InquiryLinkInterval > 0 && properties.KeepAlive)
                                {
                                    autoConnect = new System.Timers.Timer(properties.InquiryLinkInterval);
                                    autoConnect.Elapsed += new ElapsedEventHandler(OnTimedEvent);

                                    autoConnect.AutoReset = true;
                                    autoConnect.Enabled = true;
                                }
                            }
                            else
                            {
                                logger.AddtoLog("","Login to ITMDW failed ",LoggerLevel.Info);

                                IsLoginSuccess = false;
                            }

                            LoginDone.Set();
                        }
                        else if (command.Command.IndexOf(ServerCommandConstant.LogoutCommand) > -1) // Logout result
                        {
                            if (command.ResultCode == ServerCommandConstant.Successful)
                            {
                                logger.AddtoLog("","Logout success, : " + DateTime.Now.ToString(Functions.dateFormate), LoggerLevel.Info);
                                IsLoginSuccess = false;
                            }
                            else
                            {
                                logger.AddtoLog("","Login failed ", LoggerLevel.Info);
                            }
                            LogOutDone.Set();
                        }
                        else if (command.Command.IndexOf(ServerCommandConstant.InquiryRespondCommand) > -1) // Hanshack response
                        {
                            logger.AddtoLog("","Inquiry link sucess", LoggerLevel.Info);
                            InquiryIsDone.Set();
                        }
                        else //Send Regular Command
                        {
                            logger.AddtoLog("","Respond Command : CommandID : " + command.CommandID + ", Result : " + command.Result + " .Start=" + command.Submited_On.ToString("HH:mm:ss fff") + " End =" + command.Responded_On.ToString("HH:mm:ss fff"), LoggerLevel.Info);
                            AddCommandResult(command);

                        }

                    }
                }

            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            string cmdid = string.Empty;
            try
            {
                lock (locked)
                {
                    LoginDone.Reset();

                    OpenConnection();
                    if (IsConnected())
                    {

                        cmdid = SocketCommand.GenCommandID;

                        logger.AddtoLog("","Submit Login command",LoggerLevel.Info);

                        if (Send(cmdid , SocketCommand.GenLoginCommand(properties.UserName, properties.Password, cmdid)) == 0)
                        {
                            logger.AddtoLog("", "Submite Login command successful", LoggerLevel.Info);
                           
                        }
                        if (taskResult == null) GetResult();

                        LoginDone.WaitOne();
                    }
                }

            }
            catch (SocketException ex)
            {

                logger.AddtoLog("", "Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message, LoggerLevel.Error);
                ResultData.AddErrorResult(ServerCommandConstant.LoginError, cmdid, "Error while initializing connection", ServerCommandConstant.LoginCommand);
            }
            catch (FormatException ex)
            {
                logger.AddtoLog("", "Error while initializing connection : " + ex.Message, LoggerLevel.Error);
                ResultData.AddErrorResult(ServerCommandConstant.LoginError, cmdid, "Error while initializing connection", ServerCommandConstant.LoginCommand);
            }
            catch (ObjectDisposedException)
            {
                logger.AddtoLog("", "Socket has been closed", LoggerLevel.Error);
                ResultData.AddErrorResult(ServerCommandConstant.LoginError, cmdid, "Error while initializing connection", ServerCommandConstant.LoginCommand);
            }
            catch (Exception ex)
            {
                logger.AddtoLog("", "Error while initializing connection : " + ex.Message, LoggerLevel.Error);
                ResultData.AddErrorResult(ServerCommandConstant.LoginError, cmdid, "Error while initializing connection", ServerCommandConstant.LoginCommand);
            }

            return IsLoginSuccess;
        }

        public void Execute(string Command)
        {
            RestartTimer();
            //lock (locked)
            {
                if (IsConnected())
                {
                    string CommandID = SocketCommand.GenCommandID;
                    if (Send(CommandID, SocketCommand.GenerateCommand(Command, CommandID)) == 0)
                    {
                        logger.AddtoLog(CommandID, "Submite command <" + Command + "> succeed, ID : " + CommandID, LoggerLevel.Info);
                    }
                    else
                    {
                        logger.AddtoLog(CommandID, "Submite command <" + Command + "> failed, ID : " + CommandID, LoggerLevel.Info);

                    }

                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="CommandResults"></param>
        public void AddCommandResult(CommandProperty CommandResults)
        {
            Task.Factory.StartNew(delegate { CommandResult.Add(CommandResults); }, CommandResults, TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CommandResults"></param>
        public CommandProperty GetCommandResult()
        {
            CommandProperty Commandproperty = null;

            //CommandResult.TryTake(out Commandproperty);
            Commandproperty =CommandResult.Take();

            return Commandproperty;
        }

        /// <summary>
        /// Log Out connection
        /// </summary>
        public void LogOut()
        {

            lock (locked)
            {
                WaitIsRespondCompleted(DateTime.Now);

                IsLogOut = true;

                if (autoConnect != null)
                {
                    autoConnect.Stop();
                    autoConnect.Dispose();
                }

                if (IsConnected() && IsLoginSuccess)
                {
                    string cmdid = SocketCommand.GenCommandID;
                    if (Send(cmdid, SocketCommand.GenerateLogout(cmdid)) == 0)
                    {
                        logger.AddtoLog(cmdid,"Submit LogOut command success", LoggerLevel.Info);
                    }
                    else
                    {
                        logger.AddtoLog(cmdid,"Submit LogOut command faile",LoggerLevel.Info);
                       
                    }
                    LogOutDone.WaitOne(TimeSpan.FromMinutes(1));
                }
                Close();
            }
        }

        private void WaitIsRespondCompleted(DateTime startTime)
        {
            if (ResultData.CommandCount > 0 && startTime.AddMinutes(1) >= DateTime.Now)
            {
                Thread.Sleep(200);
                WaitIsRespondCompleted(startTime);
            }
        }

    }
}
