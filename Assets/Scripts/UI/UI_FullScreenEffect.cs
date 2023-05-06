using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_FullScreenEffect : MonoBehaviour
{
    protected Image _image;
    protected Tweener _tweener;
    protected bool state;
    
    public abstract void Enable();
    public abstract void Disable();
}
