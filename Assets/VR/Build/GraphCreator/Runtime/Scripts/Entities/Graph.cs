using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Dtos;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;
using VR.Build.GraphCreator.Runtime.Scripts.Exceptions;

namespace VR.Build.GraphCreator
{
    public abstract class Graph
    {
        private List<Node> _nodes;
        private Node _rootNode;
        
        private string _name;

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
            _nodes.Add(new Node
            {
                TargetGraph = this
            });
        }

        public void EditNode(Guid targetNode, NodeDto nodeDto)
        {
            var nodeToEdit = FindNodeInList(_nodes, targetNode);
            nodeToEdit.TargetObject = nodeDto.TargetObject;
            nodeToEdit.PreviousNode = nodeToEdit.PreviousNode;
            nodeToEdit.NextNode = nodeToEdit.NextNode;
        }

        private static Node FindNodeInList(List<Node> nodes, Guid id)
        {
            foreach (var node in nodes.Where(node => node.ID.Equals(id)))
            {
                return node;
            }

            throw new NodeNotFoundException($"Node with ID {id} could not be found!");
        }
        
    }
}