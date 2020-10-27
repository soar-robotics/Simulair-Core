using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulairCorePack.Core;
public class DummyRobotSpawner : MonoBehaviour
{
    Vector3 startPos = new Vector3(1.0f, 0.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            RobotManager.Instance.SpawnRobot("123123", "tahameg", startPos);
            startPos.x += 1.0f;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            RobotManager.Instance.SpawnRobot("123123", "qweqwe", startPos);
            startPos.x += 1.0f;
        }
    }
}
