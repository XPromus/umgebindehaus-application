using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionTest : MonoBehaviour {
    
    [SerializeField] private List<Transform> list;
    private HashSet<Collider> colliders = new HashSet<Collider>();
    
    //MAYBE COMBINE LIST INTO ONE TRANSFORM FOR MORE SPEED
    
    void Update() {
        //Debug.Log(colliders.Count);
    }

    private void Start()
    {
        PopulateList();
    }

    private void PopulateList()
    {
        throw new NotImplementedException();
    }
    
    private void OnTriggerEnter (Collider other)
    {
        var isInList = false;
        foreach (var transformObject in list)
        {
            isInList = other.transform.IsChildOf(transformObject);
            if (isInList) break;
        }

        if (!isInList) return;
        
        colliders.Add(other);
        other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        other.gameObject.layer = 9;
    }
 
    private void OnTriggerExit (Collider other) {
        var isInList = false;
        foreach (var transformObject in list)
        {
            isInList = other.transform.IsChildOf(transformObject);
            if (isInList) break;
        }

        if (!isInList) return;
        
        colliders.Remove(other);
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        other.gameObject.layer = 6;
    }
    
}
