using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities
{
    public interface IAttributeValidator
    {
        object Validate(object logger);
    }
}
