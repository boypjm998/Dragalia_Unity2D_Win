using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMoveController_HB04 : EnemyMoveManager
{
    protected VoiceControllerEnemy voice;
    protected float timeTick = 1;
    protected Tween _tween;
    public List<Projectile_C019_4_Boss> blessedWalls = new();

    public enum MyVoiceGroup
    {
        Combo1,
        Combo3,
        Combo5,
        BlazingFount1,
        BlazingFount2,
        AffectionRing,
        Projectile,
        Buff,
        HPBelow70,
        Defeat,
        ChangePhase
    }

    //private MyVoiceGroup[] _voiceGroups;

    protected override void Awake()
    {
        base.Awake();
        voice = GetComponentInChildren<VoiceControllerEnemy>();
        GetAllAnchors();
    }

    protected override void Start()
    {
        base.Start();
        OnInit();
    }

    protected virtual void OnInit()
    {
        _statusManager.OnHPDecrease += CheckHealBuff;
        BattleStageManager.Instance.AddFieldAbility(20111);
        _statusManager.OnReviveOrDeath += (()=>voice?.BroadCastMyVoice((int) MyVoiceGroup.Defeat));
    }

    protected void CheckHealBuff(int dmg, AttackBase atkBase)
    {
        if(timeTick <= 0)
            return;
        if (_statusManager.currentHp < (int)_statusManager.maxHP * 0.3f)
        {
            timeTick = 0;
            BattleStageManager.Instance.RemoveFieldAbility(20111);

            var buffNum = _statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.PowerOfBonds);

            _statusManager.HPRegenImmediately(50 + buffNum * 50,0,false);
            Instantiate(GetProjectileOfName("fx_ability_recover_c019"),
                transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
            _tween?.Complete();
            _tween = DOVirtual.DelayedCall(60, () =>
            {
                timeTick = 1;
                BattleStageManager.Instance.AddFieldAbility(20111);
                _statusManager.OnHPDecrease?.Invoke(0,null);
            },false);
        }
    }

    /// <summary>
    /// Combo A (1 - 3)
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action01()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        var hintBar = GenerateWarningPrefab
        ("action01_1", transform.position + new Vector3(1.5f * ac.facedir, 0), Quaternion.identity,
            MeeleAttackFXLayer.transform);

        var hintTime = hintBar.GetComponent<EnemyAttackHintBarRect2D>().warningTime;

        yield return new WaitForSeconds(hintTime - 0.2f);

        anim.Play("combo1");

        var position = _behavior.targetPlayer.RaycastedPosition();
        var hintBar2 = GenerateWarningPrefab("action01_2",
            position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);

        yield return new WaitForSeconds(0.3f);


        Combo1_Attack();

        //waitforsecond(0.367f);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        anim.Play("combo2");

        yield return new WaitForSeconds(0.5f);

        Combo2_3_Attack(position);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);

        anim.Play("combo3");
        voice?.PlayMyVoice((int)MyVoiceGroup.Combo3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();

    }


    /// <summary>
    /// Combo B (4 - 5)
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action02()
    {
        yield return _canActionOnGround;
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        var position = _behavior.targetPlayer.RaycastedPosition();

        var hintBar = GenerateWarningPrefab
        ("action02_1", position, Quaternion.identity,
            RangedAttackFXLayer.transform);

        var hintTime = hintBar.GetComponent<EnemyAttackHintBarRect2D>().warningTime;

        yield return new WaitForSeconds(hintTime - 0.2f);

        anim.Play("combo4");



        yield return new WaitForSeconds(0.3f);

        Combo4_5_Attack(position);
        voice?.PlayMyVoice((int)MyVoiceGroup.Combo5);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.48f);

        anim.Play("combo5");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f);
        
        ac.SwapWeaponVisibility(false);

        Instantiate(GetProjectileOfFormatName("action02_3"), transform.position + new Vector3(ac.facedir, 0.5f),
            Quaternion.identity,RangedAttackFXLayer.transform);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f);
        
        ac.SwapWeaponVisibility(true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }

    /// <summary>
    /// Skill 1: 闪光烈焰
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action03()
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action03");
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(0.5f);

        ac.OnAttackEnter(999);

        anim.Play("s1");

        yield return new WaitForSeconds(0.2f);

        (ac as EnemyControllerHumanoidHigh).SwapWeaponVisibility(false);


        yield return new WaitForSeconds(0.3f);

        BlazingFount_Muzzle();
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 3f);

        float delay = 1.75f;
        var targetPlayer = _behavior.targetPlayer;
        DOVirtual.DelayedCall(delay, () => BlazingFount_Attack(targetPlayer), false);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f);
        (ac as EnemyControllerHumanoidHigh).SwapWeaponVisibility(true);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }

    /// <summary>
    /// 慈爱之环 Ring Of Affection
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action04(bool cameraFollow = true)
    {
        yield return _canActionOnGround;

        ac.SetHitSensor(false);
        ac.OnAttackEnter(999);


        bossBanner?.PrintSkillName("HB04_Action04");
        ac.TurnMove(_behavior.targetPlayer);

        if (cameraFollow)
        {
            StageCameraController.SwitchMainCameraFollowObject(gameObject);
            DOVirtual.DelayedCall(3f,
                () => StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer),
                false);
        }

        yield return new WaitForSeconds(1f);

        anim.Play("s2");
        ac.SwapWeaponVisibility(false);
        RingOfAffection_Effect();

        yield return new WaitForSeconds(0.24f);

        ac.SwapWeaponVisibility(true);

        yield return new WaitForSeconds(0.26f);

        RingOfAffection();
        _statusManager.ImmuneToAllControlAffliction = true;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        ac.SetHitSensor(true);

        QuitAttack();
    }



    /// <summary>
    /// Skill 1 Boost: 圣光烈焰
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action05()
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action05");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        
        yield return new WaitForSeconds(0.5f);

        

        anim.Play("s1");

        yield return new WaitForSeconds(0.2f);

        (ac as EnemyControllerHumanoidHigh).SwapWeaponVisibility(false);


        yield return new WaitForSeconds(0.3f);

        BlazingFount_Muzzle(2);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer, 3f);

        float delay = 1.75f;
        var targetPlayer = _behavior.targetPlayer;
        DOVirtual.DelayedCall(delay, () => BlazingConsecration_Attack(targetPlayer), false);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f);
        (ac as EnemyControllerHumanoidHigh).SwapWeaponVisibility(true);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();
    }



    /// <summary>
    /// Blessed Wall
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action06(int dir = 1)
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action06");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(1f);

        float tweenTime = .5f;

        var posX = BlessedWallHint(dir, 4, out tweenTime);

        yield return new WaitForSeconds(tweenTime - 0.5f);

        anim.Play("s3");

        yield return new WaitForSeconds(0.5f);

        BlessedWallAttack(dir, posX);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }



    /// <summary>
    /// Stunning glare
    /// </summary>
    /// <param name="punishment"></param>
    /// <param name="needGround"></param>
    /// <returns></returns>
    public IEnumerator HB04_Action07(bool punishment, bool needGround = false)
    {
        yield return _canAction;

        bossBanner?.PrintSkillName("HB04_Action07");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        if (punishment)
        {
            ac.SetGravityScale(0);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        
        ac.rigid.velocity = Vector2.zero;
        var lockedDir = _behavior.viewerPlayer.transform.localScale.x;
        
        
        anim.Play("float");

        yield return new WaitForSeconds(0.4f);
        WarpEffect();
        yield return new WaitForSeconds(0.1f);
        
        ac.DisappearRenderer();

        Vector3 warpPos;

        if (needGround)
        {
            var targetCol = _behavior.viewerPlayer.RaycastedPlatform();
            if (lockedDir == 1)
            {
                warpPos = new Vector3(
                    Mathf.Min(targetCol.bounds.max.x - 1,_behavior.viewerPlayer.transform.position.x + 8),
                    _behavior.viewerPlayer.transform.position.y);
            }
            else
            {
                warpPos = new Vector3(
                    Mathf.Max(targetCol.bounds.min.x + 1,_behavior.viewerPlayer.transform.position.x - 8),
                    _behavior.viewerPlayer.transform.position.y);
            }
        }else{
        warpPos = _behavior.viewerPlayer.transform.position +
                          new Vector3(lockedDir * 8, 0, 0);
        }

        
        
        transform.position = warpPos.SafePosition(Vector2.zero);
        WarpEffect();
        yield return new WaitForSeconds(0.1f);
        
        
        ac.AppearRenderer();
        ac.TurnMove(_behavior.viewerPlayer);

        int difficultyModifier = _behavior.difficulty < 4 ? 1 : 3;

        StunningGlareAttack(punishment?_behavior.difficulty * 2:difficultyModifier);

        yield return new WaitForSeconds(1f);
        
        ac.ResetGravityScale();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();


    }
    
    /// <summary>
    /// Stunning glare (Fixed Position)
    /// </summary>
    /// <param name="punishment"></param>
    /// <param name="needGround"></param>
    /// <returns></returns>
    public IEnumerator HB04_Action07_V()
    {
        yield return _canAction;

        bossBanner?.PrintSkillName("HB04_Action07");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        
        yield return new WaitForSeconds(1f);
        
        
        ac.rigid.velocity = Vector2.zero;
        var lockedDir = _behavior.viewerPlayer.transform.localScale.x;
        
        
        anim.Play("float");

        yield return new WaitForSeconds(0.8f);
        
        ac.TurnMove(_behavior.viewerPlayer);
        

        StunningGlareAttack(_behavior.difficulty);

        yield return new WaitForSeconds(1f);
        
        ac.ResetGravityScale();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
        
    }
    
    
    
    /// <summary>
    /// 空灵长枪3型
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action08()
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action08");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(1.5f);
        

        anim.Play("s3");
        
        ProjectilesTypeIII();

        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }


    public IEnumerator HB04_Action09()
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action08");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(1.5f);
        

        anim.Play("s3");
        
        ProjectilesTypeI();

        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }
    
    public IEnumerator HB04_Action10()
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action08");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(1.5f);
        

        anim.Play("s3");
        
        ProjectilesTypeII();

        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }


    /// <summary>
    /// 瞬移
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action11(Vector2 warpPosition, bool nearPlatform = false)
    {
        yield return _canAction;

        if (nearPlatform)
        {
            var nodes = AStar.GetAllNodes();
            float nearestDistance = 999;
            Collider2D nearestCollider = null;
            foreach (var node in nodes)
            {
                if(node.platform.collider.name == gameObject.RaycastedPlatform().name)
                    continue;
                var colliderDistance = ac.HitSensor.Distance(node.platform.collider).distance;
                //print($"距离{node.platform.collider}有{colliderDistance}的距离。");
                if (colliderDistance < nearestDistance)
                {
                    nearestDistance = colliderDistance;
                    nearestCollider = node.platform.collider;
                }
                
            }
            
            print("nearestCollider is" + nearestCollider);
            print(nearestDistance);

            float targetPosX = nearestCollider.bounds.center.x;
            
            

            // if(nearestCollider.bounds.max.x < transform.position.x || nearestCollider.bounds.min.x > transform.position.x)
            //     targetPosX = Mathf.Abs(nearestCollider.bounds.max.x - transform.position.x) < 
            //              Mathf.Abs(nearestCollider.bounds.min.x - transform.position.x) ?
            //             nearestCollider.bounds.max.x : nearestCollider.bounds.min.x;
            
            warpPosition = new Vector2(targetPosX, nearestCollider.bounds.max.y + 1.3f);
            print(warpPosition);
            print(nearestCollider.bounds.max.y);
        }



        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        WarpEffect();
        yield return new WaitForSeconds(0.1f);
        
        ac.DisappearRenderer();
        
        transform.position = warpPosition.SafePosition(Vector2.zero);
        WarpEffect();
        yield return new WaitForSeconds(0.1f);
        
        ac.AppearRenderer();
        ac.TurnMove(_behavior.viewerPlayer);
        
        QuitAttack();
    }

    public IEnumerator HB04_Action11_V(float xPos)
    {
        yield return _canAction;
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        WarpEffect();
        yield return new WaitForSeconds(0.1f);
        
        ac.DisappearRenderer();
        Vector3 warpPosition;

        if (_behavior.targetPlayer.transform.position.x < 0)
        {
            warpPosition = new Vector3(xPos, transform.position.y, 0);
        }else
        {
            warpPosition = new Vector3(-xPos, transform.position.y, 0);
        }
        
        
        transform.position = warpPosition.SafePosition(Vector2.zero);
        WarpEffect();
        yield return new WaitForSeconds(0.1f);
        
        ac.AppearRenderer();
        ac.TurnMove(_behavior.viewerPlayer);
        
        QuitAttack();
    }
    
    /// <summary>
    /// Blessed Wall Fixed
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action06_V(float distance)
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action06");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        StageCameraController.SwitchOverallCamera();
        DOVirtual.DelayedCall(4f, () => StageCameraController.SwitchMainCamera(),false);

        yield return new WaitForSeconds(1f);

        float tweenTime;

        var hints = BlessedWallHintDoubleDirection();

        yield return new WaitForSeconds(1f);
        
        var twL = BlessedWallHintDoubleDirectionL( hints[0],distance,1, out tweenTime);
        var hintR = BlessedWallHintDoubleDirectionR( hints[1],distance,1, out tweenTime);

        yield return new WaitForSeconds(tweenTime);

        anim.Play("s3");

        yield return new WaitForSeconds(1);

        BlessedWallAttack(1, hintR.transform.position.x - 2 * distance,true);
        BlessedWallAttack(-1, hintR.transform.position.x,true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");

        QuitAttack();


    }
    
    public IEnumerator HB04_Action11(bool up = true)
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action11");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);

        yield return new WaitForSeconds(.5f);
        
        if(up)
            UprisingStars();
        else
            FallingStars();

        anim.Play("float");
        
        yield return new WaitForSeconds(0.5f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }

    /// <summary>
    /// Buff
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action12(float buffAmount1, float buffAmount2)
    {
        yield return _canActionOnGround;

        bossBanner?.PrintSkillName("HB04_Action12");
        ac.TurnMove(_behavior.targetPlayer);
        ac.OnAttackEnter(999);
        

        anim.Play("float");
        
        yield return new WaitForSeconds(0.5f);
        
        PowerUp(buffAmount1, buffAmount2);
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();
    }



    /// <summary>
    /// Light Ball
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action13()
    {
        yield return _canActionOnGround;
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB04_Action13");
        ac.TurnMove(_behavior.targetPlayer);
        
        yield return new WaitForSeconds(0.5f);

        var hint = GenerateWarningPrefab("action13_1",
            _behavior.targetPlayer.RaycastedPosition() +
            new Vector2(0, 1.5f), Quaternion.identity,
            MeeleAttackFXLayer.transform);

        var pos = hint.transform.position;
        
        
        var targetPlatform = AStar.GetNodeOfName
            (_behavior.targetPlayer.RaycastedPlatform().name).platform;
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("combo2");

        yield return new WaitForSeconds(0.6f);
        
        InstantiateLightBall(pos, targetPlatform);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        anim.Play("idle");
        
        QuitAttack();

    }

    /// <summary>
    /// 创世巫女
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB04_Action14()
    {
        _statusManager.OnHPDecrease = null;
        
        ac.rigid.velocity = Vector2.zero;
        //StageCameraController.SwitchMainCamera();
        yield return new WaitForSeconds(0.25f);
        
        WarpEffect();
        
        yield return new WaitForSeconds(0.25f);
        
        DisappearRenderer();
        ac.SwapWeaponVisibility(false);
        
        transform.position = new Vector2(0, 8);
        ac.SetFaceDir(1);

        while (!UI_DialogDisplayer.Instance.IsEmpty)
        {
            yield return null;
            print("wait for voice");
        }

        

        yield return new WaitForSeconds(0.5f);
        

        voice?.BroadCastSpecificVoice((int)MyVoiceGroup.ChangePhase,0);
        
        yield return new WaitForSeconds(2f);

        
        
        yield return new WaitUntil(()=>voice.voice.isPlaying == false);
        StageCameraController.SwitchMainCameraFollowObject(gameObject);
        
        WarpEffect();
        
        yield return new WaitForSeconds(0.25f);
        
        AppearRenderer();
        //ac.SwapWeaponVisibility(true);
        anim.Play("transform_1");
        
        voice?.BroadCastSpecificVoice((int)MyVoiceGroup.ChangePhase,1);
        
        
        var projectileI =
            BattleStageManager.Instance.RangedAttackFXLayer.GetComponentInChildren<Projectile_C019_1_Boss>();
        if(projectileI != null)
            Destroy(projectileI.gameObject);
        
        var projectileII =
            BattleStageManager.Instance.RangedAttackFXLayer.GetComponentInChildren<Projectile_C019_2_Boss>();
        if(projectileII != null)
            Destroy(projectileII.gameObject);
        
        var projectileIII =
            BattleStageManager.Instance.RangedAttackFXLayer.GetComponentInChildren<Projectile_C019_3_Boss>();
        if(projectileIII != null)
            Destroy(projectileIII.gameObject);
        
        yield return new WaitForSeconds(1.6f);

        var fx = Instantiate(GetProjectileOfFormatName("action14_1"), transform.position + new Vector3(2.4f, 1),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(6.25f);

        voice?.BroadCastSpecificVoice((int)MyVoiceGroup.ChangePhase,2);
        bossBanner?.PrintSkillName("HB04_Action14");

        yield return new WaitForSeconds(2.5f);
        
        anim.SetTrigger("next");
        StageCameraController.SwitchMainCameraFollowObject(fx.transform.Find("HCFX_ElementOrb_01").gameObject);
        
        var bg2 = BattleEnvironmentManager.Instance.GetEnvironmentSpriteRenderer("Background2") as SpriteRenderer;
        bg2.DOColor(Color.black, 1f);


        yield return new WaitForSeconds(1.25f);
        
        CineMachineOperator.Instance.CamaraShake(15f,2f);
        
        yield return new WaitForSeconds(0.8f);
        
        
        var whiteScreenImage = GameObject.Find("FullScreenEffect").transform.Find("BlackIn").GetComponent<Image>();
        whiteScreenImage.color = new Color(1, 1, 1, 0);

        whiteScreenImage.DOFade(1, 1f);
        //StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        
        yield return new WaitForSeconds(1);

        var environmentRenderers = 
            BattleEnvironmentManager.Instance.GetAllEnvironmentRenderer();

        foreach (var renderer in environmentRenderers)
        {
            if (renderer.name == "Background2")
            {
                (renderer as SpriteRenderer).color = new Color(1, 1, 1, 0);
            }else if (renderer.name == "Background1")
            {
                (renderer as SpriteRenderer).color = new Color(1, 1, 1, 1);
            }else if (renderer.name.StartsWith("eff"))
            {
                renderer.gameObject.SetActive(true);
            }
            else
            {
                renderer.gameObject.SetActive(false);
            }
        }
        
        whiteScreenImage.DOFade(0, 0.5f);
        
        //
        anim.Play("idle");
        ac.SwapWeaponVisibility(true);
        Destroy(fx);
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        

        yield return null;
        
        QuitAttack();
        _behavior.currentMoveAction = null;


    }














    protected void Combo1_Attack()
    {

        var proj = InstantiateMeele(GetProjectileOfFormatName("action01_1"),
            transform.position + new Vector3(ac.facedir * 1.5f, 0), InitContainer(true));

        voice?.PlayMyVoice((int)MyVoiceGroup.Combo1);

    }

    protected void Combo2_3_Attack(Vector3 position)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action01_2"),
            position, InitContainer(false), ac.facedir);

    }

    protected void Combo4_5_Attack(Vector3 position)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action02_1"),
            position, InitContainer(false), 1);

        DOVirtual.DelayedCall(1.2f, () =>
        {
            var projContainer =
                InstantiateSealedContainer(GetProjectileOfFormatName("action02_2"), position, false, 1);
        }, false);
    }

    protected void BlazingFount_Muzzle(int type = 1)
    {


        if (type == 1)
        {
            var projfx = Instantiate(GetProjectileOfFormatName("action03_1"),
                transform.position + new Vector3(ac.facedir * 1.2f, 0),
                Quaternion.identity,
                RangedAttackFXLayer.transform);
            voice?.PlayMyVoice((int)MyVoiceGroup.BlazingFount1);
        }
        else
        {
            var projfx = Instantiate(GetProjectileOfFormatName("action05_1"),
                transform.position + new Vector3(ac.facedir * 1.5f, 0),
                Quaternion.identity,
                RangedAttackFXLayer.transform);
            voice?.PlayMyVoice((int)MyVoiceGroup.BlazingFount2);
        }

    }

    protected void BlazingFount_Attack(GameObject target)
    {
        if (_behavior.difficulty < 3)
        {
            GenerateWarningPrefab("action03", target.RaycastedPosition() + new Vector2(0, 1),
                Quaternion.identity, RangedAttackFXLayer.transform);

        }




        var proj = InstantiateRanged(GetProjectileOfFormatName("action03_2"),
            target.RaycastedPosition() + new Vector2(0, 1), InitContainer(false),
            1);

        var atk = proj.GetComponent<AttackFromEnemy>();

        var flashBurn = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
            41.6f, 21, 1);

        atk.AddWithConditionAll(flashBurn, 120);


    }


    protected void BlazingConsecration_Attack(GameObject target)
    {
        
        if (_behavior.difficulty < 3)
        {
            GenerateWarningPrefab("action05", target.RaycastedPosition() + new Vector2(0, 1),
                Quaternion.identity, RangedAttackFXLayer.transform);

        }
        
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action05_2"),
            target.RaycastedPosition() + new Vector2(0, 1), InitContainer(false),
            1);

        var atk = proj.GetComponent<AttackFromEnemy>();

        var flashBurn = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
            41.6f, 21, 1);

        atk.AddWithConditionAll(flashBurn, 120);


    }

    protected void RingOfAffection_Effect()
    {
        InvokeOnUseSkill(4);

        var fx = Instantiate(GetProjectileOfFormatName("action04_2"),
            transform.position,
            Quaternion.identity, MeeleAttackFXLayer.transform);


        voice?.PlayMyVoice((int)MyVoiceGroup.AffectionRing);
    }

    protected void RingOfAffection()
    {

        var bondBuff = new TimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,
            -1, -1, 1, 8104101);

        _statusManager.ObtainTimerBuff(bondBuff);

        var fx = Instantiate(GetProjectileOfFormatName("action04_1"),
            gameObject.RaycastedPosition(),
            Quaternion.identity, RangedAttackFXLayer.transform);


        //voice?.PlayMyVoice((int)MyVoiceGroup.AffectionRing);
    }

    protected float BlessedWallHint(int dir, float distance, out float tweenTime)
    {
        var hint = GenerateWarningPrefab("action06_1",
            new Vector3(
                dir == 1 ? BattleStageManager.Instance.mapBorderL : BattleStageManager.Instance.mapBorderR, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);

        var shineHint = hint.GetComponentInChildren<EnemyAttackHintBarShine>();

        if (dir == -1)
        {
            hint.transform.localScale = new Vector3(-1, 1, 1);
        }

        var safePosition =
            (_behavior.targetPlayer.transform.position - new Vector3(distance*dir, 0)).SafePosition(Vector2.zero);
        
        print(safePosition.x);
        
        
        tweenTime = shineHint.warningTime > 1f ? shineHint.warningTime - 0.5f : 0.5f;

        hint.transform.DOMoveX(safePosition.x, tweenTime);

        return safePosition.x;
    }

    protected void BlessedWallAttack(int dir, float positionX, bool setWall = false)
    {
        var projWall = InstantiateRanged(GetProjectileOfFormatName("action06_1"),
            new Vector3(dir == 1 ? BattleStageManager.Instance.mapBorderL : BattleStageManager.Instance.mapBorderR,
                11), InitContainer(false), 1);

        if (setWall)
        {
            var wallController = projWall.GetComponent<Projectile_C019_4_Boss>();
            wallController.SetEnemyMoveController(this);
            blessedWalls.Add(wallController);
        }

        

        var attack = projWall.GetComponent<AttackFromEnemy>();

        projWall.transform.DOMoveX(positionX, 2f).SetEase(Ease.Linear);

        var dmgDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageDown, 50, 20,
            100);
        
        attack.AddWithConditionAll(dmgDebuff,100);
        attack.firedir = dir;

        var collider = projWall.transform.Find("collider").GetComponent<BoxCollider2D>();
        
        print(collider.name);
        var col1 = (ac as EnemyControllerHumanoid)._groundSensor.GetComponent<Collider2D>();
        var col2 = GetComponentInChildren<EnemyOneWayPlatformEffector>().platformSensor;
        print(col1.name);
        print(col2.name);


        Physics2D.IgnoreCollision(collider,
            col1,true);
        Physics2D.IgnoreCollision(collider,
            col2,true);

    }


    protected GameObject[] BlessedWallHintDoubleDirection()
    {
        var hintL = GenerateWarningPrefab("action06_1",
            new Vector3(
                BattleStageManager.Instance.mapBorderL, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
        var shineHint = hintL.GetComponentInChildren<EnemyAttackHintBarShine>();


        var hintR = GenerateWarningPrefab("action06_1",
            new Vector3(
                BattleStageManager.Instance.mapBorderR, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
        hintR.transform.localScale = new Vector3(-1, 1, 1);

        return new GameObject[2] { hintL, hintR };
    }
    
    protected GameObject BlessedWallHintDoubleDirectionR(GameObject hintR, float distance,float stopTime, out float tweenTime)
    {
        var shineHint = hintR.GetComponentInChildren<EnemyAttackHintBarShine>();
        tweenTime = 0.5f;
        
        
        var safePosition2 =
            (_behavior.targetPlayer.transform.position - new Vector3(-distance, 0)).SafePosition(Vector2.zero);
        
        var tweenerCoreR = hintR.transform.DOMoveX(safePosition2.x, tweenTime).
            OnComplete(
            () =>
            {
                
            }
        );

        return hintR;
    }
    
    protected Tweener BlessedWallHintDoubleDirectionL(GameObject hintL, float distance,float stopTime, out float tweenTime)
    {
        
        var shineHint = hintL.GetComponentInChildren<EnemyAttackHintBarShine>();

        var safePosition =
            (_behavior.targetPlayer.transform.position - new Vector3(distance, 0)).SafePosition(Vector2.zero);
        tweenTime = 0.5f;

        var tweenerCoreL = hintL.transform.DOMoveX(safePosition.x, tweenTime).OnComplete(
            () =>
            {
                
            });

        
        
        return tweenerCoreL;
    }

    
    
    protected void StunningGlareAttack(int punishmentModifier = 1)
    {
        var proj = InstantiateRanged(GetProjectileOfFormatName("action07_1"),
            transform.position, InitContainer(true), 1);

        var atk = proj.GetComponent<ForcedAttackFromEnemy>();

        atk.target = _behavior.viewerPlayer;

        DOVirtual.DelayedCall(0.29f, () =>
        {

            var distanceX = Mathf.Abs(transform.position.x - _behavior.viewerPlayer.transform.position.x);
            
            var distanceY = Mathf.Abs(transform.position.y - _behavior.viewerPlayer.transform.position.y);

            float constDmg = 0;

            if (distanceX < 15)
            {
                constDmg += (15 - distanceX) * 6f * punishmentModifier;
            }

            if (distanceY < 10)
            {
                constDmg += (10 - distanceY) * (10 - distanceY) * 5f * punishmentModifier;
            }

            print("PunishDMG:"+constDmg);
            if (constDmg > _behavior.targetPlayer.GetComponent<StatusManager>().currentHp)
            {
                constDmg = _behavior.targetPlayer.GetComponent<StatusManager>().currentHp - 1;
            }
            
            
            var stunResDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.StunResDown,
                100, 3, 1);

            var stunDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.Stun,
                1, 5, 1);

            if (_behavior.difficulty > 3)
            {
                atk.AddWithConditionAll(stunResDebuff,100);
            }


            if (_behavior.viewerPlayer.transform.localScale.x > 0
                && _behavior.viewerPlayer.transform.position.x <= transform.position.x)
            {
                //atk.attackInfo[0].dmgModifier[0] *= punishmentModifier;
                atk.attackInfo[0].knockbackPower = 200;
                atk.AddWithConditionAll(stunDebuff,200,1);
                
            }
            else if (_behavior.viewerPlayer.transform.localScale.x < 0
                     && _behavior.viewerPlayer.transform.position.x >= transform.position.x)
            {
                //atk.attackInfo[0].dmgModifier[0] *= punishmentModifier;
                atk.attackInfo[0].knockbackPower = 200;
                atk.AddWithConditionAll(stunDebuff,200,1);
            }
            else
            {
                constDmg /= 10;
            }
            atk.attackInfo[0].constDmg.Add(constDmg);
            atk.DealDamageImmediately();
        },false);



    }

    protected void WarpEffect()
    {
        Instantiate(GetProjectileOfFormatName("action07_2"), transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
    }


    protected void ProjectilesTypeIII()
    {

        voice?.BroadCastMyVoice((int)MyVoiceGroup.Projectile);
        var sealedContainer = InstantiateSealedContainer(
            GetProjectileOfFormatName("action08_1"), Vector3.zero, false, 1);

        var controller = sealedContainer.GetComponent<Projectile_C019_1_Boss>();
        
        controller.SetEnemySource(gameObject);

        controller.targetPlayer = _behavior.targetPlayer;
        
        

    }
    
    protected void ProjectilesTypeII()
    {

        voice?.BroadCastMyVoice((int)MyVoiceGroup.Projectile);
        var sealedContainer = InstantiateSealedContainer(
            GetProjectileOfFormatName("action10_1"), Vector3.zero, false, 1);

        var controller = sealedContainer.GetComponent<Projectile_C019_3_Boss>();
        
        controller.SetEnemySource(gameObject);

        controller.targetPlayer = _behavior.targetPlayer;
        
        

    }

    protected void ProjectilesTypeI()
    {

        voice?.BroadCastMyVoice((int)MyVoiceGroup.Projectile);
        var sealedContainer = InstantiateSealedContainer(
            GetProjectileOfFormatName("action09_1"), Vector3.zero, false, 1);

        var controller = sealedContainer.GetComponent<Projectile_C019_2_Boss>();
        
        controller.SetEnemySource(gameObject);

        controller.targetPlayer = _behavior.targetPlayer;
        
        

    }

    protected void PowerUp(float buffAmount, float buffAmount2)
    {
        
        Instantiate(GetProjectileOfFormatName("action12_1"),transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        voice?.BroadCastMyVoice((int)MyVoiceGroup.Buff);
        
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            buffAmount, -1, 100, -1);
        var buff = _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            buffAmount2, 15, 50, -1);
        var buff2 = _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            10, -1, 100, 8104201);

        buff.dispellable = false;
        buff2.dispellable = false;


    }

    protected void UprisingStars(bool playVoice = true)
    {
        GenerateWarningPrefab("action11_1", new Vector3(0,-10),Quaternion.identity,
            RangedAttackFXLayer.transform);

        DOVirtual.DelayedCall(1.6f,
            () =>
            {
                if(playVoice)
                    voice?.PlayMyVoice((int)MyVoiceGroup.Combo5);
                
                var sealedContainer = InstantiateSealedContainer(
                    GetProjectileOfFormatName("action11_1"), new Vector3(0,-10), false, 1);
                GenerateWarningPrefab("action11_2", new Vector3(0,-10),Quaternion.identity,
                    RangedAttackFXLayer.transform);
            }
            , false);
        
        DOVirtual.DelayedCall(3.2f,
            () =>
            {
                var sealedContainer = InstantiateSealedContainer(
                    GetProjectileOfFormatName("action11_2"), new Vector3(0,35), false, 1);
                
            }
            , false);
    }

    protected void FallingStars(bool playVoice = true)
    {
        GenerateWarningPrefab("action11_2", new Vector3(0,-10),Quaternion.identity,
            RangedAttackFXLayer.transform);

        DOVirtual.DelayedCall(1.6f,
            () =>
            {
                if(playVoice)
                    voice?.PlayMyVoice((int)MyVoiceGroup.Combo5);
                var sealedContainer = InstantiateSealedContainer(
                    GetProjectileOfFormatName("action11_2"), new Vector3(0,35), false, 1);
                GenerateWarningPrefab("action11_1", new Vector3(0,-10),Quaternion.identity,
                    RangedAttackFXLayer.transform);
            }
            , false);
        
        DOVirtual.DelayedCall(3.2f,
            () =>
            {
                var sealedContainer = InstantiateSealedContainer(
                    GetProjectileOfFormatName("action11_1"), new Vector3(0,-10), false, 1);
                
            }
            , false);
    }

    protected void InstantiateLightBall(Vector3 position, Platform platform)
    {
        var dir = position.x > _behavior.targetPlayer.transform.position.x ? -1 : 1;
        
        var proj = InstantiateRanged(GetProjectileOfFormatName("action13_1"),
            position, InitContainer(false), dir);

        var projController = proj.GetComponent<WandingProjectile>();
        projController.SetWandingPlatform(platform);
        projController.SetFiredir(dir);

    }



}
