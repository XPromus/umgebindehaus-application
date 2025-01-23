using System.Collections.Generic;
using Sorter.v2.util;
using UnityEngine;

namespace Sorter.v2.sorters.door
{
    public class DoorSorter
    {
        private readonly GameObject floorObjectsParent;
        private readonly List<SortComponent> doorSortComponents;

        public DoorSorter(List<SortComponent> doorSortComponents, GameObject floorObjectsParent)
        {
            this.floorObjectsParent = floorObjectsParent;
            this.doorSortComponents = doorSortComponents;
        }

        public void SortDoorsInHouse()
        {
            for (var i = 0; i < floorObjectsParent.transform.childCount; i++)
            {
                var floor = floorObjectsParent.transform.GetChild(i);
                var floorDoorParent = SortDoorsToGroupsInFloor(floor);
                new DoorComponentSorter().SortDoorComponents(floorDoorParent);
            }
        }

        private Transform SortDoorsToGroupsInFloor(Transform floorParentObject)
        {
            var doorParentObject = new GameObject
            {
                name = "Doors"
            };

            foreach (var doorSortComponent in doorSortComponents)
            {
                var pointer = 0;
                while (pointer < floorParentObject.childCount)
                {
                    var objectInFloor = floorParentObject.GetChild(pointer);
                    if (!doorSortComponent.CheckComponent(objectInFloor.gameObject.name))
                    {
                        pointer++;
                        continue;
                    }

                    var doorId = GetDoorID(objectInFloor.gameObject.name);
                    var checkParent = CheckIfWindowParentExists(doorId, doorParentObject.transform);
                    if (checkParent.State)
                    {
                        objectInFloor.parent = checkParent.Parent;
                    }
                    else
                    {
                        var newDoorParent = new GameObject
                        {
                            name = doorId.ToString(),
                            transform =
                            {
                                parent = doorParentObject.transform
                            }
                        };
                        objectInFloor.parent = newDoorParent.transform;
                    }
                }
            }

            doorParentObject.transform.parent = floorParentObject;
            return doorParentObject.transform;
        }

        private int GetDoorID(string doorObjectName)
        {
            var splitName = doorObjectName.Split("_");
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
        
    }
}