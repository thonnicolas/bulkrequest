using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class LoginCommand
    {
        private byte[] user = null;
        private byte[] pwd = null;
        private byte[] bin = null;
        private byte[] log = null;
        private byte[] log_id = null;

        private string org_loginstring = string.Empty;


        public LoginCommand(string loginstring)
        {
            user = new byte[30];
            pwd = new byte[30];
            bin = new byte[4];
            log = new byte[6];
            log_id = new byte[8];
            this.org_loginstring = loginstring;
        }

        /// <summary>     
        /// Validate login command, If invalid command reture fale, else true
        /// login:XXXXXXXX??????????????????????????????******************************
        /// </summary>
        /// <param name="loginstring"></param>
        /// <returns></returns>
        public bool ValidateLoginCommand()
        {
            try
            {
                byte[] loginstring = Encoding.ASCII.GetBytes(org_loginstring);

                if (loginstring == null || loginstring.Length != 78) return false;

                int logLen = 0;
                int log_id_len = log.Length;
                int userLen = log_id_len + log_id.Length;
                int pwdLen = user.Length + userLen;
                int binLen = pwdLen + pwd.Length;


                Buffer.BlockCopy(loginstring, logLen, log, 0, log.Length);
                Buffer.BlockCopy(loginstring, log_id_len, log_id, 0, log_id.Length);
                Buffer.BlockCopy(loginstring, userLen, user, 0, user.Length);
                Buffer.BlockCopy(loginstring, pwdLen, pwd, 0, pwd.Length);
                Buffer.BlockCopy(loginstring, binLen, bin, 0, bin.Length);

                if (!SocketCommandConstant.LoginCommand.Equals(GetLoginCommand.ToUpper())) return false;
            }
            catch (Exception) { return false; }

            return true;
        }

        public string GetLoginCommand { get { return Encoding.ASCII.GetString(log).Trim('\0').Trim(); } }
        public string GetUserName { get { return Encoding.ASCII.GetString(user).Trim('\0').Trim(); } }
        public string GetPassword { get { return Encoding.ASCII.GetString(pwd).Trim('\0').Trim(); ; } }
        public string GetBindingMode { get { return Encoding.ASCII.GetString(bin).Trim('\0').Trim(); ; } }

        /// <summary>
        /// Re-generate login string and replace password with '*****'
        /// </summary>
        public string GetLoginString
        {
            get
            {
                string pass = string.Empty;
                pass = pass.PadLeft(Encoding.ASCII.GetString(pwd).Trim().Length, '*');
                pass = pass.PadLeft(pwd.Length, ' ');
                return Encoding.ASCII.GetString(log) + Encoding.ASCII.GetString(user) + pass + Encoding.ASCII.GetString(bin);
            }
        }


    }
}
