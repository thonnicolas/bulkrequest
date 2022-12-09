using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Xml;
using System.IO;

namespace Asiacell.ITADLibraries_v1.Utilities
{
    /// <summary>
    /// XmlRpcToJson
    /// </summary>
    public class XmlRpcToJson
    {
        /// <summary>
        /// Convert to JSON
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static JObject ConvertToJson(string xml)
        {
            JToken jp = new JObject();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlLoopThrough(doc, ref jp);
            }
            catch (Exception ex)
            {
 
            }

            return (JObject)jp;
        }
        /// <summary>
        /// Convert value parser
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static JToken ValueParser(string value, string type)
        {
            switch (type.ToLower())
            {
                case "boolean": return (value == "1" ? true : false);
                case "i4": return Convert.ToInt32(value);
                case "integer": return Convert.ToInt32(value);
                case "string": return value;
                //case "dateTime.iso8601": return DateTime.Parse(value);               
            }
            return value;
        }
        /// <summary>
        /// Convert XML RPC to JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jObject"></param>
        private static void XmlLoopThrough(XmlNode obj, ref JToken jObject)
        {
            if (obj.Name == "param")
            {
                //check the child value
                if (obj.FirstChild.Name == "value")
                {
                    // get the child content to assign to result attribute                                                            
                    XmlLoopThrough(obj.FirstChild, ref jObject);
                }
            }
            if (obj.Name == "value")
            {
                // check if the next value is normal data or array or struct
                // check normal data
                if (obj.HasChildNodes)
                {
                    if (obj.FirstChild.Name == "boolean" || obj.FirstChild.Name == "i4"
                        || obj.FirstChild.Name == "string" || obj.FirstChild.Name == "dateTime.iso8601"
                        || obj.FirstChild.Name == "integer")
                    {


                        // check of the normal value                        
                        jObject[obj.ParentNode.FirstChild.InnerText] = ValueParser(obj.InnerText.ToString(), obj.FirstChild.Name.ToString());
                    }
                    else if (obj.FirstChild.Name == "struct")
                    {
                        JToken linkObj = new JObject();
                        if (obj.ParentNode.FirstChild.Name == "name")
                        {
                            // then apply name with new associate array                            
                            jObject[obj.ParentNode.FirstChild.InnerText] = linkObj;
                        }
                        else
                        {
                            linkObj = jObject;
                        }

                        // create associate array                        
                        // then check for the member element
                        foreach (XmlNode m in obj.FirstChild)
                        {
                            if (m.Name == "member")
                            {
                                // do something
                                // create associate array                                
                                XmlLoopThrough(obj.LastChild, ref linkObj);
                            }
                        }
                    }
                    else if (obj.FirstChild.Name == "array")
                    {
                        JToken linkObj = new JArray();
                        if (obj.ParentNode.FirstChild.Name == "name")
                        {
                            // check if the next child value is a normal or struct value
                            // then apply name with new associate array             

                            jObject[obj.ParentNode.FirstChild.InnerText] = linkObj;

                        }
                        else
                        {
                            linkObj = jObject;
                        }

                        foreach (XmlNode m in obj.FirstChild.FirstChild)
                        {
                            // do something
                            // create array                 
                            if (m.Name == "value")
                            {
                                JToken jElement = new JObject();
                                ((JArray)linkObj).Add(jElement);
                                // add as array element
                                XmlLoopThrough(m, ref jElement);
                                jElement = null;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode child in obj.ChildNodes)
                {
                    XmlLoopThrough(child, ref jObject);
                }
            }

        }
    }
}
