using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
public class EnemyAttackHintBar : MonoBehaviour
{
    public float warningTime;

    //protected Tweener _tweener;
    protected GameObject Fill;
    protected GameObject MaxFill;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    

}
