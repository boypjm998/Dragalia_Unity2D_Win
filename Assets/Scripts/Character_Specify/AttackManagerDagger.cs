using DG.Tweening;
using GameMechanics;
using UnityEngine;


public class AttackManagerDagger : AttackManager
{
    public GameObject combo1FX;
    public GameObject combo2FX;
    public GameObject combo3FX;
    public GameObject combo4FX;
    public GameObject combo5FX;
    public GameObject dashFX;
    public GameObject skill1FX;
    public GameObject skill2FX;
    public GameObject skill3FX;
    public GameObject skill4FX;
    protected TargetAimer ta;

    void Start()
    {
        base.Start();
        ta = GetComponentInChildren<TargetAimer>();
    }

    public virtual void ComboAttack_Rush(float range,string moveName)
    {
        //向前方4格内Enemies层的敌人发一道射线
        var hit = Physics2D.OverlapArea(transform.position + new Vector3(0, 0.5f, 0),
            transform.position + new Vector3(range, -0.5f, 0), LayerMask.GetMask("Enemies"));
        
        var hitInfo = Physics2D.Raycast(transform.position, Vector2.right * ac.facedir, range,
            LayerMask.GetMask("Enemies"));
        
        //如果射线击中的敌人不为空且和自身距离小于1.5,return
        float targetPos;

        if (hitInfo.collider != null)
        {
            var distance = Mathf.Abs(hitInfo.point.x - transform.position.x);
            if (distance < 1.5f)
                return;
            targetPos = hitInfo.point.x - ac.facedir * 1.5f;
        }
        else
            targetPos = transform.position.x + ac.facedir * 3f;

        targetPos = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(targetPos, transform.position.y)).x;
        
        //print(hitInfo.collider.gameObject);

        StartCoroutine(ac.HorizontalMoveFixedTime
        (targetPos, 0.2f,
            moveName,Ease.OutCubic));

    }

    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX,transform.position,container);
    }

    public void ComboAttack1()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(combo1FX,transform.position,container);
    }
    
    public void ComboAttack2()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(combo2FX,transform.position,container);
    }
    
    public void ComboAttack3()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(combo3FX,transform.position,container);
    }
    
    public void ComboAttack4()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(combo4FX,transform.position,container);
    }
    
    public void ComboAttack5()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
        InstantiateRanged(combo5FX,transform.position,container,ac.facedir);
    }

    public virtual void Skill1(int eventID)
    {
    }
    
    public virtual void Skill2(int eventID)
    {
    }
    
    public virtual void Skill3(int eventID)
    {
    }
    
    public virtual void Skill4(int eventID)
    {
    }





}
