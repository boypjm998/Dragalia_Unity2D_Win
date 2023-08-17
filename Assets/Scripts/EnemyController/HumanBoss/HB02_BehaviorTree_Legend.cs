using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using UnityEngine;

public class HB02_BehaviorTree_Legend : HB02_BehaviorTree_2
{
    public bool isTransformed = false;
    public GameObject p2_prefab;
    protected EnemyMoveController_HB02_Legend enemyAttackManager_legend;
    
    public AudioClip bgm1;
    public AudioClip bgm2;
    protected override void Awake()
    {
        base.Awake();
        enemyAttackManager_legend = enemyAttackManager as EnemyMoveController_HB02_Legend;
        

        if(isTransformed==false)
            status.OnHPBelow0 += ToPhase2;
        if (isTransformed == false)
        {
            if (bgm1 != null)
            {
                BattleEffectManager.Instance.SetBGM(bgm1);
                //BattleEffectManager.Instance.bgmVoiceSource.clip = bgm1;
                //BattleEffectManager.Instance.bgmVoiceSource.Play();
                print("change bgm");
            }
        }
        else
        {
            BattleEffectManager.Instance.SetBGM(bgm2);
            //BattleEffectManager.Instance.bgmVoiceSource.clip = bgm2;
            BattleEffectManager.Instance.PlayBGM();
            status.OnHPBelow0 += WorldBreakEffect;
        }
    }

    protected override IEnumerator Start()
    {
        
        yield return new WaitWhile(() => GlobalController.currentGameState == GlobalController.GameState.WaitForStart
        );
        SearchTarget();
        state = 0;
        substate = 0;
        yield return null;
        
        
        yield return new WaitForSeconds(awakeTime);
        StartCoroutine(UpdateBehavior());
        
        
    }


