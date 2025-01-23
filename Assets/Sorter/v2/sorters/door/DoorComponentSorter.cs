using System;
using System.Text.RegularExpressions;
using Lean.Touch;
using Sorter.v2.doors;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Sorter.v2.sorters.door
{
    public class DoorComponentSorter
    {
        public void SortDoorComponents(Transform floorDoorParent)
        {
            for (var i = 0; i < floorDoorParent.childCount; i++)
            {
                var doorParent = floorDoorParent.GetChild(i);
                doorParent.gameObject.layer = LayerMask.NameToLayer("Interactable");
                
                if (CheckIfDoorIsMoveable(doorParent))
                {
                    var wingParent = new GameObject
                    {
                        name = "Wing",
                        layer = LayerMask.NameToLayer("Interactable"),
                        tag = "Interactable",
                        transform =
                        {
                            position = GetDoorPivot(doorParent)
                        }
                    };

                    var staticParent = new GameObject
                    {
                        name = "Static",
                        layer = LayerMask.NameToLayer("Interactable"),
                        tag = "Interactable"
                    };

                    var pointer = 0;
                    while (pointer < doorParent.childCount)
                    {
                        var doorChild = doorParent.GetChild(pointer);
                        if (CheckGameObjectName(doorChild.name, "Blendrahmen"))
                        {
                            doorChild.parent = staticParent.transform;
                        }
                        else
                        {
                            doorChild.parent = wingParent.transform;
                        }
                    }
                    
                    wingParent.transform.parent = doorParent;
                    staticParent.transform.parent = doorParent;
                    
                    AddDoorFunctionality(doorParent.gameObject, wingParent);
                }
            }
        }

        private bool CheckIfDoorIsMoveable(Transform door)
        {
            var moveableRegex = new Regex(@"\b\w*" + "Drehpunkt" + @"\w*\b");
            for (var i = 0; i < door.childCount; i++)
            {
                var child = door.GetChild(i);
                if (moveableRegex.Match(child.name).Success) return true;
            }

            return false;
        }

        private bool CheckGameObjectName(string name, string key)
        {
            return new Regex(@"\b\w*" + key + @"\w*\b").Match(name).Success;
        }

        private Vector3 GetDoorPivot(Transform doorParent)
        {
            for (var i = 0; i < doorParent.childCount; i++)
            {
                var child = doorParent.GetChild(i);
                if (CheckGameObjectName(child.name, "Drehpunkt"))
                {
                    child.gameObject.SetActive(false);
                    return GetObjectCenter(child);
                }
            }

            return Vector3.zero;
        }

        private Vector3 GetObjectCenter(Transform target)
        {
            return target.gameObject.GetComponent<Renderer>().bounds.center;
        }

        private void AddDoorFunctionality(GameObject doorParent, GameObject wing)
        {
            var doorFaceDirection = GetFaceDirection(doorParent.transform);
            
            var controller = doorParent.AddComponent<DoorController>();
            controller.DoorObject = wing;
            controller.OpeningDirection = DoorController.OpeningDirectionEnum.OUTWARDS;
            controller.DoorOpenAngle = 90f;
            controller.OpeningTime = 3f;
            controller.ShowDebugUI = true;

            UnityAction<GameObject> action = controller.UseDoor;
            var selectable = wing.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(selectable.OnSelected, action, wing);
            UnityEventTools.AddObjectPersistentListener(selectable.OnDeselected, action, wing);

            var doorCollider = wing.AddComponent<BoxCollider>();
            doorCollider.center = GetColliderCenterPoint(doorFaceDirection);
            doorCollider.size = GetColliderSize(doorFaceDirection);
        }

        private FaceDirection GetFaceDirection(Transform doorParent)
        {
            Vector3 rotationPoint = default(Vector3);
            Vector3 directionPoint = default(Vector3);

            for (var i = 0; i < doorParent.GetChild(0).childCount; i++)
            {
                var child = doorParent.GetChild(0).GetChild(i);
                if (CheckGameObjectName(child.name, "Drehpunkt"))
                {
                    rotationPoint = GetObjectCenter(child);
                } 
                else if (CheckGameObjectName(child.name, "Richtung"))
                {
                    directionPoint = GetObjectCenter(child);
                }
            }
            
            if (rotationPoint == default || directionPoint == default)
            {
                throw new Exception();
            }
            
            if (rotationPoint.x < directionPoint.x)
            {
                return FaceDirection.X_NEGATIVE;
            }

            if (rotationPoint.x > directionPoint.x)
            {
                return FaceDirection.X_POSITIVE;
            }
            
            if (rotationPoint.z < directionPoint.z)
            {
                return FaceDirection.Z_NEGATIVE;
            }

            if (rotationPoint.z > directionPoint.z)
            {
                return FaceDirection.Z_POSITIVE;
            }

            throw new Exception();
        }

        private Vector3 GetColliderCenterPoint(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.X_POSITIVE => new Vector3(0.39f, -0.26f, 0.03f),
                FaceDirection.X_NEGATIVE => new Vector3(0.39f, -0.26f, 0.03f),
                FaceDirection.Z_POSITIVE => new Vector3(0.03f, -0.26f, 0.39f),
                FaceDirection.Z_NEGATIVE => new Vector3(0.03f, -0.26f, 0.39f),
                _ => Vector3.zero
            };
        }

        private Vector3 GetColliderSize(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.X_POSITIVE => new Vector3(0.97f, 1.99f, 0.06f),
                FaceDirection.X_NEGATIVE => new Vector3(0.97f, 1.99f, 0.06f),
                FaceDirection.Z_POSITIVE => new Vector3(0.06f, 1.99f, 0.97f),
                FaceDirection.Z_NEGATIVE => new Vector3(0.06f, 1.99f, 0.97f),
                _ => Vector3.zero
            };
        }
        
        private enum FaceDirection
        {
            X_POSITIVE,
            X_NEGATIVE,
            Z_POSITIVE,
            Z_NEGATIVE,
        }    
            
    }
}