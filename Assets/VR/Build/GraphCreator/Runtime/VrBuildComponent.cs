using System;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class VrBuildComponent : MonoBehaviour
    {
        [SerializeField] private string id;
        public string ID
        {
            get => id;
            set => id = value;
        }
        
        [SerializeField] private VrInteractionManager manager;
        public VrInteractionManager Manager
        {
            get => manager;
            set => manager = value;
        }

        private bool isInObject;
        public bool IsInCorrectObject;

        public VrBuildComponentOriginal OtherVrBuildComponentOriginal { get; private set; }

        private string otherObjectId;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            isInObject = true;
            Debug.Log(ID + ": Is in Object");
            
            if (other.TryGetComponent<VrBuildComponentOriginal>(out var otherComponent))
            {
                OtherVrBuildComponentOriginal = otherComponent;
                otherObjectId = OtherVrBuildComponentOriginal.ID;
                if (otherObjectId.Equals(ID))
                {
                    IsInCorrectObject = true;
                    OtherVrBuildComponentOriginal.ChangeGhostMaterialToCorrect();
                }
                else
                {
                    IsInCorrectObject = false;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (OtherVrBuildComponentOriginal)
            {
                if (OtherVrBuildComponentOriginal.ID.Equals(ID))
                {
                    OtherVrBuildComponentOriginal.ChangeGhostMaterialToDefault();
                    IsInCorrectObject = false;
                }
            }
            
            isInObject = false;
            Debug.Log("Is out of Object");
        }
    }
}