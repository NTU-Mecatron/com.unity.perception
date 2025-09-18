using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using System.Reflection; // needed for reflection call to CleanupVisualization()

namespace UnityEngine.Perception.Utilities
{
    public class PerceptionCameraSwitcher : MonoBehaviour
    {
        [SerializeField] Camera m_Cam1;
        [SerializeField] Camera m_Cam2;

        [Tooltip("Key to switch the camera.")]
        [SerializeField] KeyCode _switchKey = KeyCode.Tab;

        private PerceptionCamera m_Pcam1;
        private PerceptionCamera m_Pcam2;
        private bool m_UsingFirst = true;

        void Start()
        {
            if (m_Cam1 != null) m_Pcam1 = m_Cam1.GetComponent<PerceptionCamera>();
            if (m_Cam2 != null) m_Pcam2 = m_Cam2.GetComponent<PerceptionCamera>();

            // Start with cam1 active
            ActivateCamera(m_Cam1, m_Pcam1, m_UsingFirst);
            ActivateCamera(m_Cam2, m_Pcam2, !m_UsingFirst);
        }

        private void Update()
        {
            // Check if the launch key is pressed
            if (Input.GetKeyDown(_switchKey))
            {
                SwitchCamera(); // Trigger the actuator
            }
        }

        public void SwitchCamera()
        {
            m_UsingFirst = !m_UsingFirst;
            ActivateCamera(m_Cam1, m_Pcam1, m_UsingFirst);
            ActivateCamera(m_Cam2, m_Pcam2, !m_UsingFirst);
        }

        private void ActivateCamera(Camera cam, PerceptionCamera pcam, bool active)
        {
            if (cam != null)
                cam.enabled = active;

            if (pcam != null)
            {
                pcam.enabled = active;
                pcam.SetVisualizationActive(active);
            }
        }
    }

}
