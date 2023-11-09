using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_HumanRanged : EnemyMoveManager
{
    public void FX_Wroth()
    {
        var fx = Instantiate(GetProjectileOfName("fx_evil_aura"),
            new Vector3(transform.position.x,transform.position.y,
                ac.ModelDepth), Quaternion.identity, MeeleAttackFXLayer.transform);
    }
    
    /// <summary>
    /// Wand Combo 1
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_ROD_Action01()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes);
        anim.Play("action01");
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.24f);

        InstantiateDirectionalRanged(GetProjectileStartWithName("fx_e_rod_01"),
            transform.position, InitContainer(false),ac.facedir,0);
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// Wand Force Strike
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_ROD_Action02()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes + 50);
        anim.Play("action06");
        ac.TurnMove(_behavior.targetPlayer);

        var hint = GenerateWarningPrefab("action01",
            _behavior.targetPlayer.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(2.5f);
        
        anim.Play("action08");

        InstantiateRanged(GetProjectileStartWithName("fx_e_rod_02"),
            hint.transform.position, InitContainer(false),1);

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }


}
