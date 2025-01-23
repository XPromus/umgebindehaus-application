using System;
using System.Text.RegularExpressions;
using Lean.Touch;
using Sorter.v2.windows;
using TMPro;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Sorter.v2.sorters.window
{
    public class WindowComponentSorter
    {
        public void SortWindowComponents(Transform floorWindowParent)
        {
            for (var i = 0; i < floorWindowParent.childCount; i++)
            {
                var windowParent = floorWindowParent.GetChild(i);
                windowParent.gameObject.layer = LayerMask.NameToLayer("Interactable");
                windowParent.gameObject.tag = "Interactable";

                if (!CheckIfWindowIsMoveable(windowParent)) continue;

                var boundsOfWindow = GetBoundsOfAllChildren(windowParent.gameObject);
                var faceDirection = GetFaceDirectionBeforeSort(windowParent);
                
                var wing1Parent = new GameObject
                {
                    name = "Wing 1",
                    layer = LayerMask.NameToLayer("Interactable"),
                    tag = "Interactable",
                    transform =
                    {
                        position = GetWingPivot(windowParent, "Flügel_1")
                    }
                };

                var wing2Parent = new GameObject
                {
                    name = "Wing 2",
                    layer = LayerMask.NameToLayer("Interactable"),
                    tag = "Interactable",
                    transform =
                    {
                        position = GetWingPivot(windowParent, "Flügel_2")
                    }
                };

                var staticParent = new GameObject
                {
                    name = "Static",
                    layer = LayerMask.NameToLayer("Interactable"),
                    tag = "Interactable"
                };

                var pointer = 0;
                while (pointer < windowParent.childCount)
                {
                    var windowChild = windowParent.GetChild(pointer);
                    
                    /*
                    if (CheckGameObjectName(windowChild.name, "Flügel_1"))
                    {
                        windowChild.parent = wing1Parent.transform;
                    } 
                    else if (CheckGameObjectName(windowChild.name, "Flügel_2"))
                    {
                        windowChild.parent = wing2Parent.transform;
                    }
                    */
                    
                    if (CheckGameObjectName(windowChild.name, "Flügel"))
                    {
                        var targetWing = GetTargetWingOfObject(windowChild.gameObject, boundsOfWindow, faceDirection);
                        windowChild.parent = targetWing switch
                        {
                            WingTarget.Wing1 => wing1Parent.transform,
                            WingTarget.Wing2 => wing2Parent.transform,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    else
                    {
                        windowChild.parent = staticParent.transform;
                    }

                    windowChild.gameObject.layer = LayerMask.NameToLayer("Interactable");
                    windowChild.gameObject.tag = "Interactable";
                }
                    
                wing1Parent.transform.parent = windowParent;
                wing2Parent.transform.parent = windowParent;
                staticParent.transform.parent = windowParent;
                    
                AddWindowFunctionality(windowParent.gameObject, wing1Parent, wing2Parent, faceDirection);
            }
        }

        private bool CheckIfWindowIsMoveable(Transform window)
        {
            var moveableRegex = new Regex(@"\b\w*" + "Drehpunkt" + @"\w*\b");
            for (var i = 0; i < window.childCount; i++)
            {
                var child = window.GetChild(i);
                if (moveableRegex.Match(child.name).Success) return true;
            }

            return false;
        }

        private bool CheckGameObjectName(string name, string key)
        {
            return new Regex(@"\b\w*" + key + @"\w*\b").Match(name).Success;
        }

        private Vector3 GetWingPivot(Transform windowParent, string wingName)
        {
            for (var i = 0; i < windowParent.childCount; i++)
            {
                var child = windowParent.GetChild(i);
                if (CheckGameObjectName(child.name, "Drehpunkt") && CheckGameObjectName(child.name, wingName))
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
        
        private void AddWindowFunctionality(GameObject windowParent, GameObject wing1, GameObject wing2, FaceDirection faceDirection)
        {
            //FaceDirection windowFaceDirection = GetFaceDirection(windowParent.transform);

            //var xrInteraction = windowParent.AddComponent<XRSimpleInteractable>();
            var controller = windowParent.AddComponent<WindowController>();
            controller.LeftPane = wing1;
            controller.RightPane = wing2;
            controller.PaneOpenAngle = 90f;
            controller.RotationAxisValue = WindowController.RotationAxis.Y;
            controller.OpeningDirectionValue = GetOpeningDirection(faceDirection);
            controller.OpeningTime = 3f;
            controller.ShowDebugUI = false;
            
            UnityAction<GameObject> action = controller.UseWindow;
            
            var wing1XRInteractable = wing1.AddComponent<XRSimpleInteractable>();
            var wing1Selectable = wing1.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(wing1Selectable.OnSelected, action, wing1);
            UnityEventTools.AddObjectPersistentListener(wing1Selectable.OnDeselected, action, wing1);
            
            var wing2XRInteractable = wing2.AddComponent<XRSimpleInteractable>();
            var wing2Selectable = wing2.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(wing2Selectable.OnSelected, action, wing2);
            UnityEventTools.AddObjectPersistentListener(wing2Selectable.OnDeselected, action, wing2);

            var colliderCenter = GetColliderCenterPoint(faceDirection);
            var colliderSize = GetColliderSize(faceDirection);
            
            var wing1Collider = wing1.AddComponent<BoxCollider>();
            wing1Collider.center = colliderCenter.wing1;
            wing1Collider.size = colliderSize.wing1;
            /*
            wing1Collider.center = new Vector3(0f, -0.36f, -0.18f);
            wing1Collider.size = new Vector3(0.07f, 1f, 0.34f);
            */
            
            var wing2Collider = wing2.AddComponent<BoxCollider>();
            wing2Collider.center = colliderCenter.wing2;
            wing2Collider.size = colliderSize.wing2;
            /*
            wing2Collider.center = new Vector3(0f, -0.36f, 0.18f);
            wing2Collider.size = new Vector3(0.07f, 1f, 0.34f);
            */
        }

        private FaceDirection GetFaceDirectionBeforeSort(Transform windowParent)
        {
            Vector3 rotationPoint = default(Vector3);
            Vector3 directionPoint = default(Vector3);

            for (var i = 0; i < windowParent.childCount; i++)
            {
                var child = windowParent.GetChild(i);
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
                return FaceDirection.XNegative;
            }

            if (rotationPoint.x > directionPoint.x)
            {
                return FaceDirection.XPositive;
            }
            
            if (rotationPoint.z < directionPoint.z)
            {
                return FaceDirection.ZNegative;
            }

            if (rotationPoint.z > directionPoint.z)
            {
                return FaceDirection.ZPositive;
            }

            throw new Exception();
        }
        
        private FaceDirection GetFaceDirection(Transform windowParent)
        {
            Vector3 rotationPoint = default(Vector3);
            Vector3 directionPoint = default(Vector3);

            for (var i = 0; i < windowParent.GetChild(0).childCount; i++)
            {
                var child = windowParent.GetChild(0).GetChild(i);
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
                return FaceDirection.XNegative;
            }

            if (rotationPoint.x > directionPoint.x)
            {
                return FaceDirection.XPositive;
            }
            
            if (rotationPoint.z < directionPoint.z)
            {
                return FaceDirection.ZNegative;
            }

            if (rotationPoint.z > directionPoint.z)
            {
                return FaceDirection.ZPositive;
            }

            throw new Exception();
        }

        private (Vector3 wing1, Vector3 wing2) GetColliderCenterPoint(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.XPositive => (
                    new Vector3(0f, -0.11f, -0.18f),
                    new Vector3(0f, -0.11f, 0.18f)
                ),
                FaceDirection.XNegative => (
                    new Vector3(0f, -0.11f, -0.18f),
                    new Vector3(0f, -0.11f, 0.18f)
                ),
                FaceDirection.ZPositive => (
                    new Vector3(-0.18f, -0.11f, 0f),
                    new Vector3(0.18f, -0.11f, 0f)
                ),
                FaceDirection.ZNegative => (
                    new Vector3(-0.18f, -0.11f, 0f),
                    new Vector3(0.18f, -0.11f, 0f)
                ),
                _ => (Vector3.zero, Vector3.zero)
            };
        }

        private (Vector3 wing1, Vector3 wing2) GetColliderSize(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.XPositive => (
                    new Vector3(0.07f, 1f, 0.34f),
                    new Vector3(0.07f, 1f, 0.34f)
                ),
                FaceDirection.XNegative => (
                    new Vector3(0.07f, 1f, 0.34f),
                    new Vector3(0.07f, 1f, 0.34f)
                ),
                FaceDirection.ZPositive => (
                    new Vector3(0.34f, 1f, 0.07f),
                    new Vector3(0.34f, 1f, 0.07f)
                ),
                FaceDirection.ZNegative => (
                    new Vector3(0.34f, 1f, 0.07f),
                    new Vector3(0.34f, 1f, 0.07f)
                ),
                _ => (Vector3.zero, Vector3.zero)
            };
        }

        private WindowController.OpeningDirection GetOpeningDirection(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.XNegative => WindowController.OpeningDirection.OUTWARDS,
                FaceDirection.XPositive => WindowController.OpeningDirection.INWARDS,
                FaceDirection.ZNegative => WindowController.OpeningDirection.INWARDS,
                FaceDirection.ZPositive => WindowController.OpeningDirection.OUTWARDS,
                _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
            };
        }

        private static Bounds GetBoundsOfAllChildren(GameObject targetGameObject)
        {
            var renderers = targetGameObject.GetComponentsInChildren<Renderer>();
            if (renderers.Length <= 0) return new Bounds();
            
            var bounds = renderers[0].bounds;
            foreach (var t in renderers)
            {
                bounds.Encapsulate(t.bounds);
            }

            return bounds;
        }

        private static WingTarget GetTargetWingOfObject(GameObject targetGameObject, Bounds windowBounds, FaceDirection faceDirection)
        {
            var windowCenter = windowBounds.center;
            var targetCenter = targetGameObject.GetComponent<Renderer>().bounds.center;

            return faceDirection switch
            {
                FaceDirection.XPositive => targetCenter.z > windowCenter.z ? WingTarget.Wing1 : WingTarget.Wing2,
                FaceDirection.XNegative => targetCenter.z > windowCenter.z ? WingTarget.Wing1 : WingTarget.Wing2,
                FaceDirection.ZPositive => targetCenter.x > windowCenter.x ? WingTarget.Wing1 : WingTarget.Wing2,
                FaceDirection.ZNegative => targetCenter.x > windowCenter.x ? WingTarget.Wing1 : WingTarget.Wing2,
                _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
            };
        }

        private enum WingTarget
        {
            Wing1,
            Wing2,
        }

        private enum FaceDirection
        {
            XPositive,
            XNegative,
            ZPositive,
            ZNegative,
        }
        
    }
}