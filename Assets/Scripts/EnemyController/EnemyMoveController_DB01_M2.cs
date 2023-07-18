using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB01_M2 : EnemyMoveManager
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyController>();
    }


    /// <summary>
    /// Start chase
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB01_M2_Action01()
    {
        yield return new WaitUntil(()=>ac.grounded);
        ac.OnAttackEnter(999);
        
        ac.SetMove(1);
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.1f);
        

        yield return new WaitUntil(() => ac.anim.GetCurrentAnimatorStateInfo(0).IsName("run"));
        (ac as EnemyControllerHumanoid).moveEnable = true;
        
        QuitAttack();
    }

    public IEnumerator DB01_M2_Action02(float reachDistanceX,float reachDistanceY)
    {
        
        ac.OnAttackEnter(999);
        
        (ac as EnemyControllerHumanoid).moveEnable = false;

        while (Mathf.Abs(_behavior.targetPlayer.transform.position.x-transform.position.x) <= reachDistanceX &&
               Mathf.Abs(_behavior.targetPlayer.transform.position.y-transform.position.y) <= reachDistanceY)
        {
            ac.TurnMove(_behavior.targetPlayer);
            anim.Play("bite",0,0);
            yield return null;
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=0.37f);
            
            Bite();
            
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=0.99f);

        }
        
        anim.Play("idle");
        
        QuitAttack();
        
        
    }

    protected void Bite()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        var proj = InstantiateMeele(projectile1, transform.position + new Vector3(ac.facedir * 2, 0, 0),
            container);
        var poison = new TimerBuff((int)BasicCalculation.BattleCondition.Poison, 35, 15,1);
        var poisonResDown = new TimerBuff((int)BasicCalculation.BattleCondition.PoisonResDown, 20, 30,1);
        
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(poison,110,1);
        proj.GetComponent<AttackFromEnemy>().AddWithConditionAll(poisonResDown,100);
    }
}
