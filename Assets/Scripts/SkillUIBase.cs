using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIBase : MonoBehaviour
{
    [SerializeField]protected int sid;  //skill id

    protected GameObject skillIcon;

    protected GameObject unableIcon;
    
    protected StatusManager sm;

    protected float spGaugeCDValue;

    protected Slider cooldownGauge;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sm = GameObject.Find("PlayerHandle").GetComponent<StatusManager>();
        cooldownGauge = transform.Find("CD").GetComponent<Slider>();
        skillIcon = transform.Find("IconBody").Find("Mask").GetChild(0).gameObject;
        unableIcon = transform.Find("IconBody").Find("UnableIcon").gameObject;
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
        }
        else
        {
            skillIcon.transform.GetChild(1).gameObject.SetActive(false);
        }
    }




}
