using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Runtime.Scripts.Dtos
{
    public struct NodeDto
    {
        public NodeDto(
            VrBuildGraphNode previousVrBuildGraphNode, 
            GameObject targetObject, 
            VrBuildGraphNode nextVrBuildGraphNode
        ) {
            PreviousVrBuildGraphNode = previousVrBuildGraphNode;
            TargetObject = targetObject;
            NextVrBuildGraphNode = nextVrBuildGraphNode;
        }

        public GameObject TargetObject { get; }
        public VrBuildGraphNode PreviousVrBuildGraphNode { get; }
        public VrBuildGraphNode NextVrBuildGraphNode { get; }
    }
}