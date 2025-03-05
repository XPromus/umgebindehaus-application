using UnityEngine;

namespace VR.Build.GraphCreator.Runtime.Scripts.Utility
{
    public static class Extensions
    {
        public static bool TryGetComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() != null;
        }
    }
}