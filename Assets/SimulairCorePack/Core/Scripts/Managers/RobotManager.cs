/// Contols spawning and removing robots.
///TODO make sure there is only one RobotoManager inside the scene 
/// 

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace SimulairCorePack.Core
{
    /// <summary>
    /// Robot Manager is a shared component that is responsible for creating and destroying robots.
    /// NOTE!: There is only one RobotManager allowed in one scene. Singleton implementation is made
    /// for providing this.
    /// </summary>
    [DisallowMultipleComponent]
    public class RobotManager : MonoBehaviour
    {
        private static RobotManager instance = null;

        public static RobotManager Instance
        {
            get => instance;
        }

        private int nextRobotIndex = 1;
        private Dictionary<string, RobotIdentifier> robots;

        private const string DEFAULT_NAME_PREFIX = "robot_";
        private void Awake()
        {
            init();
        }

        private void init()
        {
            #region init fields

            robots = new Dictionary<string, RobotIdentifier>(); 

            #endregion

            #region make sure there is only one RobotManager
            
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else if (instance == null)
            {
                instance = this;
            }
            
            #endregion

            #region temporary
                            
            //init temporary robot database 
            //entry 1
                //type_id : "123kiwi123"
                //owner_id : "tahameg"
                //resource name kiwibotv1_0


                #endregion
        }

        public void SpawnRobot(string _name, string _type_id, string _user_id, Vector3 position)
        {
            //check if type_id exists in the database
                //else throw entry not found exception
            //check if _name is valid and unique
               // else throw name exception

            string prefabName = "kiwibotv1_0"; //TODO Get this from the entry
            
            GameObject go = Resources.Load<GameObject>("robots/" + prefabName);
            if (go != null)
            {
                go = Instantiate(go, position, Quaternion.identity);
                RobotIdentifier ri = go.AddComponent<RobotIdentifier>();
                ri.Set(nextRobotIndex++, _name, _type_id, _user_id);
                robots.Add(_name, ri);
                go.name = _name;

            }
            else
            {
                Debug.Log("throw resource not found exception");
            }
        }

        public void SpawnRobot(string _type_id, string _user_id, Vector3 position)
        {
            string name = DEFAULT_NAME_PREFIX + nextRobotIndex;
            SpawnRobot(name, _type_id, _user_id, position);
        }
    }
}