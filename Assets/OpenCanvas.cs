using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCanvas : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    private void OnMouseDown()
    {
        ShowCanvas();
    }

    public void ShowCanvas() 
    {
        canvas.SetActive(true);
    }

    public void CloseCanvas()
    {
        canvas.SetActive(false);
    }
}
