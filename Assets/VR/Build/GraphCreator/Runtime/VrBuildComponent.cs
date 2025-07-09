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

        private bool isInObject = false;
        private VrBuildComponentOriginal otherVrBuildComponentOriginal;
        private string otherObjectId;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("Collision");
        }

        private void OnTriggerEnter(Collider other)
        {
            isInObject = true;
            Debug.Log("Is in Object");
            
            if (other.TryGetComponent<VrBuildComponentOriginal>(out var otherComponent))
            {
                otherVrBuildComponentOriginal = otherComponent;
                otherObjectId = otherVrBuildComponentOriginal.ID;
                if (otherObjectId.Equals(ID))
                {
                    otherVrBuildComponentOriginal.ChangeGhostMaterialToCorrect();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (otherVrBuildComponentOriginal)
            {
                if (otherVrBuildComponentOriginal.ID.Equals(ID))
                {
                    otherVrBuildComponentOriginal.ChangeGhostMaterialToDefault();
                }
            }
            
            isInObject = false;
            Debug.Log("Is out of Object");
        }

        private void OnReleased()
        {
            if (isInObject && id.Equals(otherObjectId))
            {
                Manager.Step(id);
            }
        }
    }
}