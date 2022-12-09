using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Asiacell.ITADLibraries.Utilities
{
    public class Functions
    {

        public static readonly string dateFormate = "dd/MM/yyyy HH:mm:ss"; // dd/mm/yyyy hh24:mm:ss
        public static readonly string dateFormateShort = "dd/MM/yyyy"; // dd/mm/yyyy hh24:mm:ss
        public static readonly string dateFormateLong = "dd/MM/yyyy HH:mm:ss fff"; // dd/mm/yyyy hh24:mm:ss fff

        public static readonly string dateFormateOracle = "dd/mm/yyyy hh24:mi:ss"; // dd/mm/yyyy hh24:mm:ss
        public static readonly string dateFormateOracleShort = "dd/mm/yyyy"; // dd/mm/yyyy hh24:mm:ss
                
        /**
         * Trime String
         */
        public static string Trim(object value)
        {
            string result = "";
            try
            {
                result = ToString(value).Trim();
            }
            catch (Exception)
            {

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(object value)
        {
            string result = "";
            try
            {
                result = Convert.ToString(value);
            }
            catch (Exception)
            {

            }
            return result;
        }

        /**
         * Convert string to number
         */
        public static int ToNumber(object value)
        {
            int result = 0;
            try
            {
                result = Int32.Parse(Trim(value));
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static string GetPID { get { return Guid.NewGuid().ToString("N"); } }

        /**
         * Convert string to number
         */
        public static long ToLong(object value)
        {
            long result = 0;
            try
            {
                result = long.Parse(Trim(value));
            }
            catch (Exception)
            {
            }
            return result;
        }

        /**
         * Convert string to boolean
         */
        public static bool ToBoolean(string value)
        {
            bool result = false;
            try
            {
                result = Boolean.Parse(value);
            }
            catch (Exception)
            {

            }
            return result;
        }

        /// <summary>
        /// To duble
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(string value)
        {
            double result = 0;
            try
            {
                result = Double.Parse(value);
            }
            catch (Exception)
            {

            }
            return result;
        }

        /// <summary>
        /// Convert to Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(object value)
        {
            double result = 0;
            try
            {
                result = Double.Parse(Trim(value));
            }
            catch (Exception)
            {

            }
            return result;
        }

        /// <summary>
        /// Compare string
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool CompareString(string A, string B)
        {
            return String.Compare(A, B) == 0;
        }

        /// <summary>
        /// Compare ignore case
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool CompareInoreCase(string A, string B)
        {
            bool isEqual = false;
            try
            {
                return String.Compare(A, B, true) == 0;
            }
            catch (Exception)
            {
            }
            return isEqual;

        }

        /// <summary>
        /// Is numberic
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        {
            bool isNum = false;
            try
            {
                long.Parse(value);
                isNum = true;
            }
            catch (Exception)
            {

            }
            return isNum;
        }

        /// <summary>
        /// Convert to Date
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDate(string value)
        {
            try
            {
                return DateTime.ParseExact(value, dateFormate, null);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Get current date
        /// </summary>
        /// <returns></returns>
        public static string CurrentDate()
        {
            return DateTime.Now.ToString(dateFormate);
        }


        /// <summary>
        /// Replace all
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Find"></param>
        /// <param name="ReplaceWith"></param>
        /// <returns></returns>
        public static string ReplaceAll(string Value, string Find, string ReplaceWith)
        {
            string result = "";
            try
            {
                result = Regex.Replace(Value, Find, ReplaceWith);
            }
            catch (Exception) { }

            return result;
        }

        /// <summary>
        /// Is Asiacell MSISDN
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsMSISDN(string Value)
        {

            if (!string.IsNullOrEmpty(Value) && IsNumber(Value))
            { 
                if(Value.StartsWith("96477") && Value.Length==13)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Is Asiacell IMSI
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsIMSI(string Value)
        {

            if (!string.IsNullOrEmpty(Value) && IsNumber(Value))
            {
                if (Value.StartsWith("418") && Value.Length == 15)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Correct MSISDN format 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>if value not Asiacell's MSISDN return "", else return "96477XXXXXXXX"</returns>
        public static string MSISDN(string value)
        {
            string result = "";
            int len = 0;


            if (IsNumber(value))
            {
                len = value.Length;
                if (len == 13)
                {
                    if (Regex.Match(value, "^(96477)(\\d{8})$").Success)
                        result = value;
                }
                else if (len == 11)
                {
                    if (Regex.Match(value, "^(077)(\\d{8})$").Success)
                        result = "964" + value.Substring(1, 10);
                }
                else if (len == 10)
                {
                    if (Regex.Match(value, "^(77)(\\d{8})$").Success)
                        result = "964" + value;
                }
            }

            return result;

        }

        /// <summary>
        /// Convert Arabic Numeric to English Numeric
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertArabic2English(string value)
        {
            if (value == null) return "";

            string msisdn = "";
            int n = 0;
            char[] ch = value.ToCharArray();

            for (int i = 0; i < ch.Length; i++)
            {
                n = (int)ch[i];
                if (n != 32) // 32 : Space
                {
                    if (n >= 48 && n <= 57) //English Digit
                        msisdn += ch[i].ToString();
                    else if (n >= 1632 && n <= 1641) //if is Arabic digit convert to English digit
                        msisdn += (char)(((int)ch[i]) - 1632 + 48);
                }
            }

            return msisdn;
        }

        /// <summary>
        /// 07702119995
        /// </summary>
        /// <param name="MSISDN"></param>
        /// <returns></returns>
        public static string IN_MSISDN(string MSISDN)
        {
            string newMSISDN = "";

            if (!string.IsNullOrWhiteSpace(MSISDN) && MSISDN.Length >= 10)
            {
                newMSISDN = MSISDN.Substring(MSISDN.Length - 10);
            }

            return newMSISDN;

        }


        /// <summary>
        /// Convert Hexa to inter
        /// </summary>
        /// <param name="HexValue"></param>
        /// <returns></returns>
        public static int HexToInt(string HexValue)
        {
            try { return Int32.Parse(HexValue, System.Globalization.NumberStyles.HexNumber); }
            catch { return -1; }
        }

        /// <summary>
        /// Splite By Space except space in double quote : Ex : A B "D C" --> 1:A, 2:B, 3:"D C"
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string[] SpliteBySplace(string data)
        {
            try
            {
                Regex reg = new Regex("[^\\s\"]+|\"([^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant  | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

                List<string> array = new List<string>();
                foreach (Match item in reg.Matches(data))
                {
                    array.Add(item.Value);
                }

                return array.ToArray();
            }
            catch (Exception)
            {

            }
            return null;
        }



        /// <summary>
        /// Convert decimal to Hexa
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHexa(Int64 value)
        {
            return Convert.ToString(value, 16);
        }

        /// <summary>
        /// Right to left Padding with "PadString"
        /// </summary>
        /// <param name="value">Data</param>
        /// <param name="PadChar">padding character</param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string LefPad(object value, char PadChar, int precision)
        {
            string newValue = ToString(value);
            if (newValue.Length >= precision)
                newValue = newValue.Substring(0, precision);
            else
                newValue = newValue.PadLeft(precision, PadChar);

            return newValue;
        }

        /// <summary>
        /// Left to Right Padding character
        /// </summary>
        /// <param name="value"></param>
        /// <param name="PadChar"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string RightPad(object value, char PadChar, int precision)
        {
            string newValue = ToString(value);
            if (newValue.Length >= precision)
                newValue = newValue.Substring(0, precision);
            else
                newValue = newValue.PadRight(precision, PadChar);

            return newValue;
        }

        /// <summary>
        /// Check Global Result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsCommand_Succeed(string result)
        {
            string regex = "GlobalResult=[Succeed];";
            bool isSucceed = false;

            if (result.IndexOf(regex) != -1)
                isSucceed = true;

            return isSucceed;
        }

        /// <summary>
        /// Get value by attribute name
        /// </summary>
        /// <param name="AttributeName"></param>
        /// <param name="CommandResult"></param>
        /// <returns></returns>
        public static string GetAttributeValue(string AttributeName, string CommandResult)
        {
            string value = "-1";
            try
            {
                //string pattern = "([\\||\\]]?([\\s]*)?)(" + AttributeName.ToUpper() + ")\\s?=\\s?([^\\||\\]]*)";
                string pattern = @"(^" + AttributeName.ToUpper() + @"|\W" + AttributeName.ToUpper() + @")\s?=\s?\W?([^(\|\])]*)";

                foreach (Match m in Regex.Matches(CommandResult.ToUpper(), pattern))
                {
                    value = m.Groups[2].ToString();
                    break;
                }
            }
            catch (Exception)
            { }

            return value;
        }

        #region IsNumericType
        /// <summary>
        /// Determines whether the specified value is of numeric type.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <returns>
        /// 	<c>true</c> if o is a numeric type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumericType(object o)
        {
            return (o is byte ||
                o is sbyte ||
                o is short ||
                o is ushort ||
                o is int ||
                o is uint ||
                o is long ||
                o is ulong ||
                o is float ||
                o is double ||
                o is decimal);
        }
        #endregion
        #region IsPositive
        /// <summary>
        /// Determines whether the specified value is positive.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="ZeroIsPositive">if set to <c>true</c> treats 0 as positive.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is positive; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPositive(object Value, bool ZeroIsPositive)
        {
            switch (Type.GetTypeCode(Value.GetType()))
            {
                case TypeCode.SByte:
                    return (ZeroIsPositive ? (sbyte)Value >= 0 : (sbyte)Value > 0);
                case TypeCode.Int16:
                    return (ZeroIsPositive ? (short)Value >= 0 : (short)Value > 0);
                case TypeCode.Int32:
                    return (ZeroIsPositive ? (int)Value >= 0 : (int)Value > 0);
                case TypeCode.Int64:
                    return (ZeroIsPositive ? (long)Value >= 0 : (long)Value > 0);
                case TypeCode.Single:
                    return (ZeroIsPositive ? (float)Value >= 0 : (float)Value > 0);
                case TypeCode.Double:
                    return (ZeroIsPositive ? (double)Value >= 0 : (double)Value > 0);
                case TypeCode.Decimal:
                    return (ZeroIsPositive ? (decimal)Value >= 0 : (decimal)Value > 0);
                case TypeCode.Byte:
                    return (ZeroIsPositive ? true : (byte)Value > 0);
                case TypeCode.UInt16:
                    return (ZeroIsPositive ? true : (ushort)Value > 0);
                case TypeCode.UInt32:
                    return (ZeroIsPositive ? true : (uint)Value > 0);
                case TypeCode.UInt64:
                    return (ZeroIsPositive ? true : (ulong)Value > 0);
                case TypeCode.Char:
                    return (ZeroIsPositive ? true : (char)Value != '\0');
                default:
                    return false;
            }
        }
        #endregion
        #region ToUnsigned
        /// <summary>
        /// Converts the specified values boxed type to its correpsonding unsigned
        /// type.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns>A boxed numeric object whos type is unsigned.</returns>
        public static object ToUnsigned(object Value)
        {
            switch (Type.GetTypeCode(Value.GetType()))
            {
                case TypeCode.SByte:
                    return (byte)((sbyte)Value);
                case TypeCode.Int16:
                    return (ushort)((short)Value);
                case TypeCode.Int32:
                    return (uint)((int)Value);
                case TypeCode.Int64:
                    return (ulong)((long)Value);

                case TypeCode.Byte:
                    return Value;
                case TypeCode.UInt16:
                    return Value;
                case TypeCode.UInt32:
                    return Value;
                case TypeCode.UInt64:
                    return Value;

                case TypeCode.Single:
                    return (UInt32)((float)Value);
                case TypeCode.Double:
                    return (ulong)((double)Value);
                case TypeCode.Decimal:
                    return (ulong)((decimal)Value);

                default:
                    return null;
            }
        }
        #endregion
        #region ToInteger
        /// <summary>
        /// Converts the specified values boxed type to its correpsonding integer
        /// type.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns>A boxed numeric object whos type is an integer type.</returns>
        public static object ToInteger(object Value, bool Round)
        {
            switch (Type.GetTypeCode(Value.GetType()))
            {
                case TypeCode.SByte:
                    return Value;
                case TypeCode.Int16:
                    return Value;
                case TypeCode.Int32:
                    return Value;
                case TypeCode.Int64:
                    return Value;

                case TypeCode.Byte:
                    return Value;
                case TypeCode.UInt16:
                    return Value;
                case TypeCode.UInt32:
                    return Value;
                case TypeCode.UInt64:
                    return Value;

                case TypeCode.Single:
                    return (Round ? (int)Math.Round((float)Value) : (int)((float)Value));
                case TypeCode.Double:
                    return (Round ? (long)Math.Round((double)Value) : (long)((double)Value));
                case TypeCode.Decimal:
                    return (Round ? Math.Round((decimal)Value) : (decimal)Value);

                default:
                    return null;
            }
        }
        #endregion
        #region UnboxToLong
        public static long UnboxToLong(object Value, bool Round)
        {
            switch (Type.GetTypeCode(Value.GetType()))
            {
                case TypeCode.SByte:
                    return (long)((sbyte)Value);
                case TypeCode.Int16:
                    return (long)((short)Value);
                case TypeCode.Int32:
                    return (long)((int)Value);
                case TypeCode.Int64:
                    return (long)Value;

                case TypeCode.Byte:
                    return (long)((byte)Value);
                case TypeCode.UInt16:
                    return (long)((ushort)Value);
                case TypeCode.UInt32:
                    return (long)((uint)Value);
                case TypeCode.UInt64:
                    return (long)((ulong)Value);

                case TypeCode.Single:
                    return (Round ? (long)Math.Round((float)Value) : (long)((float)Value));
                case TypeCode.Double:
                    return (Round ? (long)Math.Round((double)Value) : (long)((double)Value));
                case TypeCode.Decimal:
                    return (Round ? (long)Math.Round((decimal)Value) : (long)((decimal)Value));

                default:
                    return 0;
            }
        }
        #endregion
    }
}
