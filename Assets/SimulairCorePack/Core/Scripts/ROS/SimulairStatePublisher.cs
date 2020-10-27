/*
© Dyno Robotics, 2019
Author: Samuel Lindgren (samuel@dynorobotics.se)
Licensed under the Apache License, Version 2.0
*/

using System.Collections;
using System.Collections.Generic;
using builtin_interfaces.msg;
using UnityEngine;
using rclcs;

namespace SimulairCorePack.Core
{
    public class SimulairStatePublisher : MonoBehaviourRosNode
    {
        
        public string NodeName = "simulair";

        public float ClockPublishingFrequency = 10.0f;
        public bool PublishClock = true;

        protected override string nodeName
        {
            get { return NodeName; }
        }

        private builtin_interfaces.msg.Time clockMsg;
        private Publisher<builtin_interfaces.msg.Time> clockPublisher;

        protected override void StartRos()
        {
            clockMsg = new builtin_interfaces.msg.Time();
            clockPublisher = node.CreatePublisher<builtin_interfaces.msg.Time>("clock");

            StartCoroutine("PublishClockRoutine");
        }

        IEnumerator PublishClockRoutine()
        {
            for (;;)
            {
                if (PublishClock)
                {
                    clockMsg = new builtin_interfaces.msg.Time();
                    clockMsg.Sec = clock.Now.sec;
                    clockMsg.Nanosec = clock.Now.nanosec;
                    clockPublisher.Publish(clockMsg);
                }

                yield return new WaitForSeconds(1.0f / ClockPublishingFrequency);
            }
        }
    }
}