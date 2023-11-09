using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 绝级难度 猩红咒焰Controller
    /// </summary>
    public class Projectile_C005_3_Boss : MonoBehaviour
    {
        private List<Projectile_C005_3> fireballList = new();
        public static Projectile_C005_3_Boss Instance { get; private set; }

        private int type;

        public bool noExtraFireBalls = false;
        
        public bool transformCompleted = false;

        private void Awake()
        {
            var other = FindObjectsOfType<Projectile_C005_3_Boss>();
            if (other.Length > 1)
            {
                Destroy(gameObject);
            }

            Instance = this;
            type = Random.Range(0, 2);
            if(noExtraFireBalls)
                return;
            
            if (type == 1)
            {
                transform.Find("random1").gameObject.SetActive(true);//右上
            }
            else
            {
                transform.Find("random2").gameObject.SetActive(true);//左下
            }
        }

        void Start()
        {
            fireballList = GetComponentsInChildren<Projectile_C005_3>().ToList();
            if(type!=1)
            {
                foreach (var fireball in fireballList)
                {
                    fireball.Reverse();
                }
            }

        }

        public void ChangeAllAttackPropertiesToRed()
        {
            foreach (var fireball in fireballList)
            {
                fireball.ChangeAttackProperty(AttackFromEnemy.AvoidableProperty.Red);
            }
        }

        public void DoSpiralMovements()
        {
            transformCompleted = true;
            List<Vector2> endPoints = new();
            for (int i = 0; i < 7; i++)
            {
                var endPoint = new Vector3(16.5f - 5.5f * i, -10,0);
                endPoints.Add(endPoint);
            }
            
            
            //根据fireball的transform.position.x对fireballList进行排序
            fireballList.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            for (int i = 0; i < fireballList.Count; i++)
            {
                
                fireballList[i].DoSpiralMovementToPoint(endPoints[i],2f,Random.Range(0.9f,1.1f),3,Random.Range(0.8f,1.2f));
            }
        }

        public void DisableRotation()
        {
            foreach (var fireball in fireballList)
            {
                fireball.rotateEnable = false;
            }
        }

        public void SetRotateSpeed(float speed)
        {
            foreach (var fireball in fireballList)
            {
                fireball.SetRotateSpeed(speed);
            }
        }

        public void SetScale(float scale,float time)
        {
            foreach (var fireball in fireballList)
            {
                fireball.SetScale(scale,true,time);
            }
        }
        
        public void SetRotateRadius(float radius,float time)
        {
            foreach (var fireball in fireballList)
            {
                fireball.SetRotateRadius(radius,radius,time);
            }
        }

        public void SetAttack(float multi)
        {
            foreach (var fireball in fireballList)
            {
                fireball.SetDamageModifier(multi);
            }
        }



    }
}

