using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSpecificProjectiles
{
    ///Holy Crown: Faith ( The protection )
    public class Projectile_C003_14_Boss : MonoBehaviour
    {
        [SerializeField] private List<GameObject> meteorList = new();
        [SerializeField] private List<GameObject> starList = new();
        [SerializeField] private GameObject hintContainer;
        [SerializeField] private List<GameObject> zoneList = new();
        
        private List<Tuple<int, float>> randomizeTupleList = new();
        private int SortedID(int id) => randomizeTupleList[id].Item1;

        public void SetEnemySource(GameObject src)
        {
            var attacks = GetComponentsInChildren<AttackFromEnemy>();
            foreach (var VARIABLE in attacks)
            {
                VARIABLE.enemySource = src;
            }

            foreach (var v in starList)
            {
                v.GetComponent<AttackFromEnemy>().enemySource = src;
            }
        }

        private IEnumerator Start()
        {
            GenerateRandomList();

            yield return new WaitForSeconds(0.5f);
            
            SetHintUI(true);

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 5; i++)
            {
                MeteorDrop(meteorList[SortedID(i)],starList[SortedID(i)]);
                yield return new WaitForSeconds(0.5f);
            }
            
            //SetHintUI(false);


        }

        void GenerateRandomList()
        {
            var tempList = new List<Tuple<int, float>>();
            randomizeTupleList.Clear();
            for (int i = 0; i < 5; i++)
            {
                tempList.Add(new Tuple<int, float>(i,Random.Range(0,100)));
            }

            randomizeTupleList = tempList.OrderBy(tuple => tuple.Item2).ToList();

            foreach (var t in randomizeTupleList)
            {
                print(t.Item1 +"/"+ t.Item2);
            }
        }

        void SetHintUI(bool flag)
        {
            hintContainer.SetActive(flag);
        }

        void MeteorDrop(GameObject projectile, GameObject star)
        {
            projectile.transform.DOMoveY(-1, 1.5f).SetEase(Ease.InSine).OnComplete(
                () =>
                {
                    projectile.SetActive(false);
                    star.SetActive(true);
                });
        }

        public void WakeProtectionZone(int orderID)
        {
            StartCoroutine(ZoneAnimation(SortedID(orderID)));
            if(orderID == 4)
                Destroy(gameObject,3f);
        }

        IEnumerator ZoneAnimation(int zoneID)
        {
            zoneList[zoneID].SetActive(true);
            yield return new WaitForSeconds(3f);
            zoneList[zoneID].SetActive(false);
            
        }
    }
}

