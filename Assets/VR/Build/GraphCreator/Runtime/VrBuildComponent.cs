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
        private string otherObjectId;
        
        private void OnTriggerEnter(Collider other)
        {
            //TODO: If ID is correct change material to green
            isInObject = true;
            var component = other.gameObject.GetComponent<VrBuildComponentOriginal>();
            otherObjectId = component.ID;
            Debug.Log("Is in Object");
        }

        private void OnTriggerExit(Collider other)
        {
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