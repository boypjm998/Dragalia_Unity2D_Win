using System;
using GameMechanics;
using UnityEngine;


public class AttackManager_C002 : AttackManagerDagger
{
    private AttackContainer skill1Container;

    public GameObject skill1FX_2;
    public override void Skill1(int eventID)
    {
        if (eventID == 1)
        {
            var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
                MeeleAttackFXLayer.transform);
            skill1Container = container.GetComponent<AttackContainer>();
            InstantiateMeele(skill1FX, transform.position, container);
            skill1Container.InitAttackContainer(2,true);

        }
        else if (eventID == 5)
        {
            var subcontainer = Instantiate(BattleStageManager.Instance.attackSubContainer,
                transform.position, Quaternion.identity,
                RangedAttackFXLayer.transform);

            var container = subcontainer.GetComponent<AttackSubContainer>();
            container.parentContainer = skill1Container;
            container.InitAttackContainer(1,skill1Container.gameObject);
            container.InitAttackContainer(1,true);

            InstantiateRanged(skill1FX_2, transform.position+new Vector3(ac.facedir,0,0), subcontainer,
                1);

        }
    }

    public override void Skill2(int eventID)
    {
        Instantiate(skill2FX, transform.position - new Vector3(0, 1, 0), Quaternion.identity,
            MeeleAttackFXLayer.transform);

        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.InfernoMode,
            -1,20,1,0);

    }

    public override void Skill3(int eventID)
    {
        var container = 
            Instantiate(attackContainer, transform.position, Quaternion.identity,
                MeeleAttackFXLayer.transform);
        var proj = InstantiateMeele(skill3FX, transform.position+
            new Vector3(ac.facedir*2,0,0), container);
        container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
        if ((ac as ActorController_c002).skillBoosted)
        {
            var atk = proj.GetComponent<AttackFromPlayer>();
            atk.attackInfo[0].dmgModifier[0] *= 2;
            atk.attackInfo[0].knockbackPower = 180;
            (ac as ActorController_c002).skillBoosted = false;
        }


    }
}
