using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Asiacell.ITADLibraries_v1.LibDatabase;
using Asiacell.ITADLibraries_v1.Utilities;
using Asiacell.ITADLibraries_v1.LibLogger;
using Asiacell.ITADLibWCF_v1.Business;

namespace Asiacell.ITADLibWCF_v1.Classes
{
    public abstract class ElementObjectAbstract
    {
        // Memory store for Element Object
        protected ConcurrentDictionary<int, GSMCommand> _gsmCommands = new ConcurrentDictionary<int, GSMCommand>();        
        protected ConcurrentDictionary<string, ElementLogin> _elementLogins = new ConcurrentDictionary<string, ElementLogin>();
        protected ConcurrentDictionary<int, RangeInfo> _rangeInfos = new ConcurrentDictionary<int, RangeInfo>();
        protected ConcurrentDictionary<string, ErrorDescription> _errorDescription = new ConcurrentDictionary<string, ErrorDescription>();
        protected int _elementTypeId = 0;
        
        private LoggerEntities logger = null;
        private DBConnection db = null;

        public ElementObjectAbstract(LoggerEntities logger, DBConnection db)
        {
            this.logger = logger;
            this.db = db;
        }

        public ElementObjectAbstract(LoggerEntities logger, DBConnection db, int  elementTypeId)
        {
            this.logger = logger;
            this.db = db;
            this._elementTypeId = elementTypeId;

        }

        

