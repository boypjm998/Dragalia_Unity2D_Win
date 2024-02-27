using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class DragonAttackManager_D010 : AttackManager
{
    public List<GameObject> projectileList = new();
    public VoiceController_C010 _voiceController;
    public DragonControllerSpecial_D010 dc;
    private AttackContainer skill2Container;

    private bool skill3Boosted = false;
    

    protected override void Awake()
    {
        _statusManager = GetComponentInParent<PlayerStatusManager>();
        ac = _statusManager.GetComponent<ActorBase>();
        MeeleAttackFXLayer = ac.transform.Find("MeeleAttackFX").gameObject;
        BuffFXLayer = ac.transform.Find("BuffLayer").gameObject;
        dc = GetComponent<DragonControllerSpecial_D010>();
        skill3Boosted = UI_AdventurerSelectionMenu.CheckSkillUpgradable(10, 3) == 2;
    }

    protected override void Start()
    {
        base.Start();
        _voiceController = ac.GetComponentInChildren<VoiceController_C010>();
    }

    protected override GameObject InstantiateRanged(GameObject prefab, Vector3 position, GameObject container, int facedir,
        int rotateMode = 1)
    {
        var prefabInstance = InstantiateDirectional(prefab, position, container.transform, facedir, 0, rotateMode);
        prefabInstance.GetComponent<AttackFromPlayer>().playerpos = ac.transform;
        prefabInstance.GetComponent<AttackFromPlayer>().firedir = facedir;
        return prefabInstance;
    }


    protected void Combo1(int eventID)
    {
        _voiceController.PlayAttackVoice(4+Random.Range(0,2));
        
        var container = Instantiate(attackContainer,
            ac.transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        var projSlash = InstantiateRanged(projectileList[0],
            ac.transform.position+new Vector3(0,2,0), container, ac.facedir);
        
        var projFlying = InstantiateRanged(projectileList[1],
            ac.transform.position+new Vector3(ac.facedir,2), container, ac.facedir);

        if (ac.facedir == -1)
        {
            projFlying.GetComponentInChildren<DOTweenSimpleController>().moveDirection *= -1;
        }

    }

    

    protected void Combo2(int eventID)
    {
        _voiceController.PlayAttackVoice(4+Random.Range(0,2));
        
        var container = Instantiate(attackContainer,
            ac.transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        var projSlash = InstantiateRanged(projectileList[2],
            ac.transform.position+new Vector3(0,2,0), container, ac.facedir);
        
        var projFlying = InstantiateRanged(projectileList[3],
            ac.transform.position+new Vector3(ac.facedir,2), container, ac.facedir);
        
        var projFlying2 = InstantiateRanged(projectileList[4],
            ac.transform.position+new Vector3(ac.facedir,2), container, ac.facedir);
        
        projSlash.GetComponent<RelativePositionRetainer>().SetParent(ac.transform);
        
        if (ac.facedir == -1)
        {
            projFlying.GetComponentInChildren<DOTweenSimpleController>().moveDirection *= -1;
            projFlying2.GetComponentInChildren<DOTweenSimpleController>().moveDirection *= -1;
        }
    }
    protected void Combo3()
    {
        _voiceController.PlayAttackVoice(6);

        var container = Instantiate(projectileList[5], ac.transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        if(ac.facedir == -1)
        {
            container.transform.localScale = new Vector3(-1,1,1);
        }

        for(int i = 0; i < container.transform.childCount; i++)
        {
            try
            {
                container.transform.GetChild(i).GetComponent<AttackFromPlayer>().playerpos = ac.transform;
                container.transform.GetChild(i).GetComponent<AttackFromPlayer>().firedir = ac.facedir;
            }
            catch
            { 
                continue;
            }
        }

    }

    protected void Skill1_Smash()
    {
        var landPos = ac.gameObject.RaycastedPosition();
        var desendDistance = ac.transform.position.y - landPos.y - 1.3f;
        desendDistance = Mathf.Clamp(desendDistance, 0, 10);
        print(desendDistance);

        var tween = ac.transform.DOMoveY(ac.transform.position.y - desendDistance,
            0.15f).SetEase(Ease.OutSine);

    }

    protected void Skill1(int eventID)
    {
        
        if (skill3Boosted)
        {
            (ac as ActorController)._statusManager.FillSP(3,10);
        }
        
        var container = Instantiate(attackContainer,
            ac.transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        var projSkill = InstantiateRanged(projectileList[6],
            ac.transform.position+new Vector3(3*ac.facedir,-1.25f,0),
            container,1);

        var stormlashEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Stormlash,
            41, 21,100);
        
        var atk = projSkill.GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(stormlashEffect,120);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(1,true);

        if(_statusManager.comboHitCount >= 20)
            atk.OnAttackHit += SkillRegenDP;
        
        var conditional_eff = new ConditionalAttackEffect
        (ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {"1", ((int)BasicCalculation.BattleCondition.Stormlash).ToString()},
            new string[] {"0.3"});
        
        atk.AddConditionalAttackEffect(conditional_eff);
            
           
    }

    protected void Skill2(int eventID)
    {
        if (eventID == 1)
        {
            if (skill3Boosted)
            {
                (ac as ActorController_c010)._statusManager.FillSP(3,10);
            }
            
            var container = Instantiate(attackContainer, ac.transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
            
            skill2Container = container.GetComponent<AttackContainer>();
            
            var projSkill = InstantiateMeele(projectileList[7],
                ac.transform.position+new Vector3(0,2,0),container,ac.transform);
            
            skill2Container.GetComponent<AttackContainer>().InitAttackContainer(2,true);

            dc.StartCoroutine(dc.HorizontalMove(8f, 0.2f, "s2"));
            
            if(_statusManager.comboHitCount >= 20)
                projSkill.GetComponent<AttackFromPlayer>().OnAttackHit += SkillRegenDP;
            
            var conditional_eff = new ConditionalAttackEffect
            (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", ((int)BasicCalculation.BattleCondition.Stormlash).ToString()},
                new string[] {"0.5"});
            
            projSkill.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(conditional_eff);


        }else if (eventID == 2)
        {
            var projSkill = Instantiate(projectileList[8],
                ac.transform.position+new Vector3(ac.facedir*1.25f,3),
                Quaternion.identity, MeeleAttackFXLayer.transform);
        }else if (eventID == 3)
        {
            var container = Instantiate(BattleStageManager.Instance.attackSubContainer,
                ac.transform.position, Quaternion.identity,
                RangedAttackFXLayer.transform);
            
            var projSkill = 
                InstantiateRanged(projectileList[9],ac.transform.position+new Vector3(2*ac.facedir,2,0),
                container,ac.facedir);

            var subContainer = container.GetComponent<AttackSubContainer>();
            subContainer.InitAttackContainer(1,true);
            subContainer.InitAttackContainer(1,skill2Container.gameObject);
            dc.SetWeaponVisibility(false);
            
            if(ac.facedir == -1)
            {
                projSkill.GetComponent<DOTweenSimpleController>().moveDirection *= -1;
            }
            
            
            
        }
    }
    private void SkillRegenDP(AttackBase attackBase, GameObject go)
    {
        if (attackBase.skill_id == 1)
        {
            (_statusManager as PlayerStatusManager).ChargeDP(80f/16f);
        }else if(attackBase.skill_id == 2)
        {
            (_statusManager as PlayerStatusManager).ChargeDP(150f/16f);
        }
        attackBase.OnAttackHit -= SkillRegenDP;
    }

}
