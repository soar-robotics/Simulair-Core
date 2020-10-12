using System;
using System.Collections;
using System.Collections.Generic;
using SimulairCorePack.Components.Sensors;
using UnityEngine;
using UnityEngine.UI;

public class LaserSettings : MonoBehaviour
{
    [Serializable]
    public class SpecialSlider
    {
        public Slider slider;
        public Text value;
        
    }
    public GenericLaserScanner GLS;

    public SpecialSlider RangeMinimum;
    public SpecialSlider RangeMaximum;

    public SpecialSlider ApartureAngle;
    public SpecialSlider ScanningFreq;
    public SpecialSlider AngularResolution;

    public Toggle increment;
    public Toggle visualize;
    private void OnEnable()
    {
        if (GLS != null)
        {
            updatePlaceHolders();
        }
    }

    private void Start()
    {
        if (GLS != null)
        {
            updatePlaceHolders();
        }
    }

    public void setText()
    {
        RangeMaximum.value.text = RangeMaximum.slider.value.ToString();
        RangeMinimum.value.text = RangeMinimum.slider.value.ToString();
        ApartureAngle.value.text = ApartureAngle.slider.value.ToString();
        ScanningFreq.value.text = ScanningFreq.slider.value.ToString();
        AngularResolution.value.text = AngularResolution.slider.value.ToString();
    }

    private void updatePlaceHolders()
    {
        RangeMaximum.slider.value = GLS.RangeMaximum;
        RangeMinimum.slider.value = GLS.RangeMinimum;
        ApartureAngle.slider.value = GLS.ApertureAngle;
        ScanningFreq.slider.value = GLS.ScanningFrequency;
        AngularResolution.slider.value = GLS.AngularResolution;
        increment.isOn = GLS.UseTimeIncrement;
        visualize.isOn = GLS.Visualize;
        setText();
    }

    public void Set()
    {
        if (GLS != null)
        {
            GLS.RangeMaximum = RangeMaximum.slider.value;
            GLS.RangeMinimum = RangeMinimum.slider.value;
            GLS.ApertureAngle = ApartureAngle.slider.value;
            GLS.ScanningFrequency = ScanningFreq.slider.value;
            GLS.AngularResolution = AngularResolution.slider.value;
            GLS.UseTimeIncrement = increment.isOn;
            GLS.Visualize = visualize.isOn;
            updatePlaceHolders();
        }
    }

    public void onRangeMinimumChanged()
    {
        if (RangeMinimum.slider.value > RangeMaximum.slider.value)
        {
            RangeMaximum.slider.value = RangeMinimum.slider.value;
        }
    }

    public void onRangeMaximumChanged()
    {
        if (RangeMaximum.slider.value < RangeMinimum.slider.value)
        {
            RangeMinimum.slider.value = RangeMaximum.slider.value;
        }
    }

}
