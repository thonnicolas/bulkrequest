using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class ClientInfo
    {
        private int mUserID;
        private string mUserName;
        private string mPassword;
        private int mBufferSize;
        private int mConcurrentSession;

        public ClientInfo(int userid, string username, string password, int buffersize, int concur_session)
        {
            mUserID = userid;
            mUserName = username;
            mPassword = password;
            mBufferSize = buffersize;
            mConcurrentSession = concur_session;
        }

        public int UserID { get { return mUserID; } }
        public string UserName { get { return mUserName; } }
        public string Password { get { return mPassword; } }
        public int BufferSize { get { return mBufferSize; } }
        public int ConcurrentSession { get { return mConcurrentSession; } }
    }
}
