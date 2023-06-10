using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C001_3 : MonoBehaviour
    {
        //Otherworld Gate Controller for Ilia.
        private CircleCollider2D blackholeDragger;
        private float lifetime = 5.01f;
        private float lefttime;
        private float resetCD = 0.1f;
        private AttackFromPlayer _attackFromPlayer;
        private Coroutine timerRoutine;
        
        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject,lifetime);
            lefttime = lifetime;
            InvokeRepeating("ResetFlag",0.1f,0.1f);
            blackholeDragger = transform.GetChild(0).GetComponent<CircleCollider2D>();
            _attackFromPlayer = GetComponent<AttackFromPlayer>();
        }
    
        // Update is called once per frame
        void Update()
        {
            //blackholeDragger.
            var hitinfos =
                Physics2D.OverlapCircleAll
                    (blackholeDragger.transform.position,
                    blackholeDragger.radius,LayerMask.GetMask("Enemies"));
            if (hitinfos.Length > 0)
            {
                foreach (var hitinfo in hitinfos)
                {
                    if (hitinfo.GetComponentInParent<EnemyController>().currentKBRes + 
                        hitinfo.GetComponentInParent<StatusManager>().GetKBResBuff() >= 99)
                    {
                        continue;
                    }
    
                    var dir = -transform.InverseTransformPoint(hitinfo.transform.position).normalized;
                    hitinfo.transform.parent.position += dir * Time.deltaTime * 5f;
                }
            }
    
    
        }
    
        void ResetFlag()
        {
            _attackFromPlayer.ResetFlags();
        }
    
    
    
    
    
    }
}


