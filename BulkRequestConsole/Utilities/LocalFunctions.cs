using System;

namespace BulkRequestConsole.Utilities
{
    public class LocalFunctions
    {
        /// <summary>
        /// Round Up Decimal number
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double RoundUp(double x)
        {
            var r = Math.Round(x, 1, MidpointRounding.AwayFromZero); // Rounds "up"
            return r;
        }

        
         
    }
}
