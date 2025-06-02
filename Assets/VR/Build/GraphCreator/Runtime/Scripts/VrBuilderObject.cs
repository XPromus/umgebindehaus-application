using System;
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
            graphAssetInstance.Init(gameObject);
            var startNode = graphAssetInstance.GetStartNode();
            ProcessAndMoveToNextNode(startNode);
        }

        private void ProcessAndMoveToNextNode(VrBuildGraphNode startNode)
        {
            var nextNodeId = startNode.OnProcess(graphAssetInstance);
            if (!string.IsNullOrEmpty(nextNodeId))
            {
                var node = graphAssetInstance.GetNode(nextNodeId);
                ProcessAndMoveToNextNode(node);
            }
        }
    }
}