using System.Collections.Generic;
using UnityEngine;

public class CheckVisibility : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField]
    private List<GameObject> HotSpotList;// = new List<GameObject>();
    private int currentIndex = 0;

    public LayerMask LayerMask;

    void Start()
    {
        mainCamera = Camera.main;
        // Make list of all Hotspots
        RefreshHotSpotList();
    }

    public void RefreshHotSpotList()
    {
        HotSpotList = new List<GameObject>();
        GameObject[] hotspots = GameObject.FindGameObjectsWithTag("Hotspot");
        foreach (GameObject hotspot in hotspots)
        {
            HotSpotList.Add(hotspot);
        }
    }

    void Update()
    {
        if (HotSpotList.Count > 0)
        {
            Vector3 directionToObject = HotSpotList[currentIndex].transform.position - mainCamera.transform.position;
            RaycastHit hit;
            // Raycast from camera to object
            if (Physics.Raycast(mainCamera.transform.position, directionToObject, out hit, 2500, LayerMask))
            {
                if (hit.collider.gameObject == HotSpotList[currentIndex])
                {
                    //Debug.Log("Object is directly visible to the camera! :" + currentIndex);
                    HotSpotList[currentIndex].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    HotSpotList[currentIndex].transform.GetChild(0).gameObject.SetActive(false);
                    //Debug.Log("Object is occluded by other objects! :" + currentIndex);
                }
            }
            currentIndex = (currentIndex + 1) % HotSpotList.Count;
        }
    }
}


