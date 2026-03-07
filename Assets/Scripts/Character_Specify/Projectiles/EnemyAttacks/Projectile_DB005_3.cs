using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Shadow Forte
    /// </summary>
    public class Projectile_DB005_3 : MonoSingleton<Projectile_DB005_3>
    {
        [SerializeField] private GameObject weaponR;
        [SerializeField] private GameObject weaponL;
        [SerializeField] private GameObject weaponModel;
    
        [SerializeField] private GameObject skillPrefabL;
        [SerializeField] private GameObject skillPrefabR;
    
        [SerializeField] private GameObject hintPrefab;
        [SerializeField] private GameObject disappearFXPrefab;
    
        [SerializeField] private int weaponAppearFrame = 20;
        [SerializeField] private int weaponRDisappearFrame = 85;
        [SerializeField] private int weaponLDisappearFrame = 40;
        [SerializeField] private int weaponMAppearFrame = 120;
    
        private Coroutine attackRoutine;
        private EnemyControllerHumanoid ac;
        private GameObject _targetPlayer;
    
        //[SerializeField] private bool debug;
    
        
        public void SetTarget(GameObject target)
        {
            _targetPlayer = target;
        }
    
        private IEnumerator Start()
        {
            ac = GetComponent<EnemyControllerHumanoid>();
            _targetPlayer = BattleStageManager.Instance.GetPlayer();
    
            BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3));
            yield return new WaitForSeconds(1);
            
            yield return ac.KeepDistanceFromTarget(_targetPlayer, 
                3, 8, 12,false);
    
            yield return null;
    
            ac.TurnMove(_targetPlayer);
            Instantiate(hintPrefab, transform.position, Quaternion.identity, transform);
    
            yield return new WaitForSeconds(1);
    
            attackRoutine = StartCoroutine(ACT_DragonbroodDemonspear());
    
            yield return new WaitUntil(()=>attackRoutine == null);
            
            yield return new WaitForSeconds(1);
                
            Instantiate(disappearFXPrefab, gameObject.RaycastedPosition(), Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            Destroy(gameObject,0.15f);
    
        }
        
    
        private IEnumerator ACT_DragonbroodDemonspear()
        {
            ac.TurnMove(_targetPlayer);
            ac.anim.Play("s1");
            
            yield return null;
    
            yield return new WaitUntil(()
                => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (weaponAppearFrame / 60f) / 2.5f);
            
            weaponModel.SetActive(false);
            weaponL.SetActive(true);
            weaponR.SetActive(true);
            
            yield return new WaitUntil(()
                => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (weaponLDisappearFrame / 60f) / 2.5f);
            
            weaponL.SetActive(false);
            this.InstantiateRangedObject(skillPrefabL, transform.position,
                BattleStageManager.Instance.GetNewRangedContainer(), ac.facedir, 0, transform);
            
            yield return new WaitUntil(()
                => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (weaponRDisappearFrame / 60f) / 2.5f);
    
            weaponR.SetActive(false);
            this.InstantiateRangedObject(skillPrefabR, transform.position,
                BattleStageManager.Instance.GetNewRangedContainer(), ac.facedir, 0, transform);
            
            yield return new WaitUntil(()
                => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (weaponMAppearFrame / 60f) / 2.5f);
            weaponModel.SetActive(true);
            
            yield return new WaitUntil(()
                => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
    
            attackRoutine = null;
    
        }
        
        
        
    }
}

