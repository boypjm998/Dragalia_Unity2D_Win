using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C010 : AttackManager
{
    //private ActorBase ac;
    private TargetAimer ta;
    //StatusManager _statusManager;
    
    [Header("Projectiles")]
    public GameObject combo1Projectile;
    public GameObject combo2AProjectile;
    public GameObject combo2BProjectile;
    public GameObject dashFX;
    
    public GameObject skill1Projectile;
    public GameObject skill2Projectile;


    protected Vector2 tempPosition;
    private bool skill3Boosted = false;
    
    protected override void Awake()
    {
        base.Awake();
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ta = GetComponentInChildren<TargetAimer>();
        ac = GetComponent<ActorBase>();
        _statusManager = GetComponent<StatusManager>();
        skill3Boosted = UI_AdventurerSelectionMenu.CheckSkillUpgradable(10, 3) == 2;
    }
    
    public void ComboAttack1_Rush()
    {
        //向前方4格内Enemies层的敌人发一道射线
        var hit = Physics2D.OverlapArea(transform.position + new Vector3(0, 0.5f, 0),
            transform.position + new Vector3(10, -0.5f, 0), LayerMask.GetMask("Enemies"));
        
        var hitInfo = Physics2D.Raycast(transform.position, Vector2.right * ac.facedir, 10,
            LayerMask.GetMask("Enemies"));
        
        //如果射线击中的敌人不为空且和自身距离小于1.5,return
        float targetPos;
        
        if (hitInfo.collider != null)
        {
            var distance = Mathf.Abs(hitInfo.point.x - transform.position.x);
            if(distance < 1.5f)
                return;
            targetPos = hitInfo.point.x - ac.facedir * 1.5f;
        }else targetPos = transform.position.x + ac.facedir * 4f;
        
        //print(hitInfo.collider.gameObject);

        targetPos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(targetPos,transform.position.y)).x;

        StartCoroutine(ac.HorizontalMoveFixedTime
        (targetPos, 0.15f,
            "combo1",Ease.OutCubic));

    }

    public void ComboAttack1()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            MeeleAttackFXLayer.transform);
        var proj = Instantiate(combo1Projectile, transform.position, Quaternion.identity, container.transform);
        proj.GetComponent<AttackFromPlayer>().playerpos = transform;
        
    }

    public void ComboAttack2()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(combo2AProjectile, transform.position, container);
    }

    public void ComboAttack3()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(combo2BProjectile, transform.GetChild(0).position - new Vector3(0,0,1), container);
    }

    public void Skill1_Disappear()
    {
        
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        var boxSensor = Physics2D.OverlapAreaAll(new Vector2(transform.position.x,transform.position.y + 2f),
            new Vector2(transform.position.x + ac.facedir*11f,transform.position.y - 1f),
            LayerMask.GetMask("Enemies"));

        Transform target = null;
        Vector2 targetPos;
        if (boxSensor.Length != 0)
        {
            targetPos = new Vector2(boxSensor[0].transform.position.x, transform.position.y);
            target = boxSensor[0].transform;
        }
        else
        {
            targetPos = transform.position + new Vector3(ac.facedir * 4f, 0);
        }
        tempPosition = targetPos - new Vector2(ac.facedir * 4f, 0);


        var proj = InstantiateDirectional(skill1Projectile, targetPos, container.transform,ac.facedir,0,1);
        proj.GetComponent<AttackBase>().firedir = ac.facedir;
        proj.GetComponent<AttackFromPlayer>().playerpos = transform;
        proj.GetComponent<AttackFromPlayer>().AddWithConditionAll(new TimerBuff(999),100);
        var projController = proj.GetComponentInChildren<Projectile_C010_1>();
        projController.target = target;
        projController.notteBallPosition = transform.position;
        projController.playerAnim = ac.anim;
        ac.rigid.gravityScale = 0;
        
        if (GetComponent<ActorController>() != null)
        {
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
            if(_statusManager.comboHitCount >= 20)
                proj.GetComponent<AttackFromPlayer>().OnAttackHit += SkillRegenDP;
        }
    }
    
    public virtual void Skill4(int eventID)
    {
        if (skill3Boosted)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                15, 60, 1, 101004);
        }
        _statusManager.HPRegenImmediately(0,10,true);
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
        _statusManager.ObtainHealOverTimeBuff(10,15,true);
        
    }
    
    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX,transform.position,container);
    }

    public void Skill1_Wait()
    {
        ac.DisappearRenderer();
        ac.anim.Play("s1_2");
        ac.anim.speed = 0;
    }
    
    public void Skill1_Replay()
    {
        ac.anim.speed = 1;
    }
    
    public void Skill1_Appear()
    {
        ac.AppearRenderer();
        ac.rigid.gravityScale = ActorBase.DefaultGravity;
        if (tempPosition.x > BattleStageManager.Instance.mapBorderR)
        {
            tempPosition.x = BattleStageManager.Instance.mapBorderR;
        }
        else if (tempPosition.x < BattleStageManager.Instance.mapBorderL)
        {
            tempPosition.x = BattleStageManager.Instance.mapBorderL;
        }

        transform.position = tempPosition;
        ac.anim.speed = 1;
    }

    public void Skill2()
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        var proj = Instantiate(skill2Projectile, transform.position + new Vector3(ac.facedir * 2,0,0), Quaternion.identity, container.transform);
        proj.GetComponent<AttackFromPlayer>().playerpos = transform;
        
        proj.GetComponent<AttackFromPlayer>().AddWithConditionAll(new TimerBuff(999),100);//驱散
        
        proj.GetComponent<AttackBase>().firedir = ac.facedir;
        if (GetComponent<ActorController>() != null)
        {
            proj.GetComponent<AttackFromPlayer>().OnAttackHit += SkillRegenDP;
            if(_statusManager.comboHitCount >= 20)
                container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
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
