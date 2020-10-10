
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using UnityEngine;



namespace SimulairCorePack
{
    [System.Serializable]
    public class InitializationParams
    {
        # region Serializable fields
        //all fields must be null in default
        public string socketIP = null; 
        public int socketPort = default;
        //add more if you like. Then add tham inside GetDictionary function.

        #endregion
    
        public static InitializationParams CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<InitializationParams>(jsonString);
        }

        public string ToJSON()
        {
            InitializationParams instace = this;
            return JsonUtility.ToJson(instace);
        }

        public Dictionary<string, string> GetDictionary()
        {
            Dictionary<string, string> membersDict = new Dictionary<string, string>();
            membersDict.Add("socketIP", socketIP);
            membersDict.Add("socketPort", ((socketPort == default) ? "null" : socketPort.ToString()));
            return membersDict;
        }

    }

}