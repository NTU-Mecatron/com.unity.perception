using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySensors.ROS.Serializer;
using RosMessageTypes.Vision;
using UnityEngine.Perception.GroundTruth;
using UnitySensors.ROS.Serializer.Std;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.Perception.GroundTruth.Labelers;
using UnitySensors.ROS.Utils.Time;

namespace UnityEngine.Perception.ROS
{
    [System.Serializable]
    public class BoundingBoxSerializer : RosMsgSerializer<BoundingBoxArrayMsg>
    {
        [SerializeField, Tooltip("You need to edit this according to the resolution that you set for your display game.")]
        private Vector2 m_ImageSize;
        public Vector2 ImageSize { get => m_ImageSize; set => m_ImageSize = value; }

        [SerializeField]
        private PerceptionCamera m_PerceptionCamera;
        [SerializeField, Tooltip("Probability that a detection will be published. To simulate real life uncertainty.")]
        private float m_ConfidenceRate = 0.8f;
        [SerializeField]
        private HeaderSerializer m_Header;
        public PerceptionCamera Perception_Camera { get => m_PerceptionCamera; set => m_PerceptionCamera = value; }
        public HeaderSerializer Header { get => m_Header; set => m_Header = value; }

        List<CameraLabeler> m_Labelers;

        public override void Init()
        {
            base.Init();
            m_Header.Init();
            m_Labelers = m_PerceptionCamera.Labelers;
        }

        public override BoundingBoxArrayMsg Serialize()
        {
            _msg.header = m_Header.Serialize();
            _msg.bounding_boxes = new BoundingBoxMsg[0];

            if (!m_PerceptionCamera.enabled)
                return _msg;

            foreach (var labeler in m_Labelers)
            {
                if (labeler is not BoundingBox2DLabeler) continue;

                var boundingBoxLabeler = labeler as BoundingBox2DLabeler;
                var annotations = boundingBoxLabeler.Annotations;

                List<BoundingBoxMsg> bounding_boxes = new List<BoundingBoxMsg>();

                foreach (var annotation in annotations)
                {
                    if (Random.Range(0f, 1f) > m_ConfidenceRate) continue; // Simulate confidence rate (some boxes are not detected)

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
                _msg.bounding_boxes = bounding_boxes.ToArray();
            }
            return _msg;
        }
    }

}
