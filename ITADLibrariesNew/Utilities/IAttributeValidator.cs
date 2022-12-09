using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries_v1.LibLogger;

namespace Asiacell.ITADLibraries_v1.Utilities
{
    public interface IAttributeValidator
    {
        object Validate(object logger);
    }
}
