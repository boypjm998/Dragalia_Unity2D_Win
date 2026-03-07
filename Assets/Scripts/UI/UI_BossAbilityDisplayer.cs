using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossAbilityDisplayer : MonoBehaviour
{
    private GameObject abilityInfo;
    private Image iconImage;
    public int abilityID;
    public StatusManager stat;

    private bool mouseIsFollowing = false;
    private RectTransform _rectTransform;
    private Vector3 offset = Vector2.zero;

    [SerializeField] private TextMeshProUGUI abilityExtraMessage;
    
    public TextMeshProUGUI AbilityExtraMessage => abilityExtraMessage;
    
    void Start()
    {
        abilityInfo = transform.Find("Info").gameObject;
        //abilityInfo.transform.SetParent(transform.parent);
        iconImage = GetComponent<Image>();
        BattleStageManager.Instance.OnFieldAbilityAdd += CheckAbilityActive;
        BattleStageManager.Instance.OnFieldAbilityRemove += CheckAbilityInactive;
        BattleStageManager.Instance.OnFieldAbilityEvent += DoIconEvent;
        
        _rectTransform = abilityInfo.GetComponent<RectTransform>();
        abilityExtraMessage = GetComponentInChildren<TextMeshProUGUI>();
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
        BattleStageManager.Instance.OnFieldAbilityEvent -= DoIconEvent;
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

    protected void DoIconEvent(int id, StatusManager stat, EnemyAbilityIconEvent @event)
    {
        if (abilityExtraMessage == null)
            return;
        
        
        if (id != this.abilityID)
        {
            return;
        }
        
        if(this.stat!= null && stat != this.stat)
        {
            return;
        }

        switch (@event.Type)
        {
            case EnemyAbilityIconEvent.EventType.DisplayOrHide:
            {
                if (@event.Message == "0")
                {
                    abilityExtraMessage.color = Color.clear;
                }
                else
                {
                    abilityExtraMessage.color = Color.white;
                }
                break;
            }
            case EnemyAbilityIconEvent.EventType.PlusOrMinus:
            {
                try
                {
                    int value = int.Parse(@event.Message);
                    int current = int.Parse(abilityExtraMessage.text);
                    int final = Mathf.Clamp(current + value,0,999);
                    
                    abilityExtraMessage.text = final.ToString();
                }
                catch(Exception e)
                {
                    abilityExtraMessage.text = "0";
                    Debug.LogWarning("Error in parsing message to int");
                    return;
                }
                break;
            }
            case EnemyAbilityIconEvent.EventType.SetNumber:
            {
                int value = int.Parse(@event.Message);
                abilityExtraMessage.text = value.ToString();
                break;
            }
            case EnemyAbilityIconEvent.EventType.SetText:
            {
                abilityExtraMessage.text = @event.Message;
                break;
            }
            case EnemyAbilityIconEvent.EventType.Custom:
            {
                @event.DoAction(this);
                break;
            }
            case EnemyAbilityIconEvent.EventType.IconActive:
            {
                SetIconActive(@event.Message == "0" ? false : true);
                break;
            }
            default:break;
        }
        
        
        
    }
    
    
    
    
}
