using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUISpecial012 : SkillUIBase
{
    private ActorController_c012 _actorController;
    private Animator UIShineAnim;
    private List<GameObject> _skillIcons = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        _actorController = BattleStageManager.Instance.GetPlayer().GetComponent<ActorController_c012>();
        UIShineAnim = transform.Find("IconBody/BorderLight").GetComponent<Animator>();
        Transform _parent = skillIcon.transform.parent;

        for(int i = 0; i < _parent.childCount;i++)
        {
            _skillIcons.Add(_parent.GetChild(i).gameObject);
        }
    }

    protected override void CheckSkillCD()
    {
        if (_actorController.currentSP >= SpecialSkillGauge_C012.MaxSPPerLevel)
        {
            spGaugeCDValue = 0;
            if (sid == 1)
            {
                if (_actorController._skill1EffectNext == ActorController_c012.SkillChainState.NormalChain)
                {
                    SwapSkillIcon(0);
                    UIShineAnim.SetBool("active", true);
                }else if (_actorController._skill1EffectNext == ActorController_c012.SkillChainState.DispelChain)
                {
                    UIShineAnim.SetBool("active", true);
                    SwapSkillIcon(1);
                }else if (_actorController._skill1EffectNext == ActorController_c012.SkillChainState.BreakChain)
                {
                    UIShineAnim.SetBool("active", true);
                    SwapSkillIcon(2);
                }
                else
                {
                    SwapSkillIcon(0);
                    UIShineAnim.SetBool("active", false);
                }
            }else if (sid == 2)
            {
                if (_actorController._skill2EffectNext == ActorController_c012.SkillChainState.NormalChain)
                {
                    SwapSkillIcon(0);
                    UIShineAnim.SetBool("active", true);
                }else if (_actorController._skill2EffectNext == ActorController_c012.SkillChainState.DispelChain)
                {
                    UIShineAnim.SetBool("active", true);
                    SwapSkillIcon(1);
                }else if (_actorController._skill2EffectNext == ActorController_c012.SkillChainState.BreakChain)
                {
                    UIShineAnim.SetBool("active", true);
                    SwapSkillIcon(2);
                }
                else
                {
                    SwapSkillIcon(0);
                    UIShineAnim.SetBool("active", false);
                }
            }
        }
        else
        {
            spGaugeCDValue = 1;
            SwapSkillIcon(0);
            UIShineAnim.SetBool("active", false);
        }
        
        cooldownGauge.value = spGaugeCDValue;
        
        
        DisplaySkillCD();
    }

    protected override void SwapSkillIcon(int iconID)
    {
        //Transform _parent = skillIcon.transform.parent;

        for(int i = 0; i < _skillIcons.Count;i++)
        {
            if (i == iconID)
            {
                _skillIcons[i].SetActive(true);
                skillIcon = _skillIcons[i];
                // skillIcon = transform.Find("IconBody").Find("Mask").GetChild(iconID).gameObject;
            }
            else {
                _skillIcons[i].SetActive(false);
            }
            
        }
    }
}
