using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.Utilities
{
    public class ElementItems
    {
        public readonly string CommandResult;
        public readonly string ResutlStatus;
        public readonly string Description;
        
        /// <summary>
        ///  Element items
        /// </summary>
        /// <param name="CommandResult"></param>
        /// <param name="ResutlStatus"></param>
        /// <param name="Description"></param>
        public ElementItems(string CommandResult, string ResutlStatus, string Description)
        {
            this.CommandResult = CommandResult;
            this.ResutlStatus = ResutlStatus;
            this.Description = Description;
        }

    }
}
