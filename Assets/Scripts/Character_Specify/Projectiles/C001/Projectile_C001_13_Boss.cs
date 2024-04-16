using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Shadow Mordecai
    /// </summary>
    public class Projectile_C001_13_Boss : MonoSingleton<Projectile_C001_13_Boss>
    {
        [SerializeField] private GameObject Skill1FXPrefab1;
        [SerializeField] private GameObject Skill1FXPrefab2;
        [FormerlySerializedAs("SKill2FXPrefab")]
        [SerializeField] private GameObject Skill2FXPrefab;
        [SerializeField] private GameObject Skill2FXPrefabReversed;

        private EnemyControllerHumanoid _ac;
        private StatusManager _statusManager;

        [SerializeField] private GameObject _iliaGameObject;

        private int skill1UsedTime = 0;

        public void SetIlia(GameObject go)
        {
            _iliaGameObject = go;
        }

        private IEnumerator Start()
        {
            _ac = GetComponent<EnemyControllerHumanoid>();
            _statusManager = GetComponent<StatusManager>();
            
            var taunt = new TimerBuff((int)BasicCalculation.BattleCondition.Taunt, 1, -1, 1);
            taunt.dispellable = false;
            var kbimmune = new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune, 1, -1, 1);
            kbimmune.dispellable = false;
            _statusManager.ObtainTimerBuff(taunt);
            _statusManager.ObtainTimerBuff(kbimmune);

            var target = BattleStageManager.Instance.GetPlayer();

            yield return new WaitForSeconds(2);
            
            _ac.TurnMove(target);

            if (TauntIsEffective() == false)
            {
                yield return _ac.MoveTowardTargetOnGround(target, 
                    5, 12, 20, 15);
                yield return Skill1();
            }
            else
            {
                yield return _ac.MoveTowardTargetOnGround(target, 
                    5, 15, 20, 18);
                yield return new WaitForSeconds(0.5f);
                yield return Skill2();
            }
            
            yield return new WaitForSeconds(2);
            
            if (TauntIsEffective() == false || skill1UsedTime == 0)
            {
                yield return _ac.MoveTowardTargetOnGround(target, 
                    5, 12, 20, 15);
                yield return Skill1();
            }
            else
            {
                yield return _ac.MoveTowardTargetOnGround(target, 
                    5, 15, 20, 18);
                yield return new WaitForSeconds(0.5f);
                yield return Skill2();
            }
            
            yield return new WaitForSeconds(2);
            
            if (TauntIsEffective() == false || skill1UsedTime == 0)
            {
                yield return _ac.MoveTowardTargetOnGround(target, 
                    5, 12, 10, 15);
                yield return Skill1();
            }
            else
            {
                yield return _ac.MoveTowardTargetOnGround(target, 
                    5, 15, 10, 18);
                yield return new WaitForSeconds(0.5f);
                yield return Skill2();
            }
            
            Destroy(gameObject,0.1f);
            gameObject.SetActive(false);
            
            
        }

        private bool TauntIsEffective()
        {
            var playerPos = BattleStageManager.Instance.GetPlayer().transform.position;

            var iliaPos = _iliaGameObject.transform.position;

            var selfPos = transform.position;

            bool flag = false;

            if (playerPos.x > iliaPos.x && iliaPos.x > selfPos.x)
            {
                flag = false;
            }else if (playerPos.x < iliaPos.x && iliaPos.x < selfPos.x)
            {
                flag = false;
            }
            else flag = true;
            
            print("flag:"+flag);

            return flag;


        }

        private IEnumerator Skill1()
        {
            //30: Jump
            //60: Wing
            //105: Dash
            skill1UsedTime++;
            
            _ac.anim.Play("s1");
            
            _ac.SetGravityScale(0);

            _ac.transform.DOMoveY(transform.position.y + 1, 0.6f).SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed);

            yield return new WaitForSeconds(1f);

            Instantiate(Skill1FXPrefab1, transform.position, Quaternion.identity, transform);

            yield return new WaitForSeconds(0.75f);

            _ac.transform.DOMove((gameObject.RaycastedPosition() + new Vector2(_ac.facedir*18f, 1.3f)).SafePosition(), 0.5f).SetEase(Ease.OutCirc).SetUpdate(UpdateType.Fixed);

            this.InstantiateRangedObject(Skill1FXPrefab2, transform.position + new Vector3(_ac.facedir*2,-1),
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), _ac.facedir, 0, transform);

            yield return new WaitUntil(()=>_ac.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));

            _ac.ResetGravityScale();
            yield break;

        }

        private IEnumerator Skill2()
        {
            //12,24,36

            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(_ac, transform.position, transform, new Vector2(22, 4),
                Vector2.zero, false, 0, 1, 0, 1f, true, true);
            
            yield return new WaitForSeconds(.9f);
            
            _ac.anim.Play("s2");

            var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position,
                Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            
            yield return new WaitForSeconds(.2f);
            
            this.InstantiateRangedObject(Skill2FXPrefab, transform.position,
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), _ac.facedir, 1, transform).GetComponent<DOTweenSimpleController>().moveDirection.x*=_ac.facedir;
            
            yield return new WaitForSeconds(.2f);
            
            this.InstantiateRangedObject(Skill2FXPrefabReversed, transform.position,
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), _ac.facedir, 1, transform).GetComponent<DOTweenSimpleController>().moveDirection.x*=_ac.facedir;
            
            yield return new WaitForSeconds(.2f);
            
            this.InstantiateRangedObject(Skill2FXPrefab, transform.position,
                Instantiate(BattleStageManager.Instance.attackContainerEnemy, transform.position, Quaternion.identity,
                    BattleStageManager.Instance.RangedAttackFXLayer.transform), _ac.facedir, 1, transform).GetComponent<DOTweenSimpleController>().moveDirection.x*=_ac.facedir;
            
            yield return new WaitUntil(()=>_ac.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));

        }
        
        
        
        
    }

}
