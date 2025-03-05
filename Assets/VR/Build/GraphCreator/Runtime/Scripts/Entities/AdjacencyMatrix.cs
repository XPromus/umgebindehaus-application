using System;
using System.Collections.Generic;
using VR.Build.GraphCreator.Runtime.Scripts.Interfaces;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    public class AdjacencyMatrix : GraphData
    {
        public Dictionary<int, int> IndexDictionary { get; }
        public List<List<int>> AdjMatrix { get; }

        private AdjacencyMatrix(int size)
        {
            IndexDictionary = new Dictionary<int, int>();
            AdjMatrix = new List<List<int>>();
        }

        public void AddNode(int id)
        {
            var newSize = AdjMatrix.Count + 1;
            foreach (var row in AdjMatrix)
            {
                row.Add(0);
            }

            var newRow = new List<int>(new int[newSize]);
            AdjMatrix.Add(newRow);
            
            IndexDictionary.Add(id, newSize - 1);
        }

        public void RemoveNode(int id)
        {
            if (!IndexDictionary.TryGetValue(id, out var index))
            {
                throw new KeyNotFoundException();
            }
            
            if (index < 0 || index >= AdjMatrix.Count)
            {
                throw new IndexOutOfRangeException();
            }
            
            AdjMatrix.RemoveAt(index);
            foreach (var row in AdjMatrix)
            {
                row.RemoveAt(index);
            }

            IndexDictionary.Remove(id);
        }

        public void SetAdjacency(int sourceId, int targetId, int value)
        {
            if (!IndexDictionary.TryGetValue(sourceId, out var sourceIndex))
            {
                throw new KeyNotFoundException();
            }
            
            if (!IndexDictionary.TryGetValue(targetId, out var targetIndex))
            {
                throw new KeyNotFoundException();
            }
            
            if (sourceIndex >= AdjMatrix.Count || targetIndex >= AdjMatrix.Count)
            {
                throw new IndexOutOfRangeException();
            }

            AdjMatrix[sourceIndex][targetIndex] = value;
        }

        public void RemoveAdjacency(int sourceId, int targetId)
        {
            if (!IndexDictionary.TryGetValue(sourceId, out var sourceIndex))
            {
                throw new KeyNotFoundException();
            }
            
            if (!IndexDictionary.TryGetValue(targetId, out var targetIndex))
            {
                throw new KeyNotFoundException();
            }
            
            if (sourceIndex >= AdjMatrix.Count || targetIndex >= AdjMatrix.Count)
            {
                throw new IndexOutOfRangeException();
            }

            AdjMatrix[sourceIndex][targetIndex] = 0;
        }
        
        public int CheckAdjacencyById(int node1Id, int node2Id)
        {
            if (!IndexDictionary.TryGetValue(node1Id, out var node1Index))
            {
                throw new KeyNotFoundException();
            }

            if (!IndexDictionary.TryGetValue(node2Id, out var node2Index))
            {
                throw new KeyNotFoundException();
            }
            
            return AdjMatrix[node1Index][node2Index];
        }

        public int CheckAdjacencyByIndex(int node1Index, int node2Index)
        {
            return AdjMatrix[node1Index][node2Index];
        }

        public override string GetJsonString()
        {
            throw new NotImplementedException();
        }
    }
}