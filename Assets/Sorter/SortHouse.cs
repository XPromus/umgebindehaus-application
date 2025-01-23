using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortHouse : MonoBehaviour {

    private class SortObject {
        private String name;
        private List<Transform> transformList = new List<Transform>();

        public void SetName(String newName)
        {
            name = newName;
        }
        
        public String GetName()
        {
            return name;
        }

        public List<Transform> GetTransformList()
        {
            return transformList;
        }
    }
    
    [Serializable]
    struct Floor {
        public GameObject pane;
        public String name;
    }
    
    [Header("Floors")]
    [SerializeField] private Floor[] floors;
    [Header("House")]
    [SerializeField] private GameObject targetHouse;

    [SerializeField] private Transform halfPlane;
    
    [Header("Options")] 
    [SerializeField] private bool forceCeilingBeams;
    [SerializeField] private String ceilingBeamName;
    

    private void Awake()
    {
        Sort();
    }
    
    private void Sort()
    {
        var yRanges = GetYRanges();
        var sortedElements = CreateElementsArray(yRanges);
        if (forceCeilingBeams)
        {
            sortedElements = SortCeilingBeams(sortedElements);
        }
        for (var i = 0; i < sortedElements.Length; i++)
        {
            var newObject = new GameObject
            {
                name = floors[i].name,
                transform =
                {
                    parent = targetHouse.transform
                }
            };
            var children = sortedElements[i].ToArray();
            var sortObjects = new List<SortObject>();
            foreach (var child in children)
            {
                var splitName = child.name.Split("_")[0];
                var found = false;
                if (sortObjects.Count == 0)
                {
                    sortObjects.Add(new SortObject());
                    sortObjects[0].SetName(splitName);
                    found = true;
                }
                foreach (var sortObject in sortObjects)
                {
                    if (sortObject.GetName().Equals(splitName))
                    {
                        sortObject.GetTransformList().Add(child);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    sortObjects.Add(new SortObject());
                    sortObjects[^1].SetName(splitName);
                    sortObjects[^1].GetTransformList().Add(child);
                }
            }

            foreach (var sortObject in sortObjects)
            {
                var parentObject = new GameObject
                {
                    name = sortObject.GetName(),
                    transform =
                    {
                        parent = newObject.transform
                    }
                };
                foreach (var childObject in sortObject.GetTransformList())
                {
                    childObject.parent = parentObject.transform;
                }
            }
        }

        foreach (var floor in floors)
        {
            floor.pane.SetActive(false);
        }
    }

    private List<Transform>[] CreateElementsArray(float[] yRanges)
    {
        var sortedElements = new List<Transform>[floors.Length];
        for (var i = 0; i < sortedElements.Length; i++) {
            sortedElements[i] = new List<Transform>();
        }
        
        foreach (Transform child in targetHouse.transform)
        {
            var pos = child.GetComponent<Renderer>().bounds.center;
            for (var i = 0; i < yRanges.Length; i++) {
                if (pos.y < yRanges[i]) {
                    sortedElements[i].Add(child);
                    break;
                }
                if (i == yRanges.Length - 1 && pos.y > yRanges[^1]) {
                    sortedElements[^1].Add(child);
                    break;
                }
            }
        }
        
        return sortedElements;
    }

    private float[] GetYRanges() {
        var ranges = new float[floors.Length - 1];
        for (var i = 0; i < floors.Length - 1; i++) {
            ranges[i] = floors[i].pane.transform.position.y;
        }
        return ranges;
    }

    private List<Transform>[] SortCeilingBeams(List<Transform>[] sortedElements)
    {
        var returnList = new List<Transform>[sortedElements.Length];
        
        for (var i = 0; i < sortedElements.Length; i++)
        {
            var elements = sortedElements[i];
            if (i > 0)
            {
                foreach (var element in elements.ToList())
                {
                    var split = element.name.Split("_");
                    if (split[0].Equals(ceilingBeamName))
                    {
                        returnList[i - 1].Add(element);
                        elements.Remove(element);
                    }
                }
            }
            returnList[i] = elements;
        }

        return returnList;
    }
    
}
