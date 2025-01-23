using System.Collections.Generic;
using Sorter.v2.util;
using UnityEngine;

namespace Sorter.v2.sorters.window
{
    public class WindowSorter
    {
        private readonly GameObject floorObjectsParent;
        private readonly List<SortComponent> windowSortComponents;
        
        public WindowSorter(List<SortComponent> windowSortComponents, GameObject floorObjectsParent)
        {
            this.windowSortComponents = windowSortComponents;
            this.floorObjectsParent = floorObjectsParent;
        }

        public void SortWindowsInHouse()
        {
            for (var i = 0; i < floorObjectsParent.transform.childCount; i++)
            {
                var floor = floorObjectsParent.transform.GetChild(i);
                var floorWindowParent = SortWindowsToGroupsInFloor(floor);
                new WindowComponentSorter().SortWindowComponents(floorWindowParent);
            }
        }

        private Transform SortWindowsToGroupsInFloor(Transform floorParentObject)
        {
            var windowParentObject = new GameObject
            {
                name = "Windows"
            };
            
            foreach (var windowSortComponent in windowSortComponents)
            {
                var pointer = 0;
                while (pointer < floorParentObject.childCount)
                {
                    var objectInFloor = floorParentObject.GetChild(pointer);
                    if (!windowSortComponent.CheckComponent(objectInFloor.gameObject.name))
                    {
                        pointer++;
                        continue;
                    }
                    
                    var windowId = GetWindowID(objectInFloor.gameObject.name);
                    var checkParent = CheckIfWindowParentExists(windowId, windowParentObject.transform);
                    if (checkParent.State)
                    {
                        objectInFloor.parent = checkParent.Parent;
                    }
                    else
                    {
                        var newWindowParent = new GameObject
                        {
                            name = windowId.ToString(),
                            transform =
                            {
                                parent = windowParentObject.transform
                            }
                        };
                        objectInFloor.parent = newWindowParent.transform;
                    }
                }
            }

            windowParentObject.transform.parent = floorParentObject;
            return windowParentObject.transform;
        }
        
        private int GetWindowID(string windowObjectName)
        {
            var splitName = windowObjectName.Split("_");
            return int.Parse(splitName[1]);
        }

        private (bool State, Transform Parent) CheckIfWindowParentExists(int id, Transform parent)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (int.Parse(child.name) == id) return (true, child);
            }

            return (false, null);
        }

        private void SortMoveableWindows(Transform windowParent)
        {
            
        }
        
    }
}