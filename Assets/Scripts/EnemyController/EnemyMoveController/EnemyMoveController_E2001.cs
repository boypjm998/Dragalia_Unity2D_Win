using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_E2001 : EnemyMoveManager
{
    // Start is called before the first frame update
    

    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlying>();
    }

    public IEnumerator E2001_Action01()
    {
        //yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter();
        var hint = GenerateWarningPrefab(WarningPrefabs[0], transform.position-new Vector3(0,3,0),transform.rotation, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();

        List<Transform> hintbars = new();
        for (int i = 0; i < _behavior.playerList.Count; i++)
        {
            var hint_temp = GenerateWarningPrefab(WarningPrefabs[0],
                new Vector3(_behavior.playerList[i].transform.position.x,transform.position.y-3), Quaternion.identity, RangedAttackFXLayer.transform);
            hintbars.Add(hint_temp.transform);
        }

        yield return new WaitForSeconds(hintbar.warningTime);
        
        Action01_AroundAttack(hintbars);
        
        yield return new WaitForSeconds(0.5f);
        Destroy(hint);
        foreach (var hintbar_single in hintbars)
        {
            Destroy(hintbar_single.gameObject);
        }
        yield return new WaitForSeconds(3f);
        QuitAttack();
    }
    
    
    public IEnumerator E2001_Action01_2()
    {
        //yield return new WaitUntil(() => anim.GetBool("isGround") && !ac.hurt);
        ac.OnAttackEnter(128);
        // var hint = GenerateWarningPrefab(WarningPrefabs[3], transform.position-new Vector3(0,3,0),transform.rotation, MeeleAttackFXLayer.transform);
        // var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        ac.SetCounter(true);

        List<Transform> hintbars = new();
        for (int i = 0; i < _behavior.playerList.Count; i++)
        {
            var hint_temp = GenerateWarningPrefab(WarningPrefabs[3],
                new Vector3(_behavior.playerList[i].transform.position.x,transform.position.y-3), Quaternion.identity, RangedAttackFXLayer.transform);
            hintbars.Add(hint_temp.transform);
        }

        yield return new WaitForSeconds(2.1f);
        
        Action01_AroundAttack(hintbars);
        
        yield return new WaitForSeconds(0.5f);
        
        foreach (var hintbar_single in hintbars)
        {
            Destroy(hintbar_single.gameObject);
        }
        ac.SetCounter(false);
        yield return new WaitForSeconds(.5f);
        QuitAttack();
    }

    public IEnumerator E2001_Action02()
    {
        
        ac.OnAttackEnter();
        var hint = GenerateWarningPrefab(WarningPrefabs[1],_behavior.viewerPlayer.transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
        yield return new WaitForSeconds(2.5f);
        Action02_LockAttack(hint.transform);
        Destroy(hint,1f);
        yield return new WaitForSeconds(4f);
        QuitAttack();
    }
    
    public IEnumerator E2001_Action03_1()
    {
        
        ac.OnAttackEnter();
        yield return new WaitForSeconds(0.2f);
        Action03_PillarAttack(_behavior.targetPlayer.transform,true);
        yield return new WaitForSeconds(3f);
        QuitAttack();
    }
    
    public IEnumerator E2001_Action03_2()
    {
        
        ac.OnAttackEnter();
        yield return new WaitForSeconds(0.2f);
        Action03_PillarAttack(_behavior.targetPlayer.transform,false);
        yield return new WaitForSeconds(3f);
        QuitAttack();
    }

    public IEnumerator E2001_Action04()
    {
        ac.OnAttackEnter();
        var hint = GenerateWarningPrefab(WarningPrefabs[2],transform.position,Quaternion.identity, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        yield return new WaitForSeconds(hintbar.warningTime);
        Action04_AroundElectricRing();
        Destroy(hint);
        yield return new WaitForSeconds(.5f);
        QuitAttack();
    }

    public IEnumerator E2001_Action05(bool nil = true)
    {
        _behavior.breakable = false;
        if(!nil)
            TutorialLevelManager.Instance.PlayStoryVoiceWithDialog(30,8003,null);
        ac.OnAttackEnter();
        //(bossBanner as UI_BattleInfoCasterStory).PrintSkillName("STY0_Action03", true);
        yield return new WaitForSeconds(1f);
        Action05_VoidBurstFullScreen(nil);
        yield return new WaitForSeconds(1f);
        QuitAttack();
        _behavior.breakable = true;
    }

    /// <summary>
    /// Summon minions
    /// </summary>
    /// <returns></returns>
    public IEnumerator E2001_Action06()
    {
        ac.OnAttackEnter();
        var hint1 = GenerateWarningPrefab(WarningPrefabs[4],transform.position + new Vector3(8,0,0),Quaternion.identity, RangedAttackFXLayer.transform);
        var hint2 = GenerateWarningPrefab(WarningPrefabs[4],transform.position + new Vector3(-8,0,0),Quaternion.identity, RangedAttackFXLayer.transform);
        Action06_SummonAttack(hint1.transform.position,hint2.transform.position);
        yield return new WaitForSeconds(0.5f);
        QuitAttack();
    }
    
    public IEnumerator E2001_Action07()
    {
        //yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(128);
        ac.SetCounter(true);
        var hint = GenerateWarningPrefab(WarningPrefabs[1],transform.position,Quaternion.identity, MeeleAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBar>();
        yield return new WaitForSeconds(hintbar.warningTime);
        Action07_AroundElectric();
        Destroy(hint);
        ac.SetCounter(false);
        yield return new WaitForSeconds(.5f);
        QuitAttack();
    }

    protected override void QuitAttack()
    {
        _behavior.currentAttackAction = null;
        ac.OnAttackExit();
        //anim.Play("idle");
        currentAttackMove = null; //只有行为树在用
        OnAttackFinished?.Invoke(true);
    }


    void Action01_AroundAttack(List<Transform> hintbars)
    {
        
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        //container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
        container.GetComponent<AttackContainer>().InitAttackContainer(hintbars.Count+1,false);
        var proj = InstantiateRanged(projectile1, transform.position, container ,ac.facedir);
        foreach (var hintbar in hintbars)
        {
            var proj_temp = InstantiateRanged(projectile1, new Vector3(hintbar.position.x,transform.position.y), container,ac.facedir);
        }
        
        
        //container.GetComponent<AttackContainerEnemy>().InitAttackContainer(1);
    }

    void Action02_LockAttack(Transform transform)
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        var proj = InstantiateRanged(projectile2, transform.position, container ,1);
    }

    void Action03_PillarAttack(Transform targetTransform, bool forced)
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        if (forced)
        {
            var proj = InstantiateRanged(projectile3, targetTransform.position, container ,1);
            var atk = proj.GetComponent<ForcedAttackFromEnemy>();
            atk.enemySource = gameObject;
            atk.target = targetTransform.gameObject;
            var burnEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Burn, 128f, 12f, 1);
            atk.AddWithCondition(0,burnEffect,200,0);
            var attackDownEffect = new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff, 10f, 15f,BasicCalculation.MAXCONDITIONSTACKNUMBER);
            atk.AddWithCondition(1,attackDownEffect,100,1);
            var darkEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Blindness, -1, 15f, 1);
            atk.AddWithCondition(2,darkEffect,200,2);
        }
        else
        {
            var proj = InstantiateRanged(projectile6, targetTransform.position, container ,1);
            var atk = proj.GetComponent<AttackFromEnemy>();
            atk.enemySource = gameObject;
            var burnEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Burn, 128f, 12f, 1);
            atk.AddWithCondition(0,burnEffect,200,0);
            var attackDownEffect = new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff, 10f, 5f,BasicCalculation.MAXCONDITIONSTACKNUMBER);
            atk.AddWithCondition(1,attackDownEffect,100,1);
            var darkEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Blindness, -1, 15f, 1);
            atk.AddWithCondition(2,darkEffect,200,2);
        }

    }

    void Action04_AroundElectricRing()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        var proj = InstantiateMeele(projectile4, transform.position, container);
    }

    void Action05_VoidBurstFullScreen(bool nil)
    {
        var container = Instantiate(attackContainer, RangedAttackFXLayer.transform);
        var proj = InstantiateRanged(projectile5, container.transform.position, container ,ac.facedir);
        var atk = proj.GetComponent<ForcedAttackFromEnemy>();
        atk.enemySource = gameObject;
        atk.target = _behavior.playerList[0];
        if (_behavior.playerList.Count >= 3)
        {
            atk.extraTargets.Add(_behavior.playerList[1]);
            atk.extraTargets.Add(_behavior.playerList[2]);
        }

        if (nil)
        {
            var nilEffect = new TimerBuff((int)BasicCalculation.BattleCondition.Nihility, -1f, 600f, 1);
            atk.AddWithConditionAll(nilEffect,200);
        }
    }

    void Action06_SummonAttack(Vector3 posLeft, Vector3 posRight)
    {
        StartCoroutine(Action06_SummonAction(posLeft,posRight));
    }

    IEnumerator Action06_SummonAction(Vector3 posLeft, Vector3 posRight)
    {
        posLeft = BattleStageManager.Instance.OutOfRangeCheck(posLeft);
        posRight = BattleStageManager.Instance.OutOfRangeCheck(posRight);
        yield return new WaitForSeconds(5f);
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(2,false);
        var proj = InstantiateRanged(projectile2, posLeft, container ,1);
        var proj2 = InstantiateRanged(projectile2, posRight, container ,1);
        var enemyLayer = GameObject.Find("EnemyLayer");
        if(enemyLayer.transform.childCount>5)
            yield break;
        var enemy1 = Instantiate(projectile7, posLeft, Quaternion.identity, enemyLayer.transform);
        var enemy2 = Instantiate(projectile7, posRight, Quaternion.identity, enemyLayer.transform);
    }
    
    
    
    void Action07_AroundElectric()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        InstantiateMeele(projectile8, transform.position, container);
        
    }
    


}
