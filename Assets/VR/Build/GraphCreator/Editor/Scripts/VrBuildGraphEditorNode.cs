using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using VR.Build.GraphCreator.Editor.Types;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public class VrBuildGraphEditorNode : Node
    {
        public VrBuildGraphNode VrBuildGraphNode { get; private set; }

        private Port outputPort;
        public List<Port> Ports { get; private set; }
        
        public VrBuildGraphEditorNode(VrBuildGraphNode node)
        {
            AddToClassList("vr-build-graph-node");

            VrBuildGraphNode = node;
            var typeInfo = node.GetType();
            var info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();
            var depths = info.MenuItem.Split("/");
            foreach (var depth in depths)
            {
                AddToClassList(depth.ToLower().Replace(" ", "-"));
            }

            title = info.NodeTitle;
            name = typeInfo.Name;
            Ports = new List<Port>();
            
            if (info.HasFlowInput)
            {
                CreateFlowInputPort();
            }

            if (info.HasFlowOutput)
            {
                CreateFlowOutputPort();
            }
        }

        private void CreateFlowInputPort()
        {
            var inputPort = InstantiatePort
            (
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(PortTypes.FlowPort)
            );
            inputPort.portName = "In";
            inputPort.tooltip = "Flow Input";
            Ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }
        
        private void CreateFlowOutputPort()
        {
            outputPort = InstantiatePort
            (
                Orientation.Horizontal, 
                Direction.Output, 
                Port.Capacity.Single,
                typeof(PortTypes.FlowPort)
            );
            outputPort.portName = "Out";
            outputPort.tooltip = "Flow Output";
            Ports.Add(outputPort);
            outputContainer.Add(outputPort);
        }

        public void SavePosition()
        {
            VrBuildGraphNode.SetPosition(GetPosition());
        }
    }
}