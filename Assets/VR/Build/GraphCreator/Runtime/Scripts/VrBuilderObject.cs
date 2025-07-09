using System;
using System.Linq;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Runtime.Scripts
{
    public class VrBuilderObject : MonoBehaviour
    {
        [SerializeField]
        private VrBuildGraph graphAsset;

        private VrBuildGraph graphAssetInstance;
        private void OnEnable()
        {
            graphAssetInstance = Instantiate(graphAsset);
            ExecuteAsset();
        }

        private void ExecuteAsset()
        {
            graphAssetInstance.Init();
            var startNode = graphAssetInstance.GetStartNode();
            ProcessAndMoveToNextNode(startNode);
        }

        private void ProcessAndMoveToNextNode(VrBuildGraphNode startNode)
        {
            var nextNodeIds = startNode.OnProcess(graphAssetInstance);
            if (nextNodeIds.Length != 0)
            {
                var nodes = nextNodeIds.Select(nodeId => graphAssetInstance.GetNode(nodeId)).ToArray();
                //var node = graphAssetInstance.GetNode(nextNodeId);
                //ProcessAndMoveToNextNode(node);
                //TODO: Check why I need this.
            }
        }
    }
}