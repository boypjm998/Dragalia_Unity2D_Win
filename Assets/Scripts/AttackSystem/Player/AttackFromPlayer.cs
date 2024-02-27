using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics;
public class AttackFromPlayer : AttackBase
{
    public bool energized = false;
    public bool inspired = false;
    public GameObject self;
    public ActorBase ac;
    public int attackId { get; protected set; }

    [Header("Damage Basic Attributes")] 

    
    
    [SerializeField] protected int spGain;
    [SerializeField] protected bool isMeele;


    //public List<BasicCalculation.BasicAttackInfo> nextAttackSet;
    
    // public List<int> withConditionChance;
    // public List<int> withConditionNum; //一次上几个debuff？
    //
    // public List<int> withConditionFlags; //遍历敌人做一个数组，每个敌人代表一个condflag
    public List<int> hitFlags = new(); //遍历敌人做一个数组，每个敌人代表一个hitflag


    public GameObject hitConnectEffect;
    public Collider2D attackCollider;
    public Transform playerpos;
    public bool forcedShake = false;
    public float defaultGravity;

    
    public float hitShakeIntensity;
    [Range(0,1)]public float hitSoundVolume = 1;
    protected BattleStageManager battleStageManager;
    private Coroutine ConnectCoroutine;

    

    //public List<BattleCondition> withConditions { get; protected set; }
    
    public float damageAutoReset = 0; //自动刷新
    

    protected virtual void Awake()
    {
        _effectManager = BattleEffectManager.Instance;
        
        hitFlags = SearchEnemyList();
        BattleStageManager.Instance.OnEnemyAwake += AddFlag;
        BattleStageManager.Instance.OnAttackAwake?.Invoke(this);
        //withConditionFlags = SearchEnemyList();
        if(playerpos==null)
            playerpos = GameObject.Find("PlayerHandle").transform;
        defaultGravity = playerpos.GetComponent<Rigidbody2D>().gravityScale;
    }
    
    


    protected virtual void Start()
    {
        battleStageManager = BattleStageManager.Instance;
        chara_id = battleStageManager.chara_id;

        var stat = playerpos.GetComponent<StatusManager>();
        
        CheckSpecialConditionalEffectBeforeAttack(stat);
        
        if(attackType == BasicCalculation.AttackType.SKILL || attackType == BasicCalculation.AttackType.DSKILL)
            CheckSkillEnergyOrInspriationLevel(stat);

    }

    protected virtual void OnDestroy()
    {
        var container = gameObject.GetComponentInParent<AttackContainer>();

        container?.FinishHit();
        
        BattleStageManager.Instance.OnEnemyAwake -= AddFlag;
    }

