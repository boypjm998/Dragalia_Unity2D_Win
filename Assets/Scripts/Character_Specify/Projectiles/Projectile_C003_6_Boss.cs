using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_6_Boss : MonoBehaviour
    {
        public GameObject target;
        public bool isLocking = true;
        public int forceLevel = 0;
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (isLocking)
            {
                transform.position = new Vector3(target.transform.position.x,transform.position.y,transform.position.z);
                if (transform.position.x < BattleStageManager.Instance.mapBorderL + 6.5f)
                {
                    transform.position = new Vector3(BattleStageManager.Instance.mapBorderL + 6.5f,transform.position.y,transform.position.z);
                }

                if (transform.position.x > BattleStageManager.Instance.mapBorderR - 6.5f)
                {
                    transform.position = new Vector3(BattleStageManager.Instance.mapBorderR - 6.5f,transform.position.y,transform.position.z);
                }
            }
        }

        public void DisplayHint()
        {
            isLocking = false;
            transform.GetChild(1).gameObject.SetActive(true);
        }
        
        public void HideHint()
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }

        public void HideForcingHint()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void StartAttack()
        {
            transform.GetChild(2).gameObject.SetActive(true);
        }

        private void ToForcingState(int level)
        {
            forceLevel = level;
        }

        public void SetEnemySource(GameObject enemy)
        {
            transform.GetChild(2).GetComponent<AttackFromEnemy>().enemySource = enemy;
            transform.GetComponentInChildren<Projectile_C003_6>()._statusManager = enemy.GetComponent<StatusManager>();
        }
    }
}

