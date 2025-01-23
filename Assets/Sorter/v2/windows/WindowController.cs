using UnityEngine;

namespace Sorter.v2.windows
{
    public class WindowController : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool showDebugUI;
        public bool ShowDebugUI
        {
            get => showDebugUI;
            set => showDebugUI = value;
        }

        [Header("Fenster Einstellungen")]
        [SerializeField] private GameObject leftPane;
        public GameObject LeftPane
        {
            get => leftPane;
            set => leftPane = value;
        }
    
        [SerializeField] private GameObject rightPane;
        public GameObject RightPane
        {
            get => rightPane;
            set => rightPane = value;
        }

        [SerializeField] private float paneOpenAngle;
        public float PaneOpenAngle
        {
            get => paneOpenAngle;
            set => paneOpenAngle = value;
        }

        [SerializeField] private RotationAxis rotationAxis = RotationAxis.Y;
        public RotationAxis RotationAxisValue
        {
            get => rotationAxis;
            set => rotationAxis = value;
        }

        [SerializeField] private OpeningDirection openingDirection;
        public OpeningDirection OpeningDirectionValue
        {
            get => openingDirection;
            set => openingDirection = value;
        }
    
        [SerializeField] private float openingTime;
        public float OpeningTime
        {
            get => openingTime;
            set => openingTime = value;
        }
    
        private State windowState = State.CLOSED;
        private bool windowInUse;

        private void OnGUI()
        {
            if (!showDebugUI) return;
            var windowRect = new Rect(10, 10, 100f, 100f);
            windowRect = GUILayout.Window(0, windowRect, DebugWindowContent, "Debug Window");
        }

        private void DebugWindowContent(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Use Window"))
            {
                Debug.Log("Use Window");
                UseWindow();
            }
        
            if (GUILayout.Button("Open"))
            {
                Debug.Log("Open Window");
                OpenWindow();
            }

            if (GUILayout.Button("Close"))
            {
                Debug.Log("Close Window");
                CloseWindow();
            }
            GUILayout.EndVertical();
        }

        public void UseWindow()
        {
            Debug.Log("Use Window");
            switch (windowState)
            {
                case State.OPEN:
                    CloseWindow();
                    break;
            
                case State.CLOSED:
                    OpenWindow();
                    break;
            
                default:
                    Debug.LogError("Window state \"" + windowState +"\" not in enum");
                    break;
            }
        }
    
        public void UseWindow(GameObject o)
        {
            Debug.Log("Use Window");
            switch (windowState)
            {
                case State.OPEN:
                    CloseWindow();
                    break;
            
                case State.CLOSED:
                    OpenWindow();
                    break;
            
                default:
                    Debug.LogError("Window state \"" + windowState +"\" not in enum");
                    break;
            }
        }

        private void CloseWindow()
        {
            Debug.Log("Run closing function");
            LeanTween.rotateLocal(leftPane, Vector3.zero, openingTime);
            LeanTween.rotateLocal(rightPane, Vector3.zero, openingTime);
            windowState = State.CLOSED;
        }

        private void OpenWindow()
        {
            Debug.Log("Run opening function");
            Debug.Log(openingDirection);
            var vector = GetOpenVector(paneOpenAngle) * GetDirectionVector(openingDirection);
            LeanTween.rotateLocal(leftPane, vector, openingTime);
            LeanTween.rotateLocal(rightPane, -vector, openingTime);
            windowState = State.OPEN;
        }

        private Vector3 GetOpenVector(float angle)
        {
            return rotationAxis switch
            {
                RotationAxis.X => new Vector3(angle, 0, 0),
                RotationAxis.Y => new Vector3(0, angle, 0),
                RotationAxis.Z => new Vector3(0, 0, angle),
                _ => new Vector3(0, 0, 0)
            };
        }

        private float GetDirectionVector(OpeningDirection direction)
        {
            return direction switch
            {
                OpeningDirection.INWARDS => 1f,
                OpeningDirection.OUTWARDS => -1f,
                _ => 1f
            };
        }
    
        public enum RotationAxis
        {
            X, Y, Z
        }

        public enum OpeningDirection
        {
            INWARDS, OUTWARDS
        }

        public enum State
        {
            OPEN, CLOSED
        }
    
    }
}
