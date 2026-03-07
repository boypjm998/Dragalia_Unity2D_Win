using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Ice Platform
    /// </summary>
    public class Projectile_C007_7_Boss : MonoSingleton<Projectile_C007_7_Boss>
    {
        [SerializeField] private GameObject _icePlatformInstance;
        BoxCollider2D _boxCollider2D;
        Vector2 _initSize;
        Vector2 _initOffset;
        
        public bool IsActivated => _icePlatformInstance.activeSelf;
        public Vector2 Position => _icePlatformInstance.transform.position;
        private void Start()
        {
            _boxCollider2D = _icePlatformInstance.GetComponent<BoxCollider2D>();
            _initSize = _boxCollider2D.size;
            _initOffset = _boxCollider2D.offset;
        }
        
        public void GenerateIcePlatform(Vector2 position)
        {
            if(position.x < BattleStageManager.Instance.mapBorderR - _initSize.x / 2 &&
               position.x > BattleStageManager.Instance.mapBorderL + _initSize.x / 2)
            {
                _boxCollider2D.size = _initSize;
                _boxCollider2D.offset = _initOffset;
            }
            else if(position.x >= BattleStageManager.Instance.mapBorderR - _initSize.x / 2)
            {
                var length = _initSize.x * 0.5f + (BattleStageManager.Instance.mapBorderR - position.x);
                _boxCollider2D.size = new Vector2(length, _initSize.y);
                _boxCollider2D.offset = new Vector2(_initOffset.x + (length - _initSize.x) / 2, _initOffset.y);
            }
            else if(position.x <= BattleStageManager.Instance.mapBorderL + _initSize.x / 2)
            {
                var length = _initSize.x * 0.5f + (position.x - BattleStageManager.Instance.mapBorderL);
                _boxCollider2D.size = new Vector2(length, _initSize.y);
                _boxCollider2D.offset = new Vector2(_initOffset.x - (length - _initSize.x) / 2, _initOffset.y);
            }
            else
            {
                _boxCollider2D.size = _initSize;
                _boxCollider2D.offset = _initOffset;
            }
            
            _icePlatformInstance.transform.position = position;
            _icePlatformInstance.SetActive(true);
            BattleStageManager.Instance.RefreshMapInfo();
            
        }
        
        public void DestroyIcePlatform()
        {
            _icePlatformInstance.SetActive(false);
            BattleStageManager.Instance.RefreshMapInfo();
        }
        
        
    }

}
