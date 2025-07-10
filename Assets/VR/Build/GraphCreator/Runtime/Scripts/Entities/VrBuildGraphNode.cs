using System;
using System.Linq;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    /// <summary>
    /// Node structure for the runtime of the program.
    /// </summary>
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
        public bool Finished;
        
        //public GameObject TargetObject => mTargetObject;

        public VrBuildGraphNode()
        {
            mGuid = Guid.NewGuid().ToString();
        }
        
        public void SetPosition(Rect rect)
        {
            Position = rect;
        }
            
        public virtual string[] OnProcess(VrBuildGraph currentGraph)
        {
            var nextNodesInFlow = currentGraph.GetNodesFromOutputPort(mGuid, 0);
            if (nextNodesInFlow == null || nextNodesInFlow.Length == 0) return Array.Empty<string>();
            return nextNodesInFlow.Select(node => node.ID).ToArray();
        }
        
    }
}