using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Dtos;
using VR.Build.GraphCreator.Runtime.Scripts.Exceptions;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    [CreateAssetMenu(menuName = "VR Builder/New Graph")]
    public class VrBuildGraph : ScriptableObject
    {
        public List<VrBuildGraphNode> Nodes { get; private set; }
        private VrBuildGraphNode rootVrBuildGraphNode;
        
        private string _name;

        private VrBuildGraph()
        {
            Nodes = new List<VrBuildGraphNode>();
        }
        
        /*
        public Material DefaultGhostObjectMaterial { get; set; }
        
        public void CreateGraph()
        {
            foreach (var node in _nodes)
            {
                node.InitializeBuildComponent();
            }
        }

        public void AddNode()
        {
            _nodes.Add(new VrBuildGraphNode
            {
                TargetVrBuildGraph = this
            });
        }

        public void EditNode(Guid targetNode, NodeDto nodeDto)
        {
            var nodeToEdit = FindNodeInList(_nodes, targetNode);
            nodeToEdit.TargetObject = nodeDto.TargetObject;
            nodeToEdit.PreviousVrBuildGraphNode = nodeToEdit.PreviousVrBuildGraphNode;
            nodeToEdit.NextVrBuildGraphNode = nodeToEdit.NextVrBuildGraphNode;
        }

        private static VrBuildGraphNode FindNodeInList(List<VrBuildGraphNode> nodes, Guid id)
        {
            foreach (var node in nodes.Where(node => node.ID.Equals(id)))
            {
                return node;
            }

            throw new NodeNotFoundException($"Node with ID {id} could not be found!");
        }
        */
        
    }
}