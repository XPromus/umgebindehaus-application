using System;
using UnityEngine;

namespace Sorter.v2.doors
{
    public class DoorController : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool showDebugUI;
        public bool ShowDebugUI
        {
            get => showDebugUI;
            set => showDebugUI = value;
        }

        [Header("Door Settings")]
        [SerializeField] private GameObject doorObject;
        public GameObject DoorObject
        {
            get => doorObject;
            set => doorObject = value;
        }

        [SerializeField] private float doorOpenAngle;
        public float DoorOpenAngle
        {
            get => doorOpenAngle;
            set => doorOpenAngle = value;
        }

        [SerializeField] private OpeningDirectionEnum openingDirection;
        public OpeningDirectionEnum OpeningDirection
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

        private State doorState = State.CLOSED;
        private bool doorInUse;

        private void OnGUI()
        {
            if (!showDebugUI) return;
            var doorRect = new Rect(10, 10, 100f, 100f);
            doorRect = GUILayout.Window(0, doorRect, DebugWindowContent, "Door Debug Window");
        }
    
        private void DebugWindowContent(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Use Door"))
            {
                Debug.Log("Use Door");
                UseDoor(doorObject);
            }
        
            if (GUILayout.Button("Open"))
            {
                Debug.Log("Open Door");
                OpenDoor();
            }

            if (GUILayout.Button("Close"))
            {
                Debug.Log("Close Door");
                CloseDoor();
            }
            GUILayout.EndVertical();
        }

        public void UseDoor(GameObject o)
        {
            Debug.Log("Use Door");
            switch (doorState)
            {
                case State.OPEN:
                    CloseDoor();
                    break;
                case State.CLOSED:
                    OpenDoor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OpenDoor()
        {
            var vector = GetOpenVector(doorOpenAngle) * GetDirectionVector(openingDirection);
            LeanTween.rotateLocal(doorObject, vector, openingTime);
            doorState = State.CLOSED;
        }

        public void CloseDoor()
        {
            LeanTween.rotateLocal(doorObject, Vector3.zero, openingTime);
            doorState = State.OPEN;
        }

        private Vector3 GetOpenVector(float angle)
        {
            return new Vector3(0, angle, 0);
        }

        private float GetDirectionVector(OpeningDirectionEnum direction)
        {
            return direction switch
            {
                OpeningDirectionEnum.INWARDS => 1f,
                OpeningDirectionEnum.OUTWARDS => -1F,
                _ => 1f
            };
        }

        public enum OpeningDirectionEnum
        {
            INWARDS, OUTWARDS
        }

        public enum State
        {
            OPEN, CLOSED
        }

    }
}
