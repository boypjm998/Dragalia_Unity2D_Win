using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPC_EvadeZone : MonoBehaviour
{
    // Start is called before the first frame update


    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.name!="HitSensor")
            return;
        //如果other的parent中有NpcController组件,print("NPC_EvadeZone OnTriggerStay2D");
        var npcController = other.transform.parent.GetComponent<NpcController>();
        if (npcController != null)
        {
            if(npcController.CurrentMainRoutineType==NpcController.MainRoutineType.None)
                return;
            npcController.SendEvadeMessage();//令NPC躲避
            print(gameObject.transform.parent.name + " NPC_EvadeZone OnTriggerStay2D");
        }
    }
}