    public void ResetFlags()
    {
        hitFlags.Clear();
        
        //hitFlags.AddRange(BattleStageManager.Instance.EnemyList);

        var enemyLayer = BattleStageManager.Instance.EnemyLayer;
        for (var i = 0; i < enemyLayer.transform.childCount; i++)
            hitFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());
    }

    public override void ResetWithConditionFlags()
    {
        //withConditionFlags.Clear();

        //var enemyLayer = GameObject.Find("EnemyLayer");
        //for (var i = 0; i < enemyLayer.transform.childCount; i++)
        //    withConditionFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());

        var container = GetComponentInParent<AttackContainer>();
        container.conditionCheckDone.Clear();
        container.checkedConditions.Clear();
    }

    public virtual IEnumerator MeeleTimeStop(float time) //近战的卡肉
    {
        var animAttack = GetComponentInParent<Animator>();
        var rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        var anim = playerpos.gameObject.GetComponentInParent<Animator>();
        
        if(anim==null)
            anim = playerpos.gameObject.GetComponentInChildren<Animator>();
        
        //Debug.Log(parent);
        animAttack.speed = 0.5f;
        anim.speed = 0;
        var gravity = rigid.gravityScale;
        //print(gravity);
        rigid.gravityScale = 0;
        yield return new WaitForSeconds(time);

        RecoverFromMeeleTimeStop(DEFAULT_GRAVITY);
    }

    public virtual void RecoverFromMeeleTimeStop(float gravity)
    {
        var animAttack = GetComponentInParent<Animator>();
        if(animAttack == null)
            return;
        var rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim;
        try
        {
            anim = playerpos.gameObject.GetComponentInParent<ActorController>().anim;
        }
        catch
        {
           return;
        }
        

        
        
        
        rigid.gravityScale = gravity;
        //Debug.Log(rigid.gravityScale);
        anim.speed = 1;
        if(animAttack!=null)
            animAttack.speed = 1;
        //animAttack.speed = 1;
        ConnectCoroutine = null;
    }

    public virtual void InitAttackBasicAttributes(float knockbackPower, float knockbackForce, float knockbackTime,
        float dmgModifier, int spGain, int firedir)
    {
        //multi-hit attacks can be trigger multiple damage.
        //this.knockbackPower = knockbackPower;
        //this.knockbackForce = knockbackForce;
        //this.dmgModifier = new float[1];
        //this.dmgModifier[0] = dmgModifier;
        //this.knockbackTime = knockbackTime;
        //this.spGain = spGain;
        this.firedir = firedir;
        
        //其他地方调试好之后把上面的注释掉
        this.attackInfo[0].knockbackForce = knockbackForce;
        this.attackInfo[0].knockbackPower = knockbackPower;
        this.attackInfo[0].knockbackTime = knockbackTime;
        this.attackInfo[0].dmgModifier.Add(dmgModifier);
        this.attackInfo[0].firedir = firedir;


    }

    public virtual void InitAttackBasicAttributes(float knockbackPower, float knockbackForce, float knockbackTime,
        float[] dmgModifier, int spGain, int firedir)
    {
        // this.knockbackPower = knockbackPower;
        // this.knockbackForce = knockbackForce;
        // this.dmgModifier = dmgModifier;
        // this.knockbackTime = knockbackTime;
        this.spGain = spGain;
        this.firedir = firedir;
    }


    public virtual int GetFaceDir()
    {
        return firedir;
    }

    public virtual void PlayDestroyEffect(float shakeIntensity)
    {
        CineMachineOperator.Instance.CamaraShake(shakeIntensity, .1f);
        if (hitConnectEffect == null)
            return;
        var eff = Instantiate(hitConnectEffect, transform.position, Quaternion.identity);
        eff.name = "HitEffect0";
    }

    public virtual List<int> SearchEnemyList()
    {
        hitFlags.Clear();

        //hitFlags.AddRange(BattleStageManager.Instance.EnemyList);

        var enemyLayer = BattleStageManager.Instance.EnemyLayer;
        for (var i = 0; i < enemyLayer.transform.childCount; i++)
            if (enemyLayer.transform.GetChild(i).gameObject.activeSelf)
                hitFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());
        return hitFlags;
    }

    public virtual void DamageCheckRaycast(RaycastHit2D hitinfo)
    {
        if (hitinfo.collider != null)
            if (hitinfo.collider.CompareTag("Enemy") &&
                hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                CauseDamage(hitinfo.collider);

                Destroy(gameObject);
                
            }
    }

    public virtual void DamageCheckCollider(Collider2D hitinfo)
    {
        if (hitinfo != null)
            if (hitinfo.CompareTag("Enemy") && hitFlags.Contains(hitinfo.transform.parent.GetInstanceID()))
            {
                CauseDamage(hitinfo);

                Destroy(gameObject);

                
            }
    }

    public override Vector2 GetKBDirection(BasicCalculation.KnockBackType knockBackType, GameObject target)
    {
        var kbdirtemp = attackInfo[0].knockbackDirection;
        switch (knockBackType)
        {
            case BasicCalculation.KnockBackType.FaceDirection:
                if(firedir!=0 && ac)
                    firedir = ac.facedir;
                kbdirtemp = new Vector2(firedir * kbdirtemp.x,kbdirtemp.y);
                break;

            case BasicCalculation.KnockBackType.FromCenterRay:
                kbdirtemp = transform.InverseTransformPoint(target.transform.position);
                break;
            case BasicCalculation.KnockBackType.FromCenterFixed:
                kbdirtemp = transform.position.x > target.transform.position.x
                    ? new Vector2(-attackInfo[0].knockbackDirection.x, attackInfo[0].knockbackDirection.y)
                    : attackInfo[0].knockbackDirection;
                break;
            case BasicCalculation.KnockBackType.None:
                kbdirtemp = Vector2.zero;
                break;
        }

        return kbdirtemp;
    }


    public float GetDmgModifier()
    {
        return 0;
    }
    

    public float GetSpGain()
    {
        return spGain;
    }
    
    public float SetSpGain(int spGain)
    {
        return this.spGain = spGain;
    }


    public void AppendAttackSets(float knockbackPower, float knockbackForce, float knockbackTime, float dmgModifier)
    {
        //single Attack


        // nextKnockbackForce.Add(knockbackForce);
        // nextKnockbackPower.Add(knockbackPower);
        // nextKnockbackTime.Add(knockbackTime);
        // nextDmgModifier.Add(dmgModifier);
    }

    protected void NextWithCondition()
    {
        // if (withConditions.Count > withConditionNum[0])
        //     for (var i = 0; i < withConditionNum[0]; i++)
        //         withConditions.RemoveAt(0);
        //
        // if (withConditionChance.Count > withConditionNum[0])
        //     for (var i = 0; i < withConditionNum[0]; i++)
        //         withConditionChance.RemoveAt(0);
        //
        // if (withConditionNum.Count > 1) withConditionNum.RemoveAt(0);

        ResetWithConditionFlags();
    }

    public override void NextAttack()
    {

        if (attackInfo.Count > 1)
        {
            attackInfo.RemoveAt(0);
        }

        attackId++;
        ResetFlags();
        //NextWithCondition();
    }


    /// <summary>
    ///     <para>target:碰撞体所在的游戏对象</para>
    /// </summary>
    public virtual void CauseDamage(Collider2D collision)
    {
        hitFlags.Remove(collision.gameObject.transform.parent.GetInstanceID());
        //withConditionFlags.Remove(target.transform.parent.GetInstanceID());
        
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;

        int attackSource = 0;
        if (playerpos.GetComponent<PlayerStatusManager>()==null)
        {
            attackSource = 2;
        }

        //敌人的闪避
        try
        {
            var enemyController = collision.gameObject.transform.parent.GetComponent<EnemyController>();
            if (enemyController is EnemyControllerHumanoid)
            {
                var dodge = (enemyController as EnemyControllerHumanoid).dodging;
                if (dodge && attackType != BasicCalculation.AttackType.SKILL && attackType != BasicCalculation.AttackType.FORCE
                    && attackType != BasicCalculation.AttackType.DSKILL)
                {
                    DamageNumberManager.GenerateDodgeText(collision.gameObject.transform);
                    enemyController.OnDodgeSuccess?.Invoke(this,playerpos.gameObject);
                    return;
                }
            }
        }
        catch
        {
            //Debug.LogWarning("No EnemyControllerHumanoid");
        }
        
        //BeforeAttackHit?.Invoke(this,collision.gameObject.transform.parent.gameObject);


        var dmg = battleStageManager.CalculateHit(collision.gameObject.transform.parent.gameObject, playerpos.gameObject,this,attackSource);



        if (hitConnectEffect != null)
        {
            Instantiate(hitConnectEffect,
                collision.ClosestPoint(collision.transform.position),
                Quaternion.identity);
        }

        

        if(attackSource == 0 || forcedShake)
            CineMachineOperator.Instance.CamaraShake(hitShakeIntensity, .1f);
        
        //CineMachineOperator.Instance.CamaraShake(hitShakeIntensity, .1f);

        try
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                // 在摄像机范围内
                _effectManager.PlayHitSoundEffect(hitSoundEffect,hitSoundVolume);
            }
        }
        catch
        {
            _effectManager.PlayHitSoundEffect(hitSoundEffect,hitSoundVolume);
        }
        
        //print("PlaySound!");



        var container = gameObject.GetComponentInParent<AttackContainer>();
        container.AttackOneHit();
        OnAttackHit?.Invoke(this,collision.transform.parent.gameObject);
        
        if (container.NeedTotalDisplay() && dmg > 0)
            container.AddTotalDamage(dmg);
        
        if(destroyAfterHit)
            Destroy(gameObject);
        
        if(damageAutoReset>0)
            Invoke("NextAttack",damageAutoReset);
    }

    public override void AddWithCondition(int hitNo, BattleCondition condition, int chance, int identifier = 0)
    {
        var conditionInfo = new AttackInfo.ConditionWithAttackInfo();
        conditionInfo.condition = condition;
        conditionInfo.withConditionChance = chance;
        conditionInfo.identifier = identifier;
        for(int i = 0; i < attackInfo.Count; i++)
        {
            if (i == hitNo)
            {
                attackInfo[i].withConditions.Add(conditionInfo);
            }
        }
    }
    
    public override void AddWithConditionAll(BattleCondition condition, int chance, int identifier = 0)
    {
        var conditionInfo = new AttackInfo.ConditionWithAttackInfo();
        conditionInfo.condition = condition;
        conditionInfo.withConditionChance = chance;
        conditionInfo.identifier = identifier;
        foreach (var attack in attackInfo)
        {
            attack.withConditions.Add(conditionInfo);
        }
    }

    

    public void SetWithConditionNum(int[] series)
    {
        //withConditionNum.Clear();

        //foreach (var num in series) withConditionNum.Add(num);
    }
    
    public void ParticleCallback(GameObject other)
    {
        OnTriggerStay2D(other.GetComponentInChildren<Collider2D>());
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        print("无事发生");
    }
    
    protected void AddFlag(int id)
    {
        if (!hitFlags.Contains(id))
            hitFlags.Add(id);
    }

    public static void CheckEnergyLevel(StatusManager statusManager,bool attackSkill = false)
    {

        if(!statusManager.Energized && statusManager.
               GetConditionTotalValue((int)BasicCalculation.BattleCondition.Energy)>=5)
        {
            
            statusManager.Energized = true;
            
        }
        
    }
    
    public static void CheckInspirationLevel(StatusManager statusManager,bool attackSkill = false)
    {

        if(!statusManager.Inspired && statusManager.
               GetConditionTotalValue((int)BasicCalculation.BattleCondition.Inspiration)>=5)
        {
            
            statusManager.Inspired = true;
            
        }
        
    }
    
    public void CheckSkillEnergyOrInspriationLevel(StatusManager statusManager)
    {
        var attackContainer = GetComponentInParent<AttackContainer>();

        if(statusManager.
               GetConditionTotalValue((int)BasicCalculation.BattleCondition.Energy)>=5
           )
        {
            statusManager.Energized = true;
            energized = true;
            attackContainer.energized = true;
            
        }else if (attackContainer.energized)
        {
            energized = true;
        }

        if(statusManager.
               GetConditionTotalValue((int)BasicCalculation.BattleCondition.Inspiration)>=5)
        {
            statusManager.Inspired = true;
            inspired = true;
            attackContainer.inspired = true;
            
        }else if (attackContainer.inspired)
        {
            inspired = true;
            Debug.Log("CheckedInspirationLevel");
        }
    }
    



}