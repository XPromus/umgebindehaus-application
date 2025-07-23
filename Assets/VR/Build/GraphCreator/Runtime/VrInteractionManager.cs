using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;
using VR.Build.GraphCreator.Runtime.Scripts.NodeTypes;

namespace VR.Build.GraphCreator.Runtime
{
    public class VrInteractionManager : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private VrBuildGraph targetVrBuildGraph;
        [SerializeField] private Transform targetInteractionPosition;

        [Header("Materials")]
        [SerializeField] private Material inactiveMaterial;
        [SerializeField] private Material guideMaterial;
        [SerializeField] private Material correctGuideMaterial;
        
        [Header("Placement Options")]
        [SerializeField] private int maxObjectsInRow;
        [SerializeField] private float xSpacing;
        [SerializeField] private float ySpacing;
        [SerializeField] private bool alignOnXAxis;
        
        private VrBuildGraph targetVrBuildGraphInstance;
        private VrBuildGraph targetVrBuildGraphInstanceScript;

        /// <summary>
        /// Not intended for interaction
        /// </summary>
        private List<GameObject> targetGameObjects = new();

        private List<VrBuildComponentOriginal> targetGameObjectComponents = new();
        /// <summary>
        /// Intended for interaction with the user.
        /// </summary>
        private readonly List<GameObject> targetGameObjectsCopy = new();

        private List<VrBuildComponent> targetGameObjectCopyComponents = new();
        
        /// <summary>
        /// List of the current nodes that can be edited by the user
        /// </summary>
        private List<VrBuildGraphNode> currentNodes = new();
        private List<VrBuildComponent> currentComponents = new();
        private List<VrBuildComponentOriginal> currentOriginalComponents = new();
        private List<GameObject> currentOriginalGameObjects = new();
        private List<GameObject> currentCopyGameObjects = new();
        
