using Lean.Common;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private LeanPitchYaw YawSettings;

    [SerializeField]
    private LeanMaintainDistance DistanceSettings;

    [SerializeField]
    private MOUSE_POINTER PivotPointController;

    private float InitialDistance;
    private float InitialPitch;
    private float InitialYaw;
    


    // Start is called before the first frame update
    void Start()
    {
        InitialDistance = DistanceSettings.Distance;
        InitialPitch = YawSettings.Pitch;
        InitialYaw = YawSettings.Yaw;
    }



    public void ResetViewPort()
    {
        PivotPointController.ResetPivot();
        DistanceSettings.Distance=InitialDistance;
        YawSettings.Yaw=InitialYaw;
        YawSettings.Pitch=InitialPitch;
        //PivotPointController.ResetPivot();
    }

}
