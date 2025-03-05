using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public class VrBuildGraphEditorWindow : EditorWindow
    {
        [SerializeField]
        private VrBuildGraph mCurrentVrBuildGraph;
        private SerializedObject mSerializedObject;
        private VrBuildGraphEditorView mCurrentView;

        public VrBuildGraph CurrentVrBuildGraph => mCurrentVrBuildGraph;

        public static void Open(VrBuildGraph target)
        {
            var windows = Resources.FindObjectsOfTypeAll<VrBuildGraphEditorWindow>();
            foreach (var currentWindow in windows)
            {
                if (currentWindow.CurrentVrBuildGraph != target) continue;
                currentWindow.Focus();
                return;
            }
            
            var window = CreateWindow<VrBuildGraphEditorWindow>(typeof(VrBuildGraphEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null, typeof(VrBuildGraph)).image);
            window.Load(target);
        }
        
        /*
        [MenuItem("Window/Graph Creator")]
        public static void ShowWindow()
        {
            GetWindow(typeof(VrBuildGraphCreatorWindow));
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
        */

        private void Load(VrBuildGraph target)
        {
            mCurrentVrBuildGraph = target;
            DrawGraph();
        }
        
        private void DrawGraph()
        {
            mSerializedObject = new SerializedObject(mCurrentVrBuildGraph);
            mCurrentView = new VrBuildGraphEditorView(mSerializedObject, this);
            rootVisualElement.Add(mCurrentView);
        }
    }
}