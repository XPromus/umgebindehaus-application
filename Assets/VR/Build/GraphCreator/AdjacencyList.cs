using System;
using System.Collections.Generic;
using UnityEngine;

namespace VR.Build.GraphCreator
{
    public class AdjacencyList : GraphData
    {
        public Dictionary<int, List<int>> List { get; }

        private AdjacencyList()
        {
            List = new Dictionary<int, List<int>>();
        }

        public void AddNode(int node)
        {
            if (!List.ContainsKey(node))
            {
                List[node] = new List<int>();
            }
        }

        public void AddEdge(int src, int destination)
        {
            if (!List.ContainsKey(src))
            {
                AddNode(src);
            }

            if (!List.ContainsKey(destination))
            {
                AddNode(destination);
            }
            List[src].Add(destination);
        }
        
        public void PrintAdjacencyList()
        {
            foreach (var vertex in List)
            {
                Debug.Log(vertex.Key + " -> ");
                foreach (var neighbor in vertex.Value)
                {
                    Debug.Log(neighbor + " ");
                }
            }
        }

        public override string GetJsonString()
        {
            throw new NotImplementedException();
        }
    }
}