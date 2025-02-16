using JetBrains.Annotations;

namespace VR.Build.GraphCreator
{
    public class Node
    {
        private BuildComponent buildComponent;
        private bool satisfied;
        [CanBeNull] private Node previousNode;
        [CanBeNull] private Node nextNode;
    }
}