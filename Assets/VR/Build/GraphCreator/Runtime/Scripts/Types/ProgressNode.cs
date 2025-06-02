using UnityEngine;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Runtime.Scripts.Types
{
    [NodeInfo("Intermediate", "Intermediate")]
    public class ProgressNode : VrBuildGraphNode
    {
        [ExposeProperty]
        public string Tooltip;
        [ExposeProperty]
        public GameObjectLink TargetGameObjectLink;

        public override string OnProcess(VrBuildGraph currentGraph)
        {
            
            return base.OnProcess(currentGraph);
        }
    }
}