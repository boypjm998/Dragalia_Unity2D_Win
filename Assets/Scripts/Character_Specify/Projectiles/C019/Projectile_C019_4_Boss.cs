using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Blessed wall
    /// </summary>
    public class Projectile_C019_4_Boss : MonoBehaviour
    {
        
        private EnemyMoveController_HB04 _enemyMoveController;
        
        public void SetEnemyMoveController(EnemyMoveController_HB04 enemyMoveController)
        {
            _enemyMoveController = enemyMoveController;
        }

        private void OnDestroy()
        {
            _enemyMoveController?.blessedWalls.Remove(this);
        }
    }

}
