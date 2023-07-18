using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_3_PL : MonoBehaviour
    {
        [Range(0.1f,1f)]public float throwInterval = 0.25f;
        private GameObject myGameObject;
        private Transform enemyLayer;
        private List<Transform> targetList = new();
        [SerializeField]private List<Projectile_C003_2_PL> projectileList = new();
        private int thrownCount = 0;
        public int firedir = 1;
        private void Start()
        {
            if(myGameObject == null)
                myGameObject = GameObject.Find("PlayerHandle").gameObject;
            enemyLayer = GameObject.Find("EnemyLayer").transform;
            targetList = BattleStageManager.Instance.GetEnemyList();
            for(int i = 0 ; i < transform.childCount ; i++)
            {
                var projectile = transform.GetChild(i).GetComponent<Projectile_C003_2_PL>();
                projectile.SetFiredir(firedir);
                projectileList.Add(projectile);
                
            }
            //将targetList按照和myGameObject的距离排序
            if (targetList.Count > 0)
            {
                targetList.Sort((a, b) =>
                {
                    var aDistance = Vector2.Distance(a.position, myGameObject.transform.position);
                    var bDistance = Vector2.Distance(b.position, myGameObject.transform.position);
                    return aDistance.CompareTo(bDistance);
                });
    
            }
            else
            {
                targetList.Add(myGameObject.transform);
            }
            DistrubuteProjectileTargets(targetList);
            ThrowProjectile();
            Invoke("ThrowProjectile", throwInterval);
            Invoke("ThrowProjectile", throwInterval * 2);
            Invoke("ThrowProjectile", throwInterval * 3);
            Invoke("ThrowProjectile", throwInterval * 4);
        }
        
        public virtual List<Transform> SearchEnemyList()
        {
            var hitFlags = new List<Transform>();
    
            var enemyLayer = GameObject.Find("EnemyLayer");
            for (var i = 0; i < enemyLayer.transform.childCount; i++)
            {
                var ene = enemyLayer.transform.GetChild(i).gameObject;
                if (ene.activeSelf && ene.GetComponent<ActorBase>().HitSensor.gameObject.activeSelf)
                {
                    hitFlags.Add(enemyLayer.transform.GetChild(i));
                }

               
            }

            
            return hitFlags;
        }
    
        void DistrubuteProjectileTargets(List<Transform> targets)
        {
            for (int i = 0; i < 5; i++)
            {
                projectileList[i].SetContactTarget(targets[i % targets.Count].gameObject);
            }
        }
    
        void ThrowProjectile()
        {
            thrownCount++;
            if(thrownCount>5)
                return;
            //transform.GetChild(thrownCount - 1).gameObject.SetActive(true);
            projectileList[thrownCount - 1].gameObject.SetActive(true);
        }
    
        private void OnDestroy()
        {
            CancelInvoke();
        }
    }
}

