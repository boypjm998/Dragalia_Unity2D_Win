using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C015 : AttackManagerRanged
{
    public ChargeGauge_C015 chargeGauge;
    public GameObject figPrefab;
    
    protected UI_ForceStrikeAimerMeele specialForceStrikeIndicator;

    private TimerBuff _gaugeBuff1 = new((int)BasicCalculation.BattleCondition.DefBuff,
        25, -1, 1, 101501);

    private TimerBuff _gaugeBuff2 = new((int)BasicCalculation.BattleCondition.SkillDmgBuff,
        20, -1, 1, 101501);

    private TimerBuff _gaugeBuff3 = new((int)BasicCalculation.BattleCondition.AtkBuff,
        20, -1, 1, 101501);

    private TimerBuff _comboBuff1 = new((int)BasicCalculation.BattleCondition.CritRateBuff,
        5, -1, 3, 101502);
    private TimerBuff _comboBuff2 = new((int)BasicCalculation.BattleCondition.CritDmgBuff,
        5, -1, 10, 101502);

    private TimerBuff _explosionDebuff = new((int)BasicCalculation.BattleCondition.Resonance,
        1, -1, 4, 101503);

    private AbilityClock _buffTrigger = new(1);
    

    [SerializeField] private GameObject delayedComboShineFxPrefab;

    private int _lastFaceDirOnSkill;
    private bool _eventHasRegistered;

    private bool _skillIsEnhanced = false;
    private bool _figAvailable = false;
    private GameObject _figInstance;

    private GameObject _skill1Container;
    private GameObject _skill3Container;
    private AttackContainer _debuffContainer;
    private Vector3 _recordedPosition;
    public bool EdenModeActive => _figAvailable;

    public event Action<int> OnFigAction;

    protected override void Start()
    {
        base.Start();
        SearchChargeGauge();
        RegisterEvents();
        InitFig();
        InitDebuffContainer();
    }

    private void OnDestroy()
    {
        if (chargeGauge)
        {
            chargeGauge.OnLevelChange -= GrantBuffs;
        }
    }

    public void CheckEnhanced()
    {
        if (_figAvailable)
            _skillIsEnhanced = true;
        else _skillIsEnhanced = false;
    }

    public override void DashAttack()
    {
        var containerGo = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,containerGo);
        (ac as ActorController)?.PlayAttackVoice(0);

        var container = containerGo.GetComponent<AttackContainer>();
        container.InitAttackContainer(2,false);
        
        var subcontainer = Instantiate(BattleStageManager.Instance.attackSubContainer,
            transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform).GetComponent<AttackSubContainer>();

        subcontainer.gameObject.AddComponent<ObjectInvokeDestroy>().destroyTime = 1;

        subcontainer.parentContainer = container;
        subcontainer.InitAttackContainer(1,containerGo);
        subcontainer.InitAttackContainer(1,false);

        var projectile = Instantiate
        (dashFX[1], transform.position + new Vector3(ac.facedir * 1, 0),
            Quaternion.identity,subcontainer.transform);
        if (ac.facedir == -1)
        {
            //projectile.GetComponent<DOTweenSimpleController>().moveDirection.x *= -1;
            projectile.transform.localScale = new Vector3(-1,1,1);
        }
        
        var blast = projectile.GetComponent<BlastProjectileCustom>();
        blast.src = gameObject;
        
        GrantBlastProjectileCPGain(blast, 2);

    }
    
    public override void ForceStrikeRelease(int currentFSLV){
        
        if(currentFSLV <= 0)
            return;
        
        ac.OnAttackInterrupt?.Invoke();

        var container = InitContainer(false);
        var projectile = Instantiate
        (ForceFX[1], transform.position + new Vector3(ac.facedir * 1, 0),
            Quaternion.identity,container.transform);
        if (ac.facedir == -1)
        {
            //projectile.GetComponent<DOTweenSimpleController>().moveDirection.x *= -1;
            projectile.transform.localScale = new Vector3(-1,1,1);
        }
        
        // var blast = projectile.GetComponent<BlastProjectileCustom>();
        // blast.src = gameObject;
        // (ac as ActorController_c015).UseDelayedAnimation = false;
        //
        Action<StatusManager, StatusManager, AttackBase, float> handler = null;

        handler = (self, target, atk, dmg) =>
        {
            if(_figAvailable)
                return;
            chargeGauge.Charge(1);
        };

        projectile.GetComponent<AttackFromPlayer>().OnAttackDealDamage += handler;
        
        (ac as ActorController).PlayAttackVoice(9);
    }

    public void TurnToForceDirection()
    {
        ac.SetFaceDir(specialForceStrikeIndicator.forceDirection);
    }
    
    public void SpecialForceStrikeCharging()
    {
        print("调用spfs");
        if (specialForceStrikeIndicator == null)
        {
            var prefabIndicator = Instantiate(ForceFX[0], transform.position, Quaternion.identity,
                BuffFXLayer.gameObject.transform);
            specialForceStrikeIndicator = prefabIndicator.GetComponent<UI_ForceStrikeAimerMeele>();
            prefabIndicator.name = "ForceStrikeIndicator";
            specialForceStrikeIndicator.SetActorController(ac as ActorControllerMeeleWithFS);
            specialForceStrikeIndicator.SetMaxForceInfo
                (new float[] {(ac as ActorControllerMeeleWithFS).forcingRequireTime}.ToList());
            if ((ac as ActorControllerMeeleWithFS).maxForceLevel > 1)
            {
                List<float> forceInfo = new();
                for (int i = 0; i < (ac as ActorControllerMeeleWithFS).maxForceLevel; i++)
                {
                    forceInfo.Add((ac as ActorControllerMeeleWithFS).forcingRequireTime);
                }
                specialForceStrikeIndicator.SetMaxForceInfo(forceInfo);
            }
        }
        else
        {
            if(specialForceStrikeIndicator.gameObject.activeSelf)
                return;
            specialForceStrikeIndicator.gameObject.SetActive(true);
        }
        specialForceStrikeIndicator.SetForceDirection(ac.facedir);
        Debug.Log("Called OnForceStart");

    }
    
    public void DelayedShine()
    {
        Instantiate(delayedComboShineFxPrefab, transform.position,Quaternion.identity, RangedAttackFXLayer.transform);
    }
    
    public void Combo1_Muzzle()
    {
        InstantiateBuff(combo1FX[0], transform.position + new Vector3(ac.facedir * 1.5f, 0));
    }
    
    public void Combo2_Muzzle()
    {
        InstantiateBuff(combo2FX[0], transform.position + new Vector3(ac.facedir * 1.5f, 0));
    }
    
    public void Combo4_Muzzle()
    {
        InstantiateBuff(combo4FX[3], transform.position + new Vector3(ac.facedir * 1.5f, 0));
    }

    public void Combo1()
    {
        if ((ac as ActorController_c015).UseDelayedAnimation)
        {
            var blast = ComboProjectile(combo1FX[2]);
            GrantBlastProjectileCPGain(blast);
            (ac as ActorController_c015).UseDelayedAnimation = false;
        }
        else
        {
            var blast = ComboProjectile(combo1FX[1]);
            GrantBlastProjectileCPGain(blast);
        }
        
        
    }
    
    public void Combo2()
    {
        if ((ac as ActorController_c015).UseDelayedAnimation)
        {
            var blast = ComboProjectile(combo2FX[2]);
            (ac as ActorController_c015).UseDelayedAnimation = false;
            GrantBlastProjectileCPGain(blast);
        }
        else
        {
            var blast = ComboProjectile(combo2FX[1]);
            GrantBlastProjectileCPGain(blast);
        }
        
    }
    
    public void Combo3()
    {
        if ((ac as ActorController_c015).UseDelayedAnimation)
        {
            var blast = ComboProjectile(combo3FX[2]);
            (ac as ActorController_c015).UseDelayedAnimation = false;
            GrantBlastProjectileCPGain(blast);
        }
        else
        {
            var blast = ComboProjectile(combo3FX[1]);
            GrantBlastProjectileCPGain(blast);
        }
        
    }

    public void Combo4D()
    {
        var container = InitContainer(false);
        
        container.AddComponent<ObjectInvokeDestroy>().destroyTime = 1;
        
        var projectile = Instantiate
        (combo4FX[2], transform.position + new Vector3(ac.facedir * 1, 0),
            Quaternion.identity,container.transform);
        if (ac.facedir == -1)
        {
            //projectile.GetComponent<DOTweenSimpleController>().moveDirection.x *= -1;
            projectile.transform.localScale = new Vector3(-1,1,1);
        }
        
        var blast = projectile.GetComponent<BlastProjectileCustom>();
        blast.src = gameObject;
        (ac as ActorController_c015).UseDelayedAnimation = false;
        
        GrantBlastProjectileCPGain(blast,2);
        
    }
    
    public void Combo4()
    {
        var proj = InstantiateMeele
            (combo4FX[1], transform.position - new Vector3(0, 1), InitContainer(false));
        proj.GetComponent<AttackBase>().OnAttackDealDamage += 
            (self, target, atk, dmg) => 
            {
                if(_figAvailable)
                    return;
                chargeGauge.Charge(2); 
            };
    }

    public void ComboFlash()
    {
        Instantiate(combo4FX[0], transform.position, Quaternion.identity,RangedAttackFXLayer.transform);
    }

    public void Skill1_Muzzle(int id)
    {
        var muzzlePrefab = id == 1 ? skill1FX[0] : id == 2 ? skill1FX[1] : skill1FX[2];

        InstantiateBuff(muzzlePrefab, transform.position);
    }

    public void Skill1_Proj_Normal(bool first = false)
    {
        GameObject container = _skill1Container;
        if (first || container == null)
        {
            container = InitContainer(false, 4,true);
            _skill1Container = container;
            chargeGauge.Charge(15);
        }

        var proj = InstantiateRanged(skill1FX[3],
            transform.position + new Vector3(ac.facedir, 0.5f),
            container, ac.facedir);

        var atk = proj.GetComponent<AttackFromPlayer>();

        var burn = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72f, 12f, 100);
        
        atk.AddWithConditionAll(burn,120);

        AddResonanceEffectToAttack(atk,1);

    }

    public void Skill2()
    {

        var edenBuff = new TimerBuff((int)BasicCalculation.BattleCondition.EdenMode,
            1, 25, 1, 0);

        edenBuff.OnBuffStart += (stat) =>
        {
            _figAvailable = true;
            BattleStageManager.Instance.TriggerSkillIconEvent(2,1);
            Projectile_C015_1.Instance.SetRenderer(true);
        };

        edenBuff.OnBuffRemove += (stat) =>
        {
            _figAvailable = false;
            chargeGauge.ResetGauge();
            BattleStageManager.Instance.TriggerSkillIconEvent(2,0);
            Projectile_C015_1.Instance.SetRenderer(false);
        };

        edenBuff.dispellable = false;

        _statusManager.ObtainTimerBuff(edenBuff,true);

        Instantiate(skill2FX[0], transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
    }
    
    public void Skill1_Muzzle_Boost()
    {
        var muzzlePrefab = skill1FX[9];

        InstantiateBuff(muzzlePrefab, transform.position);
    }

    public void Skill1_MoveFig()
    {
        Projectile_C015_1.Instance.SetActive(false);
        Projectile_C015_1.Instance.MoveToPosition
            (transform.position + new Vector3(ac.facedir*2,0.5f),0.2f,Ease.OutSine);
    }
    
    public void Skill1_ReturnFig()
    {
        Projectile_C015_1.Instance.ReturnToPosition(0.3f,Ease.InOutSine);
    }

    public void Skill1_FigShine(bool boost)
    {
        var muzzlePrefab = boost ? skill1FX[5] : skill1FX[4];

        Instantiate(muzzlePrefab,_figInstance.transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);
    }

    public void Skill1_Proj_Boost(int id, bool first = false)
    {
        GameObject container = _skill1Container;
        if (first || container == null)
        {
            container = InitContainer(false, 6,true);
            _skill1Container = container;
            (_statusManager as PlayerStatusManager).FillSP(1,10);
            (_statusManager as PlayerStatusManager).FillSP(2,10);
            (_statusManager as PlayerStatusManager).FillSP(0,10);
            _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        }

        var prefab = id == 1 ? skill1FX[6] : id == 2 ? skill1FX[7] : skill1FX[8];

        var proj = InstantiateRanged(prefab,
            transform.position + new Vector3(ac.facedir, 0.5f),
            container, ac.facedir);

        var atk = proj.GetComponent<AttackFromPlayer>();

        var scor = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            41.6f, 21f, 100);
        
        atk.AddWithConditionAll(scor,120);

        if (id == 1)
        {
            AddResonanceEffectToAttack(atk,1);
        }
        else
        {
            AddResonanceEffectToAttack(atk,2);
        }
        
        
    }

    public void Skill2_FigActive(bool flag)
    {
        if(_figAvailable == false)
            return;
        
        Projectile_C015_1.Instance.isOccupied = flag;
        if(flag)
            Projectile_C015_1.Instance.SetActive(false);
    }
    
    public void Skill2_FigShine(bool boost)
    {
        if(_figAvailable == false)
            return;
        
        var muzzlePrefab = boost ? skill1FX[5] : skill1FX[4];

        Instantiate(muzzlePrefab,_figInstance.transform.position,
            Quaternion.identity, Projectile_C015_1.Instance.transform);
    }

    public void Skill2_Slash()
    {
        var proj = InstantiateMeele(skill2FX[1], transform.position,
            InitContainer(true, 1, true));

        var atk = proj.GetComponent<AttackFromPlayer>();

        atk.AddMeeleTimeStopEffect(0.2f, 0.15f);

        atk.OnAttackDealDamage += ResonanceExplosionForced;
        
        var burn = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72f, 12f, 100);
        
        atk.AddWithConditionAll(burn,120);

    }

    public void Skill3_Backward_Slash()
    {
        _skill3Container = InitContainer(false,2,true);

        var proj = InstantiateRanged(skill3FX[0], transform.position, _skill3Container,
            ac.facedir);
    }

    public void Skill3_Backward_Wave()
    {
        GameObject proj;
        
        if ((ac as ActorController_c015).Skill3IsStatic)
        {
            proj = InstantiateRanged(skill3FX[5], transform.position,
                _skill3Container, ac.facedir);

            proj.GetComponent<AttackFromPlayer>().AddMeeleTimeStopEffect(.2f, .15f);

        }
        else
        {
            proj = InstantiateRanged(skill3FX[1],
                transform.position + new Vector3(ac.facedir, 0.5f),
                _skill3Container , ac.facedir);
        }

        if (_skillIsEnhanced)
        {
            var atk = proj.GetComponent<AttackFromPlayer>();
            atk.AddWithConditionAll(new TimerBuff(999),100);
            atk.attackInfo[0].dmgModifier[0] *= 2f;
            GrantSPRegenBuff();
        }

    }

    public void Skill3_Forward_Muzzle()
    {
        InstantiateBuff(skill3FX[4], transform.position);
    }

    public void Skill3_Forward_Wave()
    {
        _recordedPosition = transform.position;
        
        _skill3Container = InitContainer(false,2,true);
        
        var proj = InstantiateRanged(skill3FX[2],
            transform.position + new Vector3(ac.facedir, 0.5f),
            _skill3Container , ac.facedir);

        if (_skillIsEnhanced)
        {
            var atk = proj.GetComponent<AttackFromPlayer>();
            atk.AddWithConditionAll(new TimerBuff(999),100);
            atk.attackInfo[0].dmgModifier[0] *= 2f;
            GrantSPRegenBuff();
        }
    }

    public void Skill3_Forward_Slash()
    {
        var proj = InstantiateRanged(skill3FX[3],
            _recordedPosition,
            _skill3Container , ac.facedir);
        
    }

    public override void Skill4(int eventID)
    {
        base.Skill4(eventID);
        if (_statusManager.currentHp <= _statusManager.maxHP * 0.3f)
        {
            _statusManager.ReliefOneDebuff();
        }
    }


    private BlastProjectileCustom ComboProjectile(GameObject prefab)
    {
        var container = InitContainer(false);
        container.AddComponent<ObjectInvokeDestroy>().destroyTime = 1;
        var projectile = Instantiate
        (prefab, transform.position + new Vector3(ac.facedir * 1, -0.25f),
            Quaternion.identity,container.transform);
        if (ac.facedir == -1)
        {
            projectile.GetComponent<DOTweenSimpleController>().moveDirection.x *= -1;
            projectile.transform.localScale = new Vector3(-1,1,1);
        }
        
        var blast = projectile.GetComponent<BlastProjectileCustom>();
        blast.src = gameObject;

        return blast;
    }
    
    protected void OnForcingUpdate()
    {
        var ac = this.ac as ActorController;
        
        if(ac.pi.buttonLeft.IsPressing && !ac.pi.buttonRight.IsPressing)
            specialForceStrikeIndicator.SetForceDirection(-1);
        
        else if(ac.pi.buttonRight.IsPressing && !ac.pi.buttonLeft.IsPressing)
            specialForceStrikeIndicator.SetForceDirection(1);
    }

    private void SearchChargeGauge()
    {
        if (!chargeGauge)
            chargeGauge = ChargeGauge_C015.Instance;
    }

    private void RegisterEvents()
    {
        if (_eventHasRegistered == false)
        {
            chargeGauge.OnLevelChange += GrantBuffs;
            _statusManager.OnComboConnect += OnComboConnect15GrantCritBuff;
            _statusManager.OnCriticalHit += OnCritGrantCritDamageBuff;
            _statusManager.OnComboReset += OnComboResetRemoveAllBuffs;
            _eventHasRegistered = true;
        }
    }

    private void OnComboConnect15GrantCritBuff()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        
        if (_statusManager.comboHitCount % 15 == 0)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff(_comboBuff1));
        }
    }

    private void OnCritGrantCritDamageBuff(AttackBase atk, int atkId)
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        
        if (_buffTrigger.Available)
        {
            _buffTrigger.StartTick();
            _statusManager.ObtainTimerBuff(new TimerBuff(_comboBuff2));
        }
    }

    private void OnComboResetRemoveAllBuffs()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        
        print("Removed All Condition with 101502");
        _statusManager.RemoveAllConditionWithSpecialID(101502);
    }

    private void GrantBuffs(int lvl)
    {
        if (GlobalController.currentGameState == GlobalController.GameState.End)
        {
            //chargeGauge.OnLevelChange -= GrantBuffs;
            return;
        }

        if (lvl == 0)
        {
            _statusManager.RemoveAllConditionWithSpecialID(101501);
            return;
        }
        
        
        if (lvl == 1)
        {
            var buff1 = new TimerBuff(_gaugeBuff1);
            buff1.dispellable = false;
            _statusManager.ObtainTimerBuff(buff1);
        }
        else if (lvl == 2)
        {
            var buff2 = new TimerBuff(_gaugeBuff2);
            buff2.dispellable = false;
            _statusManager.ObtainTimerBuff(buff2);
        }
        else if (lvl == 3)
        {
            var buff3 = new TimerBuff(_gaugeBuff3);
            buff3.dispellable = false;
            _statusManager.ObtainTimerBuff(buff3);
        }
    }

    private void GrantBlastProjectileCPGain(BlastProjectileCustom blast, int cpGainPerHit = 1)
    {
        SearchChargeGauge();

        Action<StatusManager, StatusManager, AttackBase, float> handler = null;

        handler = (self, target, atk, dmg) =>
        {
            if(_figAvailable)
                return;
            chargeGauge.Charge(cpGainPerHit);
        };

        blast.OnBlast += (atk) =>
        {
            atk.OnAttackDealDamage += handler;
        };

    }

    private void InitFig()
    {
        var figInstance = Instantiate(figPrefab, transform.position + new Vector3(0, 2.5f),
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        figInstance.AddComponent<RelativePositionRetainer>().SetParent(transform);

        var projController = figInstance.GetComponent<Projectile_C015_1>();

        projController.attackManager = this;
        
        projController.SetRenderer(false);

        _figInstance = figInstance;

    }

    private void InitDebuffContainer()
    {
        _debuffContainer = InitContainer(false,1).GetComponent<AttackContainer>();
        _debuffContainer.isEternal = true;
    }

    private void AddResonanceEffectToAttack(AttackFromPlayer atk, int withCondId = 1)
    {
        var resonance = new TimerBuff(_explosionDebuff);

        resonance.OnBuffRemove += ResonanceExplosion;
        
        atk.AddWithConditionAll(resonance,100,withCondId);
    }

    private void ResonanceExplosion(StatusManager target)
    {
        var atk = InstantiateRanged(skill4FX[0], target.transform.position,
           _debuffContainer.gameObject, 1).GetComponent<ForcedAttackFromPlayer>();

        atk.target = target.gameObject;
    }
    
    private void ResonanceExplosionForced(StatusManager self, StatusManager target, AttackBase atk, float dmg)
    {
        if(target.currentHp <= dmg)
            return;
        
        var buffList = target.GetConditionWithSpecialID(101503);

        var cnt = buffList.Count;

        if (buffList.Count > 0)
        {
            for (int i = buffList.Count - 1; i >= 0; i--)
            {
                buffList[i].OnBuffRemove -= ResonanceExplosion;
                target.RemoveConditionWithoutLog(buffList[i]);
            }
            target.OnBuffDispelledEventDelegate?.Invoke(
                new TimerBuff((int)BasicCalculation.BattleCondition.Resonance,
                    cnt,-1,3,101503));
        }
        else
        {
            return;
        }
        
        var atkForced = InstantiateRanged(skill4FX[0], target.transform.position,
            _debuffContainer.gameObject, 1).GetComponent<ForcedAttackFromPlayer>();

        atkForced.target = target.gameObject;

        atkForced.attackInfo[0].dmgModifier[0] *= cnt;

    }

    private void GrantSPRegenBuff()
    {
        TimerBuff spRegen = new TimerBuff((int)BasicCalculation.BattleCondition.SPRegen,
            2000, 10,100);
        
        spRegen.OnBuffUpdate += (stat) =>
        {
            var playerStats = stat as PlayerStatusManager;
            if (playerStats)
            {
                for (int i = 0; i < 4; i++)
                {
                    playerStats.ChargeSP(i,150);
                }
            }
        };
        
        spRegen.SetTickInterval(0.99f);

        _statusManager.ObtainTimerBuff(spRegen);
    }



}
