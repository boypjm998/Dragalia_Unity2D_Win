using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class SkillUISpecial019 : SkillUIBase
{
    private ActorController_c019 ac;
    private bool hasPowerOfBond = false;
    protected override void Start()
    {
        base.Start();
        ac = BattleStageManager.Instance.GetPlayer().GetComponent<ActorController_c019>();
    }

    protected void Update()
    {
        CheckSkillCD();
        CheckSkillVariant();
    }

    private void CheckSkillVariant()
    {
        SwapSkillIcon(ac.hasPowerOfBonds ? 1 : 0);
    }
    protected override void SwapSkillIcon(int iconID)
    {
        Transform _parent = skillIcon.transform.parent;

        for(int i = 0; i < _parent.childCount;i++)
        {
            if (i == iconID)
            {
                _parent.GetChild(i).gameObject.SetActive(true);
                skillIcon = maskTransform.GetChild(iconID).gameObject;
            }
            else {
                _parent.GetChild(i).gameObject.SetActive(false);
            }
            
        }
    }
    
}
