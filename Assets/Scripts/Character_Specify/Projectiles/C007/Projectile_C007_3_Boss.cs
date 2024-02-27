using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Ice pillar
    /// </summary>
    public class Projectile_C007_3_Boss : MonoBehaviour
    {
        [SerializeField] private GameObject level1Prefab;
        [SerializeField] private GameObject level2Prefab;

        [SerializeField] private GameObject blastInstanceLv1;
        [SerializeField] private GameObject blastInstanceLv2;
        
        public bool HasDestroyed { get; private set; }
        public Vector2 LowerPoint
        {
            get => new(transform.position.x, transform.position.y - 0.5f);
        }

        public Vector2 UpperPoint
        {
            get => new(transform.position.x, transform.position.y + 3.5f * _iceLevel);
        }

        public int Level => _iceLevel;
        
        private int _iceLevel = 1;
        private Collider2D _collider;

        private void Awake()
        {
            Projectile_C007_2_Boss.Instance?.AddIcePillar(this);
        }

        private void OnDestroy()
        {
            
            Projectile_C007_2_Boss.Instance?.RemoveIcePillar(this);
            
        }

        public void DestroyInstance()
        {
            print("Destroy"+gameObject);
            
            HasDestroyed = true;
            Projectile_C007_2_Boss.Instance?.RemoveIcePillar(this);
            
            level1Prefab.SetActive(false);
            level2Prefab.SetActive(false);
            
            if(_iceLevel == 1)
                blastInstanceLv1.SetActive(true);
            else if(_iceLevel == 2)
                blastInstanceLv2.SetActive(true);
            
            Destroy(gameObject,1.5f);
        }
        
        public void LevelUp()
        {
            print("LVUP");
            if (_iceLevel == 1)
                _iceLevel = 2;
            
            level1Prefab.SetActive(false);
            level2Prefab.SetActive(true);
        }
        
        
    }

}
