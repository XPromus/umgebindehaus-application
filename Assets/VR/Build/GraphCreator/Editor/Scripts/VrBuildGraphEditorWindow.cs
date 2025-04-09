using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        private void OnEnable()
        {
            if (mCurrentVrBuildGraph != null)
            {
                DrawGraph();
            }
        }

        private void OnGUI()
        {
            if (mCurrentVrBuildGraph != null)
            {
                if (EditorUtility.IsDirty(mCurrentVrBuildGraph))
                {
                    hasUnsavedChanges = true;
                }
                else
                {
                    hasUnsavedChanges = false;
                }
                
            }
        }

        private void Load(VrBuildGraph target)
        {
            mCurrentVrBuildGraph = target;
            DrawGraph();
        }
        
        private void DrawGraph()
        {
            mSerializedObject = new SerializedObject(mCurrentVrBuildGraph);
            mCurrentView = new VrBuildGraphEditorView(mSerializedObject, this);
            mCurrentView.graphViewChanged += OnChange;
            rootVisualElement.Add(mCurrentView);
        }

        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            hasUnsavedChanges = true;
            EditorUtility.SetDirty(mCurrentVrBuildGraph);
            return graphViewChange;
        }
    }
}