    protected override void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        switch (action_name)
        {
            case "KeepDistance":
            {
                if (_currentActionStage.args.Length == 3)
                {
                    float distanceMin = float.Parse(_currentActionStage.args[0]);
                    float distanceMax = float.Parse(_currentActionStage.args[1]);
                    float followTime = float.Parse(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_KeepDistance(distanceMin, distanceMax, followTime));
                }
                else
                {
                    float distanceMin = float.Parse(_currentActionStage.args[0]);
                    float distanceMax = float.Parse(_currentActionStage.args[1]);
                    float followTime = float.Parse(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_KeepDistance(distanceMin, distanceMax, followTime,1));
                }
                break;
            }
            case "ApproachTarget":
            {
                if (_currentActionStage.args.Length == 4)
                {
                    float arriveDistanceX = float.Parse(_currentActionStage.args[0]);
                    float arriveDistanceY = float.Parse(_currentActionStage.args[1]);
                    float triggerDistance = float.Parse(_currentActionStage.args[2]);
                    float followTime = float.Parse(_currentActionStage.args[3]);
                    currentAction = StartCoroutine(ACT_ApproachTarget(arriveDistanceX, arriveDistanceY, triggerDistance, followTime));
                }else if (_currentActionStage.args.Length == 3)
                {
                    float arriveDistanceX = float.Parse(_currentActionStage.args[0]);
                    float arriveDistanceY = float.Parse(_currentActionStage.args[1]);
                    float followTime = float.Parse(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_ApproachTarget(arriveDistanceX, arriveDistanceY, followTime));
                }else if (_currentActionStage.args.Length == 2)
                {
                    float arriveDistance = float.Parse(_currentActionStage.args[0]);
                    float followTime = float.Parse(_currentActionStage.args[1]);
                    currentAction = StartCoroutine(ACT_ApproachTarget(arriveDistance, followTime));
                }else throw new System.Exception("ApproachTarget参数数量错误");
                break;
            }
            case "EarthBarrier":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_EarthBarrier(interval));
                break;
            }
            case "FaithEnhancement":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FaithEnhancement(interval));
                break;
            }
            case "CombinedTwilightAttack":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CombinedTwilightAttack(interval));
                break;
            }
            case "ComboA":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboA(interval));
                break;
            }
            case "ComboB":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboB(interval));
                break;
            }
            case "ComboC":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboC(interval));
                break;
            }
            case "DashAttack":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DashAttack(interval));
                break;
            }
            
            case "TwilightMoon":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightMoon(interval));
                break;
            }
            case "WarpAttack":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WarpAttack(interval));
                break;
            }
            case "SpinDash":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SpinDash(interval));
                break;
            }
            case "SpinDashFast":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SpinDashRed(interval));
                break;
            }
            case "SummonOrbs":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonOrbs(interval));
                break;
            }
            case "GloriousSanctuary":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GloriousSanctuary(interval));
                break;
            }
            case "GloriousSanctuaryG":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GloriousSanctuaryG(interval));
                break;
            }
            case "GloriousSanctuaryC":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GloriousSanctuaryC(interval));
                break;
            }
            case "PickupBuff":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PickupBuff(interval));
                break;
            }
            case "ReflectionOn":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ReflectionOn(interval));
                break;
            }
            case "BusterOn":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BusterOn(interval));
                break;
            }
            case "WorldReset":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WorldReset(interval));
                break;
            }
            case "CelestialPrayer":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CelestialPrayer(interval));
                break;
            }
            case "CelestialPrayerF":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CelestialPrayerF(interval));
                break;
            }
            case "CelestialPrayerG":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CelestialPrayerG(interval));
                break;
            }
            case "TwilightCrownF":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightCrownF(interval));
                break;
            }
            case "TwilightCrownG":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightCrownG(interval));
                break;
            }
            case "TwilightCrown":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TwilightCrown(interval));
                break;
            }
            case "HolyCrownG":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HolyCrownG(interval));
                break;
            }
            case "HolyCrown":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HolyCrown(interval));
                break;
            }
            case "BackWarpSmash":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BackWarpSmash(interval));
                break;
            }
            case "BackWarpGround":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BackWarpGround(interval));
                break;
            }
            case "GalaxyPrayerOn":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BlessGalaxy(interval));
                break;
            }
            case "GalaxyPrayerOff":
            {
                ACT_BlessGalaxyOff();
                ActionEnd();
                break;
            }
            case "GenesisCrown":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HolyCrownFWithFullScreenAttack(interval));
                break;
            }
            case "GenesisCrownSingle":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_FullScreenAttack(interval));
                break;
            }
            default:break;

        }
        
        
    }

    void ToPhase2()
    {
        isAction = true;
        StopAllCoroutines();
        enemyController.SetKBRes(999);
        enemyController.SetHitSensor(false);
        enemyController.StopAllCoroutines();
        enemyController.SetMove(0);
        enemyAttackManager.StopAllCoroutines();
        enemyController.hurt = false;
        enemyController.anim.Play("idle");
        status.enabled = false;
        status.ResetAllStatusForced();
        enemyController.SetCounter(false);
        enemyController.OnAttackInterrupt?.Invoke();
        enemyController.SetFlashBody(false);
        for(int i = 0; i < enemyAttackManager.MeeleAttackFXLayer.transform.childCount; i++)
        {
            Destroy(enemyAttackManager.MeeleAttackFXLayer.transform.GetChild(i).gameObject);
        }
        
        try
        {
            Destroy(FindObjectOfType<Projectile_C003_1_Boss>().gameObject);
        }
        catch (Exception e)
        {
            
        }

        isTransformed = true;
        currentAction = StartCoroutine(ChangePhaseAnimationRoutine());

    }

    IEnumerator ChangePhaseAnimationRoutine()
    {
        ActionStart();
        currentAttackAction = currentAttackAction = 
            StartCoroutine(enemyAttackManager.HB02_Action14());
        yield return new WaitUntil(()=>currentAttackAction == null);
        yield return new WaitForSeconds(0.5f);
        var p2_boss = Instantiate(p2_prefab,transform.position,Quaternion.identity,transform.parent);
        p2_boss.GetComponent<EnemyController>().TurnMove(targetPlayer);
        //UI_BossStatus.Instance.RedirectBoss(p2_boss);
        //BattleEffectManager.Instance.bgmVoiceSource.Stop();
        BattleStageManager.currentDisplayingBossInfo = 2;
        FindObjectOfType<UI_BossStatus>().RedirectBoss(p2_boss,1);
        p2_boss.GetComponent<StatusManager>()?.OnHPChange?.Invoke();
        ActionEnd();
        yield return null;
        BattleStageManager.Instance.RemoveFieldAbility(20034);
        BattleStageManager.Instance.RemoveFieldAbility(20033);
        
        
        Destroy(gameObject);
    }

    protected IEnumerator ACT_SpinDashRed(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action15_V());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_SummonOrbs(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action16());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_GloriousSanctuaryG(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action17());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_GloriousSanctuaryC(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action18());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_PickupBuff(float interval)
    {
        
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentMoveAction = StartCoroutine(enemyAttackManager_legend.HB02_Action19());
        yield return new WaitUntil(()=>currentMoveAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_ReflectionOn(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action20());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_BusterOn(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action21());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_CelestialPrayerF(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action22());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_CelestialPrayerG(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action23());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_TwilightCrownF(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action25());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_TwilightCrownG(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action26());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }
    
    protected IEnumerator ACT_HolyCrownG(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action27());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_HolyCrownFWithFullScreenAttack(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action28());
        yield return new WaitUntil(()=>currentAttackAction == null);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action29());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_FullScreenAttack(float interval)
    {
        breakable = false;
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action29(true));
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_BackWarpSmash(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action32());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_BackWarpGround(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action31());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_BlessGalaxy(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action30());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected void ACT_BlessGalaxyOff()
    {
        enemyAttackManager_legend.HB02_Action30_Off();
    }

    protected IEnumerator ACT_WorldReset(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager_legend.HB02_Action24());
        yield return new WaitUntil(()=>currentAttackAction == null);
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected void WorldBreakEffect()
    {
        var backGroundGO = GameObject.Find("Background");
        var backGround1Sprite = backGroundGO.transform.Find("Background1").GetComponent<SpriteRenderer>();
        backGround1Sprite.DOColor(Color.white, 1f);
        
        var backGround3Sprite = backGroundGO.transform.Find("Background3").GetComponent<SpriteRenderer>();
        backGround3Sprite.DOColor(Color.clear, 1f);
        
        backGroundGO.transform.Find("GroundPic/effect").gameObject.SetActive(false);
        backGroundGO.transform.Find("GroundPic/Sprite").gameObject.SetActive(true);
        
    }

}
