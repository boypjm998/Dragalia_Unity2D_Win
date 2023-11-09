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

    private Vector3 offset = Vector2.zero;
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

    

    private void CheckAbilityActive(int id)
    {
        //print("enter_start");
        if (id == abilityID)
        {
            print("enter_in");
            SetIconActive(true);
        }
    }
    
    private void CheckAbilityInactive(int id)
    {
        //print("enter_start");
        //print(id+"/"+abilityID);
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
            var screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            //_rectTransform.anchoredPosition= screenPoint;
            UpdateInfoPanelPosition();

            
        }
        else
        {
            _rectTransform.anchoredPosition= Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = Vector3.zero;
        }
    }

    private void UpdateInfoPanelPosition()
    {
        var screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 anchoredPosition = screenPoint;

        Vector2 minPosition = new Vector2(75, 50);
        //print(_rectTransform.sizeDelta);
        Vector2 maxPosition = new Vector2(Screen.width, Screen.height) / 2;
        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, minPosition.x, maxPosition.x);
        
        _rectTransform.anchoredPosition = anchoredPosition;
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
