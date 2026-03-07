using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 在UI中控制字符串的格式，将格式化字符串显示出操作键位
/// </summary>
public class TMP_ControlStringFormat : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        var controls = new string[]
        {
            PlayerInput.GetInputKeyPath("MoveL"),//0
            PlayerInput.GetInputKeyPath("MoveR"),//1
            PlayerInput.GetInputKeyPath("MoveU"),//2
            PlayerInput.GetInputKeyPath("MoveD"),//3
            PlayerInput.GetInputKeyPath("Attack"),//4
            PlayerInput.GetInputKeyPath("Jump"),//5
            PlayerInput.GetInputKeyPath("Dodge"),//6
            PlayerInput.GetInputKeyPath("Skill1"),//7
            PlayerInput.GetInputKeyPath("Skill2"),//8
            PlayerInput.GetInputKeyPath("Skill3"),//9
            PlayerInput.GetInputKeyPath("Skill4"),//10
            PlayerInput.GetInputKeyPath("Special"),//11
            PlayerInput.GetInputKeyPath("Escape"),
            PlayerInput.GetInputKeyPath("ZoomIn"),
            PlayerInput.GetInputKeyPath("ZoomOut")
        };
        
        _textMeshProUGUI.text = String.Format(_textMeshProUGUI.text, controls);
    }
}
