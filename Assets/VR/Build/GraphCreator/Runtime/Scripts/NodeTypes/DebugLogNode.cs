using UnityEngine;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Runtime.Scripts.NodeTypes
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : VrBuildGraphNode
    {
        [ExposeProperty()]
        public string debugMessage;
        
        public override string[] OnProcess(VrBuildGraph currentGraph)
        {
            Debug.Log(debugMessage);
            return base.OnProcess(currentGraph);
        }
    }
}