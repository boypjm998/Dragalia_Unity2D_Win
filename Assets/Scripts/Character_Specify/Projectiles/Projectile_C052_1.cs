using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C052_1 : MonoSingleton<Projectile_C052_1>
    {
        [SerializeField] private float speed;
        [SerializeField] private float controllableStartTime;
        [SerializeField] private float controllableEndTime;
        
        private PlayerInput _playerInput;
        private ActorController _actorController;
        private float _currentTime = 0;

        private void Start()
        {
            _playerInput = BattleStageManager.Instance.GetPlayer().GetComponent<PlayerInput>();
            _actorController = BattleStageManager.Instance.GetPlayer().GetComponent<ActorController>();
            _actorController.OnAttackInterrupt += LoseControl;
        }

        private void OnDestroy()
        {
            _actorController.OnAttackInterrupt -= LoseControl;
        }

        private void FixedUpdate()
        {
            if (_currentTime >= controllableStartTime && _currentTime <= controllableEndTime)
            {
                float moveDistance = speed * Time.deltaTime 
                                           * ((_playerInput.buttonRight.IsPressing ? 1 : 0) - (_playerInput.buttonLeft.IsPressing ? 1 : 0));
                
                transform.position += new Vector3(moveDistance, 0);
            }
            _currentTime += Time.fixedDeltaTime;
        }

        private void LoseControl()
        {
            _currentTime = controllableEndTime;
        }
        
    }

}
