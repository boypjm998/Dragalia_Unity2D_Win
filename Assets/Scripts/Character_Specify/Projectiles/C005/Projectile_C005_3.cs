using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C005_3 : MonoBehaviour
    {
        [SerializeField]private float lengthA = 15f;
        [SerializeField]private float lengthB = 10f;
        private float angle;
        [SerializeField] private float startAngle;
        [SerializeField] private float rotateSpeed = 18;
        
        private AttackFromEnemy attackFromEnemy;
        private Tweener _tweener;
        private Tweener _tweener2;
        private Tweener _tweener3;
        private Sequence _sequence;

        public bool rotateEnable { get; set; } = true;

        void Start()
        {
            //angle = Mathf.Atan(transform.localPosition.x / transform.localPosition.y);
            //GetStartAngle();
            angle = startAngle*Mathf.Deg2Rad;
            attackFromEnemy = GetComponent<AttackFromEnemy>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if(!rotateEnable)
                return;
            
            
            MoveAround();
            angle = (angle + Time.fixedDeltaTime * rotateSpeed * Mathf.Deg2Rad) % (4*Mathf.PI);
        }
    
        private void Update()
        {
            if(!rotateEnable)
                return;
            transform.parent.rotation = Quaternion.identity;
        }
    
    
        void MoveAround()
        {
            //print(Mathf.Cos(angle));
            transform.localPosition =
                new Vector3(lengthA * Mathf.Cos(angle), lengthB * Mathf.Sin(angle));
        }
        void GetStartAngle()
        {
            if (transform.localPosition.x == 0)
            {
                if (transform.localPosition.y > 0)
                {
                    angle = 0;
                }else if (transform.localPosition.y < 0)
                {
                    angle = Mathf.PI;
                }
            }
    
            else if (transform.localPosition.y == 0)
            {
                if (transform.localPosition.x > 0)
                {
                    angle = Mathf.PI/2;
                }else if (transform.localPosition.x < 0)
                {
                    angle = Mathf.PI/2;
                }
            }
            else
            {
                angle = Mathf.Atan(transform.localPosition.y / transform.localPosition.x);
            }
    
    
        }
        
        public void SetRotateSpeed(float speed)
        {
            if (rotateSpeed < 0)
            {
                rotateSpeed = -speed;
            }
            else
            {
                rotateSpeed = speed;
            }

            
        }

        public void Reverse()
        {
            rotateSpeed = -rotateSpeed;
        }
        
        public void DisableAttack()
        {
            GetComponent<Collider2D>().enabled = false;
        }

        public void EnableAttack()
        {
            GetComponent<Collider2D>().enabled = true;
        }

        public void ChangeAttackProperty(AttackFromEnemy.AvoidableProperty property)
        {
            attackFromEnemy.ChangeAvoidability(property);
        }

        public void SetRotateRadius(float radiusA, float radiusB,float animationTime = 0f)
        {
            if (animationTime <= 0)
            {
                lengthA = radiusA;
                lengthB = radiusB;
            }
            else
            {
                _tweener2 = DOTween.To(() => lengthA, x => lengthA = x, radiusA, animationTime);
                _tweener3 = DOTween.To(() => lengthB, x => lengthB = x, radiusB, animationTime);
            }


        }

        public void SetScale(float scaleFactor,bool doscale = false,float duration = 1f)
        {
            if (doscale)
            {
                _tweener = transform.DOScale(new Vector3(scaleFactor, scaleFactor, 1), duration);
            }
            else
            {
                transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            }

            
        }
        
        public void SetDamageModifier(float dmgModifier)
        {
            attackFromEnemy.attackInfo[0].dmgModifier[0] = dmgModifier;
        }
        
        public void SetDamageFrequency(float dmgResetPeriod)
        {
            attackFromEnemy.damageAutoReset = dmgResetPeriod;
        }

        public void DoSpiralMovementToPoint(Vector3 endPoint, float duration, float amplitude, int vibrato, float elasticity)
        {
            var startPoint = transform.position;
            Vector3 midPoint = (startPoint + endPoint) / 2;

            // 创建一个序列
            Sequence sequence = DOTween.Sequence();

            // 添加从起始点A到中间点的曲线运动到序列中
            
            
            sequence.Append(transform.DOJump(midPoint, amplitude, vibrato, duration / 2, false).SetEase(Ease.OutQuad));
            sequence.Join(transform.DOScale(new Vector3(0.75f, 0.75f, 1), duration / 2).SetEase(Ease.OutElastic, elasticity));

            // 添加从中间点到终点B的曲线运动到序列中
            sequence.Append(transform.DOJump(endPoint, amplitude, vibrato, duration / 2, false).SetEase(Ease.InQuad));
            sequence.Join(transform.DOScale(new Vector3(1.5f, 1.5f, 1f), duration / 2).SetEase(Ease.OutElastic, elasticity));
        }


    }
}


