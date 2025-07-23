using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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
        public Dictionary<Edge, VrBuildGraphConnection> ConnectionDictionary { get; set; }
        
        public VrBuildGraphEditorView(SerializedObject serializedObject, VrBuildGraphEditorWindow window)
        {
            this.serializedObject = serializedObject;
            vrBuildGraph = (VrBuildGraph)serializedObject.targetObject;
            Window = window;
            GraphCreatorNodes = new List<VrBuildGraphEditorNode>();
            NodeDictionary = new Dictionary<string, VrBuildGraphEditorNode>();
            ConnectionDictionary = new Dictionary<Edge, VrBuildGraphConnection>();
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
            this.AddManipulator(new ContentZoomer());
            
            DrawNode();
            DrawConnections();
            
            graphViewChanged += OnGraphViewChangedEvent;
        }

        private void DrawConnections()
        {
            if (vrBuildGraph.connections == null) return;
            foreach (var connection in vrBuildGraph.connections)
            {
                DrawConnection(connection);
            }
        }

        private void DrawConnection(VrBuildGraphConnection connection)
        {
            var inputNode = GetNode(connection.inputPort.nodeId);
            var outputNode = GetNode(connection.outputPort.nodeId);
            if (inputNode == null || outputNode == null) { return; }

            var inputPort = inputNode.Ports[connection.inputPort.portIndex];
            var outputPort = outputNode.Ports[connection.outputPort.portIndex];
            var edge = inputPort.ConnectTo(outputPort);
            
            ConnectionDictionary.Add(edge, connection);
            AddElement(edge);
        }

        private VrBuildGraphEditorNode GetNode(string nodeId)
        {
            NodeDictionary.TryGetValue(nodeId, out var targetNode);
            return targetNode;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var allPorts = new List<Port>();
            foreach (var node in GraphCreatorNodes)
            {
                allPorts.AddRange(node.Ports);
            }

            return 
            (
                from port in allPorts 
                where port != startPort 
                where port.node != startPort.node 
                where port.direction != startPort.direction 
                where port.portType == startPort.portType 
                select port
            ).ToList();
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

                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(edge);
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Added Connections");
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    CreateEdge(edge);
                }
            }

            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            var inputNode = (VrBuildGraphEditorNode) edge.input.node;
            var inputIndex = inputNode.Ports.IndexOf(edge.input);
            
            var outputNode = (VrBuildGraphEditorNode) edge.output.node;
            var outputIndex = outputNode.Ports.IndexOf(edge.output);

            var connection = new VrBuildGraphConnection(inputNode.VrBuildGraphNode.ID, inputIndex, outputNode.VrBuildGraphNode.ID, outputIndex);
            vrBuildGraph.connections.Add(connection);
        }

        private void RemoveConnection(Edge edge)
        {
            if (!ConnectionDictionary.TryGetValue(edge, out var connection)) return;
            vrBuildGraph.connections.Remove(connection);
            ConnectionDictionary.Remove(edge);
        }
        
        private void RemoveNode(VrBuildGraphEditorNode node)
        {
            vrBuildGraph.nodes.Remove(node.VrBuildGraphNode);
            NodeDictionary.Remove(node.VrBuildGraphNode.ID);
            GraphCreatorNodes.Remove(node);
            serializedObject.Update();
        }

        private void DrawNode()
        {
            foreach (var node in vrBuildGraph.nodes)
            {
                AddNodeToGraph(node);
            }
            
            Bind();
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            windowSearchProvider.Target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), windowSearchProvider);
        }

        public void Add(VrBuildGraphNode node)
        {
            Undo.RecordObject(serializedObject.targetObject, "Added Node");
            vrBuildGraph.nodes.Add(node);
            serializedObject.Update();
            AddNodeToGraph(node);
        }

        private void AddNodeToGraph(VrBuildGraphNode node)
        {
            node.TypeName = node.GetType().AssemblyQualifiedName;
            var editorNode = new VrBuildGraphEditorNode(node, serializedObject);
            editorNode.SetPosition(new Rect(x: node.PositionX, y: node.PositionY, width: node.PositionW, height: node.PositionH));
            GraphCreatorNodes.Add(editorNode);
            NodeDictionary.Add(node.ID, editorNode);
            
            AddElement(editorNode);
        }

        private void Bind()
        {
            serializedObject.Update();
            this.Bind(serializedObject);
        }
    }
}