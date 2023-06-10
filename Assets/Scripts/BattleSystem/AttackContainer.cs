using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContainer : MonoBehaviour
{
    protected bool destroyInvoked;
    public int attackTotalNum { get; protected set; }
    public bool IfODCounter;
    protected int currentFinishedNum;
    [SerializeField]private bool needTotalDisplay;
    protected int totalDamage;
    public bool spGained;
    public List<int> conditionCheckDone;//已检查过的敌人InstanceID
    public HashSet<Tuple<int, int>> checkedConditions = new();
    public List<AttackSubContainer> SubContainers = new();
    
    //public List<int> specialConditionCheckDone;

    public int hitConnectNum { get; protected set; }

    //private int testBulletNum;

    public virtual void InitAttackContainer(int attackTotalNum, bool needTotalDisplay)
    {
        this.attackTotalNum = attackTotalNum;
        this.needTotalDisplay = needTotalDisplay;
        spGained = false;
        conditionCheckDone = new List<int>();
    }
    // Start is called before the first frame update
    protected void Start()
    {
        if(attackTotalNum == 0)
            attackTotalNum = 1;
        totalDamage = 0;
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.childCount == 0 && currentFinishedNum >= attackTotalNum)
        {
            destroyInvoked = true;
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

        GameObject.Find("DamageManager").GetComponent<DamageNumberManager>()?.SpawnTotalDamage(totalDamage);
        
    }


    public void AddTotalDamage(int dmg)
    {
        totalDamage += dmg;
    }
    public virtual void FinishHit()
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
        //print(hitConnectNum);
    }

    
    
    public void DestroyInvoke()
    {
        if (!destroyInvoked)
        {
            destroyInvoked = true;
            foreach (var subContainer in SubContainers)
            {
                print(subContainer.totalDamage);
                totalDamage += subContainer.totalDamage;
            }
            Destroy(gameObject);
        }

        
    }

    public void AddNewCheckedCondition(int instanceID, int internalConditionID)
    {
        checkedConditions.Add(new Tuple<int, int>(instanceID, internalConditionID));
    }
    
    public void RemoveCheckedCondition(int instanceID, int internalConditionID)
    {
        checkedConditions.Remove(new Tuple<int, int>(instanceID, internalConditionID));
    }


}
