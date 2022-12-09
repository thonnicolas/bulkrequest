using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF.Business
{
    // User entity
    public class User
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public int userType { get; set; }
        public int status { get; set; }
        public int groupId { get; set; }
        public string commandPermission { get; set; }
        public int maxPoolSize { get; set; }
        public int connectionTimeout { get; set; }

        public int queueMultiplyBy { get; set; }
    }
}
