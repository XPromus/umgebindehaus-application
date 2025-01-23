using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBounds : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
        CreateBoundingBox();
    }

    public void CreateBoundingBox()
    {
        var bounds = new Bounds(Vector3.zero, Vector3.one);
        var filters = GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in filters)
        {
            var matrix = transform.localToWorldMatrix.inverse * meshFilter.transform.localToWorldMatrix;
            var axisAlignedBounds = GeometryUtility.CalculateBounds(meshFilter.sharedMesh.vertices, matrix);
            bounds.Encapsulate(axisAlignedBounds);
        }

        var boundsShower = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boundsShower.name = "BOUNDS";
        boundsShower.transform.position = (transform.rotation * bounds.center) + transform.position;
        boundsShower.transform.localScale = bounds.size;
        boundsShower.transform.rotation = transform.rotation;
    }
}
