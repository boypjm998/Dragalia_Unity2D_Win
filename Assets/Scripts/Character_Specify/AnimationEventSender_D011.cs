using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_D011 : MonoBehaviour
{
    private ActorControllerSpecial ac;
    private AttackManager am;
    private NpcController_D011 npc;

    private void Awake()
    {
        ac = GetComponentInParent<ActorControllerSpecial>();
        npc = GetComponentInParent<NpcController_D011>();
    }

    private void Skill1()
    {
        
    }
}
