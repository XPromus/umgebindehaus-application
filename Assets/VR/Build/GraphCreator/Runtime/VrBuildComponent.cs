using System;
using System.Collections.Generic;
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
        private List<Collider> triggeredObjects = new List<Collider>();

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            foreach (var triggeredObject in triggeredObjects)
            {
                if (!triggeredObject.TryGetComponent<VrBuildComponentOriginal>(out var otherComponent)) continue;
                
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

        private void OnTriggerEnter(Collider other)
        {
            triggeredObjects.Add(other);
            
            isInObject = true;
            Debug.Log(ID + ": Is in Object");
            
            /*
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
            */
        }

        private void OnTriggerExit(Collider other)
        {
            triggeredObjects.Remove(other);
            
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