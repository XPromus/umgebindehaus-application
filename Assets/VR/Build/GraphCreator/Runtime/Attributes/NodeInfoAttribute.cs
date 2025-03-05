using System;

namespace VR.Build.GraphCreator.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        public string NodeTitle { get; private set; }
        public string MenuItem { get; private set; }

        public NodeInfoAttribute(string title, string menuItem = "")
        {
            NodeTitle = title;
            MenuItem = menuItem;
        }
    }
}