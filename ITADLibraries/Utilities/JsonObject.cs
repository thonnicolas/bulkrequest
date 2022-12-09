using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Asiacell.ITADLibraries.Utilities
{
    /// <summary>
    /// Json Object class
    /// </summary>
    public class JsonObject
    {

        /**
         * 
         * Example: 
         *  
        JsonObject o = new JsonObject();
                    
        string v = o.GetJsonQueryObject(jObject, "BasicService.DefaultCall").GetValue();
        or
        o.GetJsonQueryObject(obj, "BasicService.DefaultCall");            
        string v = o.GetValue();
            
        JsonObject o2 = new JsonObject();
        string v = o2.GetJsonQueryObject(jsonString, "ODBData").GetValue("ODBPB3");
        dyanmic d = o2.GetJsonQueryObject(jsonString, "ODBData").GetValue("ODBPB3");
        int b2 = o.GetJsonQueryObject(obj, "ussdEndOfCallNotificationID").GetValue();
        Assert.AreEqual(b2,1);
        int b3 = o.GetJsonQueryObject(obj, "serviceRemovalPeriod").GetValue();
        Assert.AreEqual(b3, 100);            
        Assert.AreEqual(o.GetJsonQueryObject(obj, "serviceOfferings").ElementAt(2).serviceOfferingID.Value, 3);
         * */

        private dynamic _dynamicOjb = new { };
        private JObject _jsonObject = new JObject();
        
        public dynamic ElementAt(int index)
        {
            if (this._dynamicOjb.Type == JTokenType.Array)
            {
                return this._dynamicOjb[index];
            }
            return null;
        }

        public dynamic ElementWhere(string conditionKey, string objectValue)
        {
            try
            {
                if (this._dynamicOjb.Type == JTokenType.Array)
                {
                    foreach (var m in this._dynamicOjb)
                    {
                        if (m[conditionKey].ToString().Equals(objectValue))
                        {
                            return m;
                        }
                    }
                }
            }
            catch (Exception ee) { }
            return null;
        }


        /// <summary>
        /// Get Value by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic GetValue(string name = "")
        {
            try
            {
                return GetValueByName(name);
            }
            catch (Exception e)
            {
                // Could not get the value from json object
            }

            return null;
        }

        /// <summary>
        /// Get Value By name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private dynamic GetValueByName(string name = "")
        {
            dynamic value = null;
            if (name == "")
            {
                value = this._dynamicOjb;
            }
            else
            {
                value = this._dynamicOjb[name];
            }
            
            return GetValueWithType(value);
                        
        }

        /// <summary>
        /// Query JSON object from json string
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>       
        public JsonObject GetXmlRpcToJsonQueryObject(string json, string path)
        {
            return GetJsonQueryObject(XmlRpcToJson.ConvertToJson(json), path);                        
        }

        /// <summary>
        /// Query JSON object from json string
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>       
        public JsonObject GetXmlToJsonQueryObject(string json, string path)
        {
            return GetJsonQueryObject(JObject.Parse(JsonParser.ConvertXmlToJson(json)), path);
        }

        public JsonObject GetJsonQueryObjectFromJsonString(string json, string path)
        {
            return GetJsonQueryObject(JObject.Parse(json), path);
        }

        /// <summary>
        /// Query JSON object from json string
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>       
        public JsonObject GetTextToJsonQueryObject(string json, string path)
        {
            return GetJsonQueryObject(JObject.Parse(JsonParser.ConvertTextToJson(json)), path);
        }

        /// <summary>
        /// Query JSON from JObject
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public JsonObject GetJsonQueryObject(JObject json, string path)
        {
            this._jsonObject = json;
            this._dynamicOjb = null;
            try
            {
                GetObjectFromJson(this._jsonObject, path, ref this._dynamicOjb);
              
            }
            catch (Exception ex)
            {
                this._dynamicOjb = null;
            }
            return this;
        }


        /// <summary>
        /// Get Object from json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="dynamicObj"></param>
        /// <returns></returns>
        private bool GetObjectFromJson(JToken obj, string path, ref dynamic dynamicObj)
        {
            if (obj.Path.Contains(path))
            {
                //if the result is a simple value
                if (obj.HasValues == false)
                {
                    dynamicObj = GetValueWithType(obj);
                }
                else if (obj.HasValues == true)
                {
                    dynamicObj = obj;
                }
                return true;
            }

            if (obj.HasValues)
            {
                foreach (var sub in obj)
                {
                    if (GetObjectFromJson(sub, path, ref dynamicObj) == true)
                        return false;
                }
            }

            return false;
        }

        private static dynamic GetValueWithType(JToken obj )
        {
            dynamic dynamicObj = new {};
            if (obj.Type == JTokenType.Boolean)
            {
                dynamicObj = Convert.ToBoolean(obj);
            }
            else if (obj.Type == JTokenType.String)
            {
                dynamicObj = obj.ToString();
            }
            else if (obj.Type == JTokenType.Integer)
            {
                dynamicObj = Convert.ToInt32(obj);
            }
            else if (obj.Type == JTokenType.Array)
            {
                dynamicObj = obj.Children().ToArray();
            }
            else
            {
                dynamicObj = obj;
            }
            return dynamicObj;
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="resultJson"></param>
        /// <returns></returns>
        public JObject ApplyFilter(string searchString, string json)
        {
            JObject resultJson = JObject.Parse(json);

            return ApplyFilter(searchString, resultJson);
        }

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="resultJson"></param>
        /// <returns></returns>
        private JObject ApplyFilter(string searchString, JObject resultJson)
        {
            JObject outputObject = new JObject();
            string[] searchList = searchString.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            int searchMode = 0;
            for (int i = 0; i < searchList.Length; i++)
            {
                string search = String.Empty;
                if (searchList[i].StartsWith("*"))
                {
                    searchMode = 1;
                    search = searchList[i].Replace("*", String.Empty);
                }
                else if (searchList[i].StartsWith("="))
                {
                    searchMode = 0;
                    search = searchList[i].Replace("=", String.Empty);
                }
                else
                {
                    searchMode = 2;
                    search = searchList[i].Trim();
                }

                FilterNodeByName(resultJson, outputObject, search, searchMode);
            }
            return outputObject;
        }

        /// <summary>
        /// Filter nodes by name
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="outputObject"></param>
        /// <param name="searchString"></param>
        /// <param name="searchType"></param>
        private void FilterNodeByName(JObject jo, JObject outputObject, string searchString, int searchType = 0) // searchType 0 mean equal and 1 means contains
        {

            string searchStringUpper = searchString.ToUpper().Trim();

            List<JToken> query = null;

            if (searchType == 0 || searchType == 1)
            {
                var xQuery = jo.Descendants()
                     .Where(t => t.Type == JTokenType.Property &&
                         ((((JProperty)t).Name.ToUpper().Contains(searchStringUpper) && searchType == 1) ||
                         (((JProperty)t).Name.ToUpper() == searchStringUpper && searchType == 0))
                     )
                     .Select(p => ((JProperty)p).Value);
                if (xQuery.Count() > 0)
                    query = xQuery.ToList();
                else
                    query = new List<JToken>();

            }
            else
            {

                JToken obj = jo.SelectToken(searchString);
                query = new List<JToken>();
                if (obj != null)
                    query.Add(obj);

            }

            for (int i = 0; i < query.Count(); i++)
            {
                //if (i > 0)
                //    skipCreate = true;
                var path = query.ElementAt(i).Path;
                var splittedPath = path.Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                FindAscendances(outputObject, 0, splittedPath, outputObject, query.ElementAt(i));
            }
            //return outputObject;
        }

        /// <summary>
        /// Generate all ascendance nodes link
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        /// <param name="splitted"></param>
        /// <param name="mainObject"></param>
        /// <param name="lastChild"></param>
        public void FindAscendances(JToken node, int level, string[] splitted, JObject mainObject, JToken lastChild)
        {
            // get node name
            string nodeName = splitted[level];
            string parentNode = String.Empty;

            if (level > 0)
            {
                parentNode = splitted[level - 1];
            }

            JToken child = null;
            if (nodeName.Contains("[") && nodeName.Contains("]"))
            {
                child = new JArray();
                nodeName = nodeName.Substring(0, nodeName.IndexOf('['));
            }
            else
            {
                child = new JObject();
            }


            // get next name
            if (level < splitted.Length - 1)
            {

                var checkObject = node.SelectToken(nodeName);
                if (checkObject == null)
                {
                    // create new node
                    if (parentNode.Contains("[") && parentNode.Contains("]"))
                    {
                        JObject obj = new JObject();
                        obj.Add(nodeName, child);

                        ((JArray)node).Add(obj);
                    }
                    else
                        ((JObject)node).Add(nodeName, child);
                }
                else
                    child = checkObject;

                FindAscendances(child, level + 1, splitted, mainObject, lastChild);
            }
            else
            {
                if (node.Type == JTokenType.Array)
                {
                    var lastNodeName = splitted[level];
                    JObject obj = new JObject();
                    obj.Add(lastNodeName, lastChild);
                    ((JArray)node).Add(obj);
                }
                else
                    ((JObject)node).Add(nodeName, lastChild);
            }
        }
    }
}
