using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    [Serializable]
    public class VrBuildGraphNode
    {
        [SerializeField]
        private string mGuid;
        [SerializeField]
        private GameObject mTargetObject;
        
        public string TypeName { get; set; }
        public Rect Position { get; set; }
        
        public string ID => mGuid;
        public GameObject TargetObject => mTargetObject;

        public VrBuildGraphNode()
        {
            mGuid = Guid.NewGuid().ToString();
        }
        
        public void SetPosition(Rect rect)
        {
            Position = rect;
        }
        
        /*
        public VrBuildGraph TargetVrBuildGraph { get; set; }
        
        public bool Satisfied { get; set; }
        
        public VrBuildGraphNode PreviousVrBuildGraphNode { get; set; }
        public VrBuildGraphNode NextVrBuildGraphNode { get; set; }
        public BuildComponent BuildComponent { get; private set; }
        
        public BuildComponent InitializeBuildComponent()
        {
            var newBuildComponent = TargetObject.AddComponent<BuildComponent>();
            newBuildComponent.Initialize(TargetVrBuildGraph.DefaultGhostObjectMaterial);
            BuildComponent = newBuildComponent;
            return newBuildComponent;
        }
        */
        
    }
}