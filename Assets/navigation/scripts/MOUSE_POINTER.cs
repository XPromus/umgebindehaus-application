using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.Events;
using Lean.Touch;
using Lean.Common;
using System.Runtime.CompilerServices;

public class MOUSE_POINTER : MonoBehaviour
{

    [SerializeField]
    private GameObject DefaultCenter;

    [SerializeField]
    private GameObject Position;

    [SerializeField]
    private float doubleClickTime = .5f;
    private float lastClickTime;

    [SerializeField]
    private LayerMask CollisionLayer;

    public LeanMaintainDistance CameraDistance;
    public UnityEvent DoubleClick;
    public UnityEvent SetPivotEvent;

    [SerializeField]
    private bool CheckDoubleclick = false;

    [SerializeField]
    private GameObject CameraSetup;

    private bool OnGround;

    void Update()
    {
        if (CheckDoubleclick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime)
                {
                    Debug.Log("Double click");
                    DoubleClick.Invoke();
                }

                lastClickTime = Time.time;
            }

            if (Input.GetMouseButtonDown(1))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime)
                {
                    Debug.Log("Double click");
                    DoubleClick.Invoke();
                }

                lastClickTime = Time.time;
            }
        }
    }



    public void SetPivot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2500f, CollisionLayer))
        {
            Position.transform.position = hit.point;

            if (CameraDistance)
            {
                if (CameraDistance.Distance > 1)
                {
                    CameraDistance.Distance = hit.distance;
                }
            }

            Position.transform.position = hit.point;
            //Debug.Log("normal: " + hit.normal.y.ToString());

            if (hit.normal.y == 1.0)
            {
                OnGround = true;
                Position.transform.position = Position.transform.position + new Vector3(0, 1.5f, 0);
                //Debug.Log("normal: ");
            }
            else
            {
                OnGround = false;
            }

            Position.GetComponent<LeanSelectableByFinger>().SelfSelected = true;
            SetPivotEvent.Invoke();

        }
        else
        {
            Position.transform.position = DefaultCenter.transform.position;
            Position.GetComponent<LeanSelectableByFinger>().SelfSelected = true;
            // Debug.Log("reset pivot");
        }
    }

    public void ResetPivot()
    {
        Position.transform.position = DefaultCenter.transform.position;
        Position.GetComponent<LeanSelectableByFinger>().SelfSelected = true;
        //Position.GetComponent<LeanSelectableByFinger>().SelfSelected = true;
        //Position.GetComponent<LeanSelectableByFinger>().Deselect();
        //Position.GetComponent<LeanSelectableByFinger>().Deselect();
        MoveToSelection();
        //Position.GetComponent<LeanSelectableByFinger>().Deselect();
    }

    public void MoveToSelection()
    {
        CameraSetup.GetComponent<LeanDragCamera>().MoveToSelection();
    }

    public void RotateToSelection()
    {
        if (!OnGround)
        {
            CameraSetup.GetComponent<LeanPitchYaw>().RotateToScreenPosition(Camera.main.WorldToScreenPoint(Position.transform.position));
        }

    }
}