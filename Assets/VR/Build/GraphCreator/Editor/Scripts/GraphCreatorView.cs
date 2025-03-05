using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public class GraphCreatorView : GraphView
    {
        public GraphCreatorView()
        {
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
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }
    }
}