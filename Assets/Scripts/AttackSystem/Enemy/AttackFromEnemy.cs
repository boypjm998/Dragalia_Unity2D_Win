using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics;
public class AttackFromEnemy : AttackBase
{
    protected BattleStageManager battleStageManager;
    
    public GameObject enemySource;
    
    [Header("Damage Basic Attributes")]
    // public float knockbackPower = 100;
    // public float knockbackForce;
    // public float knockbackTime;
    // public Vector2 knockbackDirection = Vector2.right;
    // public BasicCalculation.KnockBackType KBType;
    
    //[SerializeField]protected float[] dmgModifier;
    //[HideInInspector] public int firedir;
    [SerializeField] public bool isMeele;
    
    //[SerializeField] protected List<float> nextDmgModifier;
    //[SerializeField] protected List<float> nextKnockbackPower;
    //[SerializeField] protected List<float> nextKnockbackForce;
    //[SerializeField] protected List<float> nextKnockbackTime;
    
    // public List<BattleCondition> withConditions { get; protected set; }
    // public List<int> withConditionChance;
    // public List<int> withConditionNum; //一次上几个debuff？
    // public List<int> withConditionFlags;// 友军
    [HideInInspector]public List<int> hitFlags;//遍历玩家做一个数组，每个玩家代表一个hitflag


    public GameObject hitConnectEffect;
    [HideInInspector]public Collider2D attackCollider;
    [HideInInspector]public Transform selfpos;
    
    //static int DEFAULT_GRAVITY = 4;
    public Coroutine ConnectCoroutine;
    
    public bool forcedShake = false;
    public float hitShakeIntensity = 3;

    public float damageAutoReset = 0; //自动刷新
    
    public enum AvoidableProperty
    {
        Red = 0,
        Purple = 1,
        Forced = 2
    }

