using UnityEngine;


public class SkillUISpecial002 : SkillUIBase
{
    private ActorController_c002 ac;
    protected Animator UIShineAnim;

    protected override void Start()
    {
        base.Start();
        ac = FindObjectOfType<ActorController_c002>();
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
        
    }
}