        private void OnEnable()
        {
            targetVrBuildGraphInstance = Instantiate(targetVrBuildGraph);
            targetVrBuildGraphInstance.Init();
            targetGameObjects = GetTargetGameObjects(targetVrBuildGraphInstance.nodes);
            var startNode = targetVrBuildGraphInstance.GetStartNode();
            currentNodes = targetVrBuildGraphInstance.GetNodesFromOutputPort(startNode.ID, 0).ToList();
            
            //MoveGameObjectsToTargetPosition(targetGameObjects, targetInteractionPosition);
            
            CopyGameObjects();
            PlaceGameObjectCopies();
            HideOriginalGameObjects();
            ChangeNodeMaterialToGhost();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnPlacementCheck();
            }
        }

        private void OnPlacementCheck()
        {
            //TODO: Error when checking and object is in two objects at the same time
            //TODO: Fix if multiple are placed correctly (will not happen IRL but check still)
            var onPlacedCorrectly = false;
            var currentComponentCopies = GetVrBuildComponentsFromNodes(currentNodes);
            foreach (var vrBuildComponent in currentComponentCopies)
            {
                if (!vrBuildComponent.IsInCorrectObject) continue;
                onPlacedCorrectly = true;
                OnObjectPlacedCorrectly(vrBuildComponent, vrBuildComponent.OtherVrBuildComponentOriginal);
            }

            if (onPlacedCorrectly)
            {
                CheckIfNodesInLevelAreComplete();
            }
        }

        private List<VrBuildComponent> GetVrBuildComponentsFromNodes(List<VrBuildGraphNode> nodes)
        {
            var components = new List<VrBuildComponent>();
            foreach (var vrBuildGraphNode in nodes)
            {
                components.AddRange(targetGameObjectCopyComponents.Where(c => c.ID.Equals(vrBuildGraphNode.ID)));
            }

            return components;
        }

        private List<VrBuildComponentOriginal> GetVrBuildComponentOriginalsFromNodes(List<VrBuildGraphNode> nodes)
        {
            var components = new List<VrBuildComponentOriginal>();
            foreach (var vrBuildGraphNode in nodes)
            {
                components.AddRange(targetGameObjectComponents.Where(c => c.ID.Equals(vrBuildGraphNode.ID)));
            }

            return components;
        }

        private void CheckIfNodesInLevelAreComplete()
        {
            if (!currentNodes.All(c => c.Finished)) return;
            currentNodes = GetNewNodesFromCurrentNodes(currentNodes);
            InitNewComponents();
        }
        
        /// <summary>
        /// Uses a list of nodes to get all following nodes. If multiple nodes converge on a single node, duplicates from the connections are removed.
        /// </summary>
        /// <param name="nodes">Nodes, who's output ports are used to find new nodes</param>
        /// <returns>A new list of nodes</returns>
        private List<VrBuildGraphNode> GetNewNodesFromCurrentNodes(List<VrBuildGraphNode> nodes)
        {
            var returnList = new List<VrBuildGraphNode>();
            foreach (var vrBuildGraphNode in nodes)
            {
                var nodesFromOutputPort = targetVrBuildGraphInstance.GetNodesFromOutputPort(vrBuildGraphNode.ID, 0);
                
                foreach (var outputNode in nodesFromOutputPort)
                {
                    var isInList = false;
                    foreach (var nodeInList in returnList)
                    {
                        if (nodeInList.ID.Equals(outputNode.ID))
                        {
                            isInList = true;
                        }
                    }
                    
                    if (!isInList)
                    {
                        returnList.Add(outputNode);
                    }
                }
            }
            
            return returnList;
        }
        
        private void InitNewComponents()
        {
            var newCurrentOriginalComponents = GetVrBuildComponentOriginalsFromNodes(currentNodes);
            foreach (var newCurrentOriginalComponent in newCurrentOriginalComponents)
            {
                newCurrentOriginalComponent.Show();
            }
            ChangeNodeMaterialToGhost();
        }
        
        private void CopyGameObjects()
        {
            foreach (var targetGameObject in targetGameObjects)
            {
                var copy = Instantiate(targetGameObject.gameObject);
                
                var copyVrBuildComponent = copy.AddComponent<VrBuildComponent>();
                copyVrBuildComponent.ID = targetGameObject.GetComponent<VrBuildComponentOriginal>().ID;
                targetGameObjectCopyComponents.Add(copyVrBuildComponent);
                
                var copyRigidbody = copy.AddComponent<Rigidbody>();
                copyRigidbody.useGravity = false;
                copyRigidbody.isKinematic = true;
                
                Destroy(copy.GetComponent<VrBuildComponentOriginal>());
                targetGameObjectsCopy.Add(copy);
            }
        }

        private void PlaceGameObjectCopies()
        {
            var currentPosition = targetInteractionPosition.position;
            for (var i = 0; i < targetGameObjectsCopy.Count; i++)
            {
                var currentObject = targetGameObjectsCopy[i];
                var objectBounds = currentObject.GetComponent<MeshRenderer>().bounds;

                var targetPosition = new Vector3
                (
                    x: currentPosition.x + (xSpacing / 2) + objectBounds.extents.x,
                    y: currentPosition.y,
                    z: currentPosition.z
                );
                
                currentObject.transform.position = targetPosition;
                currentPosition = targetPosition + new Vector3((xSpacing / 2) + objectBounds.extents.x, 0f, 0f);
            }
            /*
            foreach (var o in targetGameObjectsCopy)
            {
                o.transform.Translate(new Vector3(xSpacing, ySpacing, 0f));
            }
            */
        }

        private Vector3 GetVectorForPlacement(int mul)
        {
            return alignOnXAxis switch
            {
                true => new Vector3(targetInteractionPosition.position.x + xSpacing * mul, 0f,
                    targetInteractionPosition.position.z + ySpacing),
                false => new Vector3(targetInteractionPosition.position.x + xSpacing, 0f,
                    targetInteractionPosition.position.z + ySpacing * mul)
            };
        }

        private void HideOriginalGameObjects()
        {
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

        private void ChangeNodeMaterialToGhost()
        {
            foreach (var targetGameObject in targetGameObjects)
            {
                var vrBuildComponentOriginal = targetGameObject.GetComponent<VrBuildComponentOriginal>();
                foreach (var vrBuildGraphNode in currentNodes)
                {
                    if (vrBuildComponentOriginal.ID.Equals(vrBuildGraphNode.ID))
                    {
                        targetGameObject.GetComponent<MeshRenderer>().material = vrBuildComponentOriginal.GhostMaterial;
                    }
                }
            }
        }

        private void OnObjectPlacedCorrectly(VrBuildComponent copy, VrBuildComponentOriginal original)
        {
            copy.gameObject.SetActive(false);
            original.ChangeMaterialToOriginal();
            foreach (var vrBuildGraphNode in currentNodes)
            {
                if (vrBuildGraphNode.ID.Equals(original.ID))
                {
                    vrBuildGraphNode.Finished = true;
                }
            }
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
                component.CorrectGhostMaterial = correctGuideMaterial;
                component.Manager = this;
                targetGameObjectComponents.Add(component);
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
    }
}