using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIBase : MonoBehaviour
{
    [SerializeField]protected int sid;  //skill id

    protected GameObject skillIcon;

    protected GameObject unableIcon;
    
    protected PlayerStatusManager sm;

    protected float spGaugeCDValue;

    protected Slider cooldownGauge;

    protected GameObject keyHint;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sm = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        cooldownGauge = transform.Find("CD").GetComponent<Slider>();
        skillIcon = transform.Find("IconBody").Find("Mask").GetChild(0).gameObject;
        unableIcon = transform.Find("IconBody").Find("UnableIcon").gameObject;
        keyHint = transform.Find("SkillText").gameObject;

        string skillButton;
        switch (this.sid)
        {
            case 1:
                skillButton = GlobalController.keySkill1.ToUpper();
                break;
            case 2:
                skillButton = GlobalController.keySkill2.ToUpper();
                break;
            case 3:
                skillButton = GlobalController.keySkill3.ToUpper();
                break;
            case 4:
                skillButton = GlobalController.keySkill4.ToUpper();
                break;
            default:
                skillButton = "UNDEFINED";
                break;
        }

        keyHint.GetComponentInChildren<TextMeshProUGUI>().text = skillButton;
    }

    // Update is called once per frame
    void Update()
    {
        //If something happened: swap skill icon to another.
        //SwapSkillIcon();

        CheckSkillCD();

    }

    protected void DisableSkill()
    {
        unableIcon.SetActive(true);
    }
    protected void EnableSkill()
    {
        unableIcon.SetActive(false);
    }


    protected void SwapSkillIcon(int iconID)
    {
        Transform _parent = skillIcon.transform.parent;

        for(int i = 0; i < _parent.childCount;i++)
        {
            if (i == iconID)
            {
                _parent.GetChild(i).gameObject.SetActive(true);
                skillIcon = transform.Find("IconBody").Find("Mask").GetChild(iconID).gameObject;
            }
            else {
                _parent.GetChild(i).gameObject.SetActive(false);
            }
            
        }
    }


    protected virtual void CheckSkillCD()
    {
        


        spGaugeCDValue = 1.0f - (sm.currentSP[sid-1] / sm.requiredSP[sid-1]);
        if (spGaugeCDValue < 0)
        {
            spGaugeCDValue = 0;
        }
        else if (spGaugeCDValue > 1)
        {
            spGaugeCDValue = 1;
        }
        cooldownGauge.value = spGaugeCDValue;



        if (spGaugeCDValue > 0)
        {
            skillIcon.transform.GetChild(1).gameObject.SetActive(true);
            keyHint.SetActive(false);
        }
        else
        {
            skillIcon.transform.GetChild(1).gameObject.SetActive(false);
            keyHint.SetActive(true);
        }
    }




}
