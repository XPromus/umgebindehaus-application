using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;
using VR.Build.GraphCreator.Runtime.Scripts.NodeTypes;

namespace VR.Build.GraphCreator.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    
    //Plan:
    // TODO: Create copy of objects
    // TODO: Place the copy for the user to use
    // TODO: Hide all original objects except the first object
    // TODO: Show the next objects in transparent as a guide
    // TODO: When the user places an object correctly, the copy disappears and the original changes material to the original material
    // TODO: After placing an object, check if new objects are available and show the new objects as transparent guides
    public class VrInteractionManager : MonoBehaviour
    {
        [SerializeField] private VrBuildGraph targetVrBuildGraph;
        [SerializeField] private Transform targetInteractionPosition;

        [SerializeField] private Material guideMaterial;
        [SerializeField] private Material correctGuideMaterial;
        
        [SerializeField] private int maxObjectsInRow;
        [SerializeField] private float xSpacing;
        [SerializeField] private float ySpacing;
        
        private VrBuildGraph targetVrBuildGraphInstance;

        /// <summary>
        /// Not intended for interaction
        /// </summary>
        private List<GameObject> targetGameObjects = new();
        /// <summary>
        /// Intended for interaction with the user.
        /// </summary>
        private List<GameObject> targetGameObjectsCopy = new();
        
        /// <summary>
        /// List of the current nodes that can be edited by the user
        /// </summary>
        private List<VrBuildGraphNode> currentNodes = new();
        
        private void OnEnable()
        {
            targetVrBuildGraphInstance = Instantiate(targetVrBuildGraph);
            targetGameObjects = GetTargetGameObjects(targetVrBuildGraphInstance.nodes);
            var startNode = targetVrBuildGraph.GetStartNode();
            currentNodes = targetVrBuildGraph.GetNodesFromOutputPort(startNode.ID, 0).ToList();
            
            MoveGameObjectsToTargetPosition(targetGameObjects, targetInteractionPosition);
            CopyGameObjects();
            PlaceGameObjectCopies();
            HideOriginalGameObjects();
        }
        
        /*
        private void ExecuteAsset()
        {
            targetVrBuildGraphInstance.Init(gameObject);
            var startNode = targetVrBuildGraphInstance.GetStartNode();
            ProcessAndMoveToNextNode(startNode);
        }

        private void ProcessAndMoveToNextNode(VrBuildGraphNode startNode)
        {
            var nextNodeId = startNode.OnProcess(targetVrBuildGraphInstance);
            if (!string.IsNullOrEmpty(nextNodeId))
            {
                var node = targetVrBuildGraphInstance.GetNode(nextNodeId);
                ProcessAndMoveToNextNode(node);
            }
        }
        */
        
        private void CopyGameObjects()
        {
            foreach (var targetGameObject in targetGameObjects)
            {
                var copy = Instantiate(targetGameObject.gameObject);
                var component = copy.AddComponent<VrBuildComponent>();
                component.ID = targetGameObject.GetComponent<VrBuildComponentOriginal>().ID;
                
                targetGameObjectsCopy.Add(copy);
            }
        }

        private void PlaceGameObjectCopies()
        {
            foreach (var o in targetGameObjectsCopy)
            {
                o.transform.Translate(new Vector3(xSpacing, ySpacing, 0f));
            }
        }

        private void HideOriginalGameObjects()
        {
            //TODO: Remove first object in the graph from the hide list.
            var objectsToHide = targetGameObjects.Where(o =>
            {
                var id = o.GetComponent<VrBuildComponentOriginal>().ID;
                return currentNodes.All(currentNode => currentNode.ID != id);
            }).ToArray();
            
            foreach (var o in objectsToHide)
            {
                o.GetComponent<VrBuildComponentOriginal>().Hide();
            }
        }

        private void InteractionLoop()
        {
            throw new NotImplementedException();
        }

        private void TriggerObjectPlaced()
        {
            throw new NotImplementedException();
        }

        private void OnObjectPlacedCorrectly()
        {
            throw new NotImplementedException();
        }

        private void OnObjectPlacedFalse()
        {
            throw new NotImplementedException();
        }

        private void OnEndReached()
        {
            throw new NotImplementedException();
        }
        
        private List<GameObject> GetTargetGameObjects(List<VrBuildGraphNode> nodes)
        {
            var progressNodes = nodes.OfType<ProgressNode>().ToArray();
            var returnList = new List<GameObject>();
            
            foreach (var progressNode in progressNodes)
            {
                var o = progressNode.TargetGameObjectLink.LinkedGameObject;
                var component = o.AddComponent<VrBuildComponentOriginal>();
                component.ID = progressNode.ID;
                component.GhostMaterial = guideMaterial;
                component.Manager = this;
                returnList.Add(o);
            }

            return returnList;
        }
        
        private void MoveGameObjectsToTargetPosition(List<GameObject> gameObjects, Transform targetPosition)
        {
            var rowIndex = 0;
            var columnIndex = 0;
            foreach (var targetGameObject in gameObjects)
            {
                targetGameObject.transform.position = targetPosition.position;
                var positionOfMesh = targetGameObject.GetComponent<MeshRenderer>().bounds.center;
                targetGameObject.transform.position -= positionOfMesh;

                targetGameObject.transform.position += new Vector3(xSpacing * columnIndex, ySpacing * rowIndex, 0);
                
                columnIndex++;
                if (columnIndex >= maxObjectsInRow)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }
        
        /*
        private Vector3 GetCenterOfMultipleObjects(List<GameObject> objects)
        {
            var bounds = new Bounds(transform.position, Vector3.zero);
            foreach (var o in objects)
            {
                var rendererComponent = o.GetComponent<Renderer>();
                bounds.Encapsulate(rendererComponent.bounds);
            }

            return bounds.center;
        }
        */
        
        public void Step(string id)
        {
            
            throw new NotImplementedException();
        }
        
    }
}