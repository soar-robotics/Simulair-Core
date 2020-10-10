using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class junkscript : MonoBehaviour
{

    private const int maxItemsToRead = 10;
    private const int maxItemsAllowed = 100;
    private Text myText;
    public static junkscript instance;
    
    public Queue<string> messages = new Queue<string>();
    private void Awake()
    {
        instance = this;
        myText = transform.GetComponent<Text>();
    }
    
    public void printLine(string value)
    {
        string newString = value.Replace(" ", "~");
        if(value != "")
            messages.Enqueue(" >" + value + "\n");
    }
    
    public void print(string value)
    {
        string newString = value.Replace(" ", "~");
        if(value != "")
            messages.Enqueue(" >" + value);
    }

    public void clc()
    {
        myText.text = "";
    }

    private void Update()
    {
        if (messages.Count > 0)
        {
            int toRead = ((messages.Count < 10) ? messages.Count : maxItemsToRead);
            for (int i = 0; i < toRead; i++)
            {
                myText.text += messages.Dequeue();
            }
            
        }

        if (messages.Count > maxItemsAllowed) //prevent memory overflow
        {
            messages.Clear();
        }

        if (myText.text.Length > 3000)
        {
            clc();
        }
    }
}