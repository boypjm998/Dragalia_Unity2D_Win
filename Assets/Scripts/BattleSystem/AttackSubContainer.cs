using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack SubContainer is a class which can seperate multiple attacks into different layers.
/// When a subcontainer finished its all child attacks, it will return to parent container AS A SINGLE ATTACK.
/// </summary>
public class AttackSubContainer : AttackContainer
{
    //private int totalDamage;
    public AttackContainer parentContainer;

    public bool hitFinished = false;
    
    void Start()
    {
        conditionCheckDone = parentContainer.conditionCheckDone;
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

        if (spGained && !parentContainer.spGained)
        {
            parentContainer.spGained = true;
        }

        if (!spGained && parentContainer.spGained)
        {
            spGained = true;
        }
    }
    
    private void OnDestroy()
    {
        
        parentContainer?.AddTotalDamage(totalDamage);

        parentContainer?.FinishHit();

    }

    public void InitAttackContainer(int attackTotalNum, GameObject _parent)
    {
        this.attackTotalNum = attackTotalNum;
        parentContainer = _parent.GetComponent<AttackContainer>();
        parentContainer.SubContainers.Add(this);
    }

    
}
