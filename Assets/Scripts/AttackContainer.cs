using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContainer : MonoBehaviour
{
    public int attackTotalNum { get; private set; }
    private int currentFinishedNum;
    private bool needTotalDisplay;
    private int totalDamage;
    public bool spGained;

    public int hitConnectNum { get; private set; }

    //private int testBulletNum;

    public void InitAttackContainer(int attackTotalNum, bool needTotalDisplay)
    {
        this.attackTotalNum = attackTotalNum;
        this.needTotalDisplay = needTotalDisplay;
        spGained = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        //testBulletNum = 0;
        totalDamage = 0;

        //AttackFromPlayer[] AttackSet = FindObjectsOfType<AttackFromPlayer>();
        //foreach (var k in AttackSet)
        //{
        //    testBulletNum++;
        //
        //}
        //print(testBulletNum);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        


        if (transform.childCount == 0 && currentFinishedNum == attackTotalNum)
        {
            Destroy(gameObject, 0.5f);
        }
    }
    private void OnDestroy()
    {
        if (needTotalDisplay)
        {
            DisplayTotalDamage();
        }
    }
    private void DisplayTotalDamage()
    {
        if (totalDamage == 0)
            return;

        GameObject.Find("DamageManager").GetComponent<DamageNumberManager>().SpawnTotalDamage(totalDamage);

        

    }


    public void AddTotalDamage(int dmg)
    {
        totalDamage += dmg;
    }
    public void FinishHit()
    {
        currentFinishedNum++;
    }
    public bool NeedTotalDisplay()
    {
        return needTotalDisplay;
    }
    public void AttackOneHit()
    {
        hitConnectNum++;
        print(hitConnectNum);
    }


}
