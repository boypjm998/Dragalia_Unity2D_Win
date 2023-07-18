using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMoveController_HB02 : EnemyMoveManager
{
    // Start is called before the first frame update
    protected VoiceController_HB02 voice;
    public int auspexGauge = 0;

    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerHumanoid>();
        voice = GetComponentInChildren<VoiceController_HB02>();
    }

    /// <summary>
    /// comboA
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action01()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);
        
        ac.anim.Play("combo1");
        voice.PlayMyVoice(VoiceController_HB02.myMoveList.ComboA);

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f);
        Combo1();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        ac.anim.Play("combo2");

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f);
        Combo2();//之后改成combo2
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ac.anim.Play("idle");
        
        yield return new WaitForSeconds(0.5f);
        QuitAttack();
    }

    /// <summary>
    /// 荣耀圣域
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action02()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB02_Action02");
        yield return new WaitForSeconds(1f);
        
        ac.anim.Play("s1");
        voice.PlayMyVoice(VoiceController_HB02.myMoveList.GloriousSanctuary);
        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        
        GloriousSanctuary();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        ac.anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// ComboB
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action03()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);
        
        ac.anim.Play("combo3");
        voice.PlayMyVoice(VoiceController_HB02.myMoveList.ComboB);

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (2f/12f));
        Combo3();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        ac.anim.Play("combo4");

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.275f);
        Combo4();//之后改成combo2
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ac.anim.Play("idle");
        
        yield return new WaitForSeconds(0.5f);
        QuitAttack();
    }
    
    /// <summary>
    /// ComboC(5)
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator HB02_Action04()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);
        
        ac.anim.Play("combo5");
        voice.PlayMyVoice(VoiceController_HB02.myMoveList.ComboC);

        yield return null;
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3f);
        var hint = GenerateWarningPrefab(WarningPrefabs[0], transform.position, transform.rotation, MeeleAttackFXLayer.transform);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f);
        Combo5();
        Destroy(hint);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        ac.anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Dash Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action05()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("dash");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.Dash);

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.167f);
        
        DashAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.24f);
        if (anim.GetBool("isGround"))
        {
            (ac as EnemyControllerHumanoid).InertiaMove(0.4f);
        }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        ac.anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Holy Crown
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action06()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("s2");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.HolyCrown);
        bossBanner?.PrintSkillName("HB02_Action06");
        
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (4/21f));
        
        HolyCrown_Muzzle();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.735f);
        
        HolyCrown();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// TwilightMoon
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action07()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.SetMove(0);
        ac.isAction = true;
        bossBanner?.PrintSkillName("HB02_Action07");
        ac.OnAttackEnter(200);
        _behavior.breakable = false;

        yield return new WaitForSeconds(0.5f);
        
        var currentPlatform = BasicCalculation.CheckRaycastedPlatform(gameObject);
        var leftbound = currentPlatform.bounds.min.x;
        var rightbound = currentPlatform.bounds.max.x;
        
        if(leftbound < BattleStageManager.Instance.mapBorderL)
            leftbound = BattleStageManager.Instance.mapBorderL;
        if(rightbound > BattleStageManager.Instance.mapBorderR)
            rightbound = BattleStageManager.Instance.mapBorderR;
        
        var marginLimit = rightbound - leftbound >= 7f ? 7f : (rightbound - leftbound) * 0.33f;
        //print(marginLimit);
        bool forcedReady = false;
        var target = _behavior.targetPlayer;
        var minDistance = 7f;
        var maxDistance = 15f;

        var posY = BasicCalculation.GetRaycastedPlatformY(target) + 4f;
        
        
        ac.TurnMove(target);
        anim.Play("forcing_enter");
        if (_behavior.difficulty == 4)
        {
            BattleEffectManager.Instance.SpawnTargetLockIndicator(target,2f);
        }

        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("forcing_idle"));
        
        var forcingPrefab = Instantiate(projectilePoolEX[1],new Vector3(target.transform.position.x,
            posY,target.transform.position.z),Quaternion.identity,RangedAttackFXLayer.transform);

        var forctingController = forcingPrefab.GetComponent<Projectile_C003_6_Boss>();
        forctingController.target = target;
        forctingController.SetEnemySource(gameObject);

        while (forctingController.forceLevel < 5)
        {
            var distance = target.transform.position.x - transform.position.x;
            if ((distance > minDistance && distance < maxDistance) || (distance < -minDistance && distance > -maxDistance))
            {
                //在右边且距离大于最小距离
                ac.SetMove(0);
                
            }
            else if(distance <= minDistance && distance >= -minDistance)
            {
                //目标在自身的右边
                if (distance > 0)
                {
                    if (target.transform.position.x - leftbound > marginLimit)
                    {
                        //1.如果目标在自身的右边，且目标远离左边界，则向左移动。
                        ac.SetFaceDir(-1);
                        ac.SetMove(0.5f);
                        //print("正在进行操作1");
                    }
                    else
                    {
                        //2.如果目标在自身的右边，且目标靠近左边界，则向右移动。
                        ac.SetFaceDir(1);
                        ac.SetMove(0.5f);
                        //print("正在进行操作2");
                    }
                }
                else
                {
                    if(target.transform.position.x - rightbound < -marginLimit)
                    {
                        //3.如果目标在自身的左边，且目标远离右边界，则向右移动。
                        ac.SetFaceDir(1);
                        ac.SetMove(0.5f);
                        //print("正在进行操作3");
                    }
                    else
                    {
                        //4.如果目标在自身的左边，且目标靠近右边界，则向左移动。
                        ac.SetFaceDir(-1);
                        ac.SetMove(0.5f);
                        //print("正在进行操作4");
                    }
                    
                }
            }
            else if (distance >= maxDistance || distance <= -maxDistance)
            {
                ac.TurnMove(target);
                ac.SetMove(0.5f);
            }
            else
            {
                ac.SetMove(0);
            }
            
            if (transform.position.x <= leftbound)
            {
                ac.SetFaceDir(1);
                ac.SetMove(0);
                print("正在进行操作5");
            }
            else if (transform.position.x >= rightbound)
            {
                ac.SetFaceDir(-1);
                ac.SetMove(0);
                print("正在进行操作6");
            }

            //maxFollowTime -= Time.deltaTime;

            yield return null;
        }
        
        ac.SetMove(0);
        anim.Play("forcing_idle");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.TwilightMoon,true);
        forctingController.DisplayHint();
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("force_strike");
        forctingController.HideForcingHint();
        
        yield return new WaitForSeconds(0.5f);
        
        forctingController.HideHint();
        forctingController.StartAttack();
        auspexGauge = 0;
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.TwilightMoon, true);
        Destroy(forcingPrefab,2f);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        anim.Play("idle");
        _behavior.breakable = true;
        QuitAttack();
    }

    /// <summary>
    /// Twilight Crown
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action08()
    { 
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);

        var hint = GenerateWarningPrefab(WarningPrefabs[1],transform.position + new Vector3(0,-1f,0),
            Quaternion.identity,MeeleAttackFXLayer.transform);
        bossBanner?.PrintSkillName("HB02_Action08");
        
        yield return new WaitForSeconds(1.4f);
        Destroy(hint,0.8f);
        
        anim.Play("s3");

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.125f);

        var container = TwilightCrown_Wave();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.TwilightCrown);
        
        yield return new WaitForSeconds(0.4f);
        
        TwilightCrown_Star(container);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        QuitAttack();
        
    }

    /// <summary>
    /// Celestial Prayer
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action09()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);

        var groundPos = BasicCalculation.GetRaycastedPlatformY(_behavior.targetPlayer) + 3f;
        var pos = new Vector3(_behavior.targetPlayer.transform.position.x, groundPos,
            _behavior.targetPlayer.transform.position.z);

        var hint = GenerateWarningPrefab(WarningPrefabs[2], pos, UnityEngine.Quaternion.identity,
            RangedAttackFXLayer.transform);
            
        bossBanner?.PrintSkillName("HB02_Action09");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.CelestialPrayer);
        //yield return new WaitForSeconds(0.5f);
        anim.Play("s4");
        Destroy(hint,0.5f);
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.23f);
        CelestialPrayer(pos);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Backwarp Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action10()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);
        DisappearRenderer();
        ac.SetHitSensor(false);
        
        Warp_Effect();
        var dir = _behavior.targetPlayer.GetComponent<ActorBase>().facedir;
        ac.SetFaceDir(dir);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1.5f);

        yield return new WaitForSeconds(1f);
        Warp_Attack(dir);
        ac.SetHitSensor(true);
        anim.Play("dash");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.Dash);

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.167f);
        
        DashAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.24f);
        if (anim.GetBool("isGround"))
        {
            (ac as EnemyControllerHumanoid).InertiaMove(0.4f);
        }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        ac.anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// 信念增幅
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action11()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.FaithEnhancement,true);
        bossBanner?.PrintSkillName("HB02_Action11");
        
        yield return new WaitForSeconds(1f);
        
        anim.Play("buff");
        Warp_Effect();
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.15f);
        FaithEnhancement();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        QuitAttack();

    }

    /// <summary>
    /// 大地屏障
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action12()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.MountainPrison,true);
        bossBanner?.PrintSkillName("HB02_Action12");
        
        StageCameraController.SwitchOverallCamera();
        yield return new WaitForSeconds(1.5f);
        anim.Play("s4");
        yield return null;

        var barrier = EarthBarrier();
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        anim.Play("idle");

        yield return new WaitForSeconds(2f);
        StageCameraController.SwitchMainCamera();
        
        QuitAttack();
    }

    /// <summary>
    /// twilight crown + twilight moon
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action13()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB02_Action08");
        var hint =  GenerateWarningPrefab(WarningPrefabs[3], transform.position-new Vector3(0,1,0), Quaternion.identity, MeeleAttackFXLayer.transform);
        yield return new WaitForSeconds(0.4f);
        
        anim.Play("s3");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.TwilightCrown);
        Destroy(hint,1f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.125f);
        var container = TwilightCrown_Wave();
        
        yield return new WaitForSeconds(0.4f);
        TwilightCrown_Star(container,true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        var fsPosX = Quick_TwilightMoonCheck();
        
        anim.Play("forcing_enter");
        
        bossBanner.PrintSkillName("HB02_Action07");
        
        var target = _behavior.targetPlayer;
        var posY = BasicCalculation.GetRaycastedPlatformY(target) + 4f;
        var forcingPrefab = Instantiate(projectilePoolEX[1],new Vector3(fsPosX,
            posY,target.transform.position.z),Quaternion.identity,RangedAttackFXLayer.transform);
        ac.TurnMove(forcingPrefab);

        var forctingController = forcingPrefab.GetComponent<Projectile_C003_6_Boss>();
        forctingController.SetEnemySource(gameObject);
        forctingController.isLocking = false;

        yield return new WaitUntil(() => forctingController.forceLevel >= 1);
        anim.Play("force_strike");
        forctingController.DisplayHint();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.TwilightMoon,true);
        yield return new WaitForSeconds(0.4f);
        forctingController.StartAttack();
        forctingController.HideHint();
        forctingController.HideForcingHint();
        var attack = forcingPrefab.GetComponentInChildren<CustomRangedFromEnemy>();
        auspexGauge = 0;
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.TwilightMoon, true);
        attack.attackInfo[0].dmgModifier[0] = 0.8f + 0.2f * forctingController.forceLevel;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        QuitAttack();
        
    }

    public void HB02_PhaseShift(float waitTime)
    {
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.LastPhase,true);
    }

    /// <summary>
    /// To legend Phase2
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action14()
    {
        Warp_Effect();
        StageCameraController.SwitchMainCamera();
        
        yield return new WaitForSeconds(0.25f);
        
        DisappearRenderer();
        voice?.BroadCastMyVoice(1);
        transform.position = Vector3.zero + new Vector3(0,3,0);
        ac.SetGravityScale(0);
        SetGroundCollider(false);
        yield return new WaitForSeconds(4f);
        yield return new WaitUntil(()=>voice.voice.isPlaying == false);
        
        anim.Play("float_in");
        yield return null;
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        
        ac.TurnMove(_behavior.targetPlayer);
        CineMachineOperator.Instance.StopCameraShake();
        CineMachineOperator.Instance.CamaraShake(2f,30f);
        AppearRenderer();
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("float");
        voice?.BroadCastMyVoice(2);
        var backGroundGO = GameObject.Find("Background");
        var backGround1Sprite = backGroundGO.transform.Find("Background1").GetComponent<SpriteRenderer>();
        backGround1Sprite.DOColor(Color.clear, 3f);
        Instantiate(projectilePoolEX[8],transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        yield return new WaitForSeconds(4f);
        yield return new WaitUntil(()=>voice.voice.isPlaying == false);
        voice?.BroadCastMyVoice(3);
        bossBanner?.PrintSkillName("HB02_Action14");
        yield return new WaitForSeconds(3.5f);
        anim.Play("smash_down");
        
        yield return new WaitForSeconds(0.1f);
        var _tweener = transform.DOMoveY(-1f, 0.75f);
        SetGroundCollider(true);
        _tweener.SetEase(Ease.InCubic);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.42f);
        
        CineMachineOperator.Instance.StopCameraShake();
        CineMachineOperator.Instance.CamaraShake(15f,0.5f);
        Instantiate(projectilePoolEX[9],transform.position-new Vector3(-ac.facedir,3,0),Quaternion.identity,RangedAttackFXLayer.transform);

        var whiteScreenImage = GameObject.Find("FullScreenEffect").transform.Find("BlackIn").GetComponent<Image>();
        whiteScreenImage.color = new Color(1, 1, 1, 0);
        
        yield return new WaitForSeconds(0.3f);
        whiteScreenImage.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(1f);
        backGroundGO.transform.Find("Background2").gameObject.SetActive(false);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        yield return new WaitUntil(()=>voice.voice.isPlaying == false);
        QuitAttack();
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        backGroundGO.transform.Find("GroundPic/effect").gameObject.SetActive(true);
        backGroundGO.transform.Find("GroundPic/Sprite").gameObject.SetActive(false);
        whiteScreenImage.DOFade(0, .5f);
    }

    /// <summary>
    /// spin attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action15()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        var hint = GenerateWarningPrefab(WarningPrefabs[4],transform.position+new Vector3(2.5f*ac.facedir,-2.5f,0),
            ac.facedir==1?Quaternion.Euler(0,0,0):Quaternion.Euler(0,180,0),RangedAttackFXLayer.transform);

        var hintbar = hint.GetComponent<EnemyAttackHintBarRect2D>();
        yield return new WaitForSeconds(hintbar.warningTime-0.8f);
        Destroy(hint,1.3f);
        
        anim.Play("spin");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.48f);
        SpinDash();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.Dash);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        QuitAttack();
    }









    #region Functions

    

    

    protected void Combo1()
    {
        var proj= Instantiate(projectile1,
            transform.position + ac.facedir * new Vector3(1, 0, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        if (ac.facedir == -1)
        {
            proj.transform.localScale = new Vector3(-1, 1, 1);
        }

        var attacks = proj.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var attack in attacks)
        {
            attack.enemySource = gameObject;
            attack.firedir = ac.facedir;
        }
    }
    
    protected void Combo2()
    {
        var proj= Instantiate(projectile2,
            transform.position + ac.facedir * new Vector3(1, 0, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        if (ac.facedir == -1)
        {
            proj.transform.localScale = new Vector3(-1, 1, 1);
        }

        var attacks = proj.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var attack in attacks)
        {
            attack.enemySource = gameObject;
            attack.firedir = ac.facedir;
        }
    }

    protected void Combo3()
    {
        var proj= Instantiate(projectile5,
            transform.position + ac.facedir * new Vector3(1, 0, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        if (ac.facedir == -1)
        {
            proj.transform.localScale = new Vector3(-1, 1, 1);
        }

        var attacks = proj.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var attack in attacks)
        {
            attack.enemySource = gameObject;
            attack.firedir = ac.facedir;
        }
    }

    protected void Combo4()
    {
        var proj= Instantiate(projectile6,
            transform.position + ac.facedir * new Vector3(1, 0, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        if (ac.facedir == -1)
        {
            proj.transform.localScale = new Vector3(-1, 1, 1);
        }

        var attacks = proj.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var attack in attacks)
        {
            attack.enemySource = gameObject;
            attack.firedir = ac.facedir;
        }
    }

    protected void Combo5()
    {
        var container = Instantiate(attackContainer,
            transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        var main_proj= Instantiate(projectile7,
            transform.position + ac.facedir * new Vector3(1f, 0, 0), Quaternion.identity,
            container.transform);
        main_proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        main_proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        main_proj.GetComponent<HomingAttack>().target = _behavior.targetPlayer.transform;
        main_proj.GetComponent<HomingAttack>().firedir = ac.facedir;
        

        var other_projs = new List<GameObject>();
        var angles = new Vector2[] 
            { new Vector2(2,1),new Vector2(2,-1),new Vector2(1,2),new Vector2(1,-2)};
        
        for (int i = 0; i < 4; i++)
        {
            var proj = Instantiate(projectile8,
                transform.position + ac.facedir * new Vector3(1f, 0, 0), Quaternion.identity,
                container.transform);
            other_projs.Add(proj);
            proj.GetComponent<HomingAttack>().target = _behavior.targetPlayer.transform;
            proj.GetComponent<HomingAttack>().angle = angles[i] * ac.facedir;
            proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
            proj.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
            proj.GetComponent<HomingAttack>().firedir = ac.facedir;
        }
    }

    protected void DashAttack()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        var proj = InstantiateMeele(projectile9, transform.position, container);
    }

    protected void GloriousSanctuary()
    {
        var fx = Instantiate(projectile3,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        var proj = Instantiate(projectile4,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);

        // var other_sanctuary = FindObjectOfType<Projectile_C003_1_Boss>();
        // if(other_sanctuary != null)
        //     Destroy(other_sanctuary.gameObject);

        var sanctuary = proj.GetComponent<Projectile_C003_1_Boss>();
        sanctuary.InitPotencyInfo(_statusManager);
        sanctuary.BossGameObject = gameObject;

        if (_behavior.difficulty == 4)
        {
            _statusManager.HPRegenImmediately(110,0);
            _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HealOverTime),15,15);
        }
        else
        {
            _statusManager.HPRegenImmediately(11,0);
            _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HealOverTime),3,15);
        }


        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff),15,60,1,0);
        
        CheckTwilightMoon();
    }

    protected void HolyCrown_Muzzle()
    {
        var fx = Instantiate(projectile10, transform.position + new Vector3(ac.facedir, 2f, 0), Quaternion.identity,
            MeeleAttackFXLayer.transform);
    }

    protected void HolyCrown()
    {
        // var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
        //     RangedAttackFXLayer.transform);
        var proj = Instantiate(projectilePoolEX[0], transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        if (ac.facedir == -1)
        {
            proj.transform.rotation = 
                Quaternion.Euler(proj.transform.rotation.eulerAngles.x,
                    180,proj.transform.rotation.eulerAngles.z);
            proj.transform.GetComponent<Projectile_C003_3_Boss>().firedir = -1;
        }
        for (int i = 0; i < proj.transform.childCount; i++)
        {
            proj.transform.GetChild(i).GetComponent<Projectile_C003_2_Boss>().enemySource = gameObject;
        }
        var holyfaithBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyFaith, -1f, 20f, 1);
        _statusManager.ObtainTimerBuff(holyfaithBuff);
        proj.GetComponent<AttackContainer>().InitAttackContainer(10,false);
        CheckTwilightMoon();

    }

    protected GameObject TwilightCrown_Wave()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            MeeleAttackFXLayer.transform);
        var proj = Instantiate(projectilePoolEX[2], transform.position+new Vector3(0,-1,0),Quaternion.identity,
            container.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(2,false);
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.RecoveryBuff,20,10);
        CheckTwilightMoon();
        
        
        return container;
    }

    protected void TwilightCrown_Star(GameObject container,bool boost=false)
    {
        if (boost)
        {
            InstantiateMeele(projectilePoolEX[7], transform.position, container);
            return;
        }

        var proj = InstantiateMeele(projectilePoolEX[3], transform.position,
            container);
    }

    protected void CelestialPrayer(Vector2 position,bool checkTwilightMoon = true)
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        var proj = Instantiate(projectilePoolEX[4], position,Quaternion.identity,
            container.transform);
        proj.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        
        if(checkTwilightMoon)
            CheckTwilightMoon();
        
    }

    protected void Warp_Effect()
    {
        var fx = Instantiate(projectilePoolEX[6], transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
    }

    protected void Warp_Attack(int dir, bool ydir = true)
    {
        AppearRenderer();

        if (ydir)
        {
            transform.position = new Vector3(_behavior.targetPlayer.transform.position.x - dir,
                _behavior.targetPlayer.transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(_behavior.targetPlayer.transform.position.x - dir,
                transform.position.y, transform.position.z);
        }


        Instantiate(projectilePoolEX[6], transform.position, Quaternion.identity,
            MeeleAttackFXLayer.transform);
    }

    protected void FaithEnhancement()
    {
        var fsBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ForceStrikeDmgBuff, 200, 15,100);
        fsBuff.dispellable = false;
        var skillDmgBuff = new TimerBuff((int)BasicCalculation.BattleCondition.SkillDmgBuff, 180, 15, 100);
        skillDmgBuff.dispellable = false;
        var skillDmgBuff2 = new TimerBuff((int)BasicCalculation.BattleCondition.SkillDmgBuff, 10, -1, 100);
        _statusManager.ObtainTimerBuff(fsBuff);
        _statusManager.ObtainTimerBuff(skillDmgBuff);
        _statusManager.ObtainTimerBuff(skillDmgBuff2,false);
        var holyfaithBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyFaith, -1f, 20f, 1);
        _statusManager.ObtainTimerBuff(holyfaithBuff,false);
    }

    protected GameObject EarthBarrier()
    {
        var posX = _behavior.targetPlayer.transform.position.x;
        var proj = Instantiate(projectilePoolEX[5], new Vector3(posX, 0, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        proj.name = "mountain";
        return proj;
    }

    protected float Quick_TwilightMoonCheck()
    {
        //向左右两边发射一条射线，检测Border层,并判断哪边距离近.
        var left = Physics2D.Raycast(transform.position, Vector2.left, 100, LayerMask.GetMask("Border"));
        var right = Physics2D.Raycast(transform.position, Vector2.right, 100, LayerMask.GetMask("Border"));
        //如果左边更近，返回左边的点+6.5f,如果右边更近，返回右边的点-6.5f
        if (left.distance > right.distance)
        {
            return left.point.x + 6.5f;
        }
        else
        {
            return right.point.x - 6.5f;
        }
        
    }

    protected void SpinDash()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        var proj = InstantiateRanged(projectilePoolEX[10],
            transform.position + new Vector3(2.5f * ac.facedir, -1.5f, 0), 
            container, ac.facedir);
    }

    protected void CheckTwilightMoon()
    {
        auspexGauge++;
        if (auspexGauge == 2)
        {
            var twilightmoonbuff = new TimerBuff((int)BasicCalculation.BattleCondition.TwilightMoon, -1, -1, 1, -1);
            twilightmoonbuff.dispellable = false;
            _statusManager.ObtainTimerBuff(twilightmoonbuff);
        }
    }
    
    public override void DisappearRenderer()
    {
        ac.rendererObject.SetActive(false);
        ac.SwapWeaponVisibility(false);
        ac.minimapIcon?.SetActive(false);
        ac.shadowCaster?.gameObject.SetActive(false);
    }

    public override void AppearRenderer()
    {
        ac.rendererObject.SetActive(true);
        ac.SwapWeaponVisibility(true);
        ac.minimapIcon?.SetActive(true);
        ac.shadowCaster?.gameObject.SetActive(true);
    }

    protected override void QuitAttack()
    {
        base.QuitAttack();
    }
    
    #endregion

    public override void PlayVoice(int id)
    {
        
        if (id == 0)
        {
            voice.BroadCastMyVoice(0);
        }
    }
    

}
