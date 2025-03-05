using UnityEditor;
using UnityEngine;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public class GraphCreatorWindow : EditorWindow 
    {
        [MenuItem("Window/Graph Creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GraphCreatorWindow));
        }

        private void OnEnable()
        {
            DrawGraph();
        }
        
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Button("Create Graph");
            GUILayout.Button("Add Node");
            GUILayout.EndHorizontal();
        }

        private void DrawGraph()
        {
            var currentView = new GraphCreatorView();
            rootVisualElement.Add(currentView);
        }
    }
}