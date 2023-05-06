using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightTrigger : MonoBehaviour
{
    NpcController npcController;
    private StandardCharacterController ac;

    private void Start()
    {
        npcController = transform.parent.GetComponent<NpcController>();
        ac = transform.parent.GetComponent<StandardCharacterController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if (npcController.CurrentMainRoutineType == NpcController.MainRoutineType.None)
        {
            if(!npcController.isFollowMode)
                return;
        }
        
        

        if (npcController.CurrentMainRoutineType == NpcController.MainRoutineType.MoveToPlatform)
        {
            //获取Trigger和other的碰撞点
            var contactPoint = other.ClosestPoint(transform.position);
            if (ac.facedir == 1)
            {
                //如果碰撞体在左边，那么无视
                if (contactPoint.x < transform.position.x)
                    return;
            }
            if(ac.facedir == -1)
            {
                //如果碰撞体在右边，那么无视
                if (contactPoint.x > transform.position.x)
                    return;
            }
            
            if (other.gameObject.layer == LayerMask.NameToLayer("AttackEnemy"))
            {
                npcController.SendForcedEvadeMessageDuringEvade(other);
                print("SendedForced");
                return;
            }

            
        }


        if (other.gameObject.layer == LayerMask.NameToLayer("AttackEnemy"))
        {
            npcController?.SendEvadeMessage(other);
            print("Sended");
        }
    }
}
