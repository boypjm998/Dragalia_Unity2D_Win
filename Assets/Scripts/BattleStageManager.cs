using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageManager : MonoBehaviour
{

    private DamageNumberManager damageNumberManager;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpChargeAll(GameObject playerHandle, float sp)
    {
        StatusManager statusManager = playerHandle.GetComponent<StatusManager>();

        //1、处理buff


        //2、SP充能

        for (int i = 0; i < statusManager.maxSkillNum; i++)
        {
            statusManager.SpGainInStatus(i, sp);
        }


    }

    public void SpCharge(GameObject playerHandle, float sp,int skillID)
    {
        StatusManager statusManager = playerHandle.GetComponent<StatusManager>();

        //1、处理buff


        //2、SP充能

        
         statusManager.SpGainInStatus(skillID, sp);
        


    }

    public virtual int PlayerHit(GameObject target, AttackFromPlayer attackStat)
    {
        GameObject player = GameObject.Find("PlayerHandle");

        //1、If target is not in invincible state.
        if (target.GetComponentInChildren<Collider2D>().isActiveAndEnabled == false)
        {
            return -1;
        }

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

            default:
                break;
        }






        int[] damageM = new int[attackStat.GetHitCount()];

        int totalDamage = 0;

        for (int i = 0; i < attackStat.GetHitCount(); i++)
        {

            //2、Calculate the damage deal to target.

            bool isCrit = false;



            int damage =
                BasicCalculation.CalculateDamageGeneral(
                player.GetComponentInChildren<StatusManager>(),
                target.GetComponentInChildren<StatusManager>(),
                attackStat.GetDmgModifier(i),
                attackStat.attackType,
                ref isCrit
                );

            damageM[i] = (int)(Mathf.Ceil(damage * Random.Range(0.95f, 1.05f)));

            //3、Special Effect




            //4、Instantiate the damage number.

            GameObject damageManager = GameObject.Find("DamageManager");
            DamageNumberManager dnm = damageManager.GetComponent<DamageNumberManager>();

            if (isCrit)
            {
                dnm.DamagePopEnemy(target.transform, damageM[i], 2);
            }
            else
            {
                dnm.DamagePopEnemy(target.transform, damageM[i], 1);
            }

            totalDamage += damageM[i];

            player.GetComponent<StatusManager>().ComboConnect();

        }

        



        //5、Calculate the SP

        AttackContainer container = attackStat.GetComponentInParent<AttackContainer>();
        if (!container.spGained)
        {
            SpChargeAll(player, attackStat.GetSpGain());
            container.spGained = true;
        }







        return totalDamage;

    }

    


}
