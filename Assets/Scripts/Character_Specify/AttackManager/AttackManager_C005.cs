using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;

public class AttackManager_C005 : AttackManagerDagger
{

    public GameObject rollAttackFX;
    public GameObject combo1MuzzleFX;
    public GameObject combo4FX2;
    public GameObject combo6FX;
    public GameObject combo7FX1;
    public GameObject combo7FX2;
    public GameObject skill1BoostFX;
    public GameObject skill2BoostFX;
    public GameObject skill3ShadowFX;
    public GameObject skill3MuzzleFX;

    protected GameObject combo4Container;
    protected GameObject skill1BoostContainer;
    
    private bool s3Boosted = false;


    protected override void Awake()
    {
        base.Awake();
        s3Boosted = UI_AdventurerSelectionMenu.CheckSkillUpgradable(5, 2) == 2;
    }

    public void RollAttack()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(rollAttackFX,transform.position,
            container,ac.facedir);
    }

    public void Combo1_Attack()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var muzzle = InstantiateMeele(combo1MuzzleFX,transform.position,
            MeeleAttackFXLayer);
        var proj = InstantiateRanged(combo1FX,transform.position,
            container, ac.facedir);
        proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
    }

    public void Combo2_Attack()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
           MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(combo2FX,transform.position,
            container);
        
        
        
    }
    public void Combo3_Attack()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(combo3FX,transform.position,
            container);

    }

    public void Combo4_Attack1()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        combo4Container = container;
        combo4Container.GetComponent<AttackContainer>().InitAttackContainer(2,false);
        
        var proj = InstantiateRanged(combo4FX,transform.position+new Vector3(0,1,0),
            container,ac.facedir);
    }

    public void Combo4_Attack2()
    {
        
        var proj = InstantiateRanged(combo4FX2,transform.position+new Vector3(ac.facedir,-2.5f,0),
            combo4Container,ac.facedir);
    }

    public void Combo5_Attack()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(combo5FX,transform.position,
            container,ac.facedir);
    }

    public void Combo6_Attack()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(combo6FX,transform.position,
            container,ac.facedir);
        
        //proj.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
    }

    public void Combo7_Attack1()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(combo7FX1,transform.position,
            container);
    }

    public void Combo7_Attack2()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(combo7FX2,transform.position+new Vector3(ac.facedir,-2.5f,0),
            container,ac.facedir);
    }


    public void Skill1()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var shadow = FindObjectOfType<Projectile_C005_4>();

        if (shadow != null)
        {
            if (shadow.attackStart == false)
            {
                shadow.attackContainer.parentContainer = container.GetComponent<AttackContainer>();
                container.GetComponent<AttackContainer>().InitAttackContainer(2,true);
                var targetTrans = ta.GetNearestTargetInRange(12,
                    0.5f, LayerMask.GetMask("Enemies"));
                var targetObject = targetTrans == null ? null : targetTrans.gameObject;
                
                
                shadow.ReleaseAttack(1,targetObject);
            }
            else
            {
                container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
            }
        }
        else
        {
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
        }
        
        var proj = InstantiateRanged(skill1FX,transform.position,
            container,ac.facedir);

        var atk = proj.GetComponent<AttackFromPlayer>();

        var vunlerableDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable, 15, 30,
            1,100503);
        var evilsbaneDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
            -1, 30,
            1);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.DefBuff),20,30,1,
            100503);
        
        atk.AddWithConditionAll(vunlerableDebuff,100);
        atk.AddWithConditionAll(evilsbaneDebuff,100,1);
        
        
        //proj.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
        var trail = proj.GetComponentInChildren<TrailRenderer>().gameObject;
        
        trail.transform.SetParent(MeeleAttackFXLayer.transform);
    }

    public void Skill2()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            MeeleAttackFXLayer.transform);

        var shadow = FindObjectOfType<Projectile_C005_4>();

        if (shadow != null)
        {
            if (shadow.attackStart == false)
            {
                shadow.attackContainer.parentContainer = container.GetComponent<AttackContainer>();
                container.GetComponent<AttackContainer>().InitAttackContainer(2,true);
                shadow.ReleaseAttack(2,null);
            }
            else
            {
                container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
            }
        }
        else
        {
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
        }



        var proj = InstantiateMeele(skill2FX,transform.position,
            container);
        var burnDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72.7f, 12,
            100);
        var atk = proj.GetComponent<AttackFromPlayer>();
        atk.AddWithConditionAll(burnDebuff,110);
        atk.AddWithConditionAll(new TimerBuff(999),100,1);
    }

    public void Skill3(int eventID)
    {
        switch (eventID)
        {
            //产生影子
            case 0:
            {

                //Instantiate(skill3MuzzleFX,transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
                var shadow = Instantiate(skill3ShadowFX,
                    transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
                if (ac.facedir == -1)
                {
                    shadow.transform.localScale = new Vector3(-1,1,1);
                }

                var shadowController = shadow.GetComponent<Projectile_C005_4>();
                shadowController.playerSource = gameObject;
                shadowController.s1FX = skill1FX;
                shadowController.s2FX = skill2FX;
                
                if (s3Boosted)
                {
                    TimerBuff critBuff = Random.Range(0, 2) == 0
                        ? new TimerBuff((int)(BasicCalculation.BattleCondition.CritRateBuff),
                            50, 3, 100, -1)
                        : new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
                            80, 3, 100, -1);
                    _statusManager.ObtainTimerBuff(critBuff);
                    shadowController.modifier = 0.9f;
                }
                
                
                
                
                
                break;
            }
            //攻击
            case 1:
            {
                var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
                    MeeleAttackFXLayer.transform);
                container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
                var proj = InstantiateMeele(skill3FX,transform.position,
                    container);
                break;
            }

        }
        
        
    }


    public void Skill5(int eventID)
    {
        if (eventID == 0)
        {
            var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
                RangedAttackFXLayer.transform);
        
            container.GetComponent<AttackContainer>().InitAttackContainer(2,true);
            skill1BoostContainer = container;
            
            _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.DefBuff),20,30,1,
                100503);
        }
        



        var proj = InstantiateRanged(skill1BoostFX,transform.position,
            skill1BoostContainer,ac.facedir);

        var atk = proj.GetComponent<AttackFromPlayer>();

        var vunlerableDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Vulnerable, 15, 30,
            1,100503);
        var evilsbaneDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.EvilsBane,
            -1, 30,
            1);
        
        atk.AddWithConditionAll(vunlerableDebuff,100);
        atk.AddWithConditionAll(evilsbaneDebuff,100,1);
        
        
        //proj.GetComponentInChildren<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
        var trail = proj.GetComponentInChildren<TrailRenderer>().gameObject;
        
        trail.transform.SetParent(MeeleAttackFXLayer.transform);
    }

    public void Skill6()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
        var proj = InstantiateMeele(skill2BoostFX,transform.position,
            container);
        var scorchDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            31.1f, 21,
            100);
        var atk = proj.GetComponent<AttackFromPlayer>();
        atk.AddWithConditionAll(scorchDebuff,110);
        atk.AddWithConditionAll(new TimerBuff(999),100,1);
    }



}
