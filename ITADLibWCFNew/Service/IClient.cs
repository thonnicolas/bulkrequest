using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF_v1.Service
{
    public interface IClient
    {
        // For interface linking
        int GetId { get; }
        void Dispose();

    }

}
