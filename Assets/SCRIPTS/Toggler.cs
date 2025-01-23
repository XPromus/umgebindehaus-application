using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Toggler : MonoBehaviour
{
    [SerializeField]
    private UnityEvent ToggleOn;

    [SerializeField]
    private UnityEvent ToggleOff;

    private bool ToggleState;

    public void ToggleOnOff()
    {
        ToggleState = !ToggleState;

        if (ToggleState)
        {
            ToggleOnEvents();
        }
        else
        {
            ToggleOffEvents();
        }
    }

   private void ToggleOnEvents()
    {
        ToggleOn.Invoke();
    }

    private void ToggleOffEvents()
    {
        ToggleOff.Invoke();
    }
}
