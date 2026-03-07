using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// 漩涡
    /// </summary>
    public class Projectile_H004_3 : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 20;
        [SerializeField] private float checkRadius = 4;

        private Projectile_H004_2 _cachedBat; // 缓存单例引用
        private bool _hasValidBat;            // 状态标记

        private void Start()
        {
            Destroy(gameObject, lifeTime);
    
            // 立即尝试缓存引用
            CacheBatReference();
        }

        private void Update()
        {
            // 当没有有效引用时尝试获取
            if (!_hasValidBat)
            {
                // 每3帧检查一次(避免频繁调用FindObjectOfType)
                CacheBatReference();
                return;
            }
    
            // 安全检查：确保缓存对象仍然有效
            if (_cachedBat == null || _cachedBat.Destruct)
            {
                _hasValidBat = false;
                return;
            }
            
            Vector2 position1 = transform.position;  // 当前对象位置
            Vector2 position2 = _cachedBat.transform.position; // 蝙蝠位置
    
            float distance = Vector2.Distance(position1, position2);
    
            if (distance <= checkRadius)
            {
                // 确保只触发一次
                if (!_cachedBat.Destruct) 
                {
                    _cachedBat.DoDeath();
                }
            }
        }

        private void CacheBatReference()
        {
            // 尝试获取单例
            var bat = Projectile_H004_2.Instance;
    
            // 验证有效性(避免获取到已销毁对象)
            _hasValidBat = bat != null;
    
            if (_hasValidBat)
            {
                _cachedBat = bat; // 缓存有效引用
            }
        }
    }
}

