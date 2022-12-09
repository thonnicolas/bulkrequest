namespace Asiacell.ITADLibraries_v1.LibSocketClient
{
    public class CommandRespondProperties
    {
        public readonly int ResultCode = -1;
        public readonly string CommandID ="";
        public readonly string Result = string.Empty;
        public readonly string ResultType = string.Empty;

        public CommandRespondProperties(int ResultCode, string CommandID, string Result, string ResultType)
        {
            this.ResultCode = ResultCode;
            this.CommandID = CommandID;
            this.Result = Result;
            this.ResultType = ResultType;
        }
    }
}
