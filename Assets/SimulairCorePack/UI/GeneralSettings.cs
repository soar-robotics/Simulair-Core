using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettings : MonoBehaviour
{
    [Serializable]
    public class SpecialSlider
    {
        public Slider slider;
        public Text value;
        
        public void setVal(float val)
        {
            slider.value = val;
            value.text = val.ToString();
        }
        
    }

    public UnityInputTeleop UIT;
    public GameViewEncoder GVE;
    public Text DebugText;

    public Toggle activateInternalTeleop;
    public Toggle debug;
    public SpecialSlider Quality;
    public SpecialSlider FPS;

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
