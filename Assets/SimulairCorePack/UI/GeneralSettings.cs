using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettings : MonoBehaviour
{
    public UnityInputTeleop UIT;

    public Toggle activateInternalTeleop;

    private void OnEnable()
    {
        updatePlaceHolders();
    }
    
    private void updatePlaceHolders()
    {
        activateInternalTeleop.isOn = UIT.enabled;
    }

    public void Set()
    {
        if (UIT != null)
        {
            UIT.enabled = activateInternalTeleop.isOn;
            updatePlaceHolders();
        }
    }
}
