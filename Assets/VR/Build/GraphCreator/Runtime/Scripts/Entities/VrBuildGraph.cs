using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.NodeTypes;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    [CreateAssetMenu(menuName = "VR Builder/New Graph")]
    public class VrBuildGraph : ScriptableObject
    {
        [SerializeReference] 
        public List<VrBuildGraphNode> nodes;

        [SerializeField] 
        public List<VrBuildGraphConnection> connections;

        private Dictionary<string, VrBuildGraphNode> nodeDictionary;

        public GameObject self;
        
        private VrBuildGraph()
        {
            nodes = new List<VrBuildGraphNode>();
            connections = new List<VrBuildGraphConnection>();
        }

        public void Init(GameObject gameObject)
        {
            self = gameObject;
            nodeDictionary = new Dictionary<string, VrBuildGraphNode>();
            foreach (var node in nodes)
            {
                nodeDictionary.Add(node.ID, node);
            }
        }

        public VrBuildGraphNode GetStartNode()
        {
            var startNodes = nodes.OfType<StartNode>().ToArray();

            if (startNodes.Length != 0) return startNodes[0];
            
            Debug.LogError("No start node found.");
            return null;
        }

        public VrBuildGraphNode GetNode(string nextNodeId)
        {
            if (nodeDictionary.TryGetValue(nextNodeId, out var node))
            {
                return node;
            }

            return null;
        }

        public VrBuildGraphNode[] GetNodesFromOutputPort(string outputNodeId, int outputPortIndex)
        {
            return connections.Where
            (
                connection => connection.outputPort.nodeId == outputNodeId || 
                connection.outputPort.portIndex == outputPortIndex
            ).Select(connection => nodeDictionary[connection.inputPort.nodeId]).ToArray();
        }

        public VrBuildGraphNode[] GetNodesFromInputPort(string inputNodeId, int inputPortIndex)
        {
            return connections.Where
            (
                connection => connection.inputPort.nodeId == inputNodeId ||
                connection.inputPort.portIndex == inputPortIndex
            ).Select(connection => nodeDictionary[connection.outputPort.nodeId]).ToArray();
        }
    }
}