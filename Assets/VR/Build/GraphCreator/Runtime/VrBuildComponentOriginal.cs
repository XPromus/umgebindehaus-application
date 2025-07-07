using System;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class VrBuildComponentOriginal : MonoBehaviour
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

        [SerializeField] private Material originalMaterial;
        public Material OriginalMaterial
        {
            get => originalMaterial;
            set => originalMaterial = value;
        }
        
        [SerializeField] private Material ghostMaterial;
        public Material GhostMaterial
        {
            get => ghostMaterial;
            set => ghostMaterial = value;
        }

        private MeshRenderer componentRenderer;
        private Collider componentCollider;

        private void Awake()
        {
            componentRenderer = GetComponent<MeshRenderer>();
            componentCollider = GetComponent<Collider>();
            OriginalMaterial = componentRenderer.material;
        }

        public void Hide()
        {
            componentRenderer.enabled = false;
            componentCollider.enabled = false;
        }

        public void ShowGhost()
        {
            componentRenderer.enabled = true;
            componentCollider.enabled = true;
            componentRenderer.material = GhostMaterial;
        }
        
        public void OnCopyPlaced()
        {
            componentRenderer.enabled = true;
            componentCollider.enabled = true;
            componentRenderer.material = OriginalMaterial;
        }
    }
}