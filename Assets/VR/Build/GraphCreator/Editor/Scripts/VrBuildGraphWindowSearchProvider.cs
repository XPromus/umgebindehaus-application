using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VR.Build.GraphCreator.Runtime.Attributes;
using VR.Build.GraphCreator.Runtime.Scripts.Entities;

namespace VR.Build.GraphCreator.Editor.Scripts
{
    public struct SearchContextElement
    {
        public object Target { get; private set; }
        public string Title { get; private set; }

        public SearchContextElement(object target, string title)
        {
            Target = target;
            Title = title;
        }
    }
    
    public class VrBuildGraphWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public VrBuildGraphEditorView Graph;
        public VisualElement Target;

        private static List<SearchContextElement> elements;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var newTree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Nodes"), 0)
            };
            elements = new List<SearchContextElement>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = type.GetCustomAttribute(typeof(NodeInfoAttribute));
                    if (attribute == null) continue;
                    var att = (NodeInfoAttribute)attribute;
                    var node = Activator.CreateInstance(type);
                    if (string.IsNullOrEmpty(att.MenuItem))
                    {
                        continue;
                    }
                            
                    elements.Add(new SearchContextElement(node, att.MenuItem));
                }
            }
            
            elements.Sort((entry1, entry2) =>
            {
                var splits1 = entry1.Title.Split("/");
                var splits2 = entry2.Title.Split("/");
                for (var i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length) return 1;
                    var value = string.Compare(splits1[i], splits2[i], StringComparison.Ordinal);
                    if (value == 0) continue;
                    
                    if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                    {
                        return splits1.Length < splits2.Length ? 1 : -1;
                    }

                    return value;
                }

                return 0;
            });

            var groups = new List<string>();
            foreach (var element in elements)
            {
                var entryTitle = element.Title.Split("/");
                var groupName = "";
                for (var i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        newTree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                var entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()))
                {
                    level = entryTitle.Length,
                    userData = new SearchContextElement(element.Target, element.Title)
                };
                newTree.Add(entry);
            }

            return newTree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = Graph.ChangeCoordinatesTo(Graph, context.screenMousePosition - Graph.Window.position.position);
            var graphMousePosition = Graph.contentViewContainer.WorldToLocal(windowMousePosition);
            var element = (SearchContextElement)SearchTreeEntry.userData;
            var node = (VrBuildGraphNode)element.Target;
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            Graph.Add(node);
            return true;
        }
    }
}