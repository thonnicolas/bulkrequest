using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibWCF.Business;

namespace BulkRequestConsole.Classes
{
    public interface IResultPushToDB
    {
        void AddToDB(object item);
        void IncreaseRequestCount();
        int GetRequestGroup();
        void setParameter1(string values);
        string getParameter1();
    }
}
