using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Asiacell.ITADLibraries_v1.Utilities
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
                        ConvertPlainTextValueToJson(obj.Name, obj.ChildNodes[0].InnerText, ref jObject);
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
    }
}
