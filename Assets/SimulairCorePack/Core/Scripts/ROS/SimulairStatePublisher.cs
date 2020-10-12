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

public class SimulairStatePublisher : MonoBehaviourRosNode
{
    public string NodeName = "simulair";
    public string OdometryTopic = "odom";
    //TODO(taha) dont allow others to publish clock

    public float OdometryPublishingFrequency = 10.0f;
    public float TfPublishingFrequency = 10.0f;
    public float ClockPublishingFrequency = 10.0f;

    public Rigidbody BaseRigidbody;

    public Transform GlobalReferanceFrame;

    public bool PublishTf = true;
    public bool PublishClock = true;
    protected override string nodeName { get { return NodeName; } }

    private nav_msgs.msg.Odometry odometryMsg; //=> odom msg var to be sent in every spin
    private Publisher<nav_msgs.msg.Odometry> odometryPublisher;

    private tf2_msgs.msg.TFMessage tfMsg; //=> tf msg var to be sent in every spin
    private Publisher<tf2_msgs.msg.TFMessage> tfPublisher;
    
    private builtin_interfaces.msg.Time clockMsg;
    private Publisher<builtin_interfaces.msg.Time> clockPublisher;

    //private List<double> poseCovarianceDiagonal = new List<double> { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.03d };
    //private List<double> twistCovarianceDiagonal = new List<double> { 0.001d, 0.001d, 0.001d, 0.001d, 0.001d, 0.03d };

    protected override void StartRos()
    {
        odometryMsg = CreateOdometryMsg();
        odometryPublisher = node.CreatePublisher<nav_msgs.msg.Odometry>(OdometryTopic);

        tfMsg = CreateTfMsg();
        tfPublisher = node.CreatePublisher<tf2_msgs.msg.TFMessage>("tf");
        
        clockMsg = new builtin_interfaces.msg.Time();
        clockPublisher = node.CreatePublisher<builtin_interfaces.msg.Time>("clock");
            
        if (BaseRigidbody == null)
        {
            BaseRigidbody = GetComponent<Rigidbody>();
        }

        StartCoroutine("PublishOdometryRoutine");
        
        if (PublishTf)
        {
            StartCoroutine("PublishTfRoutine");
        }

        if (PublishClock)
        {
            StartCoroutine("PublishClockRoutine");
        }
    }

    IEnumerator PublishOdometryRoutine()
    {
        for (;;)
        {
            odometryPublisher.Publish(odometryMsg);
            yield return new WaitForSeconds(1.0f / OdometryPublishingFrequency);
        }
    }
    IEnumerator PublishTfRoutine()
    {
        for (;;)
        {
            tfPublisher.Publish(tfMsg);
            yield return new WaitForSeconds(1.0f / TfPublishingFrequency);
        }
    }

    IEnumerator PublishClockRoutine()
    {
        for (;;)
        {
            clockMsg = new builtin_interfaces.msg.Time();
            clockMsg.Sec = clock.Now.sec;
            clockMsg.Nanosec = clock.Now.nanosec;
            clockPublisher.Publish(clockMsg);
            yield return new WaitForSeconds(1.0f / ClockPublishingFrequency);
        }
    }

    private nav_msgs.msg.Odometry CreateOdometryMsg()
    {
        var msg = new nav_msgs.msg.Odometry();

        msg.Child_frame_id = "/base_footprint";
        msg.Header.Frame_id = "/odom";
        
        // msg.Pose.Covariance = poseCovarianceDiagonal.CovarianceMatrixFromDiagonal();
        // msg.Twist.Covariance = twistCovarianceDiagonal.CovarianceMatrixFromDiagonal();

        return msg;
    }

    private tf2_msgs.msg.TFMessage CreateTfMsg()
    {
        var msg = new tf2_msgs.msg.TFMessage
        {
            Transforms = new geometry_msgs.msg.TransformStamped[1],
        };

        var transformStamped = new geometry_msgs.msg.TransformStamped();
        transformStamped.Header.Frame_id = "/odom";
        transformStamped.Child_frame_id = "/base_footprint";
        msg.Transforms[0] = transformStamped;

        return msg;
    }
    
    

    private void FixedUpdate()
    {
        odometryMsg.Header.Update(clock);
        if (GlobalReferanceFrame == null)
        {
            odometryMsg.Pose.Pose.LocalUnity2Ros(BaseRigidbody.transform);
        } 
        else
        {
            odometryMsg.Pose.Pose.Unity2Ros(BaseRigidbody.transform, GlobalReferanceFrame);
        }

        odometryMsg.Twist.Twist.Unity2Ros(BaseRigidbody);
        
        
        if (PublishTf)
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
        }
    }

}
