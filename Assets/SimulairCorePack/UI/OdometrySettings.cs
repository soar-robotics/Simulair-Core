using System;
using System.Collections;
using System.Collections.Generic;
using SimulairCorePack.Core;
using UnityEngine;
using UnityEngine.UI;

public class OdometrySettings : MonoBehaviour
{
    /*
    public Toggle TfToggle;
    public InputField TfFreq;
    
    public Toggle ClockToggle;
    public InputField ClockFreq;
    
    public InputField OdomFreq;

    public SimulairStatePublisher SSP;

    private void OnEnable()
    {
        if (SSP != null)
        {
            updatePlaceHolders();
        }
    }

    private void Start()
    {
        if (SSP != null)
        {
            updatePlaceHolders();
        }
    }

    private void updatePlaceHolders()
    {
        TfToggle.isOn = SSP.PublishTf;
        TfFreq.text = ((int)SSP.TfPublishingFrequency).ToString();
        
        ClockToggle.isOn = SSP.PublishClock;
        ClockFreq.text = ((int)SSP.ClockPublishingFrequency).ToString();

        OdomFreq.text = ((int)SSP.OdometryPublishingFrequency).ToString();
    }

    public void Set()
    {
        if (SSP != null)
        {
            SSP.PublishTf = TfToggle.isOn;
            SSP.TfPublishingFrequency = float.Parse(TfFreq.text);

            SSP.PublishClock = TfFreq.isFocused;
            SSP.ClockPublishingFrequency = float.Parse(ClockFreq.text);

            SSP.OdometryPublishingFrequency = float.Parse(OdomFreq.text);
            updatePlaceHolders();
        }
    }
    */
}
