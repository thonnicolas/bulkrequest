using System;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities_v1
{
    public class Encryption
    {
        private string validatekey = "";
        private string key = "";
        private LoggerEntities logger = null;

        //create construction
        public Encryption(string key, LoggerEntities logger)
        {

            //alidatekey = System.Environment.UserDomainName;
            this.key = key + validatekey;

            this.logger = logger;
        }

        /// <summary>
        /// This function will be used to encrypt plain text.
        /// The encryption is based on EncryptionKey set in section in web.config.
        /// </summary>
        /// <param name="plainText"> this text will be encrypte with assiage key </param>
        /// <returns></returns>
        public string EncryptTripleDES(string plainText)
        {
            string TripleDES = "****The Key is not valid****";

            byte[] Buffer = new byte[0];

            try
            {
                System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(key));
                DES.Mode = System.Security.Cryptography.CipherMode.ECB;
                System.Security.Cryptography.ICryptoTransform DESEncrypt = DES.CreateEncryptor();
                Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(plainText);
                TripleDES = Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex)
            {
                logger.AddtoLog("Error in convert plan text", ex, LoggerLevel.Error);
            }
            return TripleDES;

        }
    }
}
