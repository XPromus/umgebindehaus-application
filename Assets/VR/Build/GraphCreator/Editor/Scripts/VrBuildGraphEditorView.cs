using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public class VrBuildGraphEditorView : GraphView
    {
        private readonly SerializedObject serializedObject;
        private readonly VrBuildGraph vrBuildGraph;
        private readonly VrBuildGraphWindowSearchProvider windowSearchProvider;
        
        public VrBuildGraphEditorWindow Window { get; private set; }
        public List<VrBuildGraphEditorNode> GraphCreatorNodes { get; set; }
        public Dictionary<string, VrBuildGraphEditorNode> NodeDictionary { get; set; }
        
        public VrBuildGraphEditorView(SerializedObject serializedObject, VrBuildGraphEditorWindow window)
        {
            this.serializedObject = serializedObject;
            vrBuildGraph = (VrBuildGraph)serializedObject.targetObject;
            Window = window;
            GraphCreatorNodes = new List<VrBuildGraphEditorNode>();
            NodeDictionary = new Dictionary<string, VrBuildGraphEditorNode>();
            windowSearchProvider = ScriptableObject.CreateInstance<VrBuildGraphWindowSearchProvider>();
            windowSearchProvider.Graph = this;
            nodeCreationRequest = ShowSearchWindow;
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>
            (
                "Assets/VR/Build/GraphCreator/Editor/Stylesheets/graphView.uss"
            );
            styleSheets.Add(styleSheet);
            
            var background = new GridBackground
            {
                name = "Grid"
            };
            Add(background);
            background.SendToBack();
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            windowSearchProvider.Target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), windowSearchProvider);
        }

        public void Add(VrBuildGraphNode node)
        {
            Undo.RecordObject(serializedObject.targetObject, "Added Node");
            vrBuildGraph.Nodes.Add(node);
            serializedObject.Update();
            AddNodeToGraph(node);
        }

        private void AddNodeToGraph(VrBuildGraphNode node)
        {
            node.TypeName = node.GetType().AssemblyQualifiedName;
            var editorNode = new VrBuildGraphEditorNode();
            editorNode.SetPosition(node.Position);
            GraphCreatorNodes.Add(editorNode);
            NodeDictionary.Add(node.ID.ToString(), editorNode);
            
            AddElement(editorNode);
        }
    }
}