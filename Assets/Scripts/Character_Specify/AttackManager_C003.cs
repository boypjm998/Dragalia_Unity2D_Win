using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;


public class AttackManager_C003 : AttackManagerRanged
{
    protected GameObject skill3Container;
    
    
    public void TwilightMoon()
    {
        //TODO:check twilight moon
        
        
        
        

        var fsPrefab = FindObjectOfType<Projectile_C003_6_PL>();

        if (fsPrefab != null)
        {
            Destroy(fsPrefab.gameObject);
        }
        
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var targetTransform = ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 8f,
            LayerMask.GetMask("Enemies"));
        
        if(targetTransform == null)
            targetTransform = ta.GetNearestTargetInRangeDirection(-ac.facedir,20f, 8f,
                LayerMask.GetMask("Enemies"));
        if (targetTransform == null)
            targetTransform = transform;
        
        

        var targetCol = BasicCalculation.CheckRaycastedPlatform(targetTransform.gameObject);

        var position = new Vector3(targetTransform.position.x, targetCol.bounds.max.y, 0);
        
        
        var fx = Instantiate(ForceFX[0], position, Quaternion.identity,container.transform);

        fsPrefab = fx.GetComponent<Projectile_C003_6_PL>();
        fsPrefab.ta = ta;
        fsPrefab.ac = ac as ActorController_c003;
        fsPrefab.target = targetTransform.gameObject;

    }

    public void TwilightMoonRelease(int forceLevel = 0)
    {
        if(forceLevel<=0)
            return;
        
        
        
        (ac as ActorController_c003).PlayAttackVoice(4);
        
        var acZena = ac as ActorController_c003;
        
        
        
        acZena.auspexGauge = 0;
        
        
        _statusManager.RemoveConditionWithLog(
            _statusManager.GetConditionsOfType((int)
                (BasicCalculation.BattleCondition.TwilightMoon))[0]);
        
        
        var pos = acZena.forceAttackPosition;
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        var proj = InstantiateRanged(ForceFX[1], pos, container, 1);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(1,true);

        var attack = proj.GetComponent<AttackFromPlayer>();

        switch (forceLevel)
        {
            case 2:
            {
                attack.attackInfo[0].dmgModifier[0] = 5;
                attack.attackInfo[0].knockbackPower = 135;
                attack.SetSpGain(240);
                break;
            }
            case 3:
            {
                attack.attackInfo[0].dmgModifier[0] = 6;
                attack.attackInfo[0].knockbackPower = 150;
                attack.SetSpGain(370);
                break;
            }
            case 4:
            {
                attack.attackInfo[0].dmgModifier[0] = 7;
                attack.attackInfo[0].knockbackPower = 170;
                attack.SetSpGain(490);
                break;
            }
            case 5:
            {
                attack.attackInfo[0].dmgModifier[0] = 10;
                attack.attackInfo[0].knockbackPower = 200;
                attack.SetSpGain(650);
                break;
            }
        }
        


    }

    public override void ComboAttack1()
    {
        var target =
            ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 4f, LayerMask.GetMask("Enemies"),1);
        
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(3,false);
        
        var shineFx = Instantiate(combo1FX[1],transform.position+new Vector3(ac.facedir,0,0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        var fx1 = InstantiateRanged(combo1FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 2).normalized;
        fx1.GetComponent<HomingAttack>().target = target;
        
        
        var fx2 = InstantiateRanged(combo1FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx2.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0);
        fx2.GetComponent<HomingAttack>().target = target;
        
        var fx3 = InstantiateRanged(combo1FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx3.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, -2).normalized;
        fx3.GetComponent<HomingAttack>().target = target;



    }

    public override void ComboAttack2()
    {
        var target =
            ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 4f, LayerMask.GetMask("Enemies"),1);
        
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(3,false);
        
        var shineFx = Instantiate(combo1FX[1],transform.position+new Vector3(ac.facedir,0,0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        var fx1 = InstantiateRanged(combo2FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 2).normalized;
        fx1.GetComponent<HomingAttack>().target = target;
        
        
        var fx2 = InstantiateRanged(combo2FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx2.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0);
        fx2.GetComponent<HomingAttack>().target = target;
        
        var fx3 = InstantiateRanged(combo2FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx3.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, -2).normalized;
        fx3.GetComponent<HomingAttack>().target = target;
    }
    
    public override void ComboAttack3()
    {
        var target =
            ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 4f, LayerMask.GetMask("Enemies"),1);
        
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(4,false);
        
        
        var shineFx = Instantiate(combo1FX[1],transform.position+new Vector3(ac.facedir,0,0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        
        var fx1 = InstantiateRanged(combo3FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 3).normalized;
        fx1.GetComponent<HomingAttack>().target = target;
        
        
        var fx2 = InstantiateRanged(combo3FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx2.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0.3f).normalized;
        fx2.GetComponent<HomingAttack>().target = target;
        
        var fx3 = InstantiateRanged(combo3FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx3.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, -3).normalized;
        fx3.GetComponent<HomingAttack>().target = target;
        
        var fx4 = InstantiateRanged(combo3FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx4.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, -0.3f).normalized;
        fx4.GetComponent<HomingAttack>().target = target;
    }
    
    public override void ComboAttack4()
    {
        var target =
            ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 4f, LayerMask.GetMask("Enemies"),1);
        
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(5,false);
        
        var shineFx = Instantiate(combo1FX[1],transform.position+new Vector3(ac.facedir,0,0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        var fx1 = InstantiateRanged(combo4FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 3).normalized;
        fx1.GetComponent<HomingAttack>().target = target;
        
        
        var fx2 = InstantiateRanged(combo4FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx2.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,1).normalized;
        fx2.GetComponent<HomingAttack>().target = target;
        
        var fx3 = InstantiateRanged(combo4FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx3.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, -1).normalized;
        fx3.GetComponent<HomingAttack>().target = target;
        
        var fx4 = InstantiateRanged(combo4FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx4.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 0).normalized;
        fx4.GetComponent<HomingAttack>().target = target;
        
        var fx5 = InstantiateRanged(combo4FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx5.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0).normalized;
        fx5.GetComponent<HomingAttack>().target = target;
    }
    
    public override void ComboAttack5()
    {
        var target =
            ta.GetNearestTargetInRangeDirection(ac.facedir, 20f, 4f, LayerMask.GetMask("Enemies"),1);
        
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(6,false);
        
        var shineFx = Instantiate(combo1FX[1],transform.position+new Vector3(ac.facedir,0,0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        var fx1 = InstantiateRanged(combo5FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 4).normalized;
        fx1.GetComponent<HomingAttack>().target = target;
        
        
        var fx2 = InstantiateRanged(combo5FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx2.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0.3f).normalized;
        fx2.GetComponent<HomingAttack>().target = target;
        
        var fx3 = InstantiateRanged(combo5FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx3.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 1).normalized;
        fx3.GetComponent<HomingAttack>().target = target;
        
        var fx4 = InstantiateRanged(combo5FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx4.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, -1).normalized;
        fx4.GetComponent<HomingAttack>().target = target;
        
        var fx5 = InstantiateRanged(combo5FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx5.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,-0.3f).normalized;
        fx5.GetComponent<HomingAttack>().target = target;
        
        var fx6 = InstantiateRanged(combo5FX[0],
            transform.position + new Vector3(ac.facedir,0,0),
            container, ac.facedir,0);

        fx6.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,-4).normalized;
        fx6.GetComponent<HomingAttack>().target = target;
    }

    public override void Skill1(int eventID)
    {
        var proj = Instantiate(skill1FX[1], transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var muzzleFX = Instantiate(skill1FX[0], transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);

        var controller = proj.GetComponent<Projectile_C003_1>();
        controller.playerGameObject = gameObject;
        controller.InitPotencyInfo(_statusManager);
        
        //TODO: ADD AUSPEX GAUGE
        
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff),15,60,1,0);
        _statusManager.HPRegenImmediately(130,0,true);
        _statusManager.ObtainHealOverTimeBuff(29,15);
        //_statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HealOverTime),29,15);

    }

    public override void Skill2(int eventID)
    {
        if (eventID == 1)
        {
            var fx = Instantiate(skill2FX[0],
                transform.position + new Vector3(ac.facedir, 2f, 0), Quaternion.identity,
                MeeleAttackFXLayer.transform);
        }else if (eventID == 2)
        {
            var proj = Instantiate(skill2FX[1], transform.position, Quaternion.identity,
                RangedAttackFXLayer.transform);
            if (ac.facedir == -1)
            {
                proj.transform.rotation =
                    Quaternion.Euler(proj.transform.rotation.eulerAngles.x,
                        180, proj.transform.rotation.eulerAngles.z);
                proj.transform.GetComponent<Projectile_C003_3_PL>().firedir = -1;
            }

            for (int i = 0; i < proj.transform.childCount; i++)
            {
                proj.transform.GetChild(i).GetComponent<Projectile_C003_2_PL>().playerSource = gameObject;
            }

            var holyfaithBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyFaith, -1f, 20f, 1);
            _statusManager.ObtainTimerBuff(holyfaithBuff);
            proj.GetComponent<AttackContainer>().InitAttackContainer(10, true);
        }
    }

    public override void Skill3(int eventID)
    {
        if (eventID == 1)
        {
            var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
                MeeleAttackFXLayer.transform);
            var proj = Instantiate(skill3FX[0], transform.position-new Vector3(0,1,0),Quaternion.identity,
                container.transform);
            container.GetComponent<AttackContainer>().InitAttackContainer(2,true);
        
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.RecoveryBuff,20,10);
            
            skill3Container = container;
        }else if (eventID == 2)
        {
            var proj = InstantiateMeele(skill3FX[1], transform.position,
                skill3Container);
        }
    }

    public override void Skill4(int eventID)
    {
        var healFXNum = _statusManager.GetConditionStackNumber((int)
            BasicCalculation.BattleCondition.HealOverTime);
        
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
           RangedAttackFXLayer.transform);
        
        
        var targetTransform = ta.GetNearestTargetInRangeDirection(ac.facedir, 25f, 10f,
            LayerMask.GetMask("Enemies"));
        
        if(targetTransform == null)
            targetTransform = ta.GetNearestTargetInRangeDirection(-ac.facedir,20f, 10f,
                LayerMask.GetMask("Enemies"));
        if (targetTransform == null)
            targetTransform = transform;
        
        var targetCol = BasicCalculation.CheckRaycastedPlatform(targetTransform.gameObject);

        var position = new Vector3(targetTransform.position.x, targetCol.bounds.max.y+2, 0);


        if (healFXNum < 3)
        {
            var fx = Instantiate(skill4FX[0], position, Quaternion.identity,container.transform);

            var atk = fx.GetComponent<AttackFromPlayer>();
            
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);

            if (healFXNum == 1)
            {
                foreach (var atkInfo in atk.attackInfo)
                {
                    atkInfo.dmgModifier[0] *= 1.1f;
                }
            }else if (healFXNum == 2)
            {
                foreach (var atkInfo in atk.attackInfo)
                {
                    atkInfo.dmgModifier[0] *= 1.2f;
                }
            }
        }
        else
        {
            //神圣祷告信念
            var fx = Instantiate(skill4FX[1], position, Quaternion.identity,container.transform);

            var atk = fx.GetComponent<AttackFromPlayer>();
            
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
            
        }






    }
}
