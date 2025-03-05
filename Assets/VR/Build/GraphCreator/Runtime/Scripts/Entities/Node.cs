using System;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    public class Node
    {
        public Guid ID { get; private set; } = Guid.NewGuid();

        public Graph TargetGraph { get; set; }
        
        public bool Satisfied { get; set; }
        public GameObject TargetObject { get; set; }
        public Node PreviousNode { get; set; }
        public Node NextNode { get; set; }
        public BuildComponent BuildComponent { get; private set; }

        public BuildComponent InitializeBuildComponent()
        {
            var newBuildComponent = TargetObject.AddComponent<BuildComponent>();
            newBuildComponent.Initialize(TargetGraph.DefaultGhostObjectMaterial);
            BuildComponent = newBuildComponent;
            return newBuildComponent;
        }
    }
}