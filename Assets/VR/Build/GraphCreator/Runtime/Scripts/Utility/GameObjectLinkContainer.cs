using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Utility
{
    /// <summary>
    /// Container the creates the link. Is attached to the object that should get a link.
    /// </summary>
    public class GameObjectLinkContainer : MonoBehaviour
    {
        /// <summary>
        /// The created asset, that should reference the object in the scene
        /// </summary>
        [SerializeField]
        private GameObjectLink objectLink;

        private void Awake()
        {
            if (objectLink)
            {
                objectLink.SetLinkedObject(gameObject);
            }
        }
    }
}