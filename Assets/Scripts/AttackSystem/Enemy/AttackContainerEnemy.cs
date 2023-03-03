using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;

public class AttackContainerEnemy : AttackContainer
{
    // Start is called before the first frame update

    
    void Start()
    {
        /*if (attackTotalNum <= 0)
        {
            attackTotalNum = 1;
        }*/
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
    
    public virtual void InitAttackContainer(int attackTotalNum)
    {
        this.attackTotalNum = attackTotalNum;
        conditionCheckDone = new List<int>();
    }

    

}
