using System.Collections.Generic;
using GameMechanics;
using UnityEngine;


public class AttackManagerRanged : AttackManager
{
    public GameObject[] combo1FX;
    public GameObject[] combo2FX;
    public GameObject[] combo3FX;
    public GameObject[] combo4FX;
    public GameObject[] combo5FX;
    public GameObject[] dashFX;
    public GameObject[] skill1FX;
    public GameObject[] skill2FX;
    public GameObject[] skill3FX;
    public GameObject[] skill4FX;
    public GameObject[] ForceFX;
    protected TargetAimer ta;
    
    protected GameObject Shotpoints;
    
    
    
    

    protected override void Start()
    {
        base.Start();
        ta = GetComponentInChildren<TargetAimer>();
        Shotpoints = transform.Find("Shotpoints").gameObject;
    }

    public virtual void ComboAttack1()
    {
    }
    public virtual void ComboAttack2()
    {
    }
    public virtual void ComboAttack3()
    {
    }
    public virtual void ComboAttack4()
    {
    }
    public virtual void ComboAttack5()
    {
    }
    
    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,container);
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
        _statusManager.HPRegenImmediately(0,10,true);
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
        //Instantiate(healbuff, transform.position, Quaternion.identity, BuffFXLayer.transform);
        _statusManager.ObtainHealOverTimeBuff(10,15,true);
    }
    
    protected Transform FindShotpointInChildren(string childName)
    {
        var child = Shotpoints.transform.Find(childName);

        if (child != null)
            return child;

        return null;
    }
    
    public virtual void ForceStrikeRelease(int forcelevel = 0)
    {
        

    }
    

}
