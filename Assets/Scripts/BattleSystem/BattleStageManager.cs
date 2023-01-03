using System.Collections;
using System.IO;
using UnityEngine;

public class BattleStageManager : MonoBehaviour
{
    public int chara_id;
    private AssetBundle assetBundle;

    private DamageNumberManager damageNumberManager;

    private DamageNumberManager dnm;
    public GameObject buffLogPrefab;
    private GameObject player;
    public GameObject boss;
    public float mapBorderL { get; private set; }
    public float mapBorderR { get; private set; }
    public float mapBorderT { get; private set; }

    private void Awake()
    {
        LoadDependency("ui_general");
        LoadPlayer(1);
        //FindPlayer();
        LinkBossStatus();
    }


    private void Start()
    {
        GetMapBorderInfo();
        //player = GameObject.Find("PlayerHandle");

        var damageManager = GameObject.Find("DamageManager");
        dnm = damageManager.GetComponent<DamageNumberManager>();
        
    }

    // Update is called once per frame
    

    #region LoadAssets

    protected void FindPlayer()
    {
        player = GameObject.Find("PlayerHandle");
    }

    protected virtual void LoadDependency(string name)
    {
        AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
        
    }

    protected virtual void LoadPlayer(int characterID)
    {
        assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "c001"));
        var plr = assetBundle.LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = Instantiate(plr, new Vector3(4.5f, -6.5f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        player = plrclone;


        player.GetComponent<AttackManager>().RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");

        var buffLayer = player.transform.Find("BuffLayer");
        var bufftxt = 
            Instantiate(buffLogPrefab, buffLayer.position + new Vector3(0, 2), Quaternion.identity, buffLayer);

    }

    #endregion
    
    /// <summary>
    ///   <para>获得地图边界</para>
    /// </summary>
    public void GetMapBorderInfo()
    {
        var borderInfoL = GameObject.Find("BorderLeft");
        var borderInfoR = GameObject.Find("BorderRight");
        var borderInfoT = GameObject.Find("BorderTop");

        mapBorderL = borderInfoL.GetComponent<BoxCollider2D>().offset.x +
                     borderInfoL.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoL.transform.position.x;
        mapBorderR = borderInfoR.GetComponent<BoxCollider2D>().offset.x -
            borderInfoR.GetComponent<BoxCollider2D>().size.x * 0.5f + borderInfoR.transform.position.x;
        mapBorderT = borderInfoT.GetComponent<BoxCollider2D>().offset.y -
                     borderInfoT.GetComponent<BoxCollider2D>().size.y * 0.5f + borderInfoT.transform.position.y;
    }

    /// <summary>
    /// test link
    /// </summary>
    public virtual void LinkBossStatus()
    {
        //summon boss
        var bossStat = GameObject.Find("UI").transform.Find("BossStatusBar").gameObject;
        bossStat.GetComponentInChildren<UI_BossStatus>().SetBoss(boss);
        bossStat.SetActive(true);
        
    }

    public void SpChargeAll(GameObject playerHandle, float sp)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();

        //1、计算技速BUFF


        //2、自充sp

        for (var i = 0; i < playerStatusManager.maxSkillNum; i++) playerStatusManager.SpGainInStatus(i, sp);
    }

    public void SpCharge(GameObject playerHandle, float sp, int skillID)
    {
        var playerStatusManager = playerHandle.GetComponent<PlayerStatusManager>();


        playerStatusManager.SpGainInStatus(skillID, sp);
    }

    #region DamageModule

    public virtual int PlayerHit(GameObject target, AttackFromPlayer attackStat)
    {
        //GameObject player = GameObject.Find("PlayerHandle");
        
        //print("进了playerhit");

        //1.If target is not in invincible state.
        if (!target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled) return -1;


        //Attack Callback
        switch (attackStat.attackType)
        {
            case BasicCalculation.AttackType.STANDARD:
                player.GetComponent<ActorController>().OnStandardAttackConnect(attackStat);
                break;
            case BasicCalculation.AttackType.SKILL:
                player.GetComponent<ActorController>().OnSkillConnect(attackStat);
                break;
            case BasicCalculation.AttackType.OTHER:
                player.GetComponent<ActorController>().OnOtherAttackConnect(attackStat);
                break;
        }

        var targetStat = target.GetComponentInChildren<StatusManager>();

        var damageM = new int[attackStat.GetHitCount()];

        var totalDamage = 0;

        var playerstat = player.GetComponentInChildren<PlayerStatusManager>();

        for (var i = 0; i < attackStat.GetHitCount(); i++)
        {
            //2.Calculate the damage deal to target.

            var isCrit = false;
            


            var damage =
                BasicCalculation.CalculateDamagePlayer(
                    playerstat,
                    targetStat,
                    attackStat.GetDmgModifier(i),
                    attackStat,
                    ref isCrit
                );

            damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

            //3.Special Effect


            //4.Instantiate the damage number.


            if (isCrit)
                dnm.DamagePopEnemy(target.transform, damageM[i], 2);
            else
                dnm.DamagePopEnemy(target.transform, damageM[i], 1);

            totalDamage += damageM[i];

            player.GetComponent<PlayerStatusManager>().ComboConnect();
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();

        //Affliction/Debuff
        if (attackStat.withConditions.Count > 0)
            if (!container.conditionCheckDone.Contains(target.GetInstanceID()))
            {
                for (var i = 0; i < attackStat.withConditionNum[0]; i++)
                {
                    if (!CheckAffliction(attackStat.withConditionChance[i], 0))
                    {
                        continue;//检查异常抗性，这里还没有获取Resistance！！
                        //之后要Get到抗性。
                    }


                    var newEffect = attackStat.withConditions[i].effect;
                    print(newEffect);
                    if (StatusManager.IsDotAffliction(attackStat.withConditions[i].buffID))
                        newEffect = 5f / 300f * newEffect * BasicCalculation.CalculateAttackInfo(playerstat) /
                                    BasicCalculation.CalculateDefenseInfo(targetStat);
                    

                    
                    if (attackStat.withConditions[i].maxStackNum > 1)
                    {
                        targetStat.ObtainTimerBuff
                        (attackStat.withConditions[i].buffID,
                            newEffect,
                            attackStat.withConditions[i].duration,
                            attackStat.withConditions[i].DisplayType,
                            attackStat.withConditions[i].maxStackNum);
                    }
                    else
                    {
                        targetStat.ObtainUnstackableTimerBuff
                        (attackStat.withConditions[i].buffID,
                            newEffect,
                            attackStat.withConditions[i].duration,
                            attackStat.withConditions[i].DisplayType,
                            attackStat.withConditions[i].specialID
                        );
                    }
                }

                container.conditionCheckDone.Add(target.GetInstanceID());
            }

        //KnockBack 击退
        var kbtemp = attackStat.knockbackDirection;
        kbtemp = attackStat.GetKBDirection(attackStat.KBType, target);
        target.GetComponentInParent<EnemyController>().
            TakeDamage(attackStat.knockbackPower,
                attackStat.knockbackTime, 
                attackStat.knockbackForce, kbtemp);

        //5.Calculate the SP
        

        if (!container.spGained)
        {
            SpChargeAll(player, attackStat.GetSpGain());
            container.spGained = true;
        }


        //Enemy Take Damage

        targetStat.currentHp -= totalDamage;
        


        return totalDamage;
    }

    public virtual int TargetHeal(GameObject target, float healPotency, float healPotencyPercentage)
    {
        var stat = target.GetComponent<StatusManager>();
        //1. 计算回血Part1
        var damage = BasicCalculation.CalculateHPRegenGeneral(stat, healPotency, healPotencyPercentage);

        var damageM = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

        dnm.HealPop(damageM, target.transform);

        stat.currentHp += damageM;

        return damageM;
    }

    public int TargetDot(GameObject target, BasicCalculation.BattleCondition condition)
    {
        var stat = target.GetComponent<StatusManager>();

        var modifier = stat.GetConditionTotalValue((int)condition);


        //Check target.HP

        var damage = (int)modifier;

        var damageM = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

        dnm.DotPop(damageM, target.transform, condition);

        stat.currentHp -= damageM;

        return damageM;
    }

    public int EnemyHit(GameObject target, GameObject self, AttackFromEnemy attackStat)
    {
        //1.If target is not in invincible state.
        if (target.transform.Find("HitSensor").GetComponent<Collider2D>().isActiveAndEnabled == false) return -1;

        //Attack Callback

        var targetStat = target.GetComponentInParent<PlayerStatusManager>();
        var damageM = new int[attackStat.GetHitCount()];
        var totalDamage = 0;
        
        //var selfstat = attackStat.enemySource;
        //if (selfstat == null)
        var selfstat = self.GetComponentInChildren<StatusManager>();
        //}
        
        


        for (var i = 0; i < attackStat.GetHitCount(); i++)
        {
            //2.Calculate the damage deal to target.

            var isCrit = false;
            var damage =
                BasicCalculation.CalculateDamageEnemy(
                    selfstat,
                    targetStat,
                    attackStat.GetDmgModifier(i),
                    attackStat,
                    ref isCrit
                );

            damageM[i] = (int)Mathf.Ceil(damage * Random.Range(0.95f, 1.05f));

            //3.Special Effect


            //4.Instantiate the damage number.


            if (isCrit)
                dnm.DamagePopPlayer(target.transform, damageM[i], true);
            else
                dnm.DamagePopPlayer(target.transform, damageM[i], false);

            totalDamage += damageM[i];
        }

        var container = attackStat.GetComponentInParent<AttackContainer>();

        //Affliction/Debuff
        if (attackStat.withConditions.Count > 0)
            if (!container.conditionCheckDone.Contains(target.GetInstanceID()))
            {
                for (var i = 0; i < attackStat.withConditionNum[0]; i++)
                {
                    if (attackStat.withConditions[i].buffID == 999)
                    {
                        targetStat.DispellTimerBuff();
                        continue;
                    }


                    var newEffect = attackStat.withConditions[i].effect;
                    print(newEffect);
                    if (StatusManager.IsDotAffliction(attackStat.withConditions[i].buffID))
                        newEffect = 5f / 300f * newEffect * BasicCalculation.CalculateAttackInfo(selfstat) /
                                    BasicCalculation.CalculateDefenseInfo(targetStat);

                    if (attackStat.withConditions[i].maxStackNum > 1)
                    {
                        targetStat.ObtainTimerBuff
                        (attackStat.withConditions[i].buffID,
                            newEffect,
                            attackStat.withConditions[i].duration,
                            attackStat.withConditions[i].DisplayType,
                            attackStat.withConditions[i].maxStackNum);
                    }
                    else
                    {
                        targetStat.ObtainUnstackableTimerBuff
                        (attackStat.withConditions[i].buffID,
                            newEffect,
                            attackStat.withConditions[i].duration,
                            attackStat.withConditions[i].DisplayType,
                            attackStat.withConditions[i].specialID
                            );
                    }


                }

                container.conditionCheckDone.Add(target.GetInstanceID());
            }
        
        //击退Knockback
        if (attackStat.knockbackPower > 99)
        {
            var kbdirtemp = attackStat.knockbackDirection;
            kbdirtemp = attackStat.GetKBDirection(attackStat.KBType, target);
            target.GetComponentInParent<ActorController>().
                TakeDamage(attackStat.knockbackTime,attackStat.knockbackForce,kbdirtemp);
        }

        

        //Damage
        targetStat.currentHp -= totalDamage;
        
        return totalDamage;
    }

    #endregion

    protected bool CheckAffliction(int chance, int resistance)
    {
        //检测异常状态是不是上的去
        if (resistance >= 100)
        {
            return false;
        }

        var rand = Random.Range(0, 100);
        {
            if (rand <= chance)
            {
                return true;
            }
            else return false;
        }


    }

    /// <summary>
    ///   <para>返回obj脚下踩着的地面</para>
    /// </summary>
    public Collider2D GetContactGround(GameObject obj)
    {
        return null;
    }
    
    



}