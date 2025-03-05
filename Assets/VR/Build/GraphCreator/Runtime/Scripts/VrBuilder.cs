using System;
using System.Collections.Generic;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts
{
    public class VrBuilder : MonoBehaviour
    {
        /// <summary>
        /// Parts that should be put together.
        /// Order from 0 shows order in which parts should be combined during execution.
        /// Only top level objects are allowed.
        /// </summary>
        [Tooltip("Parts that should be put together. Order from 0 shows order in which parts should be combined. Only top level objects are allowed.")] 
        [SerializeField] private List<GameObject> partsToBuild;
        /// <summary>
        /// Parts that should be put together.
        /// Order from 0 shows order in which parts should be combined during execution.
        /// Only top level objects are allowed.
        /// </summary>
        public List<GameObject> PartsToBuild
        {
            set => partsToBuild = value;
            get => partsToBuild;
        }

        [SerializeField] private string buildName;
        public string BuildName
        {
            get => buildName;
            set => buildName = value;
        }
        
        [SerializeField] private bool startIsTarget;
        [SerializeField] private Material ghostMaterial;
        
        private List<Transform> worldCoordsOfParts;
        private List<Transform> localCoordsOfPartsToCenterOfParts;
        private List<GameObject> ghostObjects;
        
        private void Start()
        {
            var totalBounds = GetBoundsOfObjects(PartsToBuild);
            var parentObject = Instantiate(new GameObject
            {
                name = buildName,
                transform =
                {
                    position = totalBounds.center,
                    rotation = Quaternion.identity
                }
            });
            
            ghostObjects = CreateGhostObjects(parentObject.transform);
        }

        private List<GameObject> CreateGhostObjects(Transform parent)
        {
            var returnList = new List<GameObject>();
            foreach (var part in PartsToBuild)
            {
                var partClone = Instantiate(part, parent: parent);
                partClone.GetComponent<MeshRenderer>().material = ghostMaterial;
                partClone.SetActive(false);
                returnList.Add(partClone);
            }

            return returnList;
        }

        private static Bounds GetBoundsOfObjects(List<GameObject> gameObjects)
        {
            if (gameObjects.Count <= 0) throw new Exception();
            var totalBounds = new Bounds();
            foreach (var target in gameObjects)
            {
                var renderer = target.GetComponent<Renderer>();
                totalBounds.Encapsulate(renderer.bounds);
            }

            return totalBounds;
        }
    }
}