using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 炼金护盾
    /// </summary>
    public class Projectile_C001_6_Boss : MonoSingleton<Projectile_C001_6_Boss>
    {
        private GameObject fx;
        private GameObject elec;
        private EnemyControllerHumanoid ac;
        private EnemyMoveController_HB03_Legend _enemyMoveControllerHb03Legend;

        protected override void Awake()
        {
            base.Awake();
            ac = GetComponentInParent<EnemyControllerHumanoid>();
            _enemyMoveControllerHb03Legend = ac.GetComponent<EnemyMoveController_HB03_Legend>();
            _enemyMoveControllerHb03Legend.ShieldOn = true;
        }

        private void Start()
        {
            
            
            fx = transform.GetChild(0).gameObject;
            elec = transform.GetChild(1).gameObject;
        }
    
        public void ActiveElectricFX()
        {
            elec.SetActive(true);
        }
        
        private void OnDestroy()
        {
            _enemyMoveControllerHb03Legend.ShieldOn = false;
        }
    
        private void Update()
        {
            if (ac.minimapIcon.activeInHierarchy)
            {
                fx.SetActive(true);
            }
            else
            {
                fx.SetActive(false);
            }
        }
    }
}

