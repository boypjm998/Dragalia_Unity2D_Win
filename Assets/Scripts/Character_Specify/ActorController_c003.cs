using GameMechanics;
using UnityEngine;


public class ActorController_c003 : ActorControllerStaff
{
    public int auspexGauge = 0;
    [SerializeField] private float forcingCheckTime = 0.3f;

    public Vector2 forceAttackPosition = Vector2.zero;

    public bool skill4IsBoosted =>
        _statusManager.GetConditionStackNumber((int)(BasicCalculation.BattleCondition.HealOverTime)) >= 3;
    protected int twilightCrownHitCount = 0;
    

    protected override void Awake()
    {
        base.Awake();
        pi.buttonAttack.delayingDuration = forcingCheckTime;
        
        FindObjectOfType<AuspexGauge>().ac = this;
        
    }

    protected override void Update()
    {
        base.Update();
        CheckForceStrike();
        
    }

    protected override void CheckForceStrike()
    {
        if(_statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.TwilightMoon)<=0)
            return;
        base.CheckForceStrike();
    }


    protected override void CheckSkill()
    {
        if(pi.allowForceStrike && forceLevel>=0)
            return;
          
        if (pi.skill[0] && grounded && !pi.hurt && !pi.isSkill)
        {
            UseSkill(1);
        }

        if (pi.skill[1] && grounded && !pi.hurt && !pi.isSkill)
        {
            UseSkill(2);
        }
          
        if (pi.skill[2] && grounded && !pi.hurt && !pi.isSkill)
        {
            UseSkill(3);
        }

        if (pi.skill[3] && grounded && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
    }

    public override void FaceDirectionAutoFix(int moveID)
    {
        base.FaceDirectionAutoFix(moveID);
        switch (moveID)
        {
            case 1:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 20f, 1f,
                        LayerMask.GetMask("Enemies"),1) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 20f, 1f,
                        LayerMask.GetMask("Enemies"),1) != null)
                {
                    SetFaceDir(-facedir);
                }

                break;
            }
        }
    }

    public void PlayAttackVoice(int moveID)
    {
        voiceController.PlayAttackVoice(moveID);
    }

    public void Skill1(int eventID)
    {
        auspexGauge++;
        CheckTwilightMoon();
    }

    public void Skill2(int eventID)
    {
        if (eventID == 1)
            auspexGauge++;
        CheckTwilightMoon();
    }

    public void Skill3(int eventID)
    {
        if (eventID == 1)
        {
            auspexGauge++;
            CheckTwilightMoon();
        }

        
    }

    public void Skill4(int eventID)
    {
        if (eventID == 1)
        {
            auspexGauge++;
            if (skill4IsBoosted)
                auspexGauge++;
            
            CheckTwilightMoon();

        }

        
    }

    void CheckTwilightMoon()
    {
        if (auspexGauge >= 3 && _statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.TwilightMoon)<=0)
        {
            var twilightmoonbuff = new TimerBuff((int)BasicCalculation.BattleCondition.TwilightMoon, -1, -1, 1, -1);
            twilightmoonbuff.dispellable = false;
            _statusManager.ObtainTimerBuff(twilightmoonbuff);
        }
        
    }

    public override void OnSkillConnect(AttackBase attack_statusManager)
    {
        if(attack_statusManager.skill_id == 3)
            twilightCrownHitCount++;

        if (twilightCrownHitCount >= 10)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.HealOverTime,
                10,9,3,100301);
            twilightCrownHitCount = 0;
        }
    }
}
