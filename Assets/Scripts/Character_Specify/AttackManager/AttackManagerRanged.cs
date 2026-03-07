using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;


public class AttackManagerRanged : AttackManager
{
    public GameObject[] combo1FX;
    public GameObject[] combo2FX;
    public GameObject[] combo3FX;
    public GameObject[] combo4FX;
    public GameObject[] combo5FX;
    public GameObject[] dashFX;
    public GameObject[] skill1FX;
    public GameObject[] skill2FX;
    public GameObject[] skill3FX;
    public GameObject[] skill4FX;
    public GameObject[] ForceFX;
    protected TargetAimer ta;

    protected UI_ForceStrikeAimerArrow specialForceStrikeIndicator;
    
    protected GameObject Shotpoints;

    [SerializeField] protected BasicCalculation.RangedWeaponType weaponType;
    
    private AttackContainer _tempAttackContainer1;
    private AttackContainer _tempAttackContainer2;
    private AttackContainer _tempAttackContainer3;
    

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        ta = GetComponentInChildren<TargetAimer>();
        Shotpoints = transform.Find("Shotpoints").gameObject;
    }

    public void BowJumpShootAttack(float angle)
    {
        angle = Mathf.Round(angle);
        if (angle > 40)
        {
            angle = 40;
        }
        if (angle < -40)
        {
            angle = -40;
        }

        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.gameObject.transform);
        
        var shotPoint = 
            new Vector2
            (transform.position.x + 2.5f * ac.facedir * Mathf.Cos(angle*Mathf.Deg2Rad),
                transform.position.y + 2f * Mathf.Sin(angle*Mathf.Deg2Rad));
        
        
        var atk = InstantiateDirectionalRanged(dashFX[1],shotPoint,container,ac.facedir,angle*ac.facedir);
        var atk2 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle+3)*ac.facedir);
        var atk3 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle+6)*ac.facedir);
        var atk4 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle-6)*ac.facedir);
        var atk5 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle-3)*ac.facedir);

        

        
        
        
        
    }
    public virtual void ComboAttack1()
    {
        switch (weaponType)
        {
            case BasicCalculation.RangedWeaponType.Wand:
            {
                WandCombo1();
                break;
            }
            case BasicCalculation.RangedWeaponType.Staff:
            {
                StaffCombo1();
                break;
            }
            case BasicCalculation.RangedWeaponType.ManacasterShort:
            {
                ManacasterShortCombo1();
                break;
            }
        }
    }
    public virtual void ComboAttack2()
    {
        switch (weaponType)
        {
            case BasicCalculation.RangedWeaponType.Wand:
            {
                WandCombo2();
                break;
            }
            case BasicCalculation.RangedWeaponType.Staff:
            {
                StaffCombo2();
                break;
            }
        }
    }
    public virtual void ComboAttack3()
    {
        switch (weaponType)
        {
            case BasicCalculation.RangedWeaponType.Wand:
            {
                WandCombo3();
                break;
            }
            case BasicCalculation.RangedWeaponType.Staff:
            {
                StaffCombo3();
                break;
            }
        }
    }
    public virtual void ComboAttack4()
    {
        switch (weaponType)
        {
            case BasicCalculation.RangedWeaponType.Wand:
            {
                WandCombo4();
                break;
            }
            case BasicCalculation.RangedWeaponType.Staff:
            {
                StaffCombo4();
                break;
            }
        }
    }
    public virtual void ComboAttack5()
    {
        switch (weaponType)
        {
            case BasicCalculation.RangedWeaponType.Wand:
            {
                WandCombo5();
                break;
            }
            case BasicCalculation.RangedWeaponType.Staff:
            {
                StaffCombo5();
                break;
            }
        }
    }
    
    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,container);
        (ac as ActorController)?.PlayAttackVoice(0);
    }
    
    public void ForceStrikeCharging()
    {
        if (specialForceStrikeIndicator == null)
        {
            var prefabIndicator = Instantiate(ForceFX[0], transform.position, Quaternion.identity,
                BuffFXLayer.gameObject.transform);
            specialForceStrikeIndicator = prefabIndicator.GetComponent<UI_ForceStrikeAimerArrow>();
            prefabIndicator.name = "ForceStrikeIndicator";
            specialForceStrikeIndicator.SetActorController(ac as ActorControllerRangedWithFS);

            specialForceStrikeIndicator.SetMaxForceInfo(new float[] {(ac as ActorControllerRangedWithFS).forcingRequireTime}.ToList());
            if ((ac as ActorControllerRangedWithFS).maxForceLevel > 1)
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
            specialForceStrikeIndicator.gameObject.SetActive(true);
        }

    }
    
    public virtual void Skill1(int eventID)
    {
    }
    
    public virtual void Skill2(int eventID)
    {
        
    


    }
    
    public virtual void Skill3(int eventID)
    {
    }
    
    public virtual void Skill4(int eventID)
    {
        _statusManager.HPRegenImmediately(0,10,true);
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
        //Instantiate(healbuff, transform.position, Quaternion.identity, BuffFXLayer.transform);
        _statusManager.ObtainHealOverTimeBuff(10,15,true);
    }
    
    protected Transform FindShotpointInChildren(string childName)
    {
        var child = Shotpoints.transform.Find(childName);

        if (child != null)
            return child;

        return null;
    }
    
    public virtual void ForceStrikeRelease(int forcelevel = 0)
    {
        if(forcelevel <= 0)
            return;
        
        if (weaponType == BasicCalculation.RangedWeaponType.Bow)
        {
            
            
            var container = InitContainer(false);

            //var shotPoint = FindShotpointInChildren("StandardAttack");
        
            var atk = InstantiateDirectionalRanged(ForceFX[1],
                transform.position + new Vector3(ac.facedir *1f,0),
                container,ac.facedir,0);
            
            (ac as ActorController)?.PlayAttackVoice(9);
        }

    }

    public void BowCombo1()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");

        var atk1 = InstantiateDirectionalRanged(combo1FX[0],shotPoint.position,container,ac.facedir,0);
        var atk2 = InstantiateDirectionalRanged(combo1FX[1],shotPoint.position,container,ac.facedir,3);
        var atk3 = InstantiateDirectionalRanged(combo1FX[1],shotPoint.position,container,ac.facedir,-3);
    }
    
    public void BowCombo2_1()
    {
        
        var container = InitContainer(false);
        _tempAttackContainer1 = container.GetComponent<AttackContainer>();
        
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo2FX[0],shotPoint.position,
            _tempAttackContainer1.gameObject,ac.facedir,0);

    }
    
    public void BowCombo2_2()
    {
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo2FX[0],shotPoint.position,
            _tempAttackContainer1.gameObject,ac.facedir,0);
    }
    
    public void BowCombo3()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");

        var atk1 = InstantiateDirectionalRanged(combo3FX[0],shotPoint.position,container,ac.facedir,0);
        var atk2 = InstantiateDirectionalRanged(combo3FX[1],shotPoint.position,container,ac.facedir,3);
        var atk3 = InstantiateDirectionalRanged(combo3FX[1],shotPoint.position,container,ac.facedir,-3);

    }
    
    public void BowCombo4_1()
    {
        
        var container = InitContainer(false);
        _tempAttackContainer2 = container.GetComponent<AttackContainer>();
        
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo4FX[0],shotPoint.position,
            _tempAttackContainer2.gameObject,ac.facedir,0);

    }
    
    public void BowCombo4_2()
    {
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo4FX[0],shotPoint.position,
            _tempAttackContainer2.gameObject,ac.facedir,0);
    }

    public void BowCombo5()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");

        var atk1 = InstantiateDirectionalRanged(combo5FX[0],shotPoint.position,container,ac.facedir,0);
        var atk2 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,2);
        var atk3 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,-2);
        var atk4 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,4);
        var atk5 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,-4);
    }
    
    
    
    
    
    
    protected void ManacasterShortCombo1()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateRanged(combo1FX[0],shotPoint.position,container,ac.facedir);
    }
    
    

    /// <summary>
    /// 魔杖的Combo，要求：Muzzle放在Combo1[1]位置，否则重写
    /// </summary>
    protected void WandCombo1()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
      
        var proj = InstantiateRanged(combo1FX[0], transform.position,
            InitContainer(false),1);

        proj.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0);
        proj.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
    }

    protected void WandCombo2()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position
                                                + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
      
        var proj1 = InstantiateRanged(combo2FX[0], transform.position + new Vector3(0, 0.3f),
            InitContainer(false),1);

        proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.03f).normalized;
        proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
      
        var proj2 = InstantiateRanged(combo2FX[0], transform.position + new Vector3(0, -0.3f),
            InitContainer(false),1);

        proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.03f).normalized;
        proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
    }

    protected void WandCombo3()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
      
        var proj1 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, 0.5f),
            InitContainer(false),1);

        proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.03f).normalized;
        proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
      
        var proj2 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, 0f),
            InitContainer(false),1);

        proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0).normalized;
        proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
      
        var proj3 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, -0.5f),
            InitContainer(false),1);

        proj3.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.03f).normalized;
        proj3.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
    }

    protected void WandCombo4()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
      
        var proj1 = InstantiateRanged(combo4FX[0], transform.position + new Vector3(0, 0.4f),
            InitContainer(false),1);

        proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.025f).normalized;
        proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
      
        var proj2 = InstantiateRanged(combo4FX[0], transform.position + new Vector3(0, -0.4f),
            InitContainer(false),1);

        proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.025f).normalized;
        proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);
    }
    
    protected void WandCombo5()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);

        GameObject container = InitContainer(false,5);
      
        var proj1 = InstantiateRanged(combo5FX[0], 
            transform.position + new Vector3(ac.facedir,0),
            container,1);

        proj1.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir, 0).normalized;
        proj1.GetComponent<HomingAttack>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"),1);

        var dir = ac.facedir;
      
        //List<GameObject> projectiles = new();
        Vector2[] pointOffsets = new[]
        {
            new Vector2(ac.facedir * 0.1f, -0.75f),
            new Vector2(ac.facedir * 0.6f, -0.4f),
            new Vector2(ac.facedir * 0.6f, 0.4f),
            new Vector2(ac.facedir * 0.1f, 0.75f),
        };

        Vector2[] startAngles = new[]
        {
            new Vector2(ac.facedir, 0.1f).normalized,
            new Vector2(ac.facedir, 0.04f).normalized,
            new Vector2(ac.facedir, -0.1f).normalized,
            new Vector2(ac.facedir, -0.04f).normalized
        };

        float[] angleZ = new[]
        {
            12f, 6f, -6f, -12f
        };

        for (int i = 0; i < 4; i++)
        {
            var proj = InstantiateRanged(combo5FX[1],
                transform.position + (Vector3)pointOffsets[i],container
                ,dir);

            proj.name = "combo5_" + i;
            //Mathf.Rad2Deg * Mathf.Atan2(startAngles[i].y,startAngles[i].x)
            print(proj.name);
      
            proj.GetComponent<HomingAttack>().angle = new Vector2(ac.facedir,0);
            //proj.transform.rotation = Quaternion.Euler(0,0,angleZ[i]);
            proj.GetComponent<HomingAttack>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 22, 3,
                LayerMask.GetMask("Enemies"),1);
        }


      
    }

    protected void StaffCombo1()
    {
        var proj = InstantiateRanged(combo1FX[0], transform.position+ new Vector3(ac.facedir, 0),
            InitContainer(false),ac.facedir);
        proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
    }
    
    protected void StaffCombo2()
    {
        var proj = InstantiateRanged(combo2FX[0], transform.position+ new Vector3(ac.facedir, 0),
            InitContainer(false),ac.facedir);
        proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
    }
    
    protected void StaffCombo3()
    {
        var container = InitContainer(false,2);
        var proj1 = InstantiateRanged(combo3FX[0], transform.position+ new Vector3(ac.facedir, 0),
            container,ac.facedir);
        proj1.GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
        DOVirtual.DelayedCall(0.2f, () =>
        {
            InstantiateRanged(combo3FX[0], transform.position+ new Vector3(ac.facedir, 0),
                container, ac.facedir).GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;;
        },false);
    }
    
    protected void StaffCombo4()
    {
        var proj = InstantiateRanged(combo4FX[0], transform.position+ new Vector3(ac.facedir, 0),
            InitContainer(false),ac.facedir);
        proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
    }
    
    protected void StaffCombo5()
    {
        var proj = InstantiateRanged(combo5FX[0], transform.position+ new Vector3(ac.facedir, 0),
            InitContainer(false),ac.facedir);
        proj.GetComponent<DOTweenSimpleController>().moveDirection.x *= ac.facedir;
    }
    
    
    
}
