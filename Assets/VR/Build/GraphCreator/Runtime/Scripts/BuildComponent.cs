using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts
{
    public class BuildComponent : MonoBehaviour
    {
        public int ID { get; set; }

        private GameObject _originalObject;
        public GameObject OriginalObject
        {
            get => _originalObject;
            set {
                _originalObject = value;
                UpdateGhostObject();
            }
        }

        public GameObject GhostObject { get; private set; }
        public Material GhostObjectMaterial { get; set; }

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        public void Initialize(Material ghostObjectMaterial)
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
            GhostObjectMaterial = ghostObjectMaterial;
            OriginalObject = gameObject;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!CompareCollisionObjectWithGhost(other.gameObject)) return;
            ClickObjectInPlace();
        }

        private void UpdateGhostObject()
        {
            if (GhostObject) Destroy(GhostObject);
            
            var newGhostObject = Instantiate(OriginalObject);
            newGhostObject.name = GetGhostObjectName();
            newGhostObject.GetComponent<MeshRenderer>().material = GhostObjectMaterial;

            if (!newGhostObject.TryGetComponent<MeshCollider>(out _))
            {
                newGhostObject.AddComponent<MeshCollider>();
            }

            GhostObject = newGhostObject;
        }

        private string GetGhostObjectName()
        {
            return OriginalObject.name + "_Ghost";
        }

        private bool CompareCollisionObjectWithGhost(GameObject collisionObject)
        {
            return GhostObject.GetInstanceID().Equals(collisionObject.GetInstanceID());
        }

        private void ClickObjectInPlace()
        {
            GhostObject.SetActive(false);
            OriginalObject.transform.position = _originalPosition;
            OriginalObject.transform.rotation = _originalRotation;
        }
    }
}