using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    /// <summary>
    /// Origa Legend: Ultimate Move
    /// </summary>
    public class Projectile_C007_8_Boss : MonoSingleton<Projectile_C007_8_Boss>, IEnemySealedContainer
    {
        [SerializeField] private LaunchPreset orbLB;
        [SerializeField] private LaunchPreset orbLT;
        [SerializeField] private LaunchPreset orbM;
        [SerializeField] private LaunchPreset orbRT;
        [SerializeField] private LaunchPreset orbRB;
        
        [SerializeField] private GameObject laserFXPrefab;
        
        private GameObject _enemySource;

        public void SetEnemySource(GameObject source)
        {
            _enemySource = source;
            transform.Find("FF").GetComponent<AttackFromEnemy>().enemySource = source;
        }
        
        private void Start()
        {
            
        }

        public int LaunchFirstTime()
        {
            var randomSeed = Random.Range(0, 4);
            
            switch(randomSeed)
            {
                case 0:
                    ReadyLaunchOrbRandom(orbLB,1);
                    ReadyLaunchOrbRandom(orbM,2,3);
                    break;
                case 1:
                    ReadyLaunchOrbRandom(orbLB,2);
                    ReadyLaunchOrbRandom(orbM,0);
                    break;
                
                case 2:
                    ReadyLaunchOrbRandom(orbRB,1);
                    ReadyLaunchOrbRandom(orbM,0,1);
                    break;
                case 3:
                    ReadyLaunchOrbRandom(orbRB,2);
                    ReadyLaunchOrbRandom(orbM,3);
                    break;
            }

            return randomSeed;

        }
        
        public void LaunchSecondTime(int randomSeed)
        {
            switch(randomSeed)
            {
                case 0:
                    ReadyLaunchOrbRandom(orbLT,1,2);
                    ReadyLaunchOrbRandom(orbRT);
                    ReadyLaunchOrbRandom(orbRB,1,2);
                    break;
                case 1:
                    ReadyLaunchOrbRandom(orbLT);
                    ReadyLaunchOrbRandom(orbRT,1,2);
                    ReadyLaunchOrbRandom(orbRB,1,2);
                    break;
                
                case 2:
                    ReadyLaunchOrbRandom(orbLT,1,2);
                    ReadyLaunchOrbRandom(orbRT);
                    ReadyLaunchOrbRandom(orbLB,1,2);
                    break;
                case 3:
                    ReadyLaunchOrbRandom(orbLT);
                    ReadyLaunchOrbRandom(orbRT,1,2);
                    ReadyLaunchOrbRandom(orbLB,1,2);
                    break;
            }
        }


        public void LaunchOneByOne()
        {
            var randomSeed = Random.Range(0, 10);
            
            switch(randomSeed)
            {
                case 0:
                    GroupI();
                    break;
                case 1:
                    GroupII();
                    break;
                case 2:
                    GroupIII();
                    break;
                case 3:
                    GroupIV();
                    break;
                case 4:
                    GroupV();
                    break;
                case 5:
                    GroupVI();
                    break;
                case 6:
                    GroupVII();
                    break;
                case 7:
                    GroupVIII();
                    break;
                case 8:
                    GroupIX();
                    break;
                case 9:
                    GroupX();
                    break;
            }

            
            
        }

        # region DelayedGroups
        private void GroupI()
        {
            ReadyLaunchOrbRandom(orbLB,0);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbLT,2),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,0,1),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbRT,1),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbRB,1),false);
        }
        
        private void GroupII()
        {
            ReadyLaunchOrbRandom(orbLB,0);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbLT,1),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,2,3),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbRT,0),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbRB,2),false);
        }
        
        private void GroupIII()
        {
            ReadyLaunchOrbRandom(orbLB,1);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbLT,2),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,3),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbRT,2),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbRB,0),false);
        }
        
        private void GroupIV()
        {
            ReadyLaunchOrbRandom(orbLB,2);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbLT,0),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,2),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbRT,1),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbRB,0),false);
        }
        
        private void GroupV()
        {
            ReadyLaunchOrbRandom(orbLB,2);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbLT,0),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,2),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbRT,2),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbRB,0,2),false);
        }
        
        private void GroupVI()
        {
            ReadyLaunchOrbRandom(orbRB,0);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbRT,2),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,2,3),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbLT,1),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbLB,1),false);
        }
        
        private void GroupVII()
        {
            ReadyLaunchOrbRandom(orbRB,0);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbRT,1),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,0,1),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbLT,0),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbLB,2),false);
        }
        
        private void GroupVIII()
        {
            ReadyLaunchOrbRandom(orbRB,1);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbRT,2),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,0),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbLT,2),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbLB,0),false);
        }
        
        private void GroupIX()
        {
            ReadyLaunchOrbRandom(orbRB,2);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbRT,0),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,1),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbLT,1),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbLB,0),false);
        }
        
        private void GroupX()
        {
            ReadyLaunchOrbRandom(orbRB,2);
            DOVirtual.DelayedCall(1.5f, () => ReadyLaunchOrbRandom(orbRT,0),false);
            DOVirtual.DelayedCall(3f, () => ReadyLaunchOrbRandom(orbM,1),false);
            DOVirtual.DelayedCall(4.5f, () => ReadyLaunchOrbRandom(orbLT,2),false);
            DOVirtual.DelayedCall(6f, () => ReadyLaunchOrbRandom(orbLB,0,2),false);
        }
        #endregion
        
        



        private void ReadyLaunchOrbRandom(LaunchPreset preset, params int[] fixedGroup)
        {
            //If fixedGroup is not empty, find all elements in preset whose index is in fixedGroup
            
            var angles = new List<int>();
            if (fixedGroup.Length > 0)
            {
                foreach (var index in fixedGroup)
                {
                    angles.Add(preset.anglePresets[index]);
                }
            }
            else
            {
                angles = preset.anglePresets;
            }
            
            if(angles.Count == 0)
                angles = preset.anglePresets;
            
            LaunchLaser(preset.orb, angles[Random.Range(0, angles.Count)]);

        }

        private void LaunchLaser(GameObject orb, int angle)
        {
            var laser = this.InstantiateDirectionalRangedObject(laserFXPrefab,
                orb.transform.position, BattleStageManager.Instance.GetNewRangedContainer(),1,angle,
                _enemySource.transform);

            DOVirtual.DelayedCall(4, 
                () => laser.transform.GetChild(0).gameObject.SetActive(true), false);

        }
        


        [Serializable]
        public class LaunchPreset
        {
            public GameObject orb;
            public List<int> anglePresets = new();
        }
        
    }

}
