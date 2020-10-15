using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class SettingsPanelManager : MonoBehaviour
{
    private Color active;
    private Color deactive;

    public GameObject[] Buttons;
    public GameObject[] Panels;
    
    private void Awake()
    {
        active = new Color32(238, 121, 83, 255);
        deactive = new Color32(152, 79, 56, 255);
        onTabButtonClick(0);
    }
    

    public void onTabButtonClick(int idx)
    {
        deactivateAll();
        Buttons[idx].GetComponent<UnityEngine.UI.Image>().color = active;
        Panels[idx].SetActive(true);
        
    }

    private void deactivateAll()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].GetComponent<UnityEngine.UI.Image>().color = deactive;
            Panels[i].SetActive(false);
        }
    }

    public void ToggleThis()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
