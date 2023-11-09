using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C010_1 : MonoBehaviour
    {
        // Start is called before the first frame update
        Vector2[] hitPoints = new Vector2[8];
        GameObject[] hitPointsObjects = new GameObject[8];
        private Animator notteBallAnim;
        public GameObject notteBall;
        private Vector2 startPoint;
        private int index = 0;
        public Animator playerAnim;
        public Vector2 notteBallPosition;
        private Sequence sequence;
        public Transform target;
        IEnumerator Start()
        {
            notteBallAnim = GetComponentInChildren<Animator>();
            notteBall = notteBallAnim.gameObject;
            notteBall.transform.position = notteBallPosition + new Vector2(0,1);
            startPoint = notteBall.transform.position;
            if(target!=null)
                transform.GetChild(2).position = new Vector3(target.position.x,transform.position.y);
            //将子物体HitPoints下的8个子物体的坐标存入数组
            for (int i = 0; i < 8; i++)
            {
                hitPoints[i] = transform.GetChild(2).GetChild(i).position;
                hitPointsObjects[i] = transform.GetChild(2).GetChild(i).gameObject;
                print(hitPoints[i]);
            }
    
            yield return new WaitUntil(() => notteBallAnim.GetCurrentAnimatorStateInfo(0).IsName("flying"));
            yield return new WaitForSeconds(0.02f);
            ProjectileAnimation();
        }
    
        // Update is called once per frame
        private void ProjectileAnimation()
        {
            sequence = DOTween.Sequence();
            notteBall.transform.position = startPoint;
            
            notteBall.transform.right = hitPoints[0] - (Vector2) notteBall.transform.position;
            //使用DoMove方法，将notteBall依次移动到数组中的8个坐标点,每次移动结束后等待0.05秒，并激活子物体HitPoints下的第i个子物体
            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                {
                    print("添加移动"+i);
                    sequence.Append(notteBall.transform.DOMove(hitPoints[i], 0.05f));
                }
                else
                {
                    print("添加移动"+i);
                    if (i == 0)
                    {
                        sequence.Append(notteBall.transform.DOMove(hitPoints[i], 0.18f));
                    }
                    else
                    {
                        sequence.Append(notteBall.transform.DOMove(hitPoints[i], 0.12f));
                    }


                }
    
                
                sequence.AppendInterval(0.03f);
                sequence.AppendCallback(() =>
                {
                    ActiveNextPoint();
                    //使notteBall的右方向看向下一个坐标点
                    int index = i;
                    if(index<8)
                        notteBall.transform.right = hitPoints[i+1] - (Vector2) notteBall.transform.position;
                    else
                    {
                        notteBall.transform.right = startPoint - (Vector2) notteBall.transform.position;
                        
                    }
                }
                );
            }
            sequence.AppendCallback(() =>
            {
                playerAnim.speed = 1;
                notteBallAnim.Play("end");
            });
            //将notteBall移动到起始点
            sequence.Append(notteBall.transform.DOMove(startPoint, 0.03f));
            //移动Ease为OutCubic
            sequence.SetEase(Ease.OutCubic);
            //结束后将notteBall隐藏
            sequence.OnComplete(() =>
            {
                notteBall.SetActive(false);
                
                //Destroy(notteBall.transform.GetChild(0).gameObject);
            });
            //开始执行
            sequence.Play();
    
        }
        
        void ActiveNextPoint()
        {
            hitPointsObjects[index].SetActive(true);
            index++;
        }

        private void OnDestroy()
        {
            if(sequence==null)
                return;
            if(sequence.IsPlaying())
                sequence.Kill();
        }
    }
    
    
}


