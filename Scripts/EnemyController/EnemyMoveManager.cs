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
    protected Enemy ac;
    public Enemy.OnTask OnAttackFinished;
    protected DragaliaEnemyBehavior _behavior;
    public Coroutine currentAttackMove;//只有行为树在用
    public Tweener _tweener;

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
    [SerializeField] protected GameObject projectile11;
    [SerializeField] protected GameObject projectile12;
    [SerializeField] protected GameObject projectile13;
    [SerializeField] protected GameObject projectile14;
    [SerializeField] protected GameObject projectile15;
    [SerializeField] protected GameObject projectile16;
    [SerializeField] protected GameObject projectile17;
    [SerializeField] protected GameObject projectile18;
    [SerializeField] protected GameObject projectile19;
    [SerializeField] protected GameObject projectile20;

    public abstract void UseMove(int moveID);
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
