using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using FMSocketIO;

namespace SimulairCorePack
{
    
    public enum InitializationMode
    {
        DEFAULT,
        SINGLE,
        JSON
    }

    public class StartupConfigurationManager : MonoBehaviour
    {
        public FMSocketIOManager socketManager;
        //public junkscript js;
        private InitializationMode _mode = InitializationMode.DEFAULT;

        private string[] args;
        private bool initialized = false;

        /// <summary>
        /// holds initialization actions. Key is name of the param. Callback string is value of the param
        /// </summary>
        private Dictionary<string, Action<string>> handles;

        // Start is called before the first frame update
        private void Awake()
        {
            parseCommandlineParams();
        }

        private void parseCommandlineParams()
        {
            try
            {
                Init();
                if (initialized)
                {
                    if (_mode == InitializationMode.JSON)
                    {
                        handleJSON(args[0]);
                    }
                    else if (_mode == InitializationMode.SINGLE)
                    {
                        handleSingle(args);
                    }
                }
            }
            catch (Exception e)
            {
                handleError();
                //js.printLine(e.Message + " " + e.Source + " " + e.Data + " " + e.StackTrace + " " );
            }

        }

        private void Init()
        {
            //js = junkscript.instance;
            handles = new Dictionary<string, Action<string>>();
            string[] args = Environment.GetCommandLineArgs().Where((v, idx) => idx != 0).ToArray();
            if (args.Length == 0)
            {
                //js.printLine("configuration is handled in default mode(no configuration).");
                return;
            }
            else
            {
                if (args[0].StartsWith("-"))
                {
                    string t = args[0].Remove(0, 1);
                    if ((t == "j" | t == "-json") && args.Length > 1)
                    {
                        _mode = InitializationMode.JSON;
                        this.args = args.Where((v, idx) => idx != 0).ToArray();
                        //js.printLine("configuration is handled in Json mode.");
                        initialized = true;
                    }
                    else
                    {
                        handleError();
                    }
                }
                else
                {
                    _mode = InitializationMode.SINGLE;
                    this.args = args;
                    //js.printLine("configuration is handled in Single mode.");
                    initialized = true;
                }
            }

            initCallbacks();
        }

        private string importJsonWithPath(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                return r.ReadToEnd();
            }
        }

        /// <summary>
        /// Calls initialization callbacks from a config.json file.
        /// </summary>
        /// <param name="path"></param>
        private void handleJSON(string path)
        {
            string jsonData;
            if (Path.IsPathRooted(path))
            {
                jsonData = importJsonWithPath(path);
            }
            else
            {
                string cwd = Directory.GetCurrentDirectory();
                jsonData = importJsonWithPath(Path.Combine(cwd, path));
            }

            InitializationParams _params = InitializationParams.CreateFromJSON(jsonData);
            foreach (var entry in _params.GetDictionary())
            {
                if (entry.Value != null) On(entry.Key, entry.Value);
            }
        }

        private void handleSingle(string[] args)
        {
            foreach (var v in args)
            {
                (string key, string val) = parseArg(v);
                //js.printLine("key: " + key +" val: " + val);
                On(key, val);
            }
        }

        private (string key, string val) parseArg(string _param)
        {

            string[] pair = _param.Split('=');
            if (pair[0] != "" && pair.Length > 1)
            {
                return (pair[0], pair[1]);
            }

            return ("null", "null");
        }

        private void printUsage()
        {
            string usage = "\n \n usage: [unity file] [MODE] [PARAM_NAME=PARAM_VALUE] \n \n" +
                           "Modes: \n" +
                           " [EMPTY]       :  Single Mode. Enter parameters with [PARAM_NAME=PARAM_VALUE] \n" +
                           "  -j, --json   :  Json Mode. Enter a path after this. \n \n" +
                           "Parameters: \n" +
                           "socketIP       : IP of the socket server. \n" +
                           "socketPort     : Port of the socket server. \n";
        }

        private void On(string param_name, string param_value)
        {
            if (handles.ContainsKey(param_name))
            {
                handles[param_name].Invoke(param_value);
                //js.printLine("key found: " + param_name);
            }
            else
            {
                //js.printLine("no such key: " + param_name);
            }

        }

        private void handleError()
        {
            printUsage();
        }

        private void initCallbacks()
        {
            handles.Add("socketIP", setSocketIP);
            handles.Add("socketPort", setSocketPort);
        }

        private void setSocketIP(string IP)
        {
            socketManager.Action_SetIP(IP);
            //js.printLine("Socket IP is set to " + IP);
        }

        private void setSocketPort(string Port)
        {
            socketManager.Action_SetPort(Port);
            //js.printLine("Socket PORT is set to " + Port);
        }
        
    }
}