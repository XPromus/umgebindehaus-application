using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour {
    [SerializeField] private Material _material;
    [SerializeField] private Transform shaderPointTransform;
    private void Update()
    {
        SetShaderPosition();
    }

    private void SetShaderPosition()
    {
        _material.SetVector("_position", shaderPointTransform.position);
    }
    
}
