using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
using CharacterSpecificProjectiles;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// FLOATING Particles of Charging Flame
    /// </summary>
    public class Projectile_C005_7_Boss : MonoBehaviour
    {
        public ParticleSystem ParticleSystem;
        public ParticleSystem.ExternalForcesModule ExternalForcesModule;
        public static Projectile_C005_7_Boss Instance { get; private set; }

        /// <summary>
        /// upward为1时，惩罚BUFF等级较高。为-1时，惩罚BUFF等级较低。
        /// </summary>
        public int upward => (int)transform.localScale.y;

        private void Awake()
        {
            Instance = this;
            ParticleSystem = GetComponent<ParticleSystem>();
            ExternalForcesModule = ParticleSystem.externalForces;
        }

        private void Start()
        {
            var flameAreas = FindObjectsOfType<Projectile_C005_4_Boss>();
            foreach (var flameArea in flameAreas)
            {
                flameArea.OperateSpecialHints();
            }

        }

        

        public void AddInflunce(ParticleSystemForceField field)
        {
            ExternalForcesModule.AddInfluence(field);
        }
        

        private void OnDestroy()
        {
            var flameAreas = FindObjectsOfType<Projectile_C005_4_Boss>();
            foreach (var flameArea in flameAreas)
            {
                flameArea.OperateSpecialHints();
            }
            Instance = null;
        }
    }
    
}

