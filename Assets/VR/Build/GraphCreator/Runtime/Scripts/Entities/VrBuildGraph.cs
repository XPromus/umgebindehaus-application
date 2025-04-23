using System.Collections.Generic;
using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    [CreateAssetMenu(menuName = "VR Builder/New Graph")]
    public class VrBuildGraph : ScriptableObject
    {
        [SerializeReference] 
        public List<VrBuildGraphNode> nodes;

        [SerializeField] 
        public List<VrBuildGraphConnection> connections;
        
        private VrBuildGraph()
        {
            nodes = new List<VrBuildGraphNode>();
            connections = new List<VrBuildGraphConnection>();
        }
    }
}