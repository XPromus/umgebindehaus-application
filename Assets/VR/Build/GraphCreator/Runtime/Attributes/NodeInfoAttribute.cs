using System;

namespace VR.Build.GraphCreator.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        public string NodeTitle { get; private set; }
        public string MenuItem { get; private set; }
        public bool HasFlowInput { get; private set; }
        public bool HasFlowOutput { get; private set; }

        public NodeInfoAttribute
        (
            string title, 
            string menuItem = "",
            bool hasFlowInput = true,
            bool hasFlowOutput = true
        )
        {
            NodeTitle = title;
            MenuItem = menuItem;
            HasFlowInput = hasFlowInput;
            HasFlowOutput = hasFlowOutput;
        }
    }
}