using System;
using UnityEngine;

namespace SimulairCorePack.Core
{
    [DisallowMultipleComponent]
    [System.Serializable]
    public class RobotIdentifier : MonoBehaviour
    {
        private int index;
        public string name;
        public string type_id;
        public string user_id;
        
        public int Index
        {
            get => index;
        }

        public void SetIndex(int _index)
        {
            this.index = _index;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        public static RobotIdentifier FromJson(string _json)
        {
            return JsonUtility.FromJson<RobotIdentifier>(_json);
        }

        public void Set(int _index, string _name, string _type_id, string _user_id)
        {
            this.name = _name;
            this.type_id = _type_id;
            this.user_id = _user_id;
            this.SetIndex(_index);
            setNamespace(_name);
        }

        private void setNamespace(string _name)
        {
            setChildNamespaceRecursive(gameObject, _name);
        }

        private void setChildNamespaceRecursive(GameObject obj, string _name){
            if (null == obj)
                return;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                    continue;
                foreach (var node in child.GetComponents<MonoBehaviourRosNode>())
                {
                    if (null == node)
                        continue;

                    node.SetNamespace(_name);
                }


                setChildNamespaceRecursive(child.gameObject, _name);
            }
        }
    }
}