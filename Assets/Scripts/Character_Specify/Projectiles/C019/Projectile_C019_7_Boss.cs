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
    /// Squared Light AOE
    /// </summary>
    public class Projectile_C019_7_Boss : MonoBehaviour,IEnemySealedContainer
    {
        [SerializeField] private GameObject attackPrefab;
        
        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private float interval = 5;

        public bool hint = false;
        
        private GameObject enemySource;

        private int clockwise;

        private static bool moveNext = false;

        public static bool MoveNext
        {
            get => moveNext;
            set
            {
                moveNext = value;
            }
        }

        private IEnumerator Start()
        {
            clockwise = Random.Range(0, 2) == 0 ? 1: -1;

            var quadrant = Random.Range(1, 5);

            //todo: 将moveNext换成时间

            var rotateAngleA = (quadrant == 1 || quadrant == 2) ? 90 : -90;
            var rotateAngleB = (quadrant == 1 || quadrant == 4) ? 0 : 180;


            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(null,
                transform.position, this.transform, new Vector2(100, 1), Vector2.zero,
                false, 1, interval / 2, rotateAngleA, 2f, true,
                false, true);
            EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(null,
                transform.position, this.transform, new Vector2(100, 1), Vector2.zero,
                false, 1, interval / 2, rotateAngleB, 2f, true,
                false, true);
            yield return new WaitForSeconds(interval/2);
            
            
            
            yield return new WaitUntil(() => moveNext);
            

            ShootProjectiles(quadrant);
            
            yield return new WaitForSeconds(interval);
            yield return new WaitUntil(() => moveNext);
            
            if (hint)
            {
                CastHint(quadrant);
                yield return new WaitForSeconds(0.1f);
            }

            ShootLightArea(quadrant);
            
            yield return new WaitForSeconds(interval);
            yield return new WaitUntil(() => moveNext);
            
            if (hint)
            {
                CastHint(quadrant + clockwise);
                yield return new WaitForSeconds(0.1f);
            }

            ShootLightArea(quadrant + clockwise);
            
            yield return new WaitForSeconds(interval);
            yield return new WaitUntil(() => moveNext);
            
            if (hint)
            {
                CastHint(quadrant + clockwise * 2);
                yield return new WaitForSeconds(0.1f);
            }

            ShootLightArea(quadrant + clockwise * 2);
            
            yield return new WaitForSeconds(interval);
            yield return new WaitUntil(() => moveNext);
            
            if (hint)
            {
                CastHint(quadrant + clockwise * 3);
                yield return new WaitForSeconds(0.1f);
            }

            ShootLightArea(quadrant + clockwise * 3);

            yield return new WaitForSeconds(3f);
            Destroy(gameObject);

        }


        public void SetEnemySource(GameObject src)
        {
            enemySource = src;
        }

        private void ShootLightArea(int quadrant)
        {
            var proj = this.InstantiateRangedObject(attackPrefab,
                transform.position, gameObject, 1, 1, enemySource.transform);

            proj.transform.localEulerAngles = new Vector3(0, 0, 90 * (quadrant - 1));

        }

        private void CastHint(int quadrant)
        {
            Vector3 offset = new Vector2(0, 50);
            quadrant = ((quadrant + 3) % 4) + 1;
            switch (quadrant)
            {
                case 1:
                    offset = new Vector2(0, 50);
                    break;
                case 2:
                    offset = new Vector2(-100, 50);
                    break;
                case 3:
                    offset = new Vector2(-100, -50);
                    break;
                case 4:
                    offset = new Vector2(0, -50);
                    break;
            }
            
            var hint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(null,
                transform.position+offset, this.transform, new Vector2(100, 100),
                Vector2.zero, false, 1, 0.1f, 0, 0.5f, 
                true, false, true);
            
            print(hint.transform.position + "Quadrant: " + quadrant);
        }

        private void ShootProjectiles(int quadrant)
        {
            var proj1 = this.InstantiateRangedObject(projectilePrefab,
                transform.position, gameObject, 1, 1, enemySource.transform);
            var proj2 = this.InstantiateRangedObject(projectilePrefab,
                transform.position, gameObject, 1, 1, enemySource.transform);

            quadrant = ((quadrant + 3) % 4) + 1;
            
            
            switch (quadrant)
            {
                case 1:
                {
                    proj1.transform.DOMoveX(100, 10f).OnComplete(()=>Destroy(proj1));
                    proj2.transform.DOMoveY(100, 10f).OnComplete(()=>Destroy(proj2));
                    break;
                }
                case 2:
                {
                    proj1.transform.DOMoveX(-100, 10f).OnComplete(()=>Destroy(proj1));
                    proj2.transform.DOMoveY(100, 10f).OnComplete(()=>Destroy(proj2));
                    break;
                }
                case 3:
                {
                    proj1.transform.DOMoveX(-100, 10f).OnComplete(()=>Destroy(proj1));
                    proj2.transform.DOMoveY(-100, 10f).OnComplete(()=>Destroy(proj2));
                    break;
                }
                case 4:
                {
                    proj1.transform.DOMoveX(100, 10f).OnComplete(()=>Destroy(proj1));
                    proj2.transform.DOMoveY(-100, 10f).OnComplete(()=>Destroy(proj2));
                    break;
                }
            }
            
            
            
        }
        
        
        
    }

}
