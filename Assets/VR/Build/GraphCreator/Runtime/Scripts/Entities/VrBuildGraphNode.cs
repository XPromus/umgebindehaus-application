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

        public virtual string OnProcess(VrBuildGraph currentGraph)
        {
            var nextNodeInFlow = currentGraph.GetNodeFromOutput(mGuid, 0);
            return nextNodeInFlow != null ? nextNodeInFlow.ID : string.Empty;
        }
        
    }
}