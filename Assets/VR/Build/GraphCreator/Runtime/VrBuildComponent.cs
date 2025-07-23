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

        public bool isInCorrectObject;

        public VrBuildComponentOriginal OtherVrBuildComponentOriginal { get; private set; }

        private string otherObjectId;
        private readonly List<Collider> triggeredObjects = new();

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!triggeredObjects.Contains(other))
            {
                triggeredObjects.Add(other);
            }
            
            CheckCollisionObjects();
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggeredObjects.Contains(other))
            {
                triggeredObjects.Remove(other);
            }
            
            CheckCollisionObjects();
            
            if (OtherVrBuildComponentOriginal)
            {
                if (OtherVrBuildComponentOriginal.ID.Equals(ID))
                {
                    OtherVrBuildComponentOriginal.ChangeGhostMaterialToDefault();
                    isInCorrectObject = false;
                }
            }
        }

        private void CheckCollisionObjects()
        {
            foreach (var triggeredObject in triggeredObjects)
            {
                if (!triggeredObject.TryGetComponent<VrBuildComponentOriginal>(out var otherComponent)) continue;
                if (!otherComponent.ID.Equals(ID)) continue;
                
                OtherVrBuildComponentOriginal = otherComponent;
                otherObjectId = OtherVrBuildComponentOriginal.ID;
                if (otherObjectId.Equals(ID))
                {
                    isInCorrectObject = true;
                    OtherVrBuildComponentOriginal.ChangeGhostMaterialToCorrect();
                }
                else
                {
                    isInCorrectObject = false;
                }
            }
        }
    }
}