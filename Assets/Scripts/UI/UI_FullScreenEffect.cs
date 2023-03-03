using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_FullScreenEffect : MonoBehaviour
{
    private Image _image;
    private Tweener _tweener;
    private bool state;

    private void Awake()
    {
        _image = GetComponent<Image>();
        state = false;
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        
        
        
    }

    public void Enable()
    {
        if (state == false && !gameObject.activeSelf)
        {
            return;
        }
        _tweener.Complete();

        state = false;
        _image = GetComponent<Image>();
        //gameObject.SetActive(true);
        //_image.color = Color.clear;
        _tweener = _image.DOColor(new Color(1, 1, 1, 0.5f),1f);
        
    }

    public void Disable()
    {
        if (state == true && gameObject.activeSelf)
        {
            return;
        }
        _tweener.Complete();
        
        state = true;
        //_image.color = new Color(1, 1, 1, 0.5f);
        _tweener = _image.DOColor(Color.clear, 1f);

    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
