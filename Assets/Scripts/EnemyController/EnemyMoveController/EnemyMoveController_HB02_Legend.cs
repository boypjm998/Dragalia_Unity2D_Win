using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyMoveController_HB02_Legend : EnemyMoveController_HB02
{
    // Start is called before the first frame update
    public bool gateOpen = false;
    protected Color originColor = new Color(70f/255f, 190/255f, 1,1);
    protected Color worldColor1 = new Color(141f/255f, 1, 134/255f,1);
    protected Color worldColor2 = new Color(172 / 255f, 35 / 255f, 1, 1);
    
    public int currentWorld;
    protected SpriteRenderer background;

    protected bool hint1displayed = false;
    protected bool hint2displayed = false;
    protected bool hint3displayed = false;
    
    private TimerBuff legendPlusBuff;

    protected override void Start()
    {
        base.Start();
        background = GameObject.Find("Background3").GetComponent<SpriteRenderer>();
        legendPlusBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DamageUp,
            5,-1,20,8000000);
        legendPlusBuff.dispellable = false;
    }

    public override IEnumerator HB02_Action04()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
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

    public IEnumerator HB02_Action15_V()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            transform.position+new Vector3(0,2.5f,0));

        yield return new WaitForSeconds(0.3f);
        
        anim.Play("spin");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.48f);
        SpinDashRed();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.Dash);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Summon Orbs
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action16()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        
        if (_behavior.difficulty == 5)
        {
            _statusManager.ObtainTimerBuff(legendPlusBuff);
        }
        
        ac.SetHitSensor(false);
        _behavior.breakable = false;
        Warp_Effect();
        StageCameraController.SwitchOverallCamera();
        
        var originColor = background.color;
        ChangeSkyColor(Color.black ,1f);
        yield return new WaitForSeconds(0.25f);
        
        DisappearRenderer();
        transform.position = Vector3.zero + new Vector3(0,5,0);
        ac.SetGravityScale(0);
        
        yield return new WaitForSeconds(1f);
        anim.Play("float_in");
        bossBanner?.PrintSkillName("HB02_Action16");
        yield return null;
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        ac.TurnMove(_behavior.targetPlayer);
        AppearRenderer();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("float");
        var gateObj = OpenGate();
        
        yield return new WaitUntil(()=>!gateOpen);
        StageCameraController.SwitchMainCamera();
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        Destroy(gateObj);
        ac.TurnMove(_behavior.targetPlayer);
        DisappearRenderer();
        
        yield return new WaitForSeconds(1f);
        anim.Play("idle");
        transform.position = new Vector3(0, -1, 0);
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        ChangeSkyColor(originColor,1f);
        
        AppearRenderer();
        ac.SetGravityScale(4);
        ac.SetHitSensor(true);
        _behavior.breakable = true;
        _behavior.controllAfflictionProtect = false;
        
        QuitAttack();
        
    }

    /// <summary>
    /// pick sanctuary buff
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action19()
    {
        yield return new WaitUntil(() => ac.hurt == false);
        ac.OnAttackEnter(_statusManager.knockbackRes+50);
        List<Projectile_C003_10_Boss> buff_zones;
        buff_zones = FindObjectsOfType<Projectile_C003_10_Boss>().ToList();
        Transform targetTransform;
        targetTransform = CheckNearestBuffZone(buff_zones);
        //var target_zone = targetTransform.GetComponent<Projectile_C003_10_Boss>();

        while (buff_zones.Count > 0)
        {

            buff_zones = FindObjectsOfType<Projectile_C003_10_Boss>().ToList();
            
            if (targetTransform == null)
            {
                targetTransform = CheckNearestBuffZone(buff_zones);
            }
            if(buff_zones.Count <= 0)
                break;

            try
            {
                if (targetTransform.position.x - transform.position.x > 1)
                {
                    ac.SetFaceDir(1);
                    ac.SetMove(1);
                }
                else if (transform.position.x - targetTransform.position.x > 1)
                {
                    ac.SetFaceDir(-1);
                    ac.SetMove(1);
                }
                else
                {
                    ac.SetMove(0);
                }
            }
            catch
            {
                break;
            }

            yield return null;


            buff_zones = FindObjectsOfType<Projectile_C003_10_Boss>().ToList();
            if(buff_zones.Count <= 0)
                break;
            if (targetTransform == null)
            {
                targetTransform = CheckNearestBuffZone(buff_zones);
            }



        }
        
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetMove(0);
        
        _behavior.controllAfflictionProtect = false;
        ac.OnMoveFinished?.Invoke(true);
        _behavior.currentMoveAction = null;
        QuitAttack();
        
    }

    /// <summary>
    /// glorious sanctuary guardian
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action17()
    {
        yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
        
        ac.OnAttackEnter(999);
        if (_behavior.difficulty == 5)
        {
            _statusManager.ObtainTimerBuff(legendPlusBuff);
        }
        
        
        StageCameraController.SwitchOverallCamera();
        ac.SetHitSensor(false);
        _behavior.breakable = false;
        Warp_Effect();
        
        yield return new WaitForSeconds(0.25f);
        DisappearRenderer();
        bossBanner?.PrintSkillName("HB02_Action17");
        RecycleSanctuaryBuffZones();
        _statusManager.ImmuneToAllControlAffliction = true;
        
        var selection = Random.Range(0f, 1f);
        print(selection);
        if (selection < 0.5f)
        {
            GenerateWarningPrefab(WarningPrefabs[5], 
                new Vector3(0,transform.position.y,0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else
        {
            GenerateWarningPrefab(WarningPrefabs[6], 
                new Vector3(0,transform.position.y,0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        
        

        yield return new WaitForSeconds(0.75f);
        transform.position = new Vector3(0,-1,0);
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        AppearRenderer();
        ac.SetHitSensor(true);

        if (!hint1displayed)
        {
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10021,BattleEffectManager.Instance?.notteHintClips[1]);
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10022,null);
            hint1displayed = true;
        }


        yield return new WaitForSeconds(3.6f);
        anim.Play("s1");

        yield return null;
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.GloriousSanctuary);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        GloriousSanctuaryGuardian(selection);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        yield return new WaitForSeconds(0.5f);
        StageCameraController.SwitchMainCamera();
        _behavior.controllAfflictionProtect = false;
        _behavior.breakable = true;
        anim.Play("idle");
        QuitAttack();


    }
    
    /// <summary>
    /// glorious sanctuary charger
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action18()
    {
        yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
        
        ac.OnAttackEnter(999);
        if (_behavior.difficulty == 5)
        {
            _statusManager.ObtainTimerBuff(legendPlusBuff);
        }
        StageCameraController.SwitchOverallCamera();
        ac.SetHitSensor(false);
        _behavior.breakable = false;
        Warp_Effect();
        
        yield return new WaitForSeconds(0.25f);
        DisappearRenderer();
        bossBanner?.PrintSkillName("HB02_Action18");
        RecycleSanctuaryBuffZones();
        _statusManager.ImmuneToAllControlAffliction = true;
        
        var selection = Random.Range(0f, 1f);
        if (selection < 0.5f)
        {
            GenerateWarningPrefab(WarningPrefabs[5], 
                new Vector3(0,transform.position.y,0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else
        {
            GenerateWarningPrefab(WarningPrefabs[6], 
                new Vector3(0,transform.position.y,0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }

        yield return new WaitForSeconds(0.75f);
        transform.position = new Vector3(0,-1,0);
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        AppearRenderer();
        ac.SetHitSensor(true);

        yield return new WaitForSeconds(3.6f);
        anim.Play("s1");

        yield return null;
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.GloriousSanctuary);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f);
        GloriousSanctuaryCharger(selection);
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        yield return new WaitForSeconds(0.5f);
        StageCameraController.SwitchMainCamera();
        _behavior.controllAfflictionProtect = false;
        _behavior.breakable = true;
        anim.Play("idle");
        QuitAttack();


    }

    /// <summary>
    /// World 1 start
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action20()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        _behavior.breakable = false;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("float_entire");
        bossBanner.PrintSkillName("HB02_Action20");
        yield return new WaitForSeconds(0.5f);
        
        ChangeSkyColor(Color.black, 1f);
        yield return new WaitForSeconds(1.2f);
        BattleStageManager.Instance.ClearAllFieldAbility();
        BattleStageManager.Instance.RemoveFieldAbility(20034);
        BattleStageManager.Instance.AddFieldAbility(20033);
        currentWorld = 1;
        
        ChangeSkyColor(worldColor1, 1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        _behavior.breakable = true;
        _behavior.controllAfflictionProtect = false;
        QuitAttack();
    }
    
    /// <summary>
    /// World 2 start
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action21()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        _behavior.breakable = false;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("float_entire");
        bossBanner.PrintSkillName("HB02_Action21");
        yield return new WaitForSeconds(0.5f);
        
        ChangeSkyColor(Color.black, 1f);
        yield return new WaitForSeconds(1.2f);
        BattleStageManager.Instance.ClearAllFieldAbility();
        BattleStageManager.Instance.RemoveFieldAbility(20033);
        BattleStageManager.Instance.AddFieldAbility(20034);
        currentWorld = 2;
        
        ChangeSkyColor(worldColor2, 1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        _behavior.breakable = true;
        _behavior.controllAfflictionProtect = false;
        QuitAttack();
    }

    /// <summary>
    /// 神圣祷告 信念
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action22()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB02_Action22");
        yield return new WaitForSeconds(1.5f);
        
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.CelestialPrayer);
        anim.Play("s4");
        var pos = CelestialPrayer_Hint();
        yield return new WaitForSeconds(0.2f);
        var pos2 = CelestialPrayer_Hint();
        yield return new WaitForSeconds(0.2f);
        var pos3 = CelestialPrayer_Hint();
        yield return new WaitForSeconds(0.1f);
        CelestialPrayer(pos);
        yield return new WaitForSeconds(0.1f);
        var pos4 = CelestialPrayer_Hint();
        yield return new WaitForSeconds(0.1f);
        CelestialPrayer(pos2);
        yield return new WaitForSeconds(0.2f);
        CelestialPrayer(pos3,false);
        yield return new WaitForSeconds(0.2f);
        CelestialPrayer(pos4,false);
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        Invoke(nameof(CelestialPrayerFaithA_Hint),2f);
        Invoke(nameof(CelestialPrayerFaithA_Attack),2.3f);
        
        Invoke(nameof(CelestialPrayerFaithB_Hint),3.2f);
        Invoke(nameof(CelestialPrayerFaithB_Attack),3.5f);
        
        QuitAttack();

    }

    /// <summary>
    /// 神圣祷告 创世
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action23()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB02_Action23");
        yield return new WaitForSeconds(1.5f);

        
        
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.CelestialPrayer);
        anim.Play("s4");

        var randomNum = Random.Range(0, 1f);
        var type = randomNum < 0.5f ? 1 : -1;


        var pos = CelestialPrayer_Hint(type * 7f);
        yield return new WaitForSeconds(0.3f);
        var pos2 = CelestialPrayer_Hint();
        yield return new WaitForSeconds(0.2f);
        CelestialPrayer(pos);
        yield return new WaitForSeconds(0.1f);
        var pos3 = CelestialPrayer_Hint();
        yield return new WaitForSeconds(0.2f);
        CelestialPrayer(pos2);
        yield return new WaitForSeconds(0.3f);
        CelestialPrayer(pos3,false);
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");

        if (type > 0)
        {
            Invoke("CelestialPrayerGenesisL_Hint",2.2f);
            Invoke("CelestialPrayerGenesisL_Attack",2.5f);
            Invoke("CelestialPrayerGenesisR_Hint",4.2f);
            Invoke("CelestialPrayerGenesisR_Attack",4.5f);
        }
        else
        {
            Invoke("CelestialPrayerGenesisR_Hint",2.2f);
            Invoke("CelestialPrayerGenesisR_Attack",2.5f);
            Invoke("CelestialPrayerGenesisL_Hint",4.2f);
            Invoke("CelestialPrayerGenesisL_Attack",4.5f);
        }
        
        QuitAttack();


    }

    /// <summary>
    /// Reset World
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action24()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("float_entire");
        bossBanner.PrintSkillName("HB02_Action24");
        yield return new WaitForSeconds(0.5f);
        
        ChangeSkyColor(Color.black, 1f);
        yield return new WaitForSeconds(1.2f);
        BattleStageManager.Instance.RemoveFieldAbility(20034);
        BattleStageManager.Instance.RemoveFieldAbility(20033);
        currentWorld = 0;
        
        ChangeSkyColor(originColor, 1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        _behavior.controllAfflictionProtect = false;
        QuitAttack();
    }

    /// <summary>
    /// 暮光之冠信念
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action25()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB02_Action25");
        yield return new WaitForSeconds(1f);

        var hint = GenerateWarningPrefab(WarningPrefabs[11], new Vector3(transform.position.x, 0, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponentInChildren<EnemyAttackHintBarRect2D>();
        hintbar.SetAc(ac);
        
        yield return new WaitForSeconds(hintbar.warningTime-0.1f);
        
        Destroy(hint,0.6f);
        anim.Play("s3");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.125f);
        
        TwilightCrownF_Wave();
        var container =TwilightCrown_Wave();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.TwilightCrown);
        yield return new WaitForSeconds(0.4f);
        
        
        TwilightCrown_Star(container);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// 暮光之冠创世
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action26()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.grounded);
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB02_Action26");
        yield return new WaitForSeconds(1f);

        var hint = GenerateWarningPrefab(WarningPrefabs[1], new Vector3(transform.position.x, 
                transform.position.y-1, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponent<EnemyAttackHintBarCircle>();
        hintbar.SetAc(ac);
        
        yield return new WaitForSeconds(hintbar.warningTime-0.1f);
        
        Destroy(hint,0.6f);
        anim.Play("s3");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.125f);
        
        var container = TwilightCrown_Wave();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.TwilightCrown);
        yield return new WaitForSeconds(0.4f);
        
        TwilightCrown_Star(container);
        TwilightCrownG_AscendingStars();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// 神圣之冠 创世（3个）
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action27()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("HB02_Action27");
        StageCameraController.SwitchOverallCamera();
        yield return new WaitForSeconds(1.5f);
        
        anim.Play("s2");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.HolyCrown);
        
        
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (4/21f));
        
        HolyCrown_Muzzle();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.735f);
        
        HolyCrownG();
        _behavior.controllAfflictionProtect = false;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        yield return new WaitForSeconds(3f);
        StageCameraController.SwitchMainCamera();
        
        QuitAttack();
    }

    /// <summary>
    /// 神圣之冠 信念（5个）
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action28()
    {
        _statusManager.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        bossBanner?.PrintSkillName("HB02_Action28");
        StageCameraController.SwitchOverallCamera();
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("s2");
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.HolyCrown);
        
        
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (4/21f));
        
        HolyCrown_Muzzle();

        if (hint2displayed == false)
        {
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10023,BattleEffectManager.Instance?.notteHintClips[1]);
        }


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.735f);
        
        HolyCrownF();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        yield return new WaitForSeconds(3.5f);
        //StageCameraController.SwitchMainCamera();
        
        QuitAttack();
    }

    /// <summary>
    /// Genesis Crown
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action29(bool isSingle = false)
    {
        _statusManager.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        _behavior.breakable = false;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        Warp_Effect();
        StageCameraController.SwitchOverallCamera();
        yield return new WaitForSeconds(0.3f);
        DisappearRenderer();
        transform.position = Vector3.zero + new Vector3(0,5,0);
        ac.SetGravityScale(0);
        
        yield return new WaitForSeconds(1f);
        anim.Play("float_in");
        bossBanner?.PrintSkillName("HB02_Action29");
        yield return null;
        Warp_Effect();
        yield return new WaitForSeconds(0.25f);
        AppearRenderer();
        ac.SetFaceDir(1);
        ac.SetHitSensor(false);
        //AppearRenderer();
        anim.Play("super_move_1");
        yield return new WaitForSeconds(0.1f);
        
        //TODO:设置天空盒的背景色

        BattleStageManager.Instance.PlayerViewEnable = false;
        var RTScene = GameObject.Find("OtherCamera").transform.Find("RT").gameObject;
        var fullScreenUI = GameObject.Find("FullScreenEffect").transform.Find("RT").gameObject;

        var cutsceneController = RTScene.GetComponent<CutSceneController_Hb02>();
        var director = RTScene.GetComponent<PlayableDirector>();
        var Texture = cutsceneController.rt;

        var rawImg = fullScreenUI.GetComponent<RawImage>();
        
        
        RTScene.SetActive(true);
        yield return null;
        cutsceneController.Replay();
        cutsceneController.SetSkyBoxColor(currentWorld);
        yield return null;
        
        rawImg.texture = Texture;
        fullScreenUI.SetActive(true);
        
        
        
        voice?.BroadCastMyVoice(4);
        

        yield return new WaitForSeconds(3f);
        
        fullScreenUI.SetActive(false);
        rawImg.texture = null;
        RTScene.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        BattleStageManager.Instance.PlayerViewEnable = true;

        var attack = GenesisCrown_Start();
        var controller = attack.GetComponent<Projectile_C003_15_Boss>();
        controller.difficulty = _behavior.difficulty;
        yield return null;
        controller.SetEnemySource(gameObject);

        if (!hint2displayed && isSingle == false)
        {
            hint2displayed = true;
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10024,BattleEffectManager.Instance?.notteHintClips[0]);
        }

        if (isSingle && !hint3displayed)
        {
            hint3displayed = true;
            UI_DialogDisplayer.Instance?.
                EnqueueDialogShared(10101,10025,BattleEffectManager.Instance?.notteHintClips[0]);
        }




        yield return new WaitForSeconds(3f);
        
        controller.DoLaserAnimation(1,0);
        yield return new WaitForSeconds(0.2f);
        CineMachineOperator.Instance.CamaraShake(10f,0.5f);
        yield return new WaitForSeconds(3.3f);
        
        
        controller.DoLaserAnimation(-1,1);
        yield return new WaitForSeconds(0.2f);
        CineMachineOperator.Instance.CamaraShake(10f,0.5f);
        yield return new WaitForSeconds(3.3f);
        
        controller.DoLaserAnimation(1,2);
        yield return new WaitForSeconds(0.2f);
        CineMachineOperator.Instance.CamaraShake(10f,0.5f);
        yield return new WaitForSeconds(3.3f);
        
        controller.DoLaserAnimation(-1,3);
        yield return new WaitForSeconds(0.2f);
        CineMachineOperator.Instance.CamaraShake(10f,0.5f);
        yield return new WaitForSeconds(3.3f);
        
        controller.DoLaserAnimation(1,4);
        Destroy(attack,1f);
        yield return new WaitForSeconds(0.2f);
        CineMachineOperator.Instance.CamaraShake(10f,0.5f);
        
        
        StageCameraController.SwitchMainCamera();
        Warp_Effect();
        DisappearRenderer();
        anim.Play("idle");
        yield return new WaitForSeconds(0.3f);
        
        transform.position = new Vector3(0, -1, 0);
        Warp_Effect();
        yield return new WaitForSeconds(0.3f);
        
        AppearRenderer();
        _behavior.controllAfflictionProtect = false;
        ac.SetHitSensor(true);
        ac.ResetGravityScale();

        _behavior.breakable = true;
        _statusManager.ImmuneToAllControlAffliction = false;
        QuitAttack();


    }

    /// <summary>
    /// Galaxy of Prayer
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action30()
    {
        _behavior.controllAfflictionProtect = true;
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        StageCameraController.SwitchOverallCamera();
        
        if (_behavior.difficulty == 5)
        {
            _statusManager.ObtainTimerBuff(legendPlusBuff);
        }
        
        anim.Play("float_entire");
        bossBanner?.PrintSkillName("HB02_Action30");
        voice?.BroadCastMyVoice(4);

        RangedAttackFXLayer.GetComponentInChildren<Projectile_C003_16_Boss>()?.DestroyMe();
        
        var container = 
            Instantiate(projectilePoolEX[28], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);

        var atks = container.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var atk in atks)
        {
            atk.enemySource = gameObject;
        }
        
        
        _behavior.controllAfflictionProtect = false;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        StageCameraController.SwitchMainCamera();
        QuitAttack();

    }
    
    public void HB02_Action30_Off()
    {
        RangedAttackFXLayer.GetComponentInChildren<Projectile_C003_16_Boss>()?.DestroyMe();
    }

    /// <summary>
    /// Backwarp Ground
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action31()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);
        ac.SetHitSensor(false);
        var dir = _behavior.targetPlayer.GetComponent<ActorBase>().facedir;
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1f);
        Warp_Effect();
        
        yield return new WaitForSeconds(0.25f);
        DisappearRenderer();
        
        
        ac.SetFaceDir(dir);
        

        yield return new WaitForSeconds(0.25f);
        Warp_Attack(dir,false);
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
    /// Warp SmashDown
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB02_Action32()
    {
        yield return new WaitUntil(() => !ac.hurt && ac.anim.GetBool("isGround"));
        ac.OnAttackEnter(200);
        ac.TurnMove(_behavior.targetPlayer);
        DisappearRenderer();
        ac.SetHitSensor(false);
        
        Warp_Effect();
        voice?.PlayMyVoice(VoiceController_HB02.myMoveList.ComboC);
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,1f);

        yield return new WaitForSeconds(0.5f);
        var dir = _behavior.targetPlayer.GetComponent<ActorBase>().facedir;
        ac.SetFaceDir(dir);
        WarpSmashSpin_Appear(dir);
        ac.SetHitSensor(true);
        
        var _tweener = transform.DOMoveY(-1f, 0.3f);
        _tweener.SetEase(Ease.InCubic);

        yield return new WaitForSeconds(0.3f);
        SpinDashRed();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        
        ac.ResetGravityScale();
        anim.Play("idle");
        
        QuitAttack();
    }


    #region Functions

    private GameObject OpenGate()
    {
        gateOpen = true;
        var gate = Instantiate(projectilePoolEX[8], new Vector3(0, -1, 0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        var gateController = gate.GetComponent<Projectile_C003_9_Boss>();
        gateController.SetController(this);
        return gate;
    }

    protected void RecycleSanctuaryBuffZones()
    {
        var zones = FindObjectsOfType<Projectile_C003_10_Boss>();
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].RecycleBuff(_statusManager);
        }
    }

    protected void GloriousSanctuaryGuardian(float randomFloat)
    {
        GameObject proj;
        if (randomFloat < 0.25f)
        {
            proj = Instantiate(projectilePoolEX[15], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }else if (randomFloat < 0.5f)
        {
            proj = Instantiate(projectilePoolEX[16], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }else if (randomFloat < 0.75f)
        {
            proj = Instantiate(projectilePoolEX[17], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else
        {
            proj = Instantiate(projectilePoolEX[18], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        var fx = Instantiate(projectile3,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        CheckTwilightMoon();

        if (currentWorld == 2)
        {
            _statusManager.HPRegenImmediately(5.5f,0);
        }
        else
        {
            _statusManager.HPRegenImmediately(22f,0);
        }


        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff),15,60,1,0);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HealOverTime),1,15);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HolyFaith),-1,15,1,-1);
    }
    protected void GloriousSanctuaryCharger(float randomFloat)
    {
        GameObject proj;
        if (randomFloat < 0.25f)
        {
            proj = Instantiate(projectilePoolEX[11], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }else if (randomFloat < 0.5f)
        {
            proj = Instantiate(projectilePoolEX[12], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }else if (randomFloat < 0.75f)
        {
            proj = Instantiate(projectilePoolEX[13], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        else
        {
            proj = Instantiate(projectilePoolEX[14], new Vector3(0, transform.position.y, 0),
                Quaternion.identity, RangedAttackFXLayer.transform);
        }
        var fx = Instantiate(projectile3,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        CheckTwilightMoon();

        if (currentWorld == 1)
        {
            _statusManager.HPRegenImmediately(11f,0);
        }
        else
        {
            _statusManager.HPRegenImmediately(5.5f,0);
        }
        
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.AtkBuff),15,60,1,0);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HealOverTime),1,15);
        _statusManager.ObtainTimerBuff((int)(BasicCalculation.BattleCondition.HolyFaith),-1,15,1,-1);
    }

    private void ChangeSkyColor(Color color,float time)
    {
        background.DOColor(color, time);
    }

    protected Vector3 CelestialPrayer_Hint(float offset = 0)
    {
        var groundPos = BasicCalculation.GetRaycastedPlatformY(_behavior.targetPlayer) + 3f;
        var pos = new Vector3(_behavior.targetPlayer.transform.position.x, groundPos,
            _behavior.targetPlayer.transform.position.z);

        var hint = GenerateWarningPrefab(WarningPrefabs[2], 
            pos+new Vector3(offset,0,0), Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        Destroy(hint,0.5f);
        return pos+new Vector3(offset,0,0);
    }
    
    protected void SpinDashRed()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        var proj = InstantiateRanged(projectilePoolEX[9],
            transform.position + new Vector3(2.5f * ac.facedir, -1.5f, 0), 
            container, ac.facedir);
    }

    private void CelestialPrayerFaithA_Hint()
    {
        GenerateWarningPrefab(WarningPrefabs[7],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    private void CelestialPrayerFaithA_Attack()
    {
        var container = Instantiate
            (projectilePoolEX[19], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        var projs = container.GetComponentsInChildren<AttackFromEnemy>();
        for(int i = 0; i < projs.Length ; i++)
        {
            projs[i].enemySource = gameObject;
            projs[i].firedir = 1;
        }
    }
    
    private void CelestialPrayerFaithB_Hint()
    {
        GenerateWarningPrefab(WarningPrefabs[8],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    private void CelestialPrayerFaithB_Attack()
    {
        var container = Instantiate
            (projectilePoolEX[20], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        var projs = container.GetComponentsInChildren<AttackFromEnemy>();
        for(int i = 0; i < projs.Length ; i++)
        {
            projs[i].enemySource = gameObject;
            projs[i].firedir = 1;
        }
    }

    private void CelestialPrayerGenesisL_Hint()
    {
        GenerateWarningPrefab(WarningPrefabs[9],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    private void CelestialPrayerGenesisL_Attack()
    {
        var container = Instantiate
            (projectilePoolEX[21], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        var projs = container.GetComponentsInChildren<AttackFromEnemy>();
        for(int i = 0; i < projs.Length ; i++)
        {
            projs[i].enemySource = gameObject;
            projs[i].firedir = 0;
        }
    }
    
    private void CelestialPrayerGenesisR_Hint()
    {
        GenerateWarningPrefab(WarningPrefabs[10],
            Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    private void CelestialPrayerGenesisR_Attack()
    {
        var container = Instantiate
            (projectilePoolEX[22], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        var projs = container.GetComponentsInChildren<AttackFromEnemy>();
        for(int i = 0; i < projs.Length ; i++)
        {
            projs[i].enemySource = gameObject;
            projs[i].firedir = 0;
        }
    }

    private GameObject TwilightCrownF_Wave()
    {
        var container = Instantiate(projectilePoolEX[23], new Vector3(transform.position.x,
            0, 0), Quaternion.identity, MeeleAttackFXLayer.transform);
        
        //CheckTwilightMoon();
        container.GetComponent<AttackContainer>().InitAttackContainer(1,false);

        var attacks = container.GetComponentsInChildren<AttackFromEnemy>();
        foreach (var VARIABLE in attacks)
        {
            VARIABLE.enemySource = gameObject;
        }
        
        //_statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.RecoveryBuff,20,10);
        return container;
    }

    private void TwilightCrownG_AscendingStars()
    {
        var container = Instantiate(attackContainer, transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(2,false);

        var target = _behavior.targetPlayer;
        float posR = transform.position.x + 18;
        float posL = transform.position.x - 18;

        if (target.transform.position.x - transform.position.x > 18)
        {
            posR = target.transform.position.x;
        }
        else if (transform.position.x - target.transform.position.x > 18)
        {
            posL = target.transform.position.x;
        }

        InstantiateRanged(projectilePoolEX[24], new Vector3(posL,-4,0), container, 1);
        InstantiateRanged(projectilePoolEX[24], new Vector3(posR,-4,0), container, 1);
        

    }

    private void HolyCrownG()
    {
        var container = 
            Instantiate(projectilePoolEX[25], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<Projectile_C003_13_Boss>().SetEnemySource(gameObject);
        
        var holyfaithBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyFaith, -1f, 20f, 1);
        _statusManager.ObtainTimerBuff(holyfaithBuff);
        CheckTwilightMoon();
        
    }
    
    private void HolyCrownF()
    {
        var container = 
            Instantiate(projectilePoolEX[26], Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        
        container.GetComponent<Projectile_C003_14_Boss>().SetEnemySource(gameObject);
        
        var holyfaithBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HolyFaith, -1f, 20f, 1);
        _statusManager.ObtainTimerBuff(holyfaithBuff);
        CheckTwilightMoon();
        
    }

    private GameObject GenesisCrown_Start()
    {
        var container = 
            Instantiate(attackContainer, Vector3.zero, Quaternion.identity, RangedAttackFXLayer.transform);
        
        var proj = 
            Instantiate(projectilePoolEX[27], new Vector3(0,5,0), Quaternion.identity, container.transform);

        return proj;
    }

    private void WarpSmashSpin_Appear(int dir)
    {

        AppearRenderer();

        ac.SetFaceDir(dir);
        
        var position = new Vector3(_behavior.targetPlayer.transform.position.x - 4*dir,
            transform.position.y+3f, transform.position.z);

        position = BattleStageManager.Instance.OutOfRangeCheck(position);

        transform.position = position;
        
        ac.SetGravityScale(0);
        
        anim.Play("smash_down",0,0.21f);
        
    }

    private Transform CheckNearestBuffZone(List<Projectile_C003_10_Boss> buff_zones)
    {
        var minDistance = 9999f;
        float distance;
        Transform selectedTransform = null;
        foreach (var buff_zone in buff_zones)
        {
            if (buff_zone.IsUsed)
                continue;
            distance = Mathf.Abs(buff_zone.transform.position.x - transform.position.x);
            if (distance < minDistance)
            {
                selectedTransform = buff_zone.transform;
                minDistance = distance;
            }
        }

        return selectedTransform;
    }

    #endregion

}
