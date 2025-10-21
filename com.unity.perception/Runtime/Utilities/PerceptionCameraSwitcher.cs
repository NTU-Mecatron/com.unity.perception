using UnityEngine;
using UnityEngine.Perception.GroundTruth;

namespace UnityEngine.Perception.Utilities
{
    public class PerceptionCameraSwitcher : MonoBehaviour
    {
        [Header("Assign two cameras (one can be a PerceptionCamera)")]
        [SerializeField] Camera m_CamA;
        [SerializeField] Camera m_CamB;

        [Header("Hotkey")]
        [SerializeField] KeyCode _switchKey = KeyCode.Tab;

        [Header("Depth Settings (same Display)")]
        [SerializeField] int displayIndex = 0;     // Display 1
        [SerializeField] float foregroundDepth = 10f;
        [SerializeField] float backgroundDepth = -10f;

        PerceptionCamera _pcamA;
        PerceptionCamera _pcamB;
        bool _useA = true;

        void Start()
        {
            _pcamA = m_CamA ? m_CamA.GetComponent<PerceptionCamera>() : null;
            _pcamB = m_CamB ? m_CamB.GetComponent<PerceptionCamera>() : null;

            ApplyState(_useA);
        }

        void Update()
        {
            if (Input.GetKeyDown(_switchKey))
            {
                _useA = !_useA;
                ApplyState(_useA);
            }
        }

        void ApplyState(bool makeAActive)
        {
            var activeCam = makeAActive ? m_CamA : m_CamB;
            var passiveCam = makeAActive ? m_CamB : m_CamA;
            var activePCam = makeAActive ? _pcamA : _pcamB;
            var passivePCam = makeAActive ? _pcamB : _pcamA;

            // --- Active camera: ON, foreground, visualization ON if it's a PCam ---
            if (activeCam)
            {
                activeCam.targetDisplay = displayIndex;
                activeCam.depth = foregroundDepth;
                activeCam.enabled = true;
            }
            if (activePCam)
            {
                // Show Labeler Visualization = true (same as ticking the inspector)
                activePCam.SetVisualizationActive(true);
                // keep component enabled so it keeps capturing
                activePCam.enabled = true;
            }

            // --- Passive camera: OFF, background, visualization OFF if it's a PCam ---
            if (passiveCam)
            {
                passiveCam.targetDisplay = displayIndex;
                passiveCam.depth = backgroundDepth;
                passiveCam.enabled = false; // fully off per your requirement
            }
            if (passivePCam)
            {
                // hide overlays but DO NOT disable the component (keeps capturing)
                passivePCam.SetVisualizationActive(false);
                // if you really want it off completely, uncomment next line:
                // passivePCam.enabled = false;
            }
        }
    }
}

