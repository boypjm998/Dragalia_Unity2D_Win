using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_Lancer : EnemyMoveManager
{
    
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
    /// Dash Long
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_LAN_HI_Action05()
    {
        
        yield return _canActionOnGround;
        
        //ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);

        var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, transform.position,
            RangedAttackFXLayer.transform, new Vector2(35, 3.5f), new Vector2(0, 0),
            false, 0, 2,ac.facedir==1?0:180);
        
        anim.Play("action06");

        yield return new WaitForSeconds(2f);

        anim.Play("float_entire");

        yield return new WaitForSeconds(0.2f);

        InstantiateRanged(GetProjectileStartWithName("fx_e_lan_05"),
            transform.position, InitContainer(false), ac.facedir).
            AddComponent<RelativePositionRetainer>().SetParent(transform);
        var endPos = Mathf.Clamp(transform.position.x + ac.facedir * 21, BattleStageManager.Instance.mapBorderL + 0.5f,
            BattleStageManager.Instance.mapBorderR -0.5f);
        var distance = Mathf.Abs(endPos - transform.position.x);

        _tweener = transform.DOMoveX(endPos, 0.4f * distance / 25f).SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed);
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }

    /// <summary>
    /// 幽灵突袭:Axe
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_LAN_HI_Action06(int num, string prefabName, bool edge=false)
    {
        yield return _canActionOnGround;
        
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("STY_Action02");
        
        anim.Play("action09");

        yield return new WaitForSeconds(0.5f);
        
        var interval = (BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL) / (num);

        var prefab = GetProjectileOfName(prefabName);

        if (num <= 1)
        {
            Instantiate(prefab, 
                new Vector3(0, transform.position.y),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else if(edge == false)
        {
            for (int i = 0; i < num; i++)
            {
                var phantom = Instantiate(prefab, 
                    new Vector3(interval*0.5f+BattleStageManager.Instance.mapBorderL + interval * i, transform.position.y),
                    Quaternion.identity, RangedAttackFXLayer.transform);
                if (i >= num / 2)
                {
                    phantom.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        else
        {
            interval = (BattleStageManager.Instance.mapBorderR - BattleStageManager.Instance.mapBorderL) / (num-1);
            for (int i = 0; i < num; i++)
            {
                var phantom = Instantiate(prefab, 
                    new Vector3(BattleStageManager.Instance.mapBorderL + interval * i, transform.position.y),
                    Quaternion.identity, RangedAttackFXLayer.transform);
                if (i >= num / 2)
                {
                    phantom.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        

    }
    
    /// <summary>
    /// 幽灵突袭:Bow
    /// </summary>
    /// <returns></returns>
    public IEnumerator HE01_LAN_HI_Action07(string prefabName, bool aim, float posX, float fillTime = 2)
    {
        yield return _canActionOnGround;
        
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("STY_Action02");
        
        anim.Play("action09");

        yield return new WaitForSeconds(0.5f);

        var prefab = GetProjectileOfName(prefabName);

        
        var phantom = Instantiate(prefab, 
                new Vector3(0, transform.position.y),
                Quaternion.identity, RangedAttackFXLayer.transform);
        var controller = phantom.GetComponent<Projectile_GhostAssault_2>();
        controller.fixedPositionValue = posX;
        controller.fillTime = fillTime;
        controller.fixedPosition = aim;
        
        if(posX< 0)
            phantom.transform.localScale = new Vector3(-1, 1, 1);
        
        
        yield return new WaitUntil(()=>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        
        QuitAttack();
        

    }
    
    
    
}
