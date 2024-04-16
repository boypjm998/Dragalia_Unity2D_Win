using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 终末风暴 保护罩和球
/// </summary>
///
namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Charging Shield
    /// </summary>
    public class Projectile_C001_10_Boss : MonoSingleton<Projectile_C001_10_Boss>
    {
        [SerializeField] private GameObject shieldOnEffect;
        [SerializeField] private GameObject shieldOffEffect;
        [SerializeField] private GameObject forcingEffect;

        public void BreakShield()
        {
            shieldOffEffect.SetActive(true);
            shieldOnEffect.SetActive(false);
        }
        
        public void ChargeOff()
        {
            forcingEffect.SetActive(false);
        }
        
        
    }
}

