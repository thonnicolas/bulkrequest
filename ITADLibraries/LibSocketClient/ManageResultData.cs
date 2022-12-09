using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibraries.LibSocketClient
{
    public class ManageResultData
    {

        private BlockingCollection<CommandRespondProperties> ResultData = null;
        private BlockingCollection<CommandRespondProperties> ResultDataLog = null;
        private ConcurrentDictionary<string, CommandProperty> CommandData = new ConcurrentDictionary<string, CommandProperty>();
        private StringBuilder Temp_Result = null;

        Logger logger = null;

        public ManageResultData()
        {
            ResultData = new BlockingCollection<CommandRespondProperties>();
            ResultDataLog = new BlockingCollection<CommandRespondProperties>();
            CommandData = new ConcurrentDictionary<string, CommandProperty>();

            Temp_Result = new StringBuilder();

            logger = new Logger(GetType());
        }


        public int CommandCount
        {
            get { return CommandData.Count; }
        }

        public void AddErrorResult(int ErrorCode,string CommandID, string Result, string ResultType)
        {
            CommandRespondProperties set_result = new CommandRespondProperties(ErrorCode, CommandID, Result, ResultType);
            ResultData.Add(set_result);
            ResultDataLog.Add(set_result);
        }

        public BlockingCollection<CommandRespondProperties> GetAllResult()
        {
            return ResultData;
        }

        public ConcurrentDictionary<string, CommandProperty> GetAllCommands()
        { return CommandData; }

        //public ConcurrentDictionary<int, CommandProperty> GetAllCommands()
        //{
        //    return CommandData;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AddResult(string data)
        {
            lock (ResultData)
            {
                int StartInd = 0;
                int EndInd = 0;
                string getResultData = (string) data;

                if (Temp_Result.Length > 0)
                {
                    getResultData = Temp_Result.ToString() + data;
                    Temp_Result.Clear();
                }

                while (IsCompletedResult(getResultData, ref StartInd, ref EndInd))
                {
                    try
                    {
                        string result = getResultData.Substring(StartInd, EndInd - StartInd);

                        CommandRespondProperties ResResult = SocketCommand.IsResponcommand(result);

                        ResultData.Add(ResResult);

                        getResultData = getResultData.Substring(EndInd + ServerCommandConstant.EndCommand.Length);
                    }
                    catch  // Occured error when the last digit of data has no double quote
                    {
                        getResultData = getResultData.Substring(EndInd + 1);
                    }
                }

                if (!String.IsNullOrWhiteSpace(getResultData)) Temp_Result.Append(getResultData);
            }
        }        

        public CommandProperty RemoveCommand(string CommandID)
        {
            CommandProperty commad = null;
            CommandData.TryRemove(CommandID, out commad);
            return commad;
        }

        /// <summary>
        /// Check if the result 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsCompletedResult(string data, ref int StartInd, ref int EndInd)
        {
            if (string.IsNullOrWhiteSpace(data)) return false;

            StartInd = data.IndexOf(ServerCommandConstant.ResultCommand, 0);
            if (StartInd < 0) return false;
            EndInd = data.IndexOf(ServerCommandConstant.EndCommand, StartInd);
            
            if (StartInd >= 0 && EndInd > StartInd)                                                
                
                
                return true;
            else
                return false;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        public CommandProperty GetResult()
        {
            CommandProperty cpropety = null;
            try
            {

                CommandRespondProperties result = ResultData.Take();

                //string GetRequestedCommand = string.Empty;
                cpropety = new CommandProperty();

                if (CommandData.TryRemove(result.CommandID, out cpropety))
                {
                    cpropety.Result = result.Result;
                    cpropety.ResultCode = result.ResultCode;
                }
                else
                {
                    cpropety = new CommandProperty();
                    cpropety.Command = "";
                    cpropety.CommandID = result.CommandID;
                    cpropety.Result = result.Result;
                }
            }
            catch (Exception e)
            {
                logger.Error("Unknow error in GetResult", e);
            }
            return cpropety;
        }

        public String GetResultDataLog()
        {
            String result = ResultDataLog.Take().Result ;


            return result;
        }


        public bool AddtoCommand(string CommandID, string Command)
        {
            CommandProperty getCommand = new CommandProperty();
            getCommand.CommandID = CommandID;
            getCommand.Command = Command;
            return CommandData.TryAdd(CommandID, getCommand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool TryGetResult(out CommandProperty command)
        {
            command = new CommandProperty();
            CommandRespondProperties result = null;

            bool IsSucceed = false;

            //string getRequestedCommand = string.Empty;

            if (ResultData.TryTake(out result))
            {
                if (CommandData.TryRemove(result.CommandID, out command))
                {
                    command.Result = result.Result;
                    IsSucceed = true;
                }
                else
                {
                    command = new CommandProperty();
                    command.CommandID = result.CommandID;
                    command.Result = result.Result;
                    command.Command = "";
                }
            }
            else
            {
                command = null;
            }
            return IsSucceed;

        }


        private void WaitIsEmpty()
        {
            if (IsEmpty()) Monitor.Wait(ResultData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsEmpty()
        {
            return ResultData.Count == 0;
        }

    }
}
