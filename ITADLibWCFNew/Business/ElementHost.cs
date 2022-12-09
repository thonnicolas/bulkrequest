using System;
using Oracle.DataAccess.Client;
using System.Collections.Generic;

namespace Asiacell.ITADLibraries_v1.Utilities
{
    #region ElementHost
    public class TBL_ELEMENT_HOST
    {

        // Columns Constant
        public int ELEMENTID { get; set; }
        public int ELEMENTTYPEID { get; set; }
        public string ELEMENT_NAME { get; set; }
        public string ELEMENT_IP { get; set; }
        public string ELEMENT_PORT { get; set; }
        public int VERSIONID { get; set; }
        public string USERID { get; set; }
        public string PASSWORD { get; set; }
        public string LOGIN_COMMAND { get; set; }
        public string LOGOUT_COMMAND { get; set; }
        public string ENCRYPTION { get; set; }
        public int BUFFER_SIZE { get; set; }
        public int SENDRECEIVE_TIMEOUT { get; set; }
        public int HANDSHAKE_INTERVAL { get; set; }
        public int MAX_POOLSIZE { get; set; }
        public int MIN_POOLSIZE { get; set; }
        public int STATUS { get; set; }

    }
    #endregion TBL_ELEMENT_HOST
}