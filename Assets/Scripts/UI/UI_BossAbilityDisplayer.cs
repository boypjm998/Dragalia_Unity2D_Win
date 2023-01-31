using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BossAbilityDisplayer : MonoBehaviour
{
    private GameObject abilityInfo;

    private bool mouseIsFollowing = false;
    // Start is called before the first frame update
    void Start()
    {
        abilityInfo = transform.Find("Info").gameObject;
        //abilityInfo.transform.SetParent(transform.parent);
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseIsFollowing)
        {
            abilityInfo.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            abilityInfo.transform.localPosition = Vector3.zero;
        }
    }
    
    public void OnMouseEnter()
    {
        //print("In");
        abilityInfo.SetActive(true);
        mouseIsFollowing = true;
    }

    public void OnMouseExit()
    {
        abilityInfo.SetActive(false);
        mouseIsFollowing = false;
    }
}
