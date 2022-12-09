using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF.Classes
{
    public class AppEnum
    {
        public enum UserType
        {
            Service = 1,
            LDAP = 2,
            Admin = 3
        }
        public enum RoundRobinManagerState
        {
            Preparing = 0,
            Running = 1,
            Stopped = 2
        }
    }
}
