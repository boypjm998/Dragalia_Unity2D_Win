using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_9_Boss : MonoBehaviour
 {
     public GameObject minon1;
     public GameObject minon2;
     public GameObject minon3;
     public GameObject minon4;
     private GameObject enemyLayer;
     private bool allMinonSummoned = false;
     private int remainMinons = 4;
     private GameObject lastMinon;
     private EnemyMoveController_HB02_Legend _parentController;
 
     public void SetController(EnemyMoveController_HB02_Legend controllerHb02Legend)
     {
         _parentController = controllerHb02Legend;
     }
 
     private void Start()
     {
         enemyLayer = GameObject.Find("EnemyLayer");
         Invoke(nameof(SummonMinon1),1f);
         Invoke(nameof(SummonMinon2),6f);
         Invoke(nameof(SummonMinon3),15f);
         Invoke(nameof(SummonMinon4),20f);

         if (_parentController.GetComponent<DragaliaEnemyBehavior>().difficulty == 5)
         {
             minon1.GetComponent<StatusManager>().maxBaseHP *= 3;
             minon2.GetComponent<StatusManager>().maxBaseHP *= 3;
             minon3.GetComponent<StatusManager>().maxBaseHP *= 3;
             minon4.GetComponent<StatusManager>().maxBaseHP *= 3;
         }
         
         
     }
 
     private void Update()
     {
         if (allMinonSummoned)
         {
             if (enemyLayer.transform.childCount <= 1)
             {
                 _parentController.gateOpen = false;
             }
 
             
         }
     }
 
     // Start is called before the first frame update
     private void OnTriggerEnter2D(Collider2D col)
     {
         if (col.GetComponentInParent<E2002_BehaviorTree>() != null)
         {
             var stat = col.GetComponentInParent<EnemyMoveController_E2002>();
             if (stat == null)
             {
                 print("stat is null");
                 return;
             }

             
             if (!stat.touched)
             {
                 stat.touched = true;
                 //自爆
             }
         }
     }
 
     void SummonMinon1()
     {
         var enemy = Instantiate(minon1,
             new Vector3(-22,-1,0),Quaternion.identity,enemyLayer.transform);
     }
 
     void SummonMinon2()
     {
         Instantiate(minon2,new Vector3(22,-1,0),Quaternion.identity,enemyLayer.transform);
     }
     
     void SummonMinon3()
     {
         Instantiate(minon3,new Vector3(-22,-1,0),Quaternion.identity,enemyLayer.transform);
     }
     
     void SummonMinon4()
     {
         lastMinon = Instantiate(minon4,new Vector3(22,-1,0),Quaternion.identity,enemyLayer.transform);
         allMinonSummoned = true;
     }
 
 
 }
}


