using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Transport
    /// </summary>
    public class Projectile_C001_2_Boss : MonoBehaviour
    {
        [SerializeField] private GameObject transportationEffect;
        public static Projectile_C001_2_Boss Instance { get; private set;}

        private void Awake()
        {
            if(Instance != null)
                Destroy(Instance.gameObject);
            Instance = this;
        }
    
        public void Transport(Transform origin)
        {
            var eff = Instantiate(transportationEffect, origin.position, Quaternion.identity);
            var eff2 = Instantiate(transportationEffect, transform.position, Quaternion.identity);
            eff2.GetComponent<AdventurerSpecial_PortalTrail>().InitAnim(origin);
            
            origin.transform.position = this.transform.position;
            
            eff.GetComponent<AdventurerSpecial_PortalTrail>().InitAnim(transform);
            
            
            Destroy(gameObject);
        }
    }
}

