using System;
using UnityEngine;
using System.Collections;


public class NpcController_C010 : NpcController
{
    public override IEnumerator DoAttack(GameObject target)
    {
        isAction = true;
        yield return new WaitUntil(()=>!ac.hurt && ac.anim.GetBool("isGround"));
        ac.TurnMove(target);
        if (!CheckDistanceX(currentTarget,maxAttackActiveDistance))
        {
            if (ac.SubMoveRoutine != null)
                StopCoroutine(ac.SubMoveRoutine);
            ac.SubMoveRoutine = null;
            print("距离太远");
            currentTarget = null;
            currentMoveCoroutine = null;
            currentMainRoutineType = MainRoutineType.None;
            isAction = false;
            yield break;
        }

        if (ac.SubMoveRoutine != null)
        {
            currentMoveCoroutine = null;
            currentMainRoutineType = MainRoutineType.None;
            isAction = false;
            yield break;
        }

        //全部改成射线检测
        if (skillCDTimer[0] <= 0 && CheckDistanceX(currentTarget,maxAttackActiveDistance))
        {
            skillCDTimer[0] = skillCD[0];
            ac.SubMoveRoutine = StartCoroutine(DoSkill_10101());
        }
        else if(skillCDTimer[1] <= 0 && CheckDistanceX(currentTarget,maxAttackActiveDistance))
        {
            skillCDTimer[1] = skillCD[1];
            ac.SubMoveRoutine = StartCoroutine(DoSkill_10102());
        }
        else if(ac.SubMoveRoutine == null)
        {
            ac.SubMoveRoutine = StartCoroutine(DoCombo());
        }
        yield return new WaitUntil(()=>ac.SubMoveRoutine == null);
        currentMoveCoroutine = null;
        currentMainRoutineType = MainRoutineType.None;
        isAction = false;
    }


    protected IEnumerator DoCombo()
    {
        ac.anim.Play("combo1");
        yield return null;
        yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        if (CheckDistanceX(currentTarget, maxAttackActiveDistance) && !CheckSkillUseable() && actionMode == ActionMode.AttackMode)
        {
            ac.anim.Play("combo2");
            yield return null;
        }
        else
        {
            yield return null;
            yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            ac.anim.Play("idle");
            ac.SubMoveRoutine = null;
            yield break;
        }

        int comboCount = 0;
        while (comboCount < 15 && CheckDistanceX(currentTarget, 3f) && !CheckSkillUseable() && actionMode == ActionMode.AttackMode)
        {
            yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            ac.anim.Play("combo2", 0, 0);
            yield return null;
            comboCount++;
        }
        
        yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);

        if (CheckSkillUseable() || actionMode != ActionMode.AttackMode)
        {
            ac.anim.Play("idle");
            ac.SubMoveRoutine = null;
            yield break;
        }

        ac.anim.Play("combo3");

        yield return null;

        yield return new WaitUntil(() => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        ac.anim.Play("idle");
        ac.SubMoveRoutine = null;
        currentMainRoutineType = MainRoutineType.None;
    }
    
    IEnumerator DoSkill_10101()
    {
        ac.anim.Play("action01");
        yield return null;
        yield return new WaitUntil(() => ac.anim.GetCurrentAnimatorStateInfo(0).IsName("action02"));
        //ac.anim.speed = 1;
        yield return null;
        yield return new WaitUntil(() => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        ac.anim.Play("idle");
        ac.SubMoveRoutine = null;
        currentMainRoutineType = MainRoutineType.None;
    }
    
    IEnumerator DoSkill_10102()
    {
        ac.anim.Play("action03");
        yield return null;
        yield return new WaitUntil(() => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        ac.anim.Play("idle");
        ac.SubMoveRoutine = null;
        currentMainRoutineType = MainRoutineType.None;
    }
}
