using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    // Record Character/Enemy's Status

    //Current Status
    public BasicCalculation.Affliction currentAffliction;
    public int maxSkillNum = 4;

    //SP

    public int[] requiredSP;


    public int[] currentSP;

    //Basic Properties (ÈýÎ¬)
    public int maxBaseHP;
    public int baseDef;
    public int baseAtk;

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
    public int burnRes = 0;
    public int paralyzeRes = 0;
    public int poisonRes = 0;
    public int frostbiteRes = 0;
    public int bogRes = 0;
    public int stunRes = 0;
    public int sleepRes = 0;
    public int blindRes = 0;
    public int frozenRes = 0;
    
    


    //Offensive Buff
    private int attackBuffA;
    private int attackBuffB;
    
    private int skillDmgBuff;
    private int spGainBuff;
    
    private int critRateBuff;
    private int critDmgBuff;
    
    private int breakPunisherBuff;
    private int punisherBuff;
    
    private int burnRateUpBuff;
    private int flashburnRateUpBuff;
    private int frostbiteRateUpBuff;
    private int bogRateUpBuff;
    private int paralysisRateUpBuff;
    
    private int knockbackPowerBuff;
    
    //Offensive Debuff
    private int attackDebuff;
    private int skillDmgDebuff;
    private int critDmgDebuff;
    private int spGainDebuff;

    //Defensive Buff & Debuff
    private int defenseBuff;
    private int defenseDebuff;
    private int damageCut;
    private int damageCutConst;
    private int lifeShield;
    private bool knockbackImmune;
    private int recoveryPotencyBuff;
    private int recoveryPotencyDebuff;
    private int healBuffNum;


    public int comboHitCount;

    
    

    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
