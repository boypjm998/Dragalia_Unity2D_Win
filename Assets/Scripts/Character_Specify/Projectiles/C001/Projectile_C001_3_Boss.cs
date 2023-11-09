using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C001_3_Boss : ProjectileControllerTest
    {
        [SerializeField] private GameObject destroyEffect;

        private AttackFromEnemy attackSet;

        // Start is called before the first frame update
        protected override void Start()
        {
            //lifeTime = 1.2f;
            base.Start();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            DoProjectileMove();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                attackSet = GetComponent<AttackFromEnemy>();
                attackSet.CauseDamage(col);
                BurstEffect();
            }

            if (col.CompareTag("Border")) BurstEffect();
        }

        protected override void DoProjectileMove()
        {
            var moveX = horizontalVelocity * Time.fixedDeltaTime;
            var moveY = verticalVelocity * Time.fixedDeltaTime;
            transform.position += transform.right * moveX;
            transform.position += transform.up * moveY;
        }

        private void BurstEffect()
        {
            var hitpoint = transform.position;
            Instantiate(destroyEffect, hitpoint, Quaternion.identity, transform.parent);
            CineMachineOperator.Instance.CamaraShake(5, .2f);

            Destroy(gameObject);
        }

        protected override void MyDestroySelf(float time)
        {
            Invoke(nameof(BurstEffect), time);
        }
    }

}
