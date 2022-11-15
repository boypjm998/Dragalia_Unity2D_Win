using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    // Record Character/Enemy's Status
    public bool isPlayer;

    //Current Status
    //public BasicCalculation.Affliction currentAffliction;
    public int maxSkillNum = 4;

    //SP

    public float[] requiredSP;
    public float[] currentSP;
    public bool[] skillRegenByAttack;
    private float spRegenPerSecond = 200;

    //Basic Properties (三维)
    public int maxBaseHP;
    public int baseDef;
    public int baseAtk;
    public int critRate;

    public int maxHP;
    public int currentHp;

    public float attackspeed = 1.0f;
    public float movespeed = 6.0f;
    public float jumpforce = 18.0f;
    public float rollspeed = 9.0f;
    public float comboConnectMaxInterval = 3.0f;

    //Other Resistance
    //Player's following properties will be 0 normally.
    //But Enemies' are Most over 0.
    public int knockbackRes = 0;
    private int burnRes = 0;
    private int paralyzeRes = 0;
    private int poisonRes = 0;
    private int frostbiteRes = 0;
    private int bogRes = 0;
    private int stunRes = 0;
    private int sleepRes = 0;
    private int blindRes = 0;
    private int frozenRes = 0;
    
    


    //Offensive Buff
    [SerializeField]private int attackBuffA = 0;
    [SerializeField]private int attackBuffB = 0;
    
    [SerializeField]private int skillDmgBuff = 0;
    [SerializeField]private int spGainBuff = 0;
    
    [SerializeField]private int critRateBuff = 0;
    [SerializeField]private int critDmgBuff = 0;
    
    private int breakPunisherBuff = 0;
    private int punisherBuff = 0;
    
    private int burnRateUpBuff;
    private int flashburnRateUpBuff;
    private int frostbiteRateUpBuff;
    private int bogRateUpBuff;
    private int paralysisRateUpBuff;
    
    private int knockbackPowerBuff;
    
    //Offensive Debuff
    [SerializeField]private int attackDebuff = 0;
    [SerializeField]private int skillDmgDebuff = 0;
    [SerializeField]private int critDmgDebuff = 0;
    [SerializeField]private int spGainDebuff = 0;

    //Defensive Buff & Debuff
    [SerializeField]private int defenseBuff = 0;
    [SerializeField]private int defenseDebuff = 0;
    [SerializeField]private int damageCut = 0;
    [SerializeField]private int damageCutConst = 0;
    private int lifeShield;
    private bool knockbackImmune = false;
    private int recoveryPotencyBuff;
    private int recoveryPotencyDebuff;
    private int healBuffNum = 0;


    public int comboHitCount = 0;
    private float lastComboRemainTime = 0;
    private Coroutine comboRoutine = null;


    private void Awake()
    {
        skillRegenByAttack = new bool[4]{ true, true, true, true };
    }

    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {
            SpGainInStatus(0, spRegenPerSecond * Time.deltaTime);
            SpGainInStatus(1, spRegenPerSecond * Time.deltaTime);
            SpGainInStatus(2, spRegenPerSecond * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        
    }

    public void SpGainInStatus(int id, float num)
    {
        //从0开始
        if(skillRegenByAttack[id])
            currentSP[id] += num;

        if (currentSP[id] > requiredSP[id])
        {
            currentSP[id] = requiredSP[id];
        }
    }

    public float GetAttackBuff()
    {
        //返回一个float.
        var totalBuff = attackBuffA + attackBuffB - attackDebuff;
        if (totalBuff > 200)
        {
            return 2;
        }
        else if (totalBuff < -50)
        {
            return -0.5f;
        }
        else {
            return 0.01f*(float)totalBuff;
        }
        
    }
    public float GetDefenseBuff()
    {
        //返回一个float.
        var totalBuff = defenseBuff - defenseDebuff;
        if (totalBuff > 200)
        {
            return 2;
        }
        else if (totalBuff < -50)
        {
            return -0.5f;
        }
        else
        {
            return 0.01f * (float)totalBuff;
        }

    }

    public int GetCritRateBuff()
    {
        return critRateBuff > 200 ? 200 : critRateBuff;
    }
    public float GetCritDamageBuff()
    {
        var totalBuff = critDmgBuff - critDmgDebuff;
        if (totalBuff > 500)
        {
            return 5;
        }
        else if (totalBuff < -120)
        {
            return -1.2f;
        }
        else
        {
            return 0.01f * (float)totalBuff;
        }


    }
    public float GetSkillDamageBuff()
    {
        var totalBuff = skillDmgBuff - skillDmgDebuff;
        if (totalBuff > 200)
        {
            return 2;
        }
        else if (totalBuff < -50)
        {
            return -0.5f;
        }
        else
        {
            return 0.01f * (float)totalBuff;
        }


    }

    public float GetDamageCut()
    {
        return damageCut > 100 ? 1 : (0.01f*(float)damageCut);
    }

    public float GetDamageCutConst()
    {
        return damageCutConst;
    }

    private IEnumerator ComboCheck()
    {
        if (comboHitCount > 0)
        {
            yield return new WaitForSeconds(comboConnectMaxInterval);
            comboHitCount = 0;
            lastComboRemainTime = 0;
        }
    }

    public void ComboConnect()
    {
        comboHitCount++;
        lastComboRemainTime = comboConnectMaxInterval;
        if(comboRoutine!=null)
            StopCoroutine(comboRoutine);
        comboRoutine = StartCoroutine(ComboCheck());
    }

    public void SetRequiredSP(int sidFromZero, float sp)
    {
        requiredSP[sidFromZero] = sp;
    }



}
