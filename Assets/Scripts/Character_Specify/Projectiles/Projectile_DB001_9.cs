using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    
    public class Projectile_DB001_9 : MonoBehaviour
    {
        [SerializeField] GameObject projectile_wind;
        public GameObject target;
        private Tweener _tweener;
        protected bool buffUsed = false;
        protected Tweener _tweenerPlayer;

        private List<ActorBase> caughtTargetsAc = new();
        private IEnumerator Start()
        {
            var hint = GetComponentInChildren<EnemyAttackHintBarCircle>();

            yield return new WaitForSeconds(0.1f);
            
            print(hint.warningTimeLeft);

            while (hint.warningTimeLeft > 1)
            {
                if (Mathf.Abs(transform.position.x - target.transform.position.x) > 1)
                {
                    _tweener = transform.DOMoveX(target.transform.position.x, 0.5f);
                }

                yield return new WaitForSeconds(0.5f);
            }


            while(hint.warningTimeLeft > 0)
            {
                print(hint.warningTimeLeft);
                yield return null;
            }
            //yield return new WaitUntil(()=>hint.warningTimeLeft <= 0);
            
            hint.gameObject.SetActive(false);

            projectile_wind.SetActive(true);

            yield return new WaitForSeconds(1f);

            GetComponent<BoxCollider2D>().enabled = true;

            yield return new WaitForSeconds(25f);
            
            projectile_wind.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = true;

            yield return new WaitUntil(() => caughtTargetsAc.Count <= 0);
            Destroy(gameObject);

        }

        private void OnTriggerStay2D(Collider2D col)
        {
            
            if (col.CompareTag("Player"))
            {
                var ac = col.GetComponentInParent<ActorController>();
                if(ac==null)
                    return;
                if (ac.hurt || !ac.grounded)
                {
                    return;
                }

                if (!buffUsed)
                {
                    var playerStat = col.GetComponentInParent<StatusManager>();
                    buffUsed = true;
                
                    playerStat.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
                        200,15);
                }

                if (ac.dodging == false && caughtTargetsAc.Contains(ac) == false)
                {
                    caughtTargetsAc.Add(ac);
                    ac.GetComponent<StatusManager>().knockbackRes = 999;
                    var _sequence =
                        ac.transform.DOJump(transform.position + new Vector3(Random.Range(-3f, 3f), 0, 0),
                            30, 1, 12).SetEase(Ease.InOutSine).OnKill
                        (() =>
                        {
                            caughtTargetsAc.Remove(ac);
                            ac.ResetGravityScale();
                            ac.SetActionUnable(false);
                            ac.GetComponent<StatusManager>().knockbackRes = 0;
                            Physics2D.IgnoreCollision(ac.HitSensor, GetComponent<Collider2D>());
                        }).OnComplete
                        (() =>
                        {
                            ac.GetComponent<StatusManager>().knockbackRes = 0;
                            caughtTargetsAc.Remove(ac);
                            ac.ResetGravityScale();
                            ac.SetActionUnable(false);
                            Physics2D.IgnoreCollision(ac.HitSensor, GetComponent<Collider2D>());
                        });

                    _sequence.OnUpdate(() => { 
                        if(ac.HitSensor.enabled == false)
                            _sequence.Kill(); 
                    });
                }




            }
        }

        private void FixedUpdate()
        {
            foreach (var ac in caughtTargetsAc)
            {
                ac.SetActionUnable(true);
                ac.SetGravityScale(0);
            }
        }

        
    }

}
