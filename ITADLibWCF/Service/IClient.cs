using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF.Service
{
    public interface IClient
    {
        // For interface linking
        int GetId { get; }
        void Dispose();

    }

}
