using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries_v1.LibServerSocket
{
    class ServerSocketCommandConstant
    {
        private const int BUFSIZE = 2048; // Size of receive buffer

        private const string mdw_terminator = "ITMDW-STRING-END\r\n";
        private const string mdw_exit_command = "ITMDW-BYE-BYE";
        private const string mdw_exit_command_confirmation = "ITMDW-BYE-BYE-OK";
        private const string mdw_hand_shake_command = "ITMDW-HANDSHAKE";
        private const string mdw_hand_shake_command_respose = "ITMDW-HANDSHAKE-OK";
        private const string mdw_send_username_command = "ITMDW-USERNAME:";
        private const string mdw_get_connection_count_command = "ITMDW-GETCOUNT";

    }
}
