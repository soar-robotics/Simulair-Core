using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class junkscript : MonoBehaviour
{
    private Text myText;
    public static junkscript instance;
    private void Awake()
    {
        myText = transform.GetComponent<Text>();
        instance = this;
    }
    
    public void printLine(string value)
    {
        string newString = value.Replace(" ", "~");
        if(value != "")
            myText.text += " >" + value + "\n";
    }
    
    public void print(string value)
    {
        string newString = value.Replace(" ", "~");
        if(value != "")
            myText.text += " >" + value;
    }

    public void clc()
    {
        myText.text = "";
    }
    
}