using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.Labelers;
using RosMessageTypes.Vision;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnitySensors.ROS.Utils.Namespacing;
using RosMessageTypes.BuiltinInterfaces;
using UnitySensors.ROS.Utils.Time;
using UnityEngine.Assertions;

namespace UnityEngine.Perception.ROS
{
    [RequireComponent(typeof(PerceptionCamera))]
    public class PublishBoundingBox : MonoBehaviour
    {       
        List<CameraLabeler> m_Labelers;
        ROSConnection m_RosConnection;
        ROSClock m_RosClock;

        [SerializeField, Tooltip("The topic name to publish bounding box data to. If the topic name does not start with a '/', the namespace of this GameObject will be prepended to the topic name.")]
        string m_Topic = "detection/bounding_boxes";

        [SerializeField, Tooltip("You need to edit this according to the resolution that you set for your display game.")]
        Vector2 m_ImageSize;

        void Start()
        {
            PerceptionCamera perceptionCamera = GetComponent<PerceptionCamera>();
            m_Labelers = perceptionCamera.Labelers;

            m_RosConnection = ROSConnection.GetOrCreateInstance();
            m_Topic = NamespaceUtils.GetResolvedTopicName(m_Topic, this.gameObject);
            m_RosConnection.RegisterPublisher<BoundingBoxArrayMsg>(m_Topic);

            m_RosClock = FindObjectOfType<ROSClock>();
            Assert.IsNotNull(m_RosClock, "No ROSClock found in the scene. Please add one to publish time data.");
        }

        void Update()
        {
            foreach (var labeler in m_Labelers)
            {
                if (labeler is not BoundingBox2DLabeler) continue;

                var boundingBoxLabeler = labeler as BoundingBox2DLabeler;
                var annotations = boundingBoxLabeler.Annotations;

                List<BoundingBoxMsg> bounding_boxes = new List<BoundingBoxMsg>();

                foreach (var annotation in annotations)
                {
                    // Original pixel-based values
                    Vector2 dimension = annotation.dimension;
                    Vector2 origin = annotation.origin;
                    Vector2 center = origin + (dimension / 2);

                    // Normalize the origin and dimension relative to the frame
                    Vector2 normalizedCenter = new Vector2(center.x / m_ImageSize.x, center.y / m_ImageSize.y);
                    Vector2 normalizedDimension = new Vector2(dimension.x / m_ImageSize.x, dimension.y / m_ImageSize.y);

                    string labelName = annotation.labelName;
                    int labelId = annotation.labelId;

                    BoundingBoxMsg boundingBoxMsg = new BoundingBoxMsg
                    {
                        label_id = (ushort)labelId,
                        label_name = labelName,
                        x = normalizedCenter.x,
                        y = normalizedCenter.y,
                        w = normalizedDimension.x,
                        h = normalizedDimension.y,
                        conf = 1.0f
                    };
                    bounding_boxes.Add(boundingBoxMsg);

                    //Debug.Log($"Label: {labelName}, ID: {labelId}, Normalized Origin: ({normalizedCenter.x}, {normalizedCenter.y}), Normalized Dimension: ({normalizedDimension.x}, {normalizedDimension.y})");
                }

                BoundingBoxArrayMsg boundingBoxArrayMsg = new BoundingBoxArrayMsg
                {
                    header = new HeaderMsg
                    {
                        stamp = new TimeMsg
                        {
                            sec = m_RosClock.Sec,
                            nanosec = m_RosClock.Nanosec
                        }
                    },
                    bounding_boxes = bounding_boxes.ToArray()
                };
                m_RosConnection.Publish(m_Topic, boundingBoxArrayMsg);
            }
        }
    }
}

