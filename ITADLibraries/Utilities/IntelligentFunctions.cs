using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.Utilities
{
    /// <summary>
    /// Functions For Intelligent Service
    /// </summary>
    public class IntelligentFunctions
    {

        /// <summary>
        /// Convert string to date
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dateFormat"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static DateTime StringToDate(string value, string dateFormat, ref bool isError)
        {
            try
            {
                return DateTime.ParseExact(value, dateFormat, null);
            }
            catch (Exception)
            {
                isError = true;
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Convert string to date
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static DateTime StringToDate(string value, string dateFormat)
        {
            try
            {
                return DateTime.ParseExact(value, dateFormat, null);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Convert Date to string with specific format
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dateFormat"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static string DateToString(DateTime date, string dateFormat, ref bool isError)
        {
            string result = string.Empty;
            try
            {
                result = date.ToString(dateFormat);                
            }
            catch (Exception) 
            {
                isError = true;
            }
            return result;
        }

        /// <summary>
        /// get current date
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }


        /// <summary>
        /// Add minutes
        /// </summary>
        /// <param name="date"></param>
        /// <param name="minuteValue"></param>
        /// <returns></returns>
        public static DateTime AddMinutes(DateTime date, double minuteValue)
        {
            try
            {
                return date.AddMinutes(minuteValue);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Add hours
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hourValue"></param>
        /// <returns></returns>
        public static DateTime AddHours(DateTime date, double hourValue)
        {
            try
            {
                return date.AddHours(hourValue);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Add days
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dayValue"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static DateTime AddDays(DateTime date, double dayValue)
        {
            try
            {
                return date.AddDays(dayValue);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Add months
        /// </summary>
        /// <param name="date"></param>
        /// <param name="monthValue"></param>
        /// <returns></returns>
        public static DateTime AddMonths(DateTime date, int monthValue)
        {
            try
            {
                return date.AddMonths(monthValue);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
