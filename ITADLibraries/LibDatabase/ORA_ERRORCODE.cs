using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Asiacell.ITADLibraries.LibDatabase
{
    class ORA_ERRORCODE
    {
        /// <summary>
        /// ORACLE instance terminated. Disconnection forced
        /// </summary>
        public const string ORA_01092 = "1092";
        /// <summary>
        /// TNS:could not resolve connect identifier
        /// </summary>
        public const string ORA_12171 = "12171";
        /// <summary>
        /// TNS:no listener
        /// </summary>
        public const string ORA_12224 = "12224";
        /// <summary>
        /// TNS:No connection possible to destination
        /// </summary>
        public const string ORA_12231 = "12231";
        /// <summary>
        /// TNS:No path available to destination
        /// </summary>
        public const string ORA_12232 = "12232";
        /// <summary>
        /// TNS:Failure to accept a connection
        /// </summary>
        public const string ORA_12233 = "12233";
        /// <summary>
        /// TNS:Redirect to destination
        /// </summary>
        public const string ORA_12234 = "12234";
        /// <summary>
        /// TNS:Failure to redirect to destination
        /// </summary>
        public const string ORA_12235 = "12235";
        /// <summary>
        /// TNS:protocol support not loaded
        /// </summary>
        public const string ORA_12236 = "12236";
        /// <summary>
        /// TNS:no listener
        /// </summary>        
        public const string ORA_12541 = "12541";
        /// <summary>
        /// Connect failed because target host or object does not exist
        /// </summary>
        public const string ORA_12545 = "12545";
        /// <summary>
        /// TNS:protocol adapter error
        /// </summary>
        public const string ORA_12560 = "12560";
        /// <summary>
        /// TNS:unknown error
        /// </summary>
        public const string ORA_12561 = "12561";
        /// <summary>
        /// connection refused
        /// </summary>
        public const string ORA_12564 = "12564";
        /// <summary>
        /// end-of-file on communication channel
        /// Cause: The connection between Client and Server process was broken.
        /// </summary>
        public const string ORA_03113 = "3113";
        /// <summary>
        /// Not connect to Oracle
        /// </summary>
        public const string ORA_03114 = "3114";
        /// <summary>
        /// connection lost contact
        /// </summary>
        public const string ORA_03135 = "3135";
        /// <summary>
        ///  inbound connection timed out
        /// </summary>
        public const string ORA_03136 = "3136";
        /// <summary>
        /// Cause: Connection was lost for the specified process ID and thread ID. This is either due to session being killed or network problems.
        /// </summary>
        public const string ORA_03142 = "3142";
        /// <summary>
        /// Cause: Connection was lost for the specified process ID. This is either due to session being killed or network problems.
        /// </summary>
        public const string ORA_03144 = "3144";

        /// <summary>
        /// All Connection Lose
        /// </summary>
        public static ArrayList ORA_CONNECTION_ERROR = new ArrayList { ORA_01092, ORA_03113, ORA_03114, ORA_03135, ORA_03136, ORA_03142, ORA_03144, ORA_12171, ORA_12224, ORA_12231, ORA_12232, ORA_12233, ORA_12234, ORA_12235, ORA_12236, ORA_12541, ORA_12545, ORA_12560, ORA_12561, ORA_12564 };

    }
}
