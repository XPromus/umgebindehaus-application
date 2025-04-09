using System.Reflection;
using UnityEditor.Experimental.GraphView;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public class VrBuildGraphEditorNode : Node
    {
        public VrBuildGraphNode VrBuildGraphNode { get; private set; }

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

            name = typeInfo.Name;
        }

        public void SavePosition()
        {
            VrBuildGraphNode.SetPosition(GetPosition());
        }
    }
}