        /// <summary>
        /// Sync users collection
        /// </summary>
        public void SyncWithDB()
        {

            try
            {
                // Get GSM commands collections
                LoadGSMCommands();
                GetElementLogin();
                LoadErrorDescription();
                if (this._rangeInfos.IsEmpty == true)
                {
                    LoadRangeInfo();
                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Tracing - "+ e.Message, LoggerLevel.Error);
            }
        }

        /// <summary>
        /// Get GSM Command By System CommandId
        /// </summary>
        /// <param name="systemCommandId"></param>
        /// <returns></returns>
        public IEnumerable<GSMCommand> GetGSMCommandBySystemCommandId(int systemCommandId)
        {
            List<GSMCommand> result = null;
            try
            {
                result = this._gsmCommands.Values.Where<GSMCommand>(g => g.SystemCommandId == systemCommandId).ToList<GSMCommand>();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Get GSM Command By System CommandId
        /// </summary>
        /// <param name="systemCommandId"></param>
        /// <returns></returns>
        public IEnumerable<GSMCommand> GetGSMCommandBySystemCommandId(int systemCommandId, RangeInfo rangeInfo)
        {
            List<GSMCommand> result = null;

            try
            {
                if (rangeInfo.SubscriberType != SubscriberTypes.AllType)
                {
                    result = this._gsmCommands.Values.Where<GSMCommand>(g => g.SystemCommandId == systemCommandId && g.VersionId == rangeInfo.VersionId && g.SubscriberType == rangeInfo.SubscriberType).OrderBy(g => g.Sequence).ToList<GSMCommand>();
                }
                else
                {
                    result = this._gsmCommands.Values.Where<GSMCommand>(g => g.SystemCommandId == systemCommandId && g.VersionId == rangeInfo.VersionId).OrderBy(g => g.Sequence).ToList<GSMCommand>();
                }
            }
            catch { }

            return result;
        }

        /// <summary>
        /// GetGSMCommands
        /// </summary>
        /// <param name="gsmCommands"></param>
        /// <param name="dbApps"></param>
        private void LoadGSMCommands()
        {
            string sql = "select g.gsmcommandid, g.gsm_command, g.systemcommandid, g.versionid, g.subscriber_type, g.sequence, g.description ";
            try
            {
                if (this._elementTypeId > 0)
                {
                    sql += @"
                    from tbl_gsm_command g, tbl_system_command s
                    where g.systemcommandid = s.systemcommandid
                    and s.elementtypeid = :elementtypeid";

                }
                else
                {
                    sql += "from tbl_gsm_command g";
                }

                ICollection<int> toBeRemoved = _gsmCommands.Keys;
                OracleParameter param = new OracleParameter("elementtypeid", this._elementTypeId);

                // collect user information to collection users
                //using (OracleDataReader reader = db.GetDataReader(sql, param))
                
                using(DataSet ds = db.GetDataSet(sql, param))
                {
                    int indx = 0;
                    
                    //while (reader.Read())
                    foreach(DataRow reader in ds.Tables[0].Rows)
                    {
                        // read user data from database
                        GSMCommand gsmCommand = new GSMCommand
                        {
                            GsmCommandId = Convert.ToInt32(reader["gsmcommandid"]),
                            GsmCommand = Convert.ToString(reader["gsm_command"]),
                            Sequence = Convert.ToInt32(reader["sequence"]),
                            SubscriberType = Convert.ToInt32(reader["subscriber_type"]),
                            SystemCommandId = Convert.ToInt32(reader["systemcommandid"]),
                            VersionId = Convert.ToInt32(reader["versionid"])
                        };

                        if (gsmCommand.SystemCommandId == 11)
                        {                            
                            Console.WriteLine("trace here" + indx.ToString());
                            
                        }

                        //check in the collection of users data.
                        _gsmCommands.AddOrUpdate(gsmCommand.GsmCommandId, gsmCommand, (key, oldUser) => gsmCommand);

                        indx++;
                        // add to remove list
                        try { toBeRemoved.Remove(gsmCommand.GsmCommandId); }
                        catch (Exception) { }
                    }

                    // remove non exists users from the memory
                    try
                    {
                        IEnumerable<int> removeItemList = (IEnumerable<int>)toBeRemoved.GetEnumerator();
                        foreach (int gsmCommandName in removeItemList)
                        {
                            GSMCommand gsmCommand;
                            _gsmCommands.TryRemove(gsmCommandName, out gsmCommand);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Unable to update GSMCommands due to - " + e.Message, LoggerLevel.Error);
            }

        }

        public bool GetParameterGetMSSIDN(RequestObject obj, out string keyParam)
        {
            string[] expect = { @"%msisdn%", @"%isdn%", @"%subscriber%" };
            return CheckForValue(obj, expect, out keyParam);
        }

        public bool GetParameterGetIMSI(RequestObject obj, out string keyParam)
        {
            string[] expect = { @"%imsi%" };

            return CheckForValue(obj, expect, out keyParam);
        }
        
        public bool CheckForValue(RequestObject obj, string[] expect, out string keyParam)
        {
            // check the client request object for MSISDN or IMSI
            foreach (NameValuePair p in obj.CommandParamaterValues)
            {
                foreach (string i in expect)
                {
                    if (p.Name.ToLower().Equals(i.ToLower()))
                    {
                        keyParam = p.Value;
                        return true;
                    }
                }
            }
            keyParam = "";
            return false;
        }

        public List<string> GetGSMCommandWithAuthentication(ref RequestObject ReqObject, RangeInfo rangeInfo, out int IsError)
        {
            List<string> commands = GetGSMCommand(ref ReqObject, rangeInfo, out IsError); 
            List<string> newCommands = new List<string>();
            foreach (var command in commands)
            {
                string authCommand = command;
                authCommand = authCommand.Replace("%USR%", rangeInfo.UserId);
                authCommand = authCommand.Replace("%PWD%", rangeInfo.Password);
                newCommands.Add(authCommand);
            }
            return newCommands;
        }

        public List<string> GetGSMCommand(ref RequestObject ReqObject, RangeInfo rangeInfo, out int IsError)
        {
            List<string> Commands = new List<string>();
            IsError = -1;
            try
            {
                List<GSMCommand> lCommads = (List<GSMCommand>)GetGSMCommandBySystemCommandId(ReqObject.CommandID, rangeInfo);
                IsError = CreateGSMCommandLists(ReqObject, IsError, Commands, lCommads);
            }
            catch (Exception e)
            {
                ReqObject.Error_Code = SystemErrorCodes.Server_Execute_SQL_Query_Failed;
                ReqObject.Description = "Cannot get GSM command due to - " + e.Message;
                logger.AddtoLog(ReqObject.Id, ReqObject.Description, LoggerLevel.Fatal);
                IsError = -1;
            }

            return Commands;

        }

        public List<string> GetGSMCommand(ref RequestObject ReqObject, out int IsError)
        {
            List<string> Commands = new List<string>();
            IsError = -1;
            try
            {
                List<GSMCommand> lCommads = (List<GSMCommand>)GetGSMCommandBySystemCommandId(ReqObject.CommandID);
                IsError = CreateGSMCommandLists(ReqObject, IsError, Commands, lCommads);
            }
            catch (Exception e)
            {
                ReqObject.Error_Code = SystemErrorCodes.Server_Execute_SQL_Query_Failed;
                ReqObject.Description = "Cannot get GSM command due to - " + e.Message;
                logger.AddtoLog(ReqObject.Id, ReqObject.Description, LoggerLevel.Fatal);
                IsError = -1;
            }

            return Commands;

        }

        private int CreateGSMCommandLists(RequestObject ReqObject, int IsError, List<string> Commands, List<GSMCommand> lCommads)
        {
            if (lCommads.Count <= 0 || lCommads == null)
            {
                IsError = -1;
                ReqObject.Description = "Invalid GSM command";
                ReqObject.Error_Code = SystemErrorCodes.Server_Invalid_GSM_Command;
                logger.AddtoLog(ReqObject.Id, ReqObject.Description, LoggerLevel.Info);
            }
            else
            {

                foreach (var _GSMCommand in lCommads)
                {
                    if (_GSMCommand != null)
                    {
                        string _command = _GSMCommand.GsmCommand;

                        foreach (var _obj in ReqObject.CommandParamaterValues)
                        {
                            if (_obj != null)
                                _command = _command.Replace(_obj.Name, _obj.Value);
                        }
                        IsError = 0;
                        Commands.Add(_command);
                    }
                }
            }
            return IsError;
        }

        /// <summary>
        /// Check in range condition
        /// </summary>
        /// <param name="value"></param>
        /// <param name="elementTypeId"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool CheckInRang(string value, int elementTypeId,  RangeInfo r)
        {
            if ((String.Compare(value, r.RangeStart) >= 0) && (String.Compare(r.RangeEnd, value) >= 0) && elementTypeId == r.ElementTypeId)
                return true;
            else
                return false;            
        }
        /// <summary>
        /// Get Range Infomation
        /// </summary>
        /// <param name="elementTypeId"></param>
        /// <param name="MSISDN"></param>
        /// <returns>Return null if not found or else a RangeInfo object</returns>
        public RangeInfo GetRangeInfo(int elementTypeId, string MSISDN)
        {
            RangeInfo rangeObject = new RangeInfo();
            List<RangeInfo> rangeInfo = new List<RangeInfo>();
            try{
                if(!(string.IsNullOrWhiteSpace(MSISDN))){
                    rangeInfo = this._rangeInfos.Values.Where<RangeInfo>(r => CheckInRang(MSISDN, elementTypeId, r)).ToList<RangeInfo>();
                }else{
                    rangeInfo = this._rangeInfos.Values.Where<RangeInfo>(r => r.ElementTypeId == elementTypeId).ToList<RangeInfo>();
                }
                if (rangeInfo != null)
                {
                    if (rangeInfo.Count > 0)
                    {
                        rangeObject = rangeInfo.ElementAt(0);
                        return rangeObject;
                    }
                    else
                    {
                        return null;
                    }                    
                }                                     
            }catch(Exception e){}

            return null;
        }

        /// <summary>
        /// Get Range Information
        /// </summary>
        private void LoadRangeInfo()
        {
            string sql = @"select id, range_start, range_end, elementtypeid, element_type_name, versionid, version, elementid, element_name, element_ip, element_port, 
                            userid, password, buffer_size, sendreceive_timeout, cityid, cityname, tplid, sub_template,  tcsi, ocsi, gprs_csi, ucsi, ucsi_non_balance_transfer, 
                            gtid, scp, scp_dpc, scp_gt, sub_class, subscriber_type, status, login_command, logout_command, encryption 
                            from vrangeinfo t ";

            if (this._elementTypeId > 0)
            {
                sql += "where t.elementtypeid = :elementtypeid";
            }
                                       
            try
            {
                ICollection<int> toBeRemoved = _rangeInfos.Keys;
                OracleParameter param = new OracleParameter("elementtypeid", this._elementTypeId);
                List<OracleParameter> oparams = new List<OracleParameter>();
                oparams.Add(param);
                using(DataSet ds = db.GetDataSet(sql, oparams))
                // get client ip address
                //using (OracleDataReader reader = db.GetDataReader(sql,oparams))                
                {
                    //while (reader.Read())
                    foreach(DataRow reader in ds.Tables[0].Rows)
                    {                        
                        RangeInfo rangeInfo = new RangeInfo
                        {
                            BufferSize = Convert.ToInt32(reader["buffer_size"]),
                            CityId = Convert.ToInt32(reader["cityid"]),
                            CityName = Convert.ToString(reader["cityname"]),
                            ElementId = Convert.ToInt32(reader["elementid"]),
                            ElementIp = Convert.ToString(reader["element_ip"]),
                            ElementName = Convert.ToString(reader["element_name"]),
                            ElementPort = Convert.ToString(reader["element_port"]),
                            ElementTypeId = Convert.ToInt32(reader["elementtypeid"]),
                            ElementTypeName = Convert.ToString(reader["element_type_name"]),
                            Encryption = Convert.ToString(reader["encryption"]),
                            Id = Convert.ToInt32(reader["id"]),
                            LoginCommand = Convert.ToString(reader["login_command"]),
                            LogoutCommand = Convert.ToString(reader["logout_command"]),
                            Password = Convert.ToString(reader["password"]),
                            RangeEnd = Convert.ToString(reader["range_end"]),
                            RangeStart = Convert.ToString(reader["range_start"]),
                            SendreceiveTimeout = Convert.ToInt32(reader["sendreceive_timeout"]),
                            Status = Convert.ToInt32(reader["status"]),
                            SubClass = Convert.ToInt32(reader["sub_class"]),
                            SubscriberType = Convert.ToInt32(reader["subscriber_type"]),
                            SubTemplate = Convert.ToInt32(reader["sub_template"]),
                            Tplid = Convert.ToInt32(reader["tplid"]),
                            UserId = Convert.ToString(reader["userid"]),
                            Version = Convert.ToString(reader["version"]),
                            VersionId = Convert.ToInt32(reader["versionid"]),
                            Tcsi = Convert.ToInt32(reader["tcsi"]),
                            Ocsi = Convert.ToInt32(reader["ocsi"]),
                            GprsCsi = Convert.ToInt32(reader["gprs_csi"]),
                            Ucsi = Convert.ToInt32(reader["Ucsi"]),
                            UcsiNonBalanceTransfer = Convert.ToInt32(reader["ucsi_non_balance_transfer"]),
                            Gtid = Convert.ToInt32(reader["gtid"]),
                            Scp = Convert.ToString(reader["scp"]),
                            ScpDpc = Convert.ToString(reader["scp_dpc"]),
                            ScpGt = Convert.ToString(reader["scp_gt"])

                        };
                        
                        //check insystemCommands the collection of users data.
                         
                         _rangeInfos.AddOrUpdate(rangeInfo.Id, rangeInfo, (key, old) => rangeInfo);
                         try { toBeRemoved.Remove(rangeInfo.Id); }
                         catch (Exception e) { }
                    }
                    try
                    {

                        // remove non exists client ip from the memory
                        IEnumerable<int> removeItemList = (IEnumerable<int>)toBeRemoved.GetEnumerator();
                        foreach (int rangeToBeRemove in removeItemList)
                        {
                            RangeInfo rangeInfoReturn;
                            _rangeInfos.TryRemove(rangeToBeRemove, out rangeInfoReturn);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Unable to get the latest data of range information due to - " + e.Message, LoggerLevel.Error);
            }
        }

        /// <summary>
        /// GetSystemCommands
        /// </summary>
        /// <param name="elementLogins"></param>
        /// <param name="dbApps"></param>
        private void GetElementLogin()
        {
            string sql = "select login_key, status, id, name, element_type_id, url, ip, port, round_robin from tbl_element_login";
            try
            {
                ICollection<string> toBeRemoved = _elementLogins.Keys;

             
                // get client ip address
                using(DataSet ds = db.GetDataSet(sql))
                //using (OracleDataReader reader = db.GetDataReader(sql))
                {
                    //while (reader.Read())
                    foreach(DataRow reader in ds.Tables[0].Rows)
                    {
                        // read user data from database
                        ElementLogin loginElement = new ElementLogin
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            ElementTypeID = Convert.ToInt32(reader["element_type_id"]),
                            LoginKey = Convert.ToString(reader["login_key"]),
                            Name = Convert.ToString(reader["name"]),
                            Url = Convert.ToString(reader["url"]),
                            Ip = Convert.ToString(reader["ip"]),
                            Port = Convert.ToInt32(reader["port"]),
                            Status = Convert.ToInt32(reader["status"]),
                            RoundRobin = Convert.ToInt32(reader["round_robin"])
                        };

                        //check insystemCommands the collection of users data.
                        _elementLogins.AddOrUpdate(loginElement.LoginKey, loginElement, (key, old) => loginElement);
                        try { toBeRemoved.Remove(loginElement.LoginKey); }
                        catch (Exception e) { }
                    }
                    try
                    {

                        // remove non exists client ip from the memory
                        IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                        foreach (string command in removeItemList)
                        {
                            ElementLogin elementLoginReturn;
                            _elementLogins.TryRemove(command, out elementLoginReturn);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Unable to update ElementLogin due to - " + e.Message, LoggerLevel.Error);
            }
        }

        public bool Validate_LoginKey(string LoginKey)
        {
            List<ElementLogin> result = null;
            try
            {
                result = this._elementLogins.Values.Where<ElementLogin>(g => g.LoginKey == LoginKey).ToList<ElementLogin>();

                if(result.Count>0 && result != null) 
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        private void LoadErrorDescription()
        {
            string sql = "select elementid, errorcode, description from tbl_errorcode_by_element";
            try
            {
                //ElementId=0: to load default error code
                if (this._elementTypeId > 0)
                {
                    sql += " where (elementid = :elementtypeid or elementid=:default_element)";
                }

                ICollection<string> toBeRemoved = _errorDescription.Keys;
                OracleParameter[] param = new OracleParameter[2];

                param[0] = new OracleParameter();
                param[0].ParameterName = "elementtypeid";
                param[0].OracleDbType = OracleDbType.Int16;
                param[0].Value = this._elementTypeId;

                param[1] = new OracleParameter();
                param[1].ParameterName = "default_element";
                param[1].OracleDbType = OracleDbType.Int16;
                param[1].Value = 0;

                // collect user information to collection users
                using(DataSet ds = db.GetDataSet(sql, param))
                //using (OracleDataReader reader = db.GetDataReader(sql, param))
                {
                    //while (reader.Read())
                    foreach(DataRow reader in ds.Tables[0].Rows)
                    {
                        // read user data from database
                        ErrorDescription errorDesc = new ErrorDescription
                        {
                            elementTypeID = Convert.ToString(reader["elementid"]),
                            errorID = Convert.ToString(reader["errorcode"]),
                            description = Convert.ToString(reader["description"])
                        };

                        string Keys = errorDesc.elementTypeID + "-" + errorDesc.errorID;
                        //check in the collection of users data.
                        _errorDescription.AddOrUpdate(Keys, errorDesc, (key, old) => errorDesc);

                        // add to remove list
                        try { toBeRemoved.Remove(Keys); }
                        catch (Exception) { }
                    }

                    // remove non exists users from the memory
                    try
                    {
                        IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                        foreach (string errorDesc in removeItemList)
                        {
                            ErrorDescription errorDescription;
                            _errorDescription.TryRemove(errorDesc, out errorDescription);
                        }
                    }
                    catch (Exception) { }

                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Unable to update ErrorDescription due to - " + e.Message, LoggerLevel.Error);
            }
        }

        public string GetErrorDescription(string errorCode,out bool IsSuccess, params object[] elementTypeID)
        {
            List<ErrorDescription> result = null;
            string _result = "Unknown";
            IsSuccess = true;
            try
            {
                if (elementTypeID == null)
                {
                    result = this._errorDescription.Values.Where<ErrorDescription>(g => g.errorID == errorCode).ToList<ErrorDescription>();
                }
                else
                {
                    result = this._errorDescription.Values.Where<ErrorDescription>(g => g.errorID == errorCode && g.elementTypeID == (string)elementTypeID[0]).ToList<ErrorDescription>();
                }

                if (result.Count > 0 && result != null)
                {
                    ErrorDescription err = result[0];
                    _result = err.description;
                }
                else
                {
                    IsSuccess = false;
                }
            }
            catch(Exception e)
            {
                IsSuccess = false;
                _result = "Unable to get error description.";
                logger.AddtoLog("Unable to get ErrorDescription due to - " + e.Message, LoggerLevel.Error);
            }

            return _result;
        }
    }
}