    [SerializeField] protected AvoidableProperty Avoidable = AvoidableProperty.Red;
    
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _effectManager = BattleEffectManager.Instance;
        //withConditions = new List<BattleCondition>();
        //withConditionNum = new List<int>();
        hitFlags = SearchPlayerList();
        //withConditionFlags = SearchPlayerList();
        battleStageManager = BattleStageManager.Instance;
        
        

    }

    protected virtual void Start()
    {
        try
        {
            CheckSpecialConditionalEffectBeforeAttack(enemySource.GetComponent<StatusManager>());
        }
        catch
        {
            Debug.LogWarning("enemySourceNotFoundWhenCheckSpecialConditionalEffect");
        }
        
    }


    // Update is called once per frame
    


    protected List<int> SearchPlayerList()
    {
        hitFlags = new List<int>();

        GameObject enemyLayer = GameObject.Find("Player");
        for (int i = 0; i < enemyLayer.transform.childCount; i++)
        {
            if (enemyLayer.transform.GetChild(i).gameObject.activeSelf)
                hitFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());

        }
        return hitFlags;
    }

    protected void ResetFlags()
    {
        hitFlags.Clear();
        hitFlags = SearchPlayerList();
    }
    
    public virtual IEnumerator MeeleTimeStop(float time)//近战的卡肉
    {

        Animator animAttack = GetComponentInParent<Animator>();
        Rigidbody2D rigid = animAttack.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = rigid.GetComponentInParent<Animator>();
        //Debug.Log(parent);
        animAttack.speed = 0.5f;
        anim.speed = 0;
        float gravity = rigid.gravityScale;
        //print(gravity);
        rigid.gravityScale = 0;
        yield return new WaitForSeconds(time);

        RecoverFromMeeleTimeStop(DEFAULT_GRAVITY);
    }

    public virtual void RecoverFromMeeleTimeStop(float gravity)
    {
        Animator animAttack = GetComponentInParent<Animator>();
        Rigidbody2D rigid = animAttack.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = rigid.GetComponentInParent<Animator>();
        rigid.gravityScale = DEFAULT_GRAVITY;
        //Debug.Log(rigid.gravityScale);
        anim.speed = 1;
        animAttack.speed = 1;
        ConnectCoroutine = null;
    }
    
    public virtual void PlayDestroyEffect(float shakeIntensity)
    {
        CineMachineOperator.Instance.CamaraShake(shakeIntensity, .1f);
        if(hitConnectEffect == null)
            return;
        GameObject eff = Instantiate(hitConnectEffect, transform.position, Quaternion.identity);
        eff.name = "HitEffect0";
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
    
    public void CauseDamage(Collider2D collision)
    {
        
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        hitFlags.Remove(collision.transform.parent.GetInstanceID());
        

        //print(collision.name);
        //print(battleStageManager);
        int dmg = battleStageManager.CalculateHit
            (collision.transform.parent.gameObject,enemySource, this,1);
        
        OnAttackHit?.Invoke(this,collision.transform.parent.gameObject);


        if (hitConnectEffect != null)
        {
            //new Vector2(collision.transform.position.x,collision.ClosestPoint(transform.position)
            
            Instantiate(hitConnectEffect, collision.ClosestPoint(collision.transform.position), Quaternion.identity);
        }
        
        try
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
            {
                // 在摄像机范围内
                _effectManager.PlayHitSoundEffect(hitSoundEffect);
            }
        }
        catch
        {
            _effectManager.PlayHitSoundEffect(hitSoundEffect);
        }
        
        //print("play");
        

        CineMachineOperator.Instance.CamaraShake(hitShakeIntensity, .1f);
        
        
        AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
        container?.AttackOneHit();
        
        if(destroyAfterHit)
            Destroy(gameObject);
        
        if(damageAutoReset>0)
            Invoke("NextAttack",damageAutoReset);
        
    }

    // public Vector2 GetKBDirection(BasicCalculation.KnockBackType knockBackType,GameObject target)
    // {
    //     var kbdirtemp = knockbackDirection;
    //     switch (knockBackType)
    //     {
    //         case BasicCalculation.KnockBackType.FaceDirection:
    //             kbdirtemp = firedir * kbdirtemp;
    //             break;
    //             
    //         case BasicCalculation.KnockBackType.FromCenterRay:
    //             kbdirtemp = transform.InverseTransformPoint(target.transform.position);
    //             break;
    //         case BasicCalculation.KnockBackType.FromCenterFixed:
    //             kbdirtemp = transform.position.x > target.transform.position.x
    //                 ? new Vector2(-knockbackDirection.x,knockbackDirection.y)
    //                 : knockbackDirection;
    //             break;
    //         case BasicCalculation.KnockBackType.None:
    //             kbdirtemp = Vector2.zero;
    //             break;
    //         default:
    //             break;
    //     }
    //
    //     return kbdirtemp;
    // }

    
    
    public override void NextAttack()
    {
        if (attackInfo.Count > 1)
        {
            attackInfo.RemoveAt(0);
        }
        

        ResetFlags();
    }
    
    public override void ResetWithConditionFlags()
    {
        // withConditionFlags.Clear();
        //
        // var enemyLayer = GameObject.Find("Player");
        // for (var i = 0; i < enemyLayer.transform.childCount; i++)
        //     withConditionFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());

        var container = GetComponentInParent<AttackContainer>();
        container.conditionCheckDone.Clear();
    }

    public void ChangeAvoidability(AvoidableProperty property)
    {
        Avoidable = property;
    }

    protected virtual void OnDestroy()
    {
        GetComponentInParent<AttackContainer>()?.FinishHit();
        CancelInvoke();
    }
    
    public virtual void DamageCheckRaycast(RaycastHit2D hitinfo)
    {
        if (hitinfo.collider != null)
            if (hitinfo.collider.CompareTag("Player") &&
                hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                CauseDamage(hitinfo.collider);

                Destroy(gameObject);
                
            }
    }

    public virtual void DamageCheckCollider(Collider2D hitinfo)
    {
        if (hitinfo != null)
            if (hitinfo.CompareTag("Player") && hitFlags.Contains(hitinfo.transform.parent.GetInstanceID()))
            {
                CauseDamage(hitinfo);

                Destroy(gameObject);

                
            }
    }
    
    
}
