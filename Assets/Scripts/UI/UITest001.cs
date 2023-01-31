using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITest001 : MonoBehaviour
{
    private GlobalController globalController;
    private TextMeshPro tmp;
    private void Start()
    {
        globalController = FindObjectOfType<GlobalController>();
        tmp = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        //tmp.text = globalController.clickEffCD.ToString();
    }
}
