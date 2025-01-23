using UnityEngine;

namespace Sorter.v2.windows
{
    public class ObjectInteraction : MonoBehaviour
    {

        public bool inUse;

        public void Interact()
        {
            inUse = !inUse;
            Debug.Log("Interaction");
        }

    }
}