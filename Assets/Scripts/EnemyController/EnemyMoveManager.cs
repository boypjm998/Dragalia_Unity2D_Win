using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

public abstract class EnemyMoveManager : MonoBehaviour
{
    public GameObject MeeleAttackFXLayer;
    public GameObject RangedAttackFXLayer;
    public GameObject BuffFXLayer;
    public GameObject attackContainer;
    public GameObject attackSubContainer;
    protected EnemyController ac;
    public EnemyController.OnTask OnAttackFinished;
    protected DragaliaEnemyBehavior _behavior;
    public Coroutine currentAttackMove;//只有行为树在用
    public Tweener _tweener;
    protected UI_BattleInfoCaster bossBanner;

    [Header("Moves")]
    [SerializeField] protected GameObject projectile1;
    [SerializeField] protected GameObject projectile2;
    [SerializeField] protected GameObject projectile3;
    [SerializeField] protected GameObject projectile4;
    [SerializeField] protected GameObject projectile5;
    [SerializeField] protected GameObject projectile6;
    [SerializeField] protected GameObject projectile7;
    [SerializeField] protected GameObject projectile8;
    [SerializeField] protected GameObject projectile9;
    [SerializeField] protected GameObject projectile10;

    [SerializeField] protected GameObject[] projectilePoolEX;

    protected StatusManager _statusManager;

    public abstract void UseMove(int moveID);


    protected virtual void Awake()
    {
        bossBanner = GameObject.Find("BattleInfoCaster").GetComponent<UI_BattleInfoCaster>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetGroundCollider(bool flag)
    {
        if (flag)
        {
            var sensor = transform.Find("GroundSensor").GetComponent<BoxCollider2D>();
            sensor.enabled = true;
        }
        else
        {
            var sensor = transform.Find("GroundSensor").GetComponent<BoxCollider2D>();
            sensor.enabled = false;
        }
    }

    public virtual void PlayVoice(int id)
    {
        
        
    }
    
}
