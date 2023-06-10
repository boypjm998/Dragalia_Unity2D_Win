using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using GameMechanics;
using CharacterSpecificProjectiles;
public class AttackManager_C001 : AttackManager
{
    [Header("AttackFXPlayers:")]
    [SerializeField]
    private GameObject Shotpoints;

    [SerializeField] private GameObject WeaponAddictiveEffect;
    
    //[SerializeField]
    //private GameObject attackContainer;
    [Header("Projectiles:")]
    public GameObject projectile1;//Roll
    public GameObject projectile2;//Std
    public GameObject projectile3;//Dash
    public GameObject projectile_m1;//Airdash

    //Test
    public GameObject projectile_s1;
    public GameObject projectile_s1_boost;
    public GameObject projectile_s2_boost;
    public GameObject projectile_s3;
    public GameObject projectile_s3_boost;
    public GameObject portal;
    public GameObject muzzleFX_1;
    public GameObject muzzleFX_2;
    public GameObject muzzleFX_3;
    public GameObject muzzleFX_4;
    

    [Header("Others:")]
    public Transform shotpoint;

    [SerializeField]
    private TargetAimer ta;
    
    protected new ActorController_c001 ac;
    
    //private PlayerStatusManager _statusManager;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ta = GetComponentInChildren<TargetAimer>();
        Shotpoints = GameObject.Find("Shotpoints");
        ac = GetComponent<ActorController_c001>();
        _statusManager = GetComponent<PlayerStatusManager>();
        
    }

    // Update is called once per frame
    

    public void rollAttack()
    {
        

        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "RollAttack");
        shotpoint = attackPointObject.transform;
        var muzzleFX = Instantiate(muzzleFX_1, shotpoint.position + new Vector3(ac.facedir*0.6f,0.4f,-3), transform.rotation, shotpoint);
        if(ac.facedir == -1)
            muzzleFX.transform.localScale = new Vector3(1, -1, 1);
        

        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(3, false);
        var fireMod = 0;
        if(ac.facedir==-1)
            fireMod = 180;

        //GameObject projectile_clone1 = Instantiate(projectile1, shotpoint.position, transform.rotation, container.transform);
        GameObject projectile_clone1 = Instantiate(projectile1, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+fireMod, transform.rotation.eulerAngles.z), container.transform);
       
        GameObject projectile_clone2 = Instantiate(projectile1, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+fireMod, transform.rotation.eulerAngles.z + 2.5f), container.transform);
        GameObject projectile_clone3 = Instantiate(projectile1, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+fireMod, transform.rotation.eulerAngles.z - 2.5f), container.transform);

        //projectile_clone1.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0, 0, 0, 0.24f, 0, ac.facedir);
        //projectile_clone2.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0, 0, 0, 0.23f, 0, ac.facedir);
        //projectile_clone3.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0, 0, 0, 0.23f, 0, ac.facedir);
    }
    public void ComboAttack1()
    {
        ta.TargetSwapByAttack();
        

        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        
        var muzzleFX = Instantiate(muzzleFX_2, attackPointObject.transform.position, Quaternion.identity, attackPointObject.transform);
        muzzleFX.transform.localScale = new Vector3(1, ac.facedir, 1);

        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(1, false);

        shotpoint = attackPointObject.transform;
        GameObject projectile_clone1 = Instantiate(projectile2, shotpoint.position, transform.rotation, container.transform);

        if (ac.facedir == -1)
        {
            //将子弹旋转欧拉角设为(0,180,0)
            projectile_clone1.transform.rotation = Quaternion.Euler(0, 180, 0);
        }


        projectile_clone1.GetComponent<AttackFromPlayer>().firedir = ac.facedir;
        //projectile_clone1.name = "Bullet1c";
        //Remember clear the signal after shoot.

    }

    // public void DashAttack()
    // {
    //     ta.TargetSwapByAttack();
    //     GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "DashAttack");
    //     shotpoint = attackPointObject.transform;
    //
    //     GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
    //
    //
    //     GameObject projectile_clone1 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 20f), container.transform);
    //     projectile_clone1.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(100, 15f, 0.3f, 0.8f, 300, ac.facedir);
    //
    //     GameObject projectile_clone2 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 13.3f), container.transform);
    //     projectile_clone2.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(100, 15f, 0.3f, 0.8f, 300, ac.facedir);
    //
    //     GameObject projectile_clone3 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 6.6f), container.transform);
    //     projectile_clone3.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(100, 15f, 0.3f, 0.8f, 300, ac.facedir);
    //
    //     GameObject projectile_clone4 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 0f), container.transform);
    //     projectile_clone4.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(100, 15f, 0.3f, 0.8f, 300, ac.facedir);
    //     //Remember clear the signal after shoot.
    //
    // }


    public void Skill1()
    {
        ta.TargetSwapByAttack();
        

        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        
        var muzzleFX = InstantiateDirectional(muzzleFX_2,attackPointObject.transform.position, attackPointObject.transform, ac.facedir);

        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(10, true);
        List<GameObject> projectiles = new List<GameObject>();
        float[] angleX = { 0.6f, 0.7f, 0.8f, 0.88f, 0.96f, 0.96f, 0.88f, 0.8f, 0.7f, 0.6f };
        float[] angleY = { 0.4f, 0.3f, 0.2f, 0.12f, 0.04f, -0.04f, -0.12f, -0.2f, -0.3f, -0.4f };
        for (int i = 0; i < 10; i++)
        {
            projectiles.Add(HomingBulletInstantiate(projectile_s1, shotpoint.position + new Vector3(Random.Range(-.5f, .5f), ac.facedir * Random.Range(-.5f, .5f)), container,
            (ac.facedir * new Vector2(angleX[i], angleY[i]).normalized), 120, 3,0.5f, 1.88f, 0, ac.facedir));
            
            projectiles[i].GetComponent<AttackFromPlayer>().
                AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                    41.6f,21f,
                    BasicCalculation.MAXCONDITIONSTACKNUMBER),120);
            
        }

        StartCoroutine(ac.HorizontalMove(-ac.movespeed * 0.5f, 0.2f,"s1"));
    }

    public void Skill1_Boost()
    {
        //_statusManager.RemoveTimerBuff(101);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,30,15);
        ta.TargetSwapByAttack();
        
        
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(1, true);
        
        GameObject laser1 = Instantiate(projectile_s1_boost, shotpoint.position,transform.rotation, container.transform);
        AttackFromPlayer atk1 = laser1.GetComponent<AttackFromPlayer>();
        atk1.GetComponent<AttackFromPlayer>().firedir = ac.facedir;
        for (int i = 0; i < 5; i++)
        {
            //atk1.AppendAttackSets(140,6.0f,0.8f,3.84f);
       
        }
        atk1.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                41.6f,21f,
                BasicCalculation.MAXCONDITIONSTACKNUMBER),120);
        

    }

    public void Skill1_Boost_PushBack()
    {
        StartCoroutine(ac.HorizontalMove(-ac.movespeed * 0.6f, 0.5f, "s1_boost"));
    }

    public void Skill2()
    {
        var gauge = FindObjectOfType<AlchemicGauge>();
        
        WeaponAddictiveEffect.transform.Find("Addictive_eff").gameObject.SetActive(true);
        //0.5s后禁用WeaponAddictiveEffect/Addictive_eff的游戏物体
        StartCoroutine(DisableWeaponAddictiveEffect(0.5f));
        
        
        _statusManager.ObtainTimerBuffs
        ((int)BasicCalculation.BattleCondition.AlchemicCatridge,
            -1,gauge.GetCatridgeNumber(),
            3,-1);
            //_statusManager.ObtainTimerBuff(1,25,3,BattleCondition.buffEffectDisplayType.Value,100);
            //_statusManager.ObtainTimerBuff(3,30,15,BattleCondition.buffEffectDisplayType.Value,100);
            //_statusManager.ObtainTimerBuff(4,30,-1,BattleCondition.buffEffectDisplayType.Value,100);
            //_statusManager.ObtainTimerBuff(2,25,8,BattleCondition.buffEffectDisplayType.Value,100);

        ac.ActiveAlchemicEnhancement();

        //
        //AlchemicGauge alchemicGauge = GameObject.Find("AlchemicGauge").GetComponent<AlchemicGauge>();

        //alchemicGauge.SetCatridgeActive();

        //
    }

    private IEnumerator DisableWeaponAddictiveEffect(float time)
    {
        yield return new WaitForSeconds(time);
        WeaponAddictiveEffect.transform.Find("Addictive_eff").gameObject.SetActive(false);
    }

    public void Skill2_Muzzle()
    {
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "DashAttack");
        shotpoint = attackPointObject.transform;
        InstantiateDirectional(muzzleFX_3,shotpoint.position, MeeleAttackFXLayer.transform,ac.facedir,1,1);
    }

    public void Skill2_Boost()
    {
        ta.TargetSwapByAttack();
        //_statusManager.RemoveTimerBuff(101);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,30,15);
        
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "DashAttack");
        shotpoint = attackPointObject.transform;
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(1, true);
        
        GameObject proj = Instantiate(projectile_s2_boost, shotpoint.position, Quaternion.identity, container.transform);
        
        
        
        
        //attackInfo.InitAttackBasicAttributes(0, 0, 0, 2.35f, 0, ac.facedir);
        //attackInfo.AppendAttackSets(1,1,1,23.54f);
        
        var currentPltformer = GetComponent<PlayerOnewayPlatformEffector>();
        proj.GetComponent<Projectile_C001_1>().SetContactTarget(currentPltformer.currentOnewayPlatform);
        proj.GetComponent<Projectile_C001_1>().InitProjectile(10*ac.facedir,12,45,ac.facedir);
        


    }

    public void Skill3()
    {
        ta.TargetSwapByAttack();
        
        
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(1, true);
        
        GameObject laser = Instantiate(projectile_s3,
            new Vector3(shotpoint.position.x+ac.facedir,shotpoint.position.y),
            transform.rotation, container.transform);
        
        //Inits are in the prefab.
        
    }
    
    public void Skill3_PushBack()
    {
        StartCoroutine(ac.HorizontalMove(-ac.movespeed * 1.5f, -20f, 0.4f,"s3"));
    }

    public void Skill3_GateOpen()
    {
        Vector3 gatePosition = new Vector3(transform.position.x + 12*ac.facedir, transform.position.y+0.3f);
        var battleManager = FindObjectOfType<BattleStageManager>();

        if (gatePosition.x >= battleManager.mapBorderR)
        {
            gatePosition.x = battleManager.mapBorderR - 0.1f;
        }else if (gatePosition.x <= battleManager.mapBorderL)
        {
            gatePosition.x = battleManager.mapBorderL + 0.1f;
        }

        var portalSet = FindObjectsOfType<AdventurerSpecial_Portal>();
        if (portalSet.Length > 0)
        {
            Destroy(portalSet[0].gameObject);
        }

        Instantiate(portal, gatePosition, Quaternion.identity, RangedAttackFXLayer.transform);

    }

    public void Skill3_Boost()
    {
        //_statusManager.RemoveTimerBuff(101);
        ta.TargetSwapByAttack();
        
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,30,15);
        
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(3, true);
        
        GameObject proj = InstantiateDirectional(projectile_s3_boost,
            new Vector3(shotpoint.position.x,shotpoint.position.y+.5f),
             container.transform,ac.facedir);
        
    }

    public void Skill3_Boost_Muzzle()
    {
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        InstantiateDirectional(muzzleFX_4,shotpoint.position+new Vector3(0.3f*ac.facedir,0,0), MeeleAttackFXLayer.transform,ac.facedir,1,1);
    }

    public void Skill4()
    {
        _statusManager.HPRegenImmediately(0,10);
        _effectManager.SpawnHealEffect(gameObject);
        //Instantiate(healbuff, transform.position, Quaternion.identity, BuffFXLayer.transform);
        _statusManager.ObtainTimerBuff
            ((int)BasicCalculation.BattleCondition.HealOverTime,
                -10,15);
        
        // if(_statusManager.healRoutine == null)
        //     _statusManager.healRoutine = StartCoroutine(_statusManager.HotRecoveryTick());
    
    }



    public override void AirDashAttack()
    {
        GameObject attackLayer = MeeleAttackFXLayer;
        GameObject container = Instantiate(attackContainer, attackLayer.transform.position, transform.rotation, MeeleAttackFXLayer.transform);

        GameObject dashEffect = Instantiate(projectile_m1, attackLayer.transform.position, transform.rotation,container.transform);
        dashEffect.name = "AirDashEffect";
        //dashEffect.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(110, 7.5f, 0.5f, 0.8f, 150, ac.facedir);
        dashEffect.GetComponent<AttackFromPlayer>().firedir = ac.facedir;
    }




    private GameObject FindShotpointInChildren(GameObject _parent, string childName)
    {
        for (int i = 0; i < _parent.transform.childCount; i++)
        {
            if (_parent.transform.GetChild(i).name == childName)
            {
                return _parent.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }

    private GameObject HomingBulletInstantiate(GameObject prefab, Vector3 shotpoint, GameObject targetLayer, Vector2 angle,
        float knockbackPower,float knockbackForce,float knockbackTime, float dmgModifier,int spGain,int firedir)
    {
        var instance = InstantiateDirectional(prefab, shotpoint, targetLayer.transform, ac.facedir);
        var attr = instance.GetComponent<HomingProjectile>();
        attr.angle = angle;
        attr.firedir = firedir;
        //attr.InitAttackBasicAttributes(knockbackPower,knockbackForce, knockbackTime, dmgModifier, spGain, firedir);

        return instance;
    }
        
}
