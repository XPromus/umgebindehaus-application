using System.Collections.Generic;
using System.Linq;
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
            
            DrawNode();
            graphViewChanged += OnGraphViewChangedEvent;
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Moved Elements");
                foreach (var movedNode in graphViewChange.movedElements.OfType<VrBuildGraphEditorNode>())
                {
                    movedNode.SavePosition();
                }
            }
            
            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Removed From Graph");
                var changedNodes = graphViewChange.elementsToRemove.OfType<VrBuildGraphEditorNode>().ToList();
                if (changedNodes.Count > 0)
                {
                    for (var i = changedNodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(changedNodes[i]);
                    }
                }
            }

            return graphViewChange;
        }

        private void RemoveNode(VrBuildGraphEditorNode node)
        {
            vrBuildGraph.Nodes.Remove(node.VrBuildGraphNode);
            NodeDictionary.Remove(node.VrBuildGraphNode.ID);
            GraphCreatorNodes.Remove(node);
            serializedObject.Update();
        }

        private void DrawNode()
        {
            foreach (var node in vrBuildGraph.Nodes)
            {
                AddNodeToGraph(node);
            }
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
            var editorNode = new VrBuildGraphEditorNode(node);
            editorNode.SetPosition(node.Position);
            GraphCreatorNodes.Add(editorNode);
            NodeDictionary.Add(node.ID, editorNode);
            
            AddElement(editorNode);
        }
    }
}