using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Runtime.Scripts.Dtos
{
    public struct NodeDto
    {
        public NodeDto(
            Node previousNode, 
            GameObject targetObject, 
            Node nextNode
        ) {
            PreviousNode = previousNode;
            TargetObject = targetObject;
            NextNode = nextNode;
        }

        public GameObject TargetObject { get; }
        public Node PreviousNode { get; }
        public Node NextNode { get; }
    }
}