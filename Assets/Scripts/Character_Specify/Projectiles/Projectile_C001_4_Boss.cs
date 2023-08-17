using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 黑洞
    /// </summary>
    public class Projectile_C001_4_Boss : MonoBehaviour
    {
        private CircleCollider2D blackholeDragger;
    
        public float force;
        public float floatPunisher = 1f;
        [SerializeField] private ParticleSystem screamingFX;

        public void SetParticleSize(float size)
        {
            var ps_main = screamingFX.main;
            ps_main.startSize = size;
            screamingFX.Play();
        }

        public float radius
        {
            get
           
            {
                return blackholeDragger.radius;
            }
            set
            {
                blackholeDragger.radius = value;
            }
        }

        private GameObject blackhole;
        
        private bool blackholeOpen = false;
    
        public int intervalFrame = 50;
        
        public bool debugReset = false;
    
        private int currentCD = 0;
    
    
        private void Awake()
        {
            blackhole = transform.GetChild(0).gameObject;
            blackholeDragger = blackhole.GetComponent<CircleCollider2D>();
            Invoke(nameof(OpenBlackHole),0.5f);
        }
    
        // Start is called before the first frame update
        void OpenBlackHole()
        {
            blackhole.SetActive(true);
            blackholeOpen = true;
        }

        private void Update()
        {
            if (debugReset)
            {
                debugReset = false;
                SetParticleSize(50f);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {

            if (!blackholeOpen)
            {
                return;
            }
            
            if(currentCD < intervalFrame)
            {
                currentCD++;
                return;
            }
            
            currentCD = 0;
            
            
            //blackholeDragger.
            var hitinfos =
                Physics2D.OverlapCircleAll
                (blackholeDragger.transform.position,
                    blackholeDragger.radius,LayerMask.GetMask("Characters"));
            if (hitinfos.Length > 0)
            {
                foreach (var hitinfo in hitinfos)
                {
                    if (hitinfo.GetComponentInParent<StatusManager>().KnockbackRes > 99)
                    {
                        continue;
                    }
    
                    Vector3 dir;
                    float distance = Vector2.Distance(hitinfo.transform.position, transform.position);
                    float distanceModifier = 1;
                    if (distance > blackholeDragger.radius * 0.5f)
                    {
                        distanceModifier = 0.4f * Mathf.Min(blackholeDragger.radius / (distance + 0.1f), 1);
                    }
    
                    var rigid = hitinfo.GetComponentInParent<Rigidbody2D>();
    
                    if (hitinfo.GetComponentInParent<ActorController>().grounded || Mathf.Abs(rigid.velocity.y) < 0.5f)
                    {
                        dir = transform.position.x > hitinfo.transform.position.x ? Vector3.right*Mathf.Floor(distanceModifier) :
                            Vector3.left*Mathf.Floor(distanceModifier);
                    }
                    else
                    {
                        //distanceModifier *= (2 + Mathf.Min(blackholeDragger.radius/(distance+0.1f),2));
                        distanceModifier *= (1+floatPunisher);
                        if(distance<blackholeDragger.radius * 0.1f)
                        {
                            distanceModifier = 1+floatPunisher/2;
                        }
                        //dir = -transform.InverseTransformPoint(hitinfo.transform.position).normalized;
                        dir = (transform.position - hitinfo.transform.parent.position).normalized * distanceModifier;
                        
                    }
    
    
                    rigid.position += (Vector2)(dir * Time.fixedDeltaTime * force);
                    print("moved transform");
                    //hitinfo.transform.parent.GetComponent<Rigidbody2D>().velocity = dir * force * Time.fixedDeltaTime;
                }
            }
        
        
        }
    }
}

