using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_c001 : MonoBehaviour
{
    [Header("AttackFXPlayers:")]
    [SerializeField]
    private GameObject Shotpoints;
    [SerializeField] 
    private GameObject RangedAttackFXLayer;
    [SerializeField]
    private GameObject MeeleAttackFXLayer;
    [SerializeField]
    private GameObject attackContainer;
    [Header("Projectiles:")]
    public GameObject projectile1;//Roll
    public GameObject projectile2;//Std
    public GameObject projectile3;//Dash
    public GameObject projectile_m1;//Airdash

    //Test
    public GameObject projectile_s1;
    public GameObject projectile_s1_boost;
    public GameObject projectile_s2_boost;

    public Transform shotpoint;

    [SerializeField]
    private TargetAimer ta;

    public int testButton;
    private ActorController_c001 ac;

    // Start is called before the first frame update
    private void Awake()
    {
        ta = GetComponentInChildren<TargetAimer>();
        //Shotpoints = GameObject.Find("Shotpoints");
        ac = GetComponent<ActorController_c001>();
        testButton = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetKey("u") && testButton==0)
        //{
        //    testButton = 30;
        //    TestDriveBuster();
        //}
        //if (testButton > 0)
        //    testButton--;
    }

    private void rollAttack()
    {
        

        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "RollAttack");
        shotpoint = attackPointObject.transform;

        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(3, false);

        GameObject projectile_clone1 = Instantiate(projectile1, shotpoint.position, transform.rotation, container.transform);
       
        GameObject projectile_clone2 = Instantiate(projectile1, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 2.5f), container.transform);
        GameObject projectile_clone3 = Instantiate(projectile1, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 2.5f), container.transform);

        projectile_clone1.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0, 0, 0, 0.24f, 0, ac.facedir);
        projectile_clone2.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0, 0, 0, 0.23f, 0, ac.facedir);
        projectile_clone3.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(0, 0, 0, 0.23f, 0, ac.facedir);
    }
    private void ComboAttack1()
    {
        ta.TargetSwapByAttack();
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");

        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(1, false);

        shotpoint = attackPointObject.transform;
        GameObject projectile_clone1 = Instantiate(projectile2, shotpoint.position, transform.rotation, container.transform);

        projectile_clone1.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(1, 0.5f, 0.3f, 1.35f, 280, ac.facedir);
        //projectile_clone1.name = "Bullet1c";
        //Remember clear the signal after shoot.

    }

    private void DashAttack()
    {
        ta.TargetSwapByAttack();
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "DashAttack");
        shotpoint = attackPointObject.transform;

        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);


        GameObject projectile_clone1 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 20f), container.transform);
        projectile_clone1.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(1, 0.6f, 0.5f, 0.8f, 300, ac.facedir);

        GameObject projectile_clone2 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 13.3f), container.transform);
        projectile_clone2.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(1, 0.6f, 0.5f, 0.8f, 300, ac.facedir);

        GameObject projectile_clone3 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 6.6f), container.transform);
        projectile_clone3.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(1, 0.6f, 0.5f, 0.8f, 300, ac.facedir);

        GameObject projectile_clone4 = Instantiate(projectile3, shotpoint.position, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 0f), container.transform);
        projectile_clone4.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(1, 0.6f, 0.5f, 0.8f, 300, ac.facedir);
        //Remember clear the signal after shoot.

    }


    private void Skill1()
    {
        ta.TargetSwapByAttack();

        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;

        //GameObject projectile_clone1 = Instantiate(projectileT, shotpoint.position+new Vector3(Random.Range(-1f,1f),Random.Range(0f,2f)), transform.rotation, RangedAttackFXLayer.transform);
        //projectile_clone1.name = "Bullet1t";
        //projectile_clone1.GetComponent<HomingProjectile>().angle = (ac.facedir * new Vector2(0.5f, 0.5f).normalized);
        //projectile_clone1.GetComponent<HomingProjectile>().InitAttackBasicAttributes(0, 1, 0, ac.facedir);
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(10, true);
        List<GameObject> projectiles = new List<GameObject>();
        float[] angleX = { 0.6f, 0.7f, 0.8f, 0.88f, 0.96f, 0.96f, 0.88f, 0.8f, 0.7f, 0.6f };
        float[] angleY = { 0.4f, 0.3f, 0.2f, 0.12f, 0.04f, -0.04f, -0.12f, -0.2f, -0.3f, -0.4f };
        for (int i = 0; i < 10; i++)
        {
            projectiles.Add(HomingBulletInstantiate(projectile_s1, shotpoint.position + new Vector3(Random.Range(-.5f, .5f), ac.facedir * Random.Range(-.5f, .5f)), container,
            (ac.facedir * new Vector2(angleX[i], angleY[i]).normalized), 1, 1,0.5f, 1.88f, 0, ac.facedir));
            
        }

        StartCoroutine(ac.HorizontalMove(-ac.movespeed * 0.5f, 0.2f,"s1"));




    }

    private void Skill1_Boost()
    {
        ta.TargetSwapByAttack();
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "StandardAttack");
        shotpoint = attackPointObject.transform;
        
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, MeeleAttackFXLayer.transform);
        container.GetComponent<AttackContainer>().InitAttackContainer(1, true);
        
        GameObject laser1 = Instantiate(projectile_s1_boost, shotpoint.position, transform.rotation, container.transform);
        AttackFromPlayer atk1 = laser1.GetComponent<AttackFromPlayer>();
        atk1.InitAttackBasicAttributes(1.5f, 2.0f, 0.8f, 3.84f, 0, ac.facedir);
        for (int i = 0; i < 5; i++)
        {
            atk1.AppendAttackSets(1.5f,2.0f,0.8f,3.84f);
        }
        

    }

    private void Skill1_Boost_PushBack()
    {
        StartCoroutine(ac.HorizontalMove(-ac.movespeed * 0.6f, 0.5f, "s1_boost"));
    }

    private void Skill2()
    {
        //¼ÓBUFF
        ac.ActiveAlchemicEnhancement();

        //
        //AlchemicGauge alchemicGauge = GameObject.Find("AlchemicGauge").GetComponent<AlchemicGauge>();

        //alchemicGauge.SetCatridgeActive();





        //
    }

    private void Skill2_Boost()
    {
        ta.TargetSwapByAttack();
        
        GameObject attackPointObject = FindShotpointInChildren(Shotpoints, "DashAttack");
        shotpoint = attackPointObject.transform;
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, RangedAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(1, true);
        
        GameObject proj = Instantiate(projectile_s2_boost, shotpoint.position, Quaternion.identity, container.transform);
        
        
        
        
        //attackInfo.InitAttackBasicAttributes(0, 0, 0, 2.35f, 0, ac.facedir);
        //attackInfo.AppendAttackSets(1,1,1,23.54f);
        
        var currentPltformer = GetComponent<PlayerOnewayPlatformEffector>();
        proj.GetComponent<Projectile_C001_1>().SetContactTarget(currentPltformer.currentOnewayPlatform);
        proj.GetComponent<Projectile_C001_1>().InitProjectile(10*ac.facedir,15,30,ac.facedir);
        


    }



    private void AirDashAttack()
    {
        GameObject attackLayer = MeeleAttackFXLayer;
        GameObject container = Instantiate(attackContainer, shotpoint.position, transform.rotation, MeeleAttackFXLayer.transform);

        GameObject dashEffect = Instantiate(projectile_m1, attackLayer.transform.position, transform.rotation,container.transform);
        dashEffect.name = "AirDashEffect";
        dashEffect.GetComponent<AttackFromPlayer>().InitAttackBasicAttributes(1, 2.5f, 0.5f, 0.8f, 150, ac.facedir);
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
        var instance = Instantiate(prefab, shotpoint, transform.rotation, targetLayer.transform);
        var attr = instance.GetComponent<HomingProjectile>();
        attr.angle = angle;
        attr.InitAttackBasicAttributes(knockbackPower,knockbackForce, knockbackTime, dmgModifier, spGain, firedir);

        return instance;
    }


}
