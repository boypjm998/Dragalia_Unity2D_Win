using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController_HumanMeele : EnemyMoveManager
{
    

    public void FX_Wroth()
    {
        
        var fx = Instantiate(GetProjectileOfName("fx_evil_aura"),
            new Vector3(transform.position.x,transform.position.y,
                ac.ModelDepth), Quaternion.identity, MeeleAttackFXLayer.transform);
        
    }
    
    
    /// <summary>
    /// Sword Combo 1
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_SWD_Action01()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes);
        anim.Play("action01");

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_swd_01"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }
    
    
    
    public IEnumerator HE01_SWD_Action03()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(_statusManager.knockbackRes);
        anim.Play("action03");

        yield return null;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_swd_03"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }


}
