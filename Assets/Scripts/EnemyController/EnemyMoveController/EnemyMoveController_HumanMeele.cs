using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
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



    public IEnumerator HE01_LAN_HI_Action01()
    {
        
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(100);ac.TurnMove(_behavior.targetPlayer);

        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, new Vector2(10, 4), new Vector2(0, 0.5f),
            true, 0, 1,0);

        var hintTime = hint.GetComponent<EnemyAttackHintBarRect2D>().warningTime;

        yield return new WaitForSeconds(hintTime);

        anim.Play("action01");

        yield return null;


        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_01"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }
    
    
    /// <summary>
    /// Continuous Stab
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_LAN_HI_Action02()
    {
        
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(100);ac.TurnMove(_behavior.targetPlayer);

        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position,
            MeeleAttackFXLayer.transform, new Vector2(14, 6), new Vector2(0, 0.5f),
            false, 0, 2,0);

        var hintTime = hint.GetComponent<EnemyAttackHintBarRect2D>().warningTime;

        yield return new WaitForSeconds(hintTime);

        anim.Play("action08");

        yield return null;


        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.12f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_02"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.24f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_02"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.36f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_02"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.48f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_02"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_02"),
            transform.position, InitContainer(true)).GetComponent<AttackFromEnemy>().attackInfo[0].dmgModifier[0]*=2;
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }
    
    
    /// <summary>
    /// spin
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_LAN_HI_Action03()
    {
        
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(100);ac.TurnMove(_behavior.targetPlayer);

        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3));

        yield return new WaitForSeconds(1);

        anim.Play("action04");

        yield return null;


        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_03"),
            transform.position, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }


    /// <summary>
    /// wrap
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_LAN_HI_Action04()
    {
        yield return _canActionOnGround;
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        var targetpos = _behavior.targetPlayer.RaycastedPosition();

        EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, targetpos,
            RangedAttackFXLayer.transform, 3, Vector2.up, false, true, 1f, 0.1f, 0.4f,
            true,true,true);

        yield return new WaitForSeconds(0.8f);

        anim.Play("action05");
        
        DisappearRenderer();
        ac.SwapWeaponVisibility(false);
        ac.SetHitSensor(false);
        transform.position = targetpos + new Vector2(0, 5);

        yield return new WaitForSeconds(0.4f);
        
        AppearRenderer();
        ac.SetHitSensor(true);
        ac.SetGravityScale(0);
        _tweener = ac.rigid.DOMoveY(targetpos.y + 1.3f, 0.12f).SetEase(Ease.OutSine);


        yield return new WaitForSeconds(0.1f);
        ac.ResetGravityScale();
        ac.SwapWeaponVisibility(true);

        InstantiateMeele(GetProjectileStartWithName("fx_e_lan_04"),
            targetpos, InitContainer(true));
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }


    /// <summary>
    /// Pass 10 parameters
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public IEnumerator HE01_SummonMinons(object[] info)
    {
        yield return _canActionOnGround;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(1);
        
        anim.Play("action09");
        
        yield return new WaitForSeconds(0.5f);
        
        var enemyName1 = (string) info[0];
        var relativePosition1 = ((Vector2)transform.position + new Vector2((float) info[1], (float) info[2])).SafePosition();
        var hp1 = (int) info[3];
        var atk1 = (int) info[4];
        
        var enemyName2 = (string) info[5];
        var relativePosition2 = ((Vector2)transform.position + new Vector2((float) info[6], (float) info[7])).SafePosition();
        var hp2 = (int) info[8];
        var atk2 = (int) info[9];
        
        var e1 = SpawnEnemyMinon(GetProjectileStartWithName(enemyName1), relativePosition1, hp1, atk1,ac.facedir);
        var e2 = SpawnEnemyMinon(GetProjectileStartWithName(enemyName2), relativePosition2, hp2, atk2,ac.facedir);

        StatusManager.StatusManagerVoidDelegate handler = null;
        
        handler = () =>
        {
            _statusManager.OnReviveOrDeath -= handler;
            if (e1 != null)
            {
                e1.GetComponent<StatusManager>().currentHp = 0;
            }

            if (e2 != null)
            {
                e2.GetComponent<StatusManager>().currentHp = 0;
            }

        };
        
        _statusManager.OnReviveOrDeath += handler;


        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
    }



}
