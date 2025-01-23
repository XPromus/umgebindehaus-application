using System;
using UnityEngine;

namespace VR.Build
{
    public class BuildComponent
    {
        private int id;
        public int ID
        {
            get => id;
            set => id = value;
        }

        private GameObject originalObject;
        private GameObject ghostObject;

        public bool CheckImpact()
        {
            throw new NotImplementedException();
        }
    }
}