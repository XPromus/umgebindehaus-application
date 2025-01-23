using System.Collections.Generic;
using Sorter.v2.util;
using UnityEngine;

namespace Sorter.v2.sorters.group
{
    public class GroupSorter
    {

        public void SortObjectsToGroups(GameObject floorsParent)
        {
            for (int i = 0; i < floorsParent.transform.childCount; i++)
            {
                var floor = floorsParent.transform.GetChild(i);
                SortObjectsInFloorToGroups(floor.gameObject);
            }
        }
        
        private void SortObjectsInFloorToGroups(GameObject floorParent)
        {
            var groupObjects = new List<GroupObject>();

            for (var i = 0; i < floorParent.transform.childCount; i++)
            {
                var child = floorParent.transform.GetChild(i);
                if (child.childCount > 0)
                {
                    continue;
                }

                var childName = SplitObjectNameToName(child.name);
                var checkResult = CheckGroupObjectsForHit(groupObjects, childName);
                
                if (checkResult.Item1)
                {
                    groupObjects[checkResult.Item2].gameObjects.Add(child);
                }
                else
                {
                    var newGroupObject = new GroupObject
                    {
                        name = childName,
                        gameObjects = new List<Transform> { child }
                    };
                    groupObjects.Add(newGroupObject);
                }
            }

            foreach (var groupObject in groupObjects)
            {
                var newGameObject = new GameObject
                {
                    name = groupObject.name,
                    transform =
                    {
                        parent = floorParent.transform
                    }
                };

                foreach (var groupObjectChild in groupObject.gameObjects)
                {
                    groupObjectChild.parent = newGameObject.transform;
                }
            }
        }
        
        private (bool, int) CheckGroupObjectsForHit(List<GroupObject> groupObjects, string targetObjectName)
        {
            for (var i = 0; i < groupObjects.Count; i++)
            {
                var groupObject = groupObjects[i];
                if (groupObject.name.Equals(targetObjectName))
                {
                    return (true, i);
                }
            }

            return (false, 0);
        }

        private string SplitObjectNameToName(string objectName)
        {
            return objectName.Split("_")[0];
        }
        
    }
}