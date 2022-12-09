using System;

namespace Asiacell.ITADLibraries_v1.Utilities
{
    public class OutputFormat
    {

        private static string GetLineFormate(string keywork, string value) { return keywork + "=" + outOpenBlock + value + outCloseBlock; }

        public const string CommandSucceed = "Succeed";
        public const string CommandFailed = "Failed";
        public const string GlobleSucceed = "Succeed";
        public const string GlobleFailed = "Failed";

        public const string outSeparator = "|";
        public const string outRecConcat = " # ";
        public const string outOpenBlock = "[";
        public const string outCloseBlock = "]";
        public const string outEndCmd = ";";
        public const string outNewLine = "\r\n";
        public const string outTap = "\t";

        private static string outbeginFormat(int Sequence) { return "Begin " + Sequence + outNewLine; }
        private static string outendFormate(int Sequence) { return "End " + Sequence + outNewLine; }

        private static string outGlobalStart(string SystemCommand) { return "Start " + SystemCommand + outNewLine; }
        private static string outGlobalEnd(string SystemCommand) { return "EndStart " + SystemCommand; }

        private static string outElementType(string ElementType) { return GetLineFormate("Element Type", ElementType) + outEndCmd + outNewLine; }
        private static string outRegion(string region) { return GetLineFormate("Region", region) + outEndCmd + outNewLine; }

        private static string outAttribute(string Attribute) { return GetLineFormate("Attr", Attribute) + outEndCmd + outNewLine; }

        private static string outActual(string MMLCommand) { return GetLineFormate("Actual", MMLCommand) + outEndCmd + outNewLine; }

        private static string outResult(bool IsSucceed)
        {
            string result = string.Empty;
            if (IsSucceed)
                result = GetLineFormate("Result", CommandSucceed) + outEndCmd + outNewLine;
            else
                result = GetLineFormate("Result", CommandFailed) + outEndCmd + outNewLine;

            return result;
        }

        private static string outGlobalResult(bool IsSucceed)
        {
            string result = string.Empty;

            if (IsSucceed)
                result = GetLineFormate("GlobalResult", GlobleSucceed) + outEndCmd + outNewLine;
            else
                result = GetLineFormate("GlobalResult", GlobleFailed) + outEndCmd + outNewLine;
            return result;
        }

        private static string outSequance(long Sequance)
        {
            string result = string.Empty;
            result = GetLineFormate("PID-SEQ", Functions.ToString(Sequance)) + outEndCmd + outNewLine;
            return result;
        }

        private static string outSystemCommandDescription(string SystemCommandDescription) { return GetLineFormate("CommandDesc", SystemCommandDescription) + outEndCmd + outNewLine; }
        private static string outDesc(string Description) { return GetLineFormate("Desc", Description) + outEndCmd + outNewLine; }
        private static string outError(string Error) { return GetLineFormate("Error", Error) + outEndCmd + outNewLine; }

        /// <summary>
        /// Start [System Command Name]
        ///     GlobalResult = Succeed/Failed;
        ///     Region = XXXXXXX
        ///     CommandDesc = Description of system command
        ///     Command Body.....
        /// EndStart [System Command Name]
        /// </summary>
        /// <param name="SystemCommand"></param>
        /// <param name="IsSucceed"></param>
        /// <param name="region"></param>
        /// <param name="SystemCommandDescription"></param>
        /// <param name="CommandBody"></param>
        /// <returns></returns>
        public static string GetGlobleFormat(string SystemCommand,long PIDSequance, bool IsSucceed, string region, string SystemCommandDescription, string CommandBody)
        {
            string result = string.Empty;
            result = outGlobalStart(SystemCommand);
            result += outTap + outSequance(PIDSequance);
            result += outTap + outGlobalResult(IsSucceed);
            result += outTap + outRegion(region);
            result += outTap + outSystemCommandDescription(SystemCommandDescription);
            result += CommandBody;
            result += outGlobalEnd(SystemCommand);
            return result;
        }


        /// <summary>
        /// Create a new formate out result as below
        ///   Begin 1
        ///       Actual = [GSM raw command and parameters];
        ///       ElementType = [GSM Network Element];
        ///       Result = [Succeed/Failed];
        ///       Attr = [Result seperaated by "|"];
        ///       Desc = [description];
        ///       Error = [ERROR-ID];
        ///   End 1
        /// </summary>
        /// <param name="Sequence"></param>
        /// <param name="ElementType"></param>
        /// <param name="MMLCommand"></param>
        /// <param name="IsSucceed"></param>
        /// <param name="Attribute"></param>
        /// <param name="Description"></param>
        /// <param name="ErrorID"></param>
        /// <returns></returns>
        public static string GetCommandBodyFormat(int Sequence, string ElementType, string MMLCommand, bool IsSucceed,
            string Attribute, string Description, string ErrorID)
        {
            string body = string.Empty;
            body = outTap + outbeginFormat(Sequence);
            body += outTap + outTap + outActual(MMLCommand);
            body += outTap + outTap + outElementType(ElementType);
            body += outTap + outTap + outResult(IsSucceed);
            body += outTap + outTap + outAttribute(Attribute);
            body += outTap + outTap + outDesc(Description);
            body += outTap + outTap + outError(ErrorID);
            body += outTap + outendFormate(Sequence);

            return body;
        }

        public static string GetCommandBodyFormat(int Sequence, string ElementType, string MMLCommand, bool IsSucceed,
            string Attribute, string Description, string CommandResult, string ErrorID)
        {
            string body = string.Empty;
            body = outTap + outbeginFormat(Sequence);
            body += outTap + outTap + outActual(MMLCommand);
            body += outTap + outTap + outElementType(ElementType);
            body += outTap + outTap + outResult(IsSucceed);
            body += outTap + outTap + outAttribute(Attribute);
            body += outTap + outTap + outDesc(Description);
            body += outTap + outTap + outError(ErrorID);


            string busi_seq = Get_Bus_Seq(CommandResult);

            if (busi_seq != "")
            {
                body += outTap + outTap + "busiSeq=" + "[" + busi_seq + "];" + outNewLine;
            }
            body += outTap + outendFormate(Sequence);

            return body;
        }

        public static string Get_Bus_Seq(string res)
        {

            int pos1 = res.IndexOf("<busiSeq>");
            int pos2 = res.IndexOf("</busiSeq>");

            if (pos1 > 0 && pos2 > pos1)
            {
                return res.Substring(pos1 + 9, pos2 - pos1 - 9);
            }

            return "";

        }

    }
}
