using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFromPlayer : AttackBase
{
    protected static readonly int DEFAULT_GRAVITY = 4;
    public GameObject self;


    [Header("Damage Basic Attributes")] 

    public float knockbackPower;
    public float knockbackForce;
    public float knockbackTime;
    public Vector2 knockbackDirection = Vector2.right;
    public BasicCalculation.KnockBackType KBType;
    
    [SerializeField] protected int spGain;
    [SerializeField] protected bool isMeele;


    //public List<BasicCalculation.BasicAttackInfo> nextAttackSet;
    
    public List<int> withConditionChance;
    public List<int> withConditionNum; //一次上几个debuff？
    public List<int> withConditionFlags; //遍历敌人做一个数组，每个敌人代表一个condflag
    public List<int> hitFlags; //遍历敌人做一个数组，每个敌人代表一个hitflag


    public GameObject hitConnectEffect;
    public Collider2D attackCollider;
    public Transform playerpos;
    public float defaultGravity;

    
    public float hitShakeIntensity;
    protected BattleStageManager battleStageManager;
    private Coroutine ConnectCoroutine;

    protected int firedir;

    public List<BattleCondition> withConditions { get; protected set; }


    
    
    //public struct ConditionalEffect
    //{
    //    public int RequiredSelfCondID;
    //    public int RequiredTargetCondID;
    //    public BasicCalculation.BattleCondition EffectID;
    //    public float EffectValue;
    //    //
    //}
    //public List<ConditionalEffect> conditionalEffect;

    protected virtual void Awake()
    {
        _effectManager = GameObject.Find("StageManager").GetComponent<BattleEffectManager>();
        nextDmgModifier = new List<float>();
        nextKnockbackForce = new List<float>();
        nextKnockbackPower = new List<float>();
        nextKnockbackTime = new List<float>();
        withConditions = new List<BattleCondition>();
        //withConditionNum = new List<int>();
        hitFlags = SearchEnemyList();
        withConditionFlags = SearchEnemyList();
        playerpos = GameObject.Find("PlayerHandle").transform;
        
    }


    protected virtual void Start()
    {
        battleStageManager = GameObject.Find("StageManager").GetComponent<BattleStageManager>();
        chara_id = battleStageManager.chara_id;
    }

    protected virtual void OnDestroy()
    {
        var container = gameObject.GetComponentInParent<AttackContainer>();

        container?.FinishHit();
        //print(container.hitConnectNum);
    }

    public void ResetFlags()
    {
        hitFlags.Clear();

        var enemyLayer = GameObject.Find("EnemyLayer");
        for (var i = 0; i < enemyLayer.transform.childCount; i++)
            hitFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());
    }

    public void ResetWithConditionFlags()
    {
        withConditionFlags.Clear();

        var enemyLayer = GameObject.Find("EnemyLayer");
        for (var i = 0; i < enemyLayer.transform.childCount; i++)
            withConditionFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());

        var container = GetComponentInParent<AttackContainer>();
        container.conditionCheckDone.Clear();
    }

    public virtual IEnumerator MeeleTimeStop(float time) //近战的卡肉
    {
        var animAttack = GetComponentInParent<Animator>();
        var rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        var anim = playerpos.gameObject.GetComponentInParent<Animator>();
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
        var rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        var anim = playerpos.gameObject.GetComponentInParent<Animator>();
        rigid.gravityScale = gravity;
        //Debug.Log(rigid.gravityScale);
        anim.speed = 1;
        animAttack.speed = 1;
        ConnectCoroutine = null;
    }

    public virtual void InitAttackBasicAttributes(float knockbackPower, float knockbackForce, float knockbackTime,
        float dmgModifier, int spGain, int firedir)
    {
        //multi-hit attacks can be trigger multiple damage.
        this.knockbackPower = knockbackPower;
        this.knockbackForce = knockbackForce;
        this.dmgModifier = new float[1];
        this.dmgModifier[0] = dmgModifier;
        this.knockbackTime = knockbackTime;
        this.spGain = spGain;
        this.firedir = firedir;
    }

    public virtual void InitAttackBasicAttributes(float knockbackPower, float knockbackForce, float knockbackTime,
        float[] dmgModifier, int spGain, int firedir)
    {
        this.knockbackPower = knockbackPower;
        this.knockbackForce = knockbackForce;
        this.dmgModifier = dmgModifier;
        this.knockbackTime = knockbackTime;
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
        hitFlags = new List<int>();

        var enemyLayer = GameObject.Find("EnemyLayer");
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
                CauseDamage(hitinfo.collider.gameObject);

                Destroy(gameObject);
                
            }
    }

    public virtual void DamageCheckCollider(Collider2D hitinfo)
    {
        if (hitinfo != null)
            if (hitinfo.CompareTag("Enemy") && hitFlags.Contains(hitinfo.transform.parent.GetInstanceID()))
            {
                CauseDamage(hitinfo.gameObject);

                Destroy(gameObject);

                
            }
    }

    public Vector2 GetKBDirection(BasicCalculation.KnockBackType knockBackType, GameObject target)
    {
        var kbdirtemp = knockbackDirection;
        switch (KBType)
        {
            case BasicCalculation.KnockBackType.FaceDirection:
                kbdirtemp = firedir * kbdirtemp;
                break;

            case BasicCalculation.KnockBackType.FromCenterRay:
                kbdirtemp = transform.InverseTransformPoint(target.transform.position);
                break;
            case BasicCalculation.KnockBackType.FromCenterFixed:
                kbdirtemp = transform.position.x > target.transform.position.x
                    ? new Vector2(-knockbackDirection.x, knockbackDirection.y)
                    : knockbackDirection;
                break;
            case BasicCalculation.KnockBackType.None:
                kbdirtemp = Vector2.zero;
                break;
        }

        return kbdirtemp;
    }

    public float GetDmgModifier()
    {
        return dmgModifier[0];
    }

    public float GetDmgModifier(int id)
    {
        return dmgModifier[id];
    }

    public int GetHitCount()
    {
        return dmgModifier.Length;
    }

    public float GetSpGain()
    {
        return spGain;
    }


    public void AppendAttackSets(float knockbackPower, float knockbackForce, float knockbackTime, float dmgModifier)
    {
        //single Attack


        nextKnockbackForce.Add(knockbackForce);
        nextKnockbackPower.Add(knockbackPower);
        nextKnockbackTime.Add(knockbackTime);
        nextDmgModifier.Add(dmgModifier);
    }

    protected void NextWithCondition()
    {
        if (withConditions.Count > withConditionNum[0])
            for (var i = 0; i < withConditionNum[0]; i++)
                withConditions.RemoveAt(0);

        if (withConditionChance.Count > withConditionNum[0])
            for (var i = 0; i < withConditionNum[0]; i++)
                withConditionChance.RemoveAt(0);

        if (withConditionNum.Count > 1) withConditionNum.RemoveAt(0);

        ResetWithConditionFlags();
    }

    public void NextAttack()
    {
        //print(nextDmgModifier.Count);
        if (nextKnockbackForce.Count > 0)
        {
            knockbackForce = nextKnockbackForce[0];
            nextKnockbackForce.RemoveAt(0);
        }

        if (nextKnockbackPower.Count > 0)
        {
            knockbackPower = nextKnockbackPower[0];
            nextKnockbackPower.RemoveAt(0);
        }

        if (nextKnockbackTime.Count > 0)
        {
            knockbackTime = nextKnockbackTime[0];
            nextKnockbackTime.RemoveAt(0);
        }

        if (nextDmgModifier.Count > 0)
        {
            dmgModifier = new float[1];
            dmgModifier[0] = nextDmgModifier[0];
            nextDmgModifier.RemoveAt(0);
        }

        ResetFlags();
        //NextWithCondition();
    }


    /// <summary>
    ///     <para>target:碰撞体所在的游戏对象</para>
    /// </summary>
    public virtual void CauseDamage(GameObject target)
    {
        hitFlags.Remove(target.transform.parent.GetInstanceID());
        withConditionFlags.Remove(target.transform.parent.GetInstanceID());

        var dmg = battleStageManager.PlayerHit(target.transform.parent.gameObject, this);


        if (hitConnectEffect != null)
            Instantiate(hitConnectEffect, new Vector2(target.transform.position.x, transform.position.y),
                Quaternion.identity);

        CineMachineOperator.Instance.CamaraShake(hitShakeIntensity, .1f);
        
        
        
        _effectManager.PlayHitSoundEffect(new Vector2(0,0));
        //print("PlaySound!");



        var container = gameObject.GetComponentInParent<AttackContainer>();
        container.AttackOneHit();
        if (container.NeedTotalDisplay() && dmg > 0)
            container.AddTotalDamage(dmg);
    }

    public void AddWithCondition(BattleCondition condition)
    {
        withConditions.Add(condition);
    }

    public void SetWithConditionNum(int[] series)
    {
        withConditionNum.Clear();

        foreach (var num in series) withConditionNum.Add(num);
    }
}