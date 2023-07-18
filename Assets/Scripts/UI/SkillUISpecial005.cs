using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUISpecial005 : SkillUIBase
{
    private ActorController_c005 ac;
    private Animator UIShineAnim;
    
    protected override void Start()
    {
        base.Start();
        ac = FindObjectOfType<ActorController_c005>();
        UIShineAnim = transform.Find("IconBody/BorderLight").GetComponent<Animator>();
    }
    
    void Update()
    {
        CheckSkillCD();
        CheckSkillBoosted();
    }
    
    protected void CheckSkillBoosted()
    {
        
        UIShineAnim.SetBool("active",ac.skillBoosted);
        SwapSkillIcon(ac.skillBoosted?1:0);
        
    }

    protected override void SwapSkillIcon(int iconID)
    {
        Transform _parent = skillIcon.transform.parent;

        for(int i = 0; i < _parent.childCount;i++)
        {
            if (i == iconID)
            {
                _parent.GetChild(i).gameObject.SetActive(true);
                skillIcon = maskTransform.GetChild(iconID).gameObject;
            }
            else {
                _parent.GetChild(i).gameObject.SetActive(false);
            }
            
        }
    }
}
