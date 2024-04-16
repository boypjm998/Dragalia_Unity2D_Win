using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using GameMechanics;
using UnityEngine;

public class Projectile_DB001_14 : MonoBehaviour,IEnemySealedContainer
{
    [SerializeField] GameObject enemySource;
    [SerializeField] protected List<GameObject> attacks = new();
    [SerializeField] private int firedir;
    [SerializeField] private float lockdirTime;
    protected int galeBlowDir;
    public GameObject currentFX;
    public GameObject arrowUI;
    public void SetEnemySource(GameObject source)
    {
        enemySource = source;
    }

    private IEnumerator Start()
    {
        
        foreach (var attack in attacks)
        {
            attack.GetComponent<AttackFromEnemy>().enemySource = enemySource;
            attack.GetComponent<AttackFromEnemy>().firedir = firedir;
        }
        
        yield return null;

        yield return new WaitForSeconds(lockdirTime);

        var behavior = enemySource.GetComponent<DragaliaEnemyBehavior>();
        var playerList = behavior.playerList;
        var blowDirList = new List<int>();
        foreach (var player in playerList)
        {
            blowDirList.Add(-player.GetComponent<ActorBase>().facedir);
        }

        foreach (var attack in attacks)
        {
            attack.GetComponent<DOTweenSimpleController>().enabled = true;
        }

        yield return new WaitForSeconds(4.5f);

        
        var blowPower = new AttackInfo();
        blowPower.knockbackPower = 200f;
        blowPower.knockbackTime = 0.3f;
        blowPower.knockbackForce = 50f;
        blowPower.KBType = BasicCalculation.KnockBackType.FixedDirection;
        blowPower.knockbackDirection = Vector2.right;

        for (int i = 0; i < playerList.Count; i++)
        {
            blowPower.knockbackDirection.x = blowDirList[i];
            playerList[i].GetComponent<ActorBase>().TakeDamage(blowPower,blowPower.knockbackDirection);
            Instantiate(currentFX, playerList[i].transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        yield return new WaitForSeconds(2f);
        
        for (int i = 0; i < playerList.Count; i++)
        {
            blowPower.knockbackDirection.x = blowDirList[i];
            playerList[i].GetComponent<ActorBase>().TakeDamage(blowPower,blowPower.knockbackDirection);
            Instantiate(currentFX, playerList[i].transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        yield return new WaitForSeconds(2f);
        
        for (int i = 0; i < playerList.Count; i++)
        {
            blowPower.knockbackDirection.x = blowDirList[i];
            playerList[i].GetComponent<ActorBase>().TakeDamage(blowPower,blowDirList[i]*Vector2.right);
            Instantiate(currentFX, playerList[i].transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        yield return new WaitForSeconds(3f);
        
        var UIArrow = 
            Instantiate(arrowUI,playerList[0].transform.position,Quaternion.identity,BattleStageManager.Instance.RangedAttackFXLayer.transform);
        if (firedir == -1)
        {
            UIArrow.transform.localScale = new Vector3(-1,1,1);
        }

        yield return new WaitForSeconds(1f);
        blowPower.firedir = firedir;
        blowPower.knockbackTime = 1f;
        blowPower.knockbackForce = 99f;
        
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].GetComponent<ActorBase>().TakeDamage(blowPower,Vector2.right*firedir);
            Instantiate(currentFX, playerList[i].transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        Destroy(gameObject,2f);

    }
}
