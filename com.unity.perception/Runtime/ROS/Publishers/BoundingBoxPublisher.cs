using UnityEngine.Perception.GroundTruth;
using RosMessageTypes.Vision;
using UnitySensors.ROS.Utils.Time;
using UnitySensors.ROS.Publisher;

namespace UnityEngine.Perception.ROS
{
    [RequireComponent(typeof(PerceptionCamera))]
    public class BoundingBoxPublisher : RosMsgPublisher<BoundingBoxSerializer, BoundingBoxArrayMsg>
    {
        public float ConfidenceRate
        {
            get => _serializer.ConfidenceRate;
            set => _serializer.ConfidenceRate = Mathf.Clamp01(value);
        }

        void Reset()
        {
            _topicName = "detection/bounding_boxes";
            _frequency = 30.0f;
            PerceptionCamera perceptionCamera = GetComponent<PerceptionCamera>();
            _serializer.ImageSize = new Vector2(1280, 720);
            _serializer.Perception_Camera = perceptionCamera;
            _serializer.Header.FrameId = "camera_link";
        }

        protected override void Start()
        {
            _serializer.Header.Source = FindFirstObjectByType<ROSClock>();
            base.Start();
        }
    }
}

