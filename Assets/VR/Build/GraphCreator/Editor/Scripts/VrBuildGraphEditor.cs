using UnityEditor;
using UnityEngine;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    [CustomEditor(typeof(VrBuildGraph))]
    public class VrBuildGraphEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                VrBuildGraphEditorWindow.Open((VrBuildGraph) target);
            }
        }
    }
}