using System;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts
{
    public class GameObjectLinkContainer : MonoBehaviour
    {
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