using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorControllerSpecial : ActorBase, IKnockbackable
{
    StatusManager _statusManager;
    public Coroutine SubMoveRoutine;
    public Vector2 HPBarOffset;

    public bool GetDodge()
    {
        return false;
    }

    public override void TakeDamage(AttackBase attackBase, Vector2 kbdir)
    {
        
    }

    public override void TakeDamage(AttackInfo attackInfo, Vector2 kbdir)
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        _statusManager = GetComponent<StatusManager>();
        _statusManager.OnHPBelow0 += CheckLife;
    }

    private void Start()
    {
        InitSimpleHealthBar();
    }

    protected void InitSimpleHealthBar()
    {


        var uiHpGauge = GetComponentInChildren<UI_SimpleHPGauge>();
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            if(uiHpGauge != null)
                Destroy(uiHpGauge.gameObject);
        }else if (uiHpGauge != null)
        {
            return;
        }

        var hpbar = Instantiate(BattleStageManager.Instance.simpleHealthBar,
            transform.position + (Vector3)HPBarOffset, Quaternion.identity, transform);
        
        hpbar.name = "SimpleHPGauge";
        
        
    }
    
    protected virtual void CheckLife()
    {
        
        OnDeath();
        
    }

    public override void SetFaceDir(int dir)
    {
        facedir = dir;
        if (facedir == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
            //transform.localScale = new Vector3(1, 1, 1);
            //rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //transform.localScale = new Vector3(1, 1, -1);
            //rigid.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        
    }

    protected void OnDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        
        var enemyBehaviors = FindObjectsOfType<DragaliaEnemyBehavior>();
        foreach (var behavior in enemyBehaviors)
        {
            behavior.RemoveTarget(gameObject);
        }
        ResetGravityScale();
        GetComponentInChildren<AudioSource>().Stop();
        
        transform.Find("MeeleAttackFX").gameObject.SetActive(false);
        transform.Find("HitSensor").gameObject.SetActive(false);
        anim.SetBool("defeat",true);
        //MoveManager.PlayVoice(0);//死亡
        anim.SetBool("hurt",false);
        OnAttackInterrupt?.Invoke();
        
        
        
        
        anim.speed = 1;
        GetComponent<NpcController>().enabled = false;
        //MoveManager.SetGroundCollider(true);
        //MoveManager.enabled = false;
        //MoveManager.StopAllCoroutines();
        //_behavior.StopAllCoroutines();
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {

            //meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }

        //_behavior.enabled = false;
        _statusManager.enabled = false;
        _statusManager.StopAllCoroutines();

        //yield return new WaitUntil(()=>!anim.GetCurrentAnimatorStateInfo(0).IsName("hurt"));
        yield return null;
        
        anim.Play("defeat");
        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);
        anim.speed = 0;

        

    }
}
