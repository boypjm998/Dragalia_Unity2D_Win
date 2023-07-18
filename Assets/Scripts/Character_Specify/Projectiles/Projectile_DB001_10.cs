using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile_DB001_10 : MonoBehaviour
{
    [SerializeField] private SpriteRenderer windCenter;
    [SerializeField] private GameObject windMeshCircle;
    [SerializeField] private GameObject windMeshCenter;
    [SerializeField] private bool isSky;
    
    private AttackFromEnemy attackFromEnemy;

    private List<Platform> mapInfo;
    private void Awake()
    {
        attackFromEnemy = GetComponent<AttackFromEnemy>();
        windMeshCircle.SetActive(false);
        windMeshCenter.SetActive(false);
        windCenter.color = new Color(1, 1, 1, 0);
        attackFromEnemy.AddWithConditionAll(
            new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,50,20,100),
            100);
    }

    private IEnumerator Start()
    {
        windCenter.DOColor(Color.white, 2f);
        //yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1.5f);
        
        windMeshCircle.SetActive(true);

        if (!isSky)
        {
            DisableAllCollisionWithPlatform();
            Invoke("RecoverAllCollisionWithPlatform",10f);
        }

        
        
        yield return new WaitForSeconds(1f);
        
        windMeshCenter.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        Destroy(gameObject, 10f);

    }

    private void Update()
    {
        attackFromEnemy.attackInfo[0].knockbackDirection.x = Random.Range(0, 0.1f);
    }

    protected void DisableAllCollisionWithPlatform()
    {
        var PlayerLayer = GameObject.Find("Player").transform;

        mapInfo = BattleStageManager.InitMapInfo();
        
        for(int i = 0; i < PlayerLayer.transform.childCount; i++)
        {
            var child = PlayerLayer.GetChild(i);
            
            foreach (var pltform in mapInfo)
            {
                Physics2D.IgnoreCollision(child.Find("Platform Sensor").GetComponent<Collider2D>(),
                    pltform.collider, true);
            }
        }
        
        
        
        
    }
    
    protected void RecoverAllCollisionWithPlatform()
    {
        //忽略Platforms层与Player层的碰撞
        var PlayerLayer = GameObject.Find("Player").transform;

        //var pltforms = BattleStageManager.InitMapInfo();
        
        for(int i = 0; i < PlayerLayer.transform.childCount; i++)
        {
            var child = PlayerLayer.GetChild(i);
            
            foreach (var pltform in mapInfo)
            {
                Physics2D.IgnoreCollision(child.Find("Platform Sensor").GetComponent<Collider2D>(),
                    pltform.collider, false);
            }
        }
        
    }


}
