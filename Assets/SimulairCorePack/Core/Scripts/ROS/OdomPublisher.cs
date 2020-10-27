using System.Collections;
using UnityEngine;
using rclcs;

namespace SimulairCorePack.Core
{
    public class OdomPublisher : MonoBehaviourRosNode
    {
        private bool ready = false;
        private RobotIdentifier thisRobot;
        
        public GameObject baseScan;
        
        public string NodeName = "odom_publisher";
        
        private string odometryTopic = "odom";

        public float OdomPublishingFrequency = 10.0f;

        private Rigidbody BaseRigidbody;

        public Transform GlobalReferanceFrame;

        private nav_msgs.msg.Odometry odomMsg;
        private Publisher<nav_msgs.msg.Odometry> odomPublisher;
        
        protected override string nodeName
        {
            get
            {
                return NodeName;
            }
        }
        
        protected override void StartRos()
        {
            BaseRigidbody = GetComponent<Rigidbody>();
            if (BaseRigidbody == null)
            {
                Debug.LogError("Base Footprint should have Rigidbody component!");
                Destroy(gameObject);
            }
            
            StartCoroutine("WaitForRobotIdentifier");
        }
        
        IEnumerator WaitForRobotIdentifier()
        {
            while (thisRobot == null)
            {
                thisRobot = transform.parent.GetComponent<RobotIdentifier>();
                yield return null;
            }
            
            odomMsg = CreateOdometryMsg(thisRobot.name+"/odom", thisRobot.name+"/base_footprint");
            odomPublisher = node.CreatePublisher<nav_msgs.msg.Odometry>(odometryTopic);
            ready = true;
            StartCoroutine("PublishOdomRoutine");
        }
        private void Start()
        {
            GlobalReferanceFrame = GameObject.Find("odomOrigin").transform;
        }

        IEnumerator PublishOdomRoutine()
        {
            for (;;)
            {
                odomPublisher.Publish(odomMsg);
                yield return new WaitForSeconds(1.0f / OdomPublishingFrequency);
            }
        }
        
        /// <summary>
        /// Creates a odometry message from frame that is specified with "childFrameId" -to-> "frameID" 
        /// </summary>
        /// <param name="frameId">ID of the parent frame.(without forward slash)</param>
        /// <param name="childFrameId">ID of the child frame.(without forward slash)</param>
        /// <returns></returns>
        private nav_msgs.msg.Odometry CreateOdometryMsg(string frameId, string childFrameId) 
        {


            nav_msgs.msg.Odometry msg = new nav_msgs.msg.Odometry();
            msg.Header.Frame_id = "/" + frameId;
            msg.Child_frame_id = "/" + childFrameId;

            return msg;
        }

        private void FixedUpdate()
        {
            if (ready)
            {
                odomMsg.Header.Update(clock);
                if (GlobalReferanceFrame == null)
                {
                    odomMsg.Pose.Pose.LocalUnity2Ros(BaseRigidbody.transform);
                }
                else
                {
                    odomMsg.Pose.Pose.Unity2Ros(BaseRigidbody.transform, GlobalReferanceFrame);
                }

                odomMsg.Twist.Twist.Unity2Ros(BaseRigidbody);
            }
        }
    }
}