using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_C019 : AnimationEventSender
{
    private AttackManager_C019 _attackManagerSp;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _attackManagerSp = transform.parent.parent.GetComponent<AttackManager_C019>();
    }

    protected void Skill2()
    {
        _attackManagerSp.Skill2();
    }


}
