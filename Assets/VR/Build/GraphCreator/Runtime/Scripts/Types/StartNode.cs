using UnityEngine;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Runtime.Scripts.Types
{
    [NodeInfo("Start", "Start", false, true)]
    public class StartNode : VrBuildGraphNode
    {
        public override string OnProcess(VrBuildGraph currentGraph)
        {
            Debug.Log("Hello world from: Start Node");
            return base.OnProcess(currentGraph);
        }
    }
}