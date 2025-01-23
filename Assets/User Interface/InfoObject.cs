using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoObject : MonoBehaviour {
    
    [Header("Content")]
    [SerializeField] private string objectName;
    [SerializeField] private string infoText;
    [SerializeField] private Image[] images;

    [Header("Settings")] 
    [SerializeField] private float width = 500f;
    [SerializeField] private float height = 800f;
    
    [SerializeField] private bool active;
    private bool scaling = true;
    Rect windowRect;

    private void Start()
    {
        windowRect = new Rect(10, 10, width, height);
    }

    public void Toggle()
    {
        active = !active;
    }

    private void OnGUI()
    {
        if (!active) return;
        DrawInfoWindow();
    }

    private void DrawInfoWindow()
    {
        windowRect = GUILayout.Window(0, windowRect, WindowContent, objectName);
    }

    private void WindowContent(int windowID)
    {
        GUILayout.TextArea(infoText);
        DrawImages();
    }

    private void DrawImages()
    {
        GUILayout.BeginHorizontal();
        foreach (var image in images)
        {
            //TODO: Insert images
        }
        GUILayout.EndHorizontal();
    }

    private void DrawBottomButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Button("Schließen");
        GUILayout.EndHorizontal();
    }
    
}
