using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using VR.Build.GraphCreator.Editor.Types;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    /// <summary>
    /// Node structure for the unity editor.
    /// </summary>
    public class VrBuildGraphEditorNode : Node
    {
        public VrBuildGraphNode VrBuildGraphNode { get; private set; }

        private Port outputPort;
        private SerializedObject serializedObject;
        private SerializedProperty serializedProperty;
        public List<Port> Ports { get; private set; }
        
        public VrBuildGraphEditorNode(VrBuildGraphNode node, SerializedObject vrBuildGraphObject)
        {
            AddToClassList("vr-build-graph-node");

            serializedObject = vrBuildGraphObject;
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
            
            //Create output first so it is index 0
            if (info.HasFlowOutput)
            {
                CreateFlowOutputPort();
            }
            
            if (info.HasFlowInput)
            {
                CreateFlowInputPort();
            }

            foreach (var property in typeInfo.GetFields())
            {
                if (property.GetCustomAttribute<ExposePropertyAttribute>() is { } exposeProperty)
                {
                    var field = DrawPropertyField(property.Name);
                    //field.RegisterValueChangeCallback(OnFieldChangeCallback);
                }
            }
            
            RefreshExpandedState();
        }

        private void OnFieldChangeCallback(SerializedPropertyChangeEvent evt)
        {
            throw new System.NotImplementedException();
        }

        private PropertyField DrawPropertyField(string propertyName)
        {
            if (serializedProperty == null)
            {
                FetchSerializedProperty();
            }

            var prop = serializedProperty.FindPropertyRelative(propertyName);
            var field = new PropertyField(prop)
            {
                bindingPath = prop.propertyPath
            };
            extensionContainer.Add(field);
            return field;
        }

        private void FetchSerializedProperty()
        {
            SerializedProperty nodes = serializedObject.FindProperty("nodes");
            if (nodes.isArray)
            {
                var size = nodes.arraySize;
                for (var i = 0; i < nodes.arraySize; i++)
                {
                    var element = nodes.GetArrayElementAtIndex(i);
                    var elementId = element.FindPropertyRelative("mGuid");
                    if (elementId.stringValue == VrBuildGraphNode.ID)
                    {
                        serializedProperty = element;
                    }
                }
            }
        }

        private void CreateFlowInputPort()
        {
            var inputPort = InstantiatePort
            (
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Multi,
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
                Port.Capacity.Multi,
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