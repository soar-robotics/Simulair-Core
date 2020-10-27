using System;
using System.Collections;
using UnityEngine;
using rclcs;

namespace SimulairCorePack.Core
{
    public class TfPublisher : MonoBehaviourRosNode
    {
        private bool ready = false;
        private RobotIdentifier thisRobot;
        public GameObject baseScan;
        
        public string NodeName = "tf_publisher";
        
        private string tfTopic = "tf";
        private string staticTfTopic = "tf_static";
        
        public float TfPublishingFrequency = 10.0f;

        private Rigidbody BaseRigidbody;

        public Transform GlobalReferanceFrame;

        private tf2_msgs.msg.TFMessage tfMsg;
        private Publisher<tf2_msgs.msg.TFMessage> tfPublisher;

        private tf2_msgs.msg.TFMessage tfStaticMsg;
        private Publisher<tf2_msgs.msg.TFMessage> tfStaticPublisher;

        private bool tfStaticInitialized = false;

        protected override string nodeName
        {
            get
            {
                return NodeName;
            }
        }

        protected override void StartRos()
        {
            DeactivateNamespace();
            BaseRigidbody = GetComponent<Rigidbody>();
            if (BaseRigidbody == null)
            {
                Debug.LogError("Base Footprint should have Rigidbody component!");
                Destroy(gameObject);
            }

            StartCoroutine("WaitForRobotIdentifier");
        }
        
        private void Start()
        {
            GlobalReferanceFrame = GameObject.Find("odomOrigin").transform;
        }

        IEnumerator WaitForRobotIdentifier()
        {
            while (thisRobot == null)
            {
                thisRobot = transform.parent.GetComponent<RobotIdentifier>();
                yield return null;
            }
            tfMsg = CreateTfMsg(thisRobot.name + "/odom", thisRobot.name + "/base_footprint");
            tfPublisher = node.CreatePublisher<tf2_msgs.msg.TFMessage>(tfTopic);

            tfStaticMsg = CreateTfMsg(thisRobot.name + "/base_footprint", thisRobot.name + "/base_scan");
            tfStaticPublisher = node.CreatePublisher<tf2_msgs.msg.TFMessage>(staticTfTopic);
            ready = true;
            StartCoroutine("PublishTfRoutine");
        }
        IEnumerator PublishTfRoutine()
        {
            for (;;)
            {
                tfPublisher.Publish(tfMsg);
                tfStaticPublisher.Publish(tfStaticMsg);
                yield return new WaitForSeconds(1.0f / TfPublishingFrequency);
            }
        }
        
        /// <summary>
        /// Creates a Transform message from frame that is specified with "childFrameId" -to-> "frameID" 
        /// </summary>
        /// <param name="frameId">ID of the parent frame.(without forward slash)</param>
        /// <param name="childFrameId">ID of the child frame.(without forward slash)</param>
        /// <returns></returns>
        private tf2_msgs.msg.TFMessage CreateTfMsg(string frameId, string childFrameId) 
        {
            tf2_msgs.msg.TFMessage msg = new tf2_msgs.msg.TFMessage
            {
                Transforms = new geometry_msgs.msg.TransformStamped[1],
            };

            geometry_msgs.msg.TransformStamped transformStamped = new geometry_msgs.msg.TransformStamped();
            transformStamped.Header.Frame_id = "/" + frameId;
            transformStamped.Child_frame_id = "/" + childFrameId;
            msg.Transforms[0] = transformStamped;

            return msg;
        }

        private void FixedUpdate()
        {
            if (ready)
            {
                tfMsg.Transforms[0].Header.Update(clock);
                if (GlobalReferanceFrame == null)
                {
                    tfMsg.Transforms[0].Transform.LocalUnity2Ros(BaseRigidbody.transform);
                }
                else
                {
                    tfMsg.Transforms[0].Transform.Unity2Ros(BaseRigidbody.transform, GlobalReferanceFrame);
                }

                tfStaticMsg.Transforms[0].Header.Update(clock);
                if (!tfStaticInitialized)
                {
                    tfStaticMsg.Transforms[0].Transform.Unity2Ros(baseScan.transform, BaseRigidbody.transform);
                    tfStaticInitialized = true;
                }
            }
        }
    }
}