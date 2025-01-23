
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DOF_Controll : MonoBehaviour
{

    //PostProcessVolume Volume;
    public Volume Volume;
    private DepthOfField DOF;
    //DepthOfField DOF;

    //public PostProcessProfile PostProfile;
    public GameObject TargetObject;


    // Start is called before the first frame update
    void Start()
    {
        Volume.profile.TryGet<DepthOfField>(out DOF);
        //DOF.active = true;

    }

    
    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(TargetObject.transform.position, transform.position);
        DOF.focusDistance.value = dist;
    }

    public void SetPosition()
    {
        float dist = Vector3.Distance(TargetObject.transform.position, transform.position);
        GetComponent<Lean.Common.LeanMaintainDistance>().Distance = dist;

    }

}

