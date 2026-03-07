using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUISpecial003 : SkillUIBase
{
    private ActorController_c003 ac;
    private PlayerStatusManager _statusManager;
    protected Animator UIShineAnim;

    protected override void Start()
    {
        base.Start();
        ac = FindObjectOfType<ActorController_c003>();
        _statusManager = ac.GetComponent<PlayerStatusManager>();
        UIShineAnim = transform.Find("IconBody/BorderLight").GetComponent<Animator>();
    }

    void Update()
    {
        CheckSkillCD();
        CheckSkillBoosted();
    }

    protected void CheckSkillBoosted()
    {
        
        UIShineAnim.SetBool("active",
            ac.skill4IsBoosted && _statusManager.currentSP[3]>=_statusManager.requiredSP[3]);
        
    }
}
