using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Asiacell.ITADLibraries_v1.Utilities
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
    }
}
