using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionTest : MonoBehaviour {
    
    [SerializeField] private Transform list;
    private HashSet<Collider> colliders = new HashSet<Collider>();
    
    void Update() {
        //Debug.Log(colliders.Count);
    }
    
    private void OnTriggerEnter (Collider other)
    {
        if (!other.transform.IsChildOf(list)) return;
        colliders.Add(other);
        other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        other.gameObject.layer = 9;
    }
 
    private void OnTriggerExit (Collider other) {
        if (!other.transform.IsChildOf(list)) return;
        colliders.Remove(other);
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        other.gameObject.layer = 6;
    }
    
}
