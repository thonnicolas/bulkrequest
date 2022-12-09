using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities_v1
{
    public class Decryption
    {
        private string validatekey = "";
        private string key = "";
        private LoggerEntities logger = null;
        
        //create construction
        public Decryption(string key, LoggerEntities logger)
        {
            
            //validatekey = System.Environment.UserDomainName;
            this.key = key + validatekey;
            this.logger = logger;
        }
        /// <summary>
        /// This function will be used to descrypt "encrypted text" back to plan text.
        /// The encryption is based on DescryptionKey set in section in web.config
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public string DecryptTripleDES(string encryptedText)
        {
            string DecTripleDES = "****The Key is not valid****";
            byte[] Buffer = new byte[0];
            try
            {
                System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(key));
                DES.Mode = System.Security.Cryptography.CipherMode.ECB;
                System.Security.Cryptography.ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                Buffer = Convert.FromBase64String(encryptedText);
                DecTripleDES = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex)
            {
                logger.AddtoLog(ex, LoggerLevel.Error);
            }
            return DecTripleDES;
        }
    }
}
