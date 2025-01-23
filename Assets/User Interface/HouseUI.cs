using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseUI : MonoBehaviour {
    
    struct Element {
        public Transform elementTransform;
        public string name;
    }
    
    struct Floor {
        public Transform floorTransform;
        public Element[] elements;
        public string name;
    }
    
    [SerializeField] private Transform house;
    private Floor[] floors;

    private bool showHouseUI = false;
    
    private void Start()
    {
        floors = new Floor[house.childCount];
        for (var i = 0; i < house.childCount; i++)
        {
            var floor = house.GetChild(i);
            var newFloor = new Floor();
            newFloor.name = floor.name;
            newFloor.floorTransform = floor;
            newFloor.elements = GetElementsFromFloor(floor);
            floors[i] = newFloor;
        }
    }
    
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show House UI"))
        {
            showHouseUI = !showHouseUI;
        }
        GUILayout.EndHorizontal();

        if (showHouseUI)
        {
            DrawHouseUI();
        }
        
    }

    private void DrawHouseUI()
    {
        GUILayout.BeginHorizontal();
        foreach (var floor in floors)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(floor.name);
            foreach (var element in floor.elements)
            {
                if (GUILayout.Button(element.name))
                {
                    var state = CheckFirstChild(element.elementTransform);
                    SetObjectsInScene(element.elementTransform, !state);
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    private void SetObjectsInScene(Transform parentObject, bool state)
    {
        for (var i = 0; i < parentObject.childCount; i++)
        {
            var child = parentObject.GetChild(i);
            child.GetComponent<MeshRenderer>().enabled = state;
        }
    }

    private Element[] GetElementsFromFloor(Transform floor)
    {
        var elements = new Element[floor.childCount];
        for (var i = 0; i < elements.Length; i++)
        {
            var child = floor.GetChild(i);
            var newElement = new Element();
            newElement.name = child.name;
            newElement.elementTransform = child;
            elements[i] = newElement;
        }
        return elements;
    }

    private bool CheckFirstChild(Transform parent)
    {
        var child = parent.GetChild(0);
        return child.GetComponent<MeshRenderer>().enabled;
    }
    
}
