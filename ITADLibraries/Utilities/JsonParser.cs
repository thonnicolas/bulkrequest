using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Asiacell.ITADLibraries.Utilities
{
    public class JsonParser
    {
        /// <summary>
        /// Convert XML To JSON
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string ConvertXmlToJson(string xmlText)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlText);
            doc.ToString();
            JToken jp = new JObject();
            LoopThroughXmlForJsonOutput(doc, ref jp);
            return jp.ToString();
        }

        /// <summary>
        /// Created by Vathna Chan
        /// Created on 04/08/2014 11:00
        /// Convert XML to JSON conversion with dumplicate item and remove empty input
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string ConvertXmlDuplicateItemToJson(string xmlText)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlText);
            doc.ToString();
            JToken jp = new JObject();
            XmlToJsonOutputWithDumplicate(doc, ref jp);
            return jp.ToString();
        }

        public static string ConvertXmlWithAttributeToJson(string xmlText)
        {
            XmlDocument xml = new XmlDocument();
            string result =string.Empty;
            try
            {
                xml.LoadXml(xmlText);
                result = JsonConvert.SerializeXmlNode(xml);
                //var x = JObject.Parse(result);
            }
            catch (Exception)
            {
                result = xmlText;
            }
            return result; //x.ToString();
        }
        /// <summary>
        /// Convert Plain text to Json
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertTextToJson(string text)
        {
            JToken obj = new JObject();
            ConvertPlainTextValueToJson("respond", text, ref obj);
            return obj.ToString();
        }

        /// <summary>
        /// Convert Plain text to Json
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertTextToJson(string text, string nameValueDelimiter, string nameValueSeperator)
        {
            JToken obj = new JObject();
            ConvertPlainTextValueToJson("respond", text, ref obj, nameValueDelimiter, nameValueSeperator);
            return obj.ToString();
        }

        public static string ConvertTextToJsonV2(string text, string nameValueDelimiter, string nameValueSeperator)
        {
            JToken obj = new JObject();
            ConvertPlainTextValueToJsonV2("respond", text, ref obj, nameValueDelimiter, nameValueSeperator);
            return obj.ToString();
        }

        /// <summary>
        /// Convert Plain text to Json
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertTextToJson(int index, string text, string nameValueDelimiter, string nameValueSeperator)
        {
            JToken obj = new JObject();
            ConvertPlainTextValueToJson(string.Concat("respond", index.ToString()), text, ref obj, nameValueDelimiter, nameValueSeperator);
            return obj.ToString();
        }

        /// <summary>
        /// Loop through xml for XML to JSON conversion
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jObject"></param>
        private static void LoopThroughXmlForJsonOutput(XmlNode obj, ref JToken jObject)
        {
            if (obj.HasChildNodes)
            {
                if (obj.ChildNodes.Count == 1 && obj.ChildNodes[0].HasChildNodes == false)
                {
                    if (obj.Name == "completeHLRResponse")
                    {
                        
                        ConvertPlainTextValueToJsonV2(obj.Name, obj.ChildNodes[0].InnerText, ref jObject, System.Environment.NewLine, "=");
                    }
                    else
                    {
                        // add normal value
                        jObject[obj.Name] = obj.ChildNodes[0].InnerText;
                    }
                }
                else
                {
                    JToken linkObj = new JObject();
                    jObject[obj.Name] = linkObj;
                    foreach (XmlNode xNode in obj)
                    {
                        LoopThroughXmlForJsonOutput(xNode, ref linkObj);
                    }
                }
            }
        }


        /// <summary>
        /// Loop through xml for XML with dumplicate to JSON conversion by Vathna
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jObject"></param>
        private static void LoopThroughXmlForJsonOutputV3(XmlNode obj, ref JToken jObject)
        {
            if (obj.HasChildNodes)
            {
                if (obj.ChildNodes.Count == 1 && obj.ChildNodes[0].HasChildNodes == false)
                {
                    if (obj.Name == "completeHLRResponse")
                    {

                        ConvertPlainTextValueToJsonV2(obj.Name, obj.ChildNodes[0].InnerText, ref jObject, System.Environment.NewLine, "=");
                    }
                    else
                    {
                        // add normal value
                        jObject[obj.Name] = obj.ChildNodes[0].InnerText;
                    }
                }
                else
                {
                    JToken linkObj = new JObject();
                    jObject[obj.Name] = linkObj;
                    foreach (XmlNode xNode in obj)
                    {
                        LoopThroughXmlForJsonOutput(xNode, ref linkObj);
                    }
                }
            }
        }

        /// <summary>
        /// Convert plain text value to json
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="jObject"></param>
        private static void ConvertPlainTextValueToJson(string name, string value, ref JToken jObject)
        {
            string carriagereturn = "\r\n";
            string spliter = "=";

            ParseTextToJsonObject(name, value, ref jObject, carriagereturn, spliter);   
        }

        /// <summary>
        /// Convert plain text value to json
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="jObject"></param>
        private static void ConvertPlainTextValueToJson(string name, string value, ref JToken jObject, string nameValueDelimiter, string nameValueSeperator)
        {            
            ParseTextToJsonObject(name, value, ref jObject, nameValueDelimiter, nameValueSeperator);
        }

        /// <summary>
        /// Convert plain text value to json
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="jObject"></param>
        private static void ConvertPlainTextValueToJsonV2(string name, string value, ref JToken jObject, string nameValueDelimiter, string nameValueSeperator)
        {
            ParseTextToJsonObjectV2(name, value, ref jObject, nameValueDelimiter, nameValueSeperator);
        }

        private static void ParseTextToJsonObject(string name, string value, ref JToken jObject, string carriagereturn, string spliter)
        {
            // parse special attribute value
            string specialValue = value;
            string[] strLines = specialValue.Split(carriagereturn.ToCharArray());

            string tracker = "";
            JToken linkObj = new JObject();
            jObject[name] = linkObj;
            string lookForValue = "";
            JToken preserveLink = new JObject();
            if (strLines.Count() > 0)
            {
                for (int i = 0; i < strLines.Count(); i++)
                {
                    //loop through
                    //input.Contains("Net");
                    //jObject[strLines[i]] = strLines[i];
                    string check = strLines[i].Trim();
                    string[] el = check.Split(spliter.ToCharArray());
                    /*
                     * Test only
                    if (el[0].Trim().Equals("MscOrVlrAreaRoamingRestrict"))
                    {
                        Console.WriteLine(check);
                    }
                     * 
                     */
                    if (el.Count() == 2 && check.StartsWith("=") && !(check.Trim().Equals("")) && el[0].Equals(""))
                    {
                        //tracker = el[0].Trim();
                        if (lookForValue.Length > 0)
                        {
                            if (preserveLink != null)
                            {
                                preserveLink[lookForValue] = el[1].Trim();
                            }
                            else
                            {
                                linkObj[lookForValue] = el[1].Trim();
                            }
                            lookForValue = "";
                        }
                    }
                    else if (el.Count() == 2 && check.EndsWith("="))
                    {
                         if (tracker != "")
                        {
                            lookForValue = el[0].Trim().Replace(" ", "");                            
                        }
                    }
                    else if (el.Count() == 1 && check.StartsWith("\"") && check.EndsWith("\""))
                    {
                        tracker = el[0].Trim().Replace(" ", "").Replace("\"", "");
                        preserveLink = new JObject();
                        linkObj[tracker] = preserveLink;
                    }
                    else if (el.Count() == 1 && !check.StartsWith("\"") && !check.EndsWith("\"") && check.Length > 0)
                    {
                        lookForValue = el[0].Trim().Replace(" ", "");
                    }
                    else if (el.Count() == 2 && !(check.Trim().Equals("")) && !el[0].Equals("") && !el[1].Equals(""))
                    {
                        // add 
                        if (tracker.Length > 0)
                        {

                            preserveLink[el[0].Trim().Replace(" ", "")] = el[1].Trim();
                        }
                        else
                        {
                            linkObj[el[0].Trim().Replace(" ", "")] = el[1].Trim();
                        }
                    }
                    else
                    {
                        // do thing
                    }
                }
            }
        }

      

        private static void ParseTextToJsonObjectV2(string name, string value, ref JToken jObject, string carriagereturn, string spliter)
        {
            // parse special attribute value
            string specialValue = value;
            string[] strLines = specialValue.Split(carriagereturn.ToCharArray());

            string tracker = "";
            JToken linkObj = new JObject();
            jObject[name] = linkObj;
            string lookForValue = "";
            JToken preserveLink = new JObject();
            int pairStep = 0;
            if (strLines.Count() > 0)
            {
                for (int i = 0; i < strLines.Count(); i++)
                {
                    //loop through
                    //input.Contains("Net");
                    //jObject[strLines[i]] = strLines[i];
                    string check = strLines[i].Trim();
                    string[] el = check.Split(spliter.ToCharArray());
                    /*
                     * Test only
                    if (el[0].Trim().Equals("MscOrVlrAreaRoamingRestrict"))
                    {
                        Console.WriteLine(check);
                    }
                     * 
                     */
                    if (check.Trim().Equals("CFB"))
                    {
                        Console.WriteLine(check);
                    }

                    // Assign the look for value
                    if (el.Count() == 2 && check.StartsWith("=") && !(check.Trim().Equals("")) && el[0].Equals(""))
                    {
                        //tracker = el[0].Trim();
                        if (lookForValue.Length > 0)
                        {
                            if (preserveLink != null)
                            {
                                preserveLink[lookForValue] = el[1].Trim();
                            }
                            else
                            {
                                linkObj[lookForValue] = el[1].Trim();
                            }
                            lookForValue = "";
                        }
                    }
                    else if (el.Count() == 2 && check.EndsWith("="))
                    {
                        if (tracker != "")
                        {
                            lookForValue = el[0].Trim().Replace(" ", "");                            
                        }
                        //else
                        //{
                        //    linkObj[el[0]] = "";
                        //}

                    }
                    else if (el.Count() == 1 && check.StartsWith("\"") && check.EndsWith("\""))
                    {
                        tracker = el[0].Trim().Replace(" ", "").Replace("\"", "");
                        preserveLink = new JObject();
                        linkObj[tracker] = preserveLink;

                    }
                    else if (el.Count() == 1 && !check.StartsWith("\"") && !check.EndsWith("\"") && check.Length > 0)
                    {
                        if(tracker.Equals("BasicService") 
                            && (
                            el[0].Contains("Telephony") == true
                            || el[0].Contains("Emergency Call") == true
                            || el[0].Contains("Short Message") == true
                            || el[0].Contains("Telephony") == true
                            || el[0].Contains("General-DataCDS") == true
                             
                            )
                            ){

                            preserveLink[el[0].Trim().Replace(" ", "").Replace("(","").Replace(")","")] = true;
                            lookForValue = "";
                        }
                        else if(lookForValue != "" ) 
                        {
                            //preserveLink[lookForValue] = el[0].Trim().Replace(" ", "");
                            // if look for value is not empty
                            // and 
                            //lookForValue = el[0].Trim().Replace(" ", "");

                            // add 
                            if (tracker.Length > 0)
                            {
                                string xname = FixDuplicate(preserveLink, lookForValue);
                                preserveLink[xname] = el[0].Trim();
                                lookForValue = "";
                            }
                            else
                            {
                                string xname = FixDuplicate(linkObj, lookForValue);
                                linkObj[xname] = el[0].Trim();
                                lookForValue = "";
                            }

                            //lookForValue = "";
                        }
                        else
                        {
                            lookForValue = el[0].Trim().Replace(" ", "");
                        }   
                    }
                    else if (el.Count() == 2 && !(check.Trim().Equals("")) && !el[0].Equals("") && !el[1].Equals("") && String.IsNullOrWhiteSpace(lookForValue) != true && pairStep > 0)
                    {

                        tracker = lookForValue;
                        bool checkContainKey = false;
                        try
                        {
                            var x = linkObj[tracker];
                            if (x != null)
                                checkContainKey = true;
                        }
                        catch
                        {
                        }
                        if (checkContainKey == false)
                        {
                            preserveLink = new JObject();
                            linkObj[tracker] = preserveLink;
                        }

                        if (tracker.Length > 0)
                        {
                            string xname = FixDuplicate(preserveLink, el[0].Trim().Replace(" ", ""));
                            preserveLink[xname] = el[1].Trim();
                        }
                        lookForValue = "";
                    }
                    else if (el.Count() == 2 && !(check.Trim().Equals("")) && !el[0].Equals("") && !el[1].Equals("") && check.IndexOf("%") == -1)
                    { // add
                        if (pairStep == 0)
                        {
                            pairStep++;
                            lookForValue = "";
                        }

                        if (tracker.Length > 0)
                        {
                            string xname = FixDuplicate(preserveLink, el[0].Trim().Replace(" ", ""));
                            preserveLink[xname] = el[1].Trim();
                        }
                        else
                        {
                            string xname = FixDuplicate(linkObj, el[0].Trim().Replace(" ", ""));
                            linkObj[xname] = el[1].Trim();
                        }
                    }
                    else
                    {
                        // do thing
                    }
                }
            }
        }

        /// <summary>
        /// Loop through xml for XML to JSON conversion with dumplicate item and remove empty input
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jObject"></param>
        private static void XmlToJsonOutputWithDumplicate(XmlNode obj, ref JToken jObject)
        {
            if (obj.HasChildNodes)
            {
                if (obj.ChildNodes.Count == 1 && obj.ChildNodes[0].HasChildNodes == false)
                {
                    jObject[obj.Name] = obj.ChildNodes[0].InnerText;
                }
                else
                {
                    JToken linkObj = new JObject();
                    string ObjName = FixDuplicate(jObject, obj.Name);
                    jObject[ObjName] = linkObj;
                    foreach (XmlNode xNode in obj)
                    {
                        XmlToJsonOutputWithDumplicate(xNode, ref linkObj);
                    }
                }
            }


        }

        private static string FixDuplicate(JToken preserveLink, string name)
        {
            if (preserveLink[name] != null)
            {
                int i = 1;
                string subname = name;
                while (true)
                {

                    if (preserveLink[subname + i] == null)
                    {
                        return subname + i;
                    }
                    i++;
                }
            }
            return name;
        }

        private static void ParseTextToJsonObjectV3(string name, string value, ref JToken jObject, string carriagereturn, string spliter)
        {
            // parse special attribute value
            string specialValue = value;
            string[] strLines = specialValue.Split(carriagereturn.ToCharArray());

            string tracker = "";
            JToken linkObj = new JObject();
            jObject[name] = linkObj;
            string lookForValue = "";
            JToken preserveLink = new JObject();
            int pairStep = 0;
            bool isUsingDoubleQuoteParent = false;
            if (strLines.Count() > 0)
            {
                for (int i = 0; i < strLines.Count(); i++)
                {
                    //loop through
                    //input.Contains("Net");
                    //jObject[strLines[i]] = strLines[i];
                    string check = strLines[i].Trim();
                    string[] el = check.Split(spliter.ToCharArray());
                    if (check.IndexOf("General-DataCDS") > -1)
                        System.Console.WriteLine();
                    /*
                     * Test only
                    if (el[0].Trim().Equals("MscOrVlrAreaRoamingRestrict"))
                    {
                        Console.WriteLine(check);
                    }
                     * 
                     */
                    if (el.Count() == 2 && check.StartsWith("=") && !(check.Trim().Equals("")) && el[0].Equals(""))
                    {
                        tracker = el[0].Trim();
                        if (lookForValue.Length > 0)
                        {
                            if (preserveLink != null)
                            {
                                preserveLink[lookForValue] = el[1].Trim();
                            }
                            else
                            {
                                linkObj[lookForValue] = el[1].Trim();
                            }
                            lookForValue = "";
                        }
                    }                    
                    else if (el.Count() == 2 && check.EndsWith("="))
                    {
                        linkObj[el[0]] = "";
                    }
                    else if (el.Count() == 1 && check.StartsWith("\"") && check.EndsWith("\""))
                    {
                        tracker = el[0].Trim().Replace(" ", "").Replace("\"", "");
                        preserveLink = new JObject();
                        linkObj[tracker] = preserveLink;
                        isUsingDoubleQuoteParent = true;
                    }
                    else if (el.Count() == 1 && !check.StartsWith("\"") && !check.EndsWith("\"") && check.Length > 0 )
                    {
                        lookForValue = el[0].Trim().Replace(" ", "");
                        //string nextline = null;
                        //try
                        //{
                        //    nextline = strLines[i + 1].Trim();
                        //    string[] el2 = nextline.Split(spliter.ToCharArray());
                        //    if (el2.Count() == 2 && nextline.StartsWith("=") && !(nextline.Trim().Equals("")) && el2[0].Equals(""))
                        //        lookForValue = el[0].Trim().Replace(" ", "");
                        //    else if (el2.Count() == 2 && !(nextline.Trim().Equals("")) && !el2[0].Equals("") && !el2[1].Equals("") && nextline.IndexOf("%") == -1)
                        //    {                                                                
                        //        if (isUsingDoubleQuoteParent == false)
                        //        {
                        //            lookForValue = el[0].Trim().Replace(" ", "");
                        //        }
                        //    }                            
                        //}
                        //catch
                        //{
                        //}                                                
                    }
                    else if (el.Count() == 2 && !(check.Trim().Equals("")) && !el[0].Equals("") && !el[1].Equals("") && String.IsNullOrWhiteSpace(lookForValue) != true && pairStep > 0)
                    {
                       
                        tracker = lookForValue;
                        bool checkContainKey = false;
                        try
                        {
                            var x = linkObj[tracker];
                            if (x != null)
                                checkContainKey = true;
                        }
                        catch
                        {
                        }
                        if (checkContainKey == false)
                        {
                            preserveLink = new JObject();
                            linkObj[tracker] = preserveLink;
                        }

                        if (tracker.Length > 0)
                        {
                            preserveLink[el[0].Trim().Replace(" ", "")] = el[1].Trim();
                        }
                    }
                    else  if (el.Count() == 2 && !(check.Trim().Equals("")) && !el[0].Equals("") && !el[1].Equals("") && check.IndexOf("%") == -1){ // add
                        if (pairStep == 0)
                        {
                            pairStep++;
                            lookForValue = "";
                        }                        

                        if (tracker.Length > 0)
                        {

                            preserveLink[el[0].Trim().Replace(" ", "")] = el[1].Trim();
                        }
                        else
                        {
                            linkObj[el[0].Trim().Replace(" ", "")] = el[1].Trim();
                        }                        
                    }
                    else
                    {
                        // do thing
                    }
                }
            }
        }

        public static string ConvertVPNResultToJson(string result, ref string responseCode)
        {
            string pattern = @"(\w*)\s?,\s?("".*?""|\w*)";
            string pattern1 = @""".*?""";
            RegexOptions option = RegexOptions.IgnoreCase;
            string match = string.Empty;
            string tempName = string.Empty;
            object oldValue = null;
            string deliminator = ",";
            Dictionary<object, object> str = new Dictionary<object, object>();
            string json = string.Empty;
            responseCode = "-1";

            //Add response code the JSON result
            if (result.Contains("RESP:"))
            {
                int startIndex = result.IndexOf(":") + 1;
                int endIndex = 0;

                if (result.IndexOf(":", startIndex) > -1)
                    endIndex = result.IndexOf(":", startIndex) - startIndex;
                else
                    endIndex = result.IndexOf(";") - startIndex;

                responseCode = result.Substring(startIndex, endIndex);
                str.Add("responseCode", responseCode);
            }

            //Split parameters by its deliminator
            //Colon(:) - Separate between each CAI parameter
            //Comma(,) - Separate between parameter value and sub parameter
            foreach (Match m in Regex.Matches(result, pattern, option))
            {
                match = m.Value.ToString();
                string name = match.Substring(0, match.IndexOf(",")).Trim();
                string value = match.Substring(match.IndexOf(",") + 1).Trim();

                if (string.IsNullOrEmpty(name))
                {
                    AddOrUpdate(str, tempName, value, ref oldValue, value, deliminator);
                }
                else
                {
                    str.Add(name, value);
                    tempName = name;
                }
            }

            //Loop through each element of dictionary
            //Format only those with multiple value
            //a value with comma consider as multiple value
            List<object> keys = new List<object>(str.Keys); //Cannot update IEnumerable value using foreach loop
            foreach (var key in keys)
            {
                object obj = str[key];
                string temp = obj.ToString();
                if (temp.Contains(","))
                {
                    List<object> arr = new List<object>(); //To store value as list
                    Dictionary<object, object> subItem = null;

                    //For values look like this 
                    //DN,"protocolVersion=CAP V2,applicationName=JasManObj,nodeName=jambala"
                    //features,"pnpFeatureName=PNP,vpnServiceName=JVPN,providerName=JamTel,applicationName=JasManObj,nodeName=JAMBALA:0:Activated",
                    //         "ocsFeatureName=OCS,vpnServiceName=JVPN,providerName=JamTel,applicationName=JasManObj,nodeName=jambala:1:Activated",
                    if (temp.Contains(",") && temp.Contains('"') && temp.Contains("="))
                    {
                        foreach (Match m in Regex.Matches(temp, pattern1, option))
                        {
                            string name = string.Empty;
                            string value = string.Empty;
                            string[] items = m.Value.ToString().Split(',');
                            subItem = new Dictionary<object, object>();
                            foreach (string item in items)
                            {
                                if (item.Contains("="))
                                {
                                    name = item.Substring(0, item.IndexOf("=")).Replace("\"", "").Trim();
                                    value = item.Substring(item.IndexOf("=") + 1).Replace("\"", "").Trim();
                                }
                                else
                                {
                                    value = item.Replace("\"", "").Trim();
                                }

                                AddOrUpdate(subItem, name, value, ref oldValue, value, deliminator);
                            }
                            arr.Add(subItem);
                        }

                        if (arr.Count == 1)
                            TryUpdate(str, key, subItem, temp);
                        else
                            TryUpdate(str, key, arr, temp);
                    }
                    else
                    {
                        //For value look like this 
                        //"4010:-:-:1:0:Local","4011:-:-:1:1:Local" or 
                        //"37555,5,31161545861,11","47555,5,4684045861,10" or
                        //"46,[[03,1,46],[06,0,46]],[00],0,46":ctOidCcTable,"10,2,31","40,2,31
                        if (temp.Contains(",") && temp.Contains('"'))
                        {
                            foreach (Match m in Regex.Matches(temp, pattern1, option))
                            {
                                arr.Add(m.Value.ToString().Replace("\"", "").Trim());
                            }
                        }
                        //For value look like this
                        //9647717267021,9647717267022,9647717267023,9647717267024
                        //27880,37880,47880,57880
                        else
                        {
                            string[] items = temp.Split(',');
                            foreach (string i in items)
                            {
                                arr.Add(i.Replace("\"", "").Trim());
                            }
                        }

                        TryUpdate(str, key, arr, temp);
                    }
                }
                else
                {
                    //Omit double quote for all values
                    TryUpdate(str, key, temp.Replace("\"", "").Trim(), temp);
                }
            }

            json = JsonConvert.SerializeObject(str, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        private static void AddOrUpdate(Dictionary<object, object> dic, object key, object value, ref object oldValue, string newValue, string deliminator)
        {
            if (dic.ContainsKey(key))
            {
                oldValue = dic[key];
                dic[key] = oldValue + deliminator + newValue;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        private static void TryUpdate(Dictionary<object, object> dic, object key, object newValue, object valueToCompare)
        {
            if (dic.ContainsKey(key))
            {
                if (dic[key] == valueToCompare)
                {
                    dic[key] = newValue;
                }
            }
        }
    }
}
