using System.Collections.Generic;
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
    
    protected GameObject Shotpoints;

    [SerializeField] protected BasicCalculation.RangedWeaponType weaponType;
    
    

    protected override void Start()
    {
        base.Start();
        ta = GetComponentInChildren<TargetAimer>();
        Shotpoints = transform.Find("Shotpoints").gameObject;
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
        }
    }
    
    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,container);
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
            LayerMask.GetMask("Enemies"));
      
        var proj2 = InstantiateRanged(combo2FX[0], transform.position + new Vector3(0, -0.3f),
            InitContainer(false),1);

        proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.03f).normalized;
        proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
    }

    protected void WandCombo3()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
      
        var proj1 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, 0.5f),
            InitContainer(false),1);

        proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.03f).normalized;
        proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
      
        var proj2 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, 0f),
            InitContainer(false),1);

        proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0).normalized;
        proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
      
        var proj3 = InstantiateRanged(combo3FX[0], transform.position + new Vector3(0, -0.5f),
            InitContainer(false),1);

        proj3.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.03f).normalized;
        proj3.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
    }

    protected void WandCombo4()
    {
        var muzzleFX = Instantiate(combo1FX[1], transform.position + new Vector3(ac.facedir, 0),
            Quaternion.identity, RangedAttackFXLayer.transform);
      
        var proj1 = InstantiateRanged(combo4FX[0], transform.position + new Vector3(0, 0.4f),
            InitContainer(false),1);

        proj1.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, -0.025f).normalized;
        proj1.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
      
        var proj2 = InstantiateRanged(combo4FX[0], transform.position + new Vector3(0, -0.4f),
            InitContainer(false),1);

        proj2.GetComponent<HomingAttackWithoutRotate>().angle = new Vector2(ac.facedir, 0.025f).normalized;
        proj2.GetComponent<HomingAttackWithoutRotate>().target = ta.GetNearestTargetInRangeDirection(ac.facedir, 21, 3,
            LayerMask.GetMask("Enemies"));
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
            LayerMask.GetMask("Enemies"));

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
                LayerMask.GetMask("Enemies"));
        }


      
    }

}
