using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFromPlayer : MonoBehaviour
{
    public GameObject self;
    protected BattleStageManager battleStageManager;



    //Damage Basic Attributes
    protected float knockbackPower;
    protected float knockbackForce;
    protected float dmgModifier;
    protected int spGain;
    
    protected int firedir;
    
    


    
    public List<int> hitFlags;//遍历敌人做一个数组，每个敌人代表一个hitflag

    
    public GameObject hitConnectEffect;
    public Collider2D attackCollider;
    public Transform playerpos;
    public float defaultGravity;
    Coroutine ConnectCoroutine;
    static int DEFAULT_GRAVITY = 4;
    protected int comboGroupID;
    protected int totalDamage;
    public BasicCalculation.AttackType attackType;
    public float hitShakeIntensity;

    protected virtual void Start()
    {
        battleStageManager = GameObject.Find("StageManager").GetComponent<BattleStageManager>();
    }

    public virtual void ResetFlags()
    {
        hitFlags.Clear();
        GameObject enemyLayer = GameObject.Find("EnemyLayer");
        for (int i = 0; i < enemyLayer.transform.childCount; i++)
        {
            hitFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());

        }
    }

    public virtual IEnumerator MeeleTimeStop(float time)//近战的卡肉
    {

        Animator animAttack = GetComponentInParent<Animator>();
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = playerpos.gameObject.GetComponentInParent<Animator>();
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
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = playerpos.gameObject.GetComponentInParent<Animator>();
        rigid.gravityScale = gravity;
        //Debug.Log(rigid.gravityScale);
        anim.speed = 1;
        animAttack.speed = 1;
        ConnectCoroutine = null;
    }

    public virtual void InitAttackBasicAttributes(float knockbackPower,float knockbackForce,float dmgModifier,int spGain,int firedir)
    {
        this.knockbackPower = knockbackPower;
        this.knockbackForce = knockbackForce;
        this.dmgModifier = dmgModifier;
        this.spGain = spGain;
        this.firedir = firedir;
        print(this.dmgModifier);
    }

    public virtual int GetFaceDir()
    {
        return firedir;
    }

    public virtual void PlayDestroyEffect(float shakeIntensity)
    {
        CineMachineOperator.Instance.CamaraShake(shakeIntensity, .1f);
        GameObject eff = Instantiate(hitConnectEffect, transform.position, Quaternion.identity);
        eff.name = "HitEffect0";
    }

    public virtual List<int> SearchEnemyList()
    {
        hitFlags = new List<int>();

        GameObject enemyLayer = GameObject.Find("EnemyLayer");
        for (int i = 0; i < enemyLayer.transform.childCount; i++)
        {
            if (enemyLayer.transform.GetChild(i).gameObject.activeSelf)
                hitFlags.Add(enemyLayer.transform.GetChild(i).GetInstanceID());

        }
        return hitFlags;

    }

    public virtual void DamageCheckRaycast(RaycastHit2D hitinfo)
    {
        if (hitinfo.collider != null)
        {
            if (hitinfo.collider.CompareTag("Enemy") && hitFlags.Contains(hitinfo.collider.transform.parent.GetInstanceID()))
            {
                //print(hitShakeIntensity);
                PlayDestroyEffect(hitShakeIntensity);
                hitFlags.Remove(hitinfo.collider.transform.parent.GetInstanceID());
                Destroy(gameObject);


                hitinfo.collider.GetComponent<Enemy>().TakeDamage();

                int dmg = battleStageManager.PlayerHit(hitinfo.collider.gameObject, this);

                AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
                if (container.NeedTotalDisplay() || dmg>=0)
                    container.AddTotalDamage(dmg);

            }

        }
    }

    public virtual void DamageCheckCollider(Collider2D hitinfo)
    {
        if (hitinfo != null)
        {
            if (hitinfo.CompareTag("Enemy") && hitFlags.Contains(hitinfo.transform.parent.GetInstanceID()))
            {
                //print(hitShakeIntensity);
                PlayDestroyEffect(hitShakeIntensity);

                Destroy(gameObject);

                hitinfo.GetComponent<Enemy>().TakeDamage();

                int dmg = battleStageManager.PlayerHit(hitinfo.gameObject, this);


                AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
                if (container.NeedTotalDisplay())
                    container.AddTotalDamage(dmg);

            }

        }
    }


    public float GetDmgModifier()
    {
        return dmgModifier;
    }

    public float GetSpGain()
    {
        return spGain;
    }



}
