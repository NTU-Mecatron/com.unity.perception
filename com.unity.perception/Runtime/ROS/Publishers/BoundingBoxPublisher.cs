using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using RosMessageTypes.Vision;
using UnitySensors.ROS.Utils.Time;
using UnitySensors.ROS.Publisher;

namespace UnityEngine.Perception.ROS
{
    [RequireComponent(typeof(PerceptionCamera))]
    public class BoundingBoxPublisher : RosMsgPublisher<BoundingBoxSerializer, BoundingBoxArrayMsg>
    {
        void Reset()
        {
            _topicName = "detection/bounding_boxes";
            _frequency = 30.0f;
            PerceptionCamera perceptionCamera = GetComponent<PerceptionCamera>();
            _serializer.ImageSize = new Vector2(1280, 720);
            _serializer.Perception_Camera = perceptionCamera;
            _serializer.Header.Source = FindObjectOfType<ROSClock>();
            _serializer.Header.FrameId = "camera_link";
        }
    }
}

