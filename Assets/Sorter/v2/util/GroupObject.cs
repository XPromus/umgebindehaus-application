using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sorter.v2.util
{
    [Serializable]
    public struct GroupObject
    {
        public string name;
        public List<Transform> gameObjects;
    }
}