using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Utility
{
    /// <summary>
    /// Link asset for the graph, to be able to access objects in the scene.
    /// </summary>
    [CreateAssetMenu(menuName = "Logic/Game Object Link")]
    public class GameObjectLink : ScriptableObject
    {
        public GameObject LinkedGameObject { get; private set; }
        
        public bool TryGetLinkedObject(out GameObject linkedObject)
        {
            linkedObject = LinkedGameObject;
            return linkedObject;
        }

        public void SetLinkedObject(GameObject linkedObject)
        {
            if (!LinkedGameObject)
            {
                LinkedGameObject = linkedObject;
            }
        }
    }
}