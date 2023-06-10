using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossAbilityDisplayer : MonoBehaviour
{
    private GameObject abilityInfo;
    private Image iconImage;
    public int abilityID;

    private bool mouseIsFollowing = false;

    private RectTransform _rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        abilityInfo = transform.Find("Info").gameObject;
        //abilityInfo.transform.SetParent(transform.parent);
        iconImage = GetComponent<Image>();
        BattleStageManager.Instance.OnFieldAbilityAdd += CheckAbilityActive;
        BattleStageManager.Instance.OnFieldAbilityRemove += CheckAbilityInactive;
        _rectTransform = abilityInfo.GetComponent<RectTransform>();
    }

    void OnDestory()
    {
        BattleStageManager.Instance.OnFieldAbilityAdd -= CheckAbilityActive;
        BattleStageManager.Instance.OnFieldAbilityRemove -= CheckAbilityInactive;
    }

    private void CheckAbilityActive(int id)
    {
        print("enter_start");
        if (id == abilityID)
        {
            print("enter_in");
            SetIconActive(true);
        }
    }
    
    private void CheckAbilityInactive(int id)
    {
        print("enter_start");
        print(id+"/"+abilityID);
        if (id == this.abilityID)
        {
            print("enter_in");
            SetIconActive(false);
        }
    }

    private void OnDestroy()
    {
        BattleStageManager.Instance.OnFieldAbilityAdd -= CheckAbilityActive;
        BattleStageManager.Instance.OnFieldAbilityRemove -= CheckAbilityInactive;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseIsFollowing)
        {
            //_rectTransform.anchoredPosition
            
            //abilityInfo.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _rectTransform.anchoredPosition= Camera.main.ScreenToWorldPoint(Input.mousePosition);

            
        }
        else
        {
            _rectTransform.anchoredPosition= Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //abilityInfo.transform.localPosition = Vector3.zero;
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

    public void SetIconActive(bool flag)
    {
        if (flag)
        {
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.color = Color.gray;
        }
    }
}
