using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController_D011 : NpcControllerFlyingDragon
{
    protected override IEnumerator DoAttack(GameObject target)
    {
        isAction = true;
        //yield return new WaitUntil( ()=>ac.anim.GetBool("isGround") );
        ac.TurnMove(target);
        if (!CheckDistanceX(currentTarget,6))
        {
            if (ac.SubMoveRoutine != null)
                ac.StopCoroutine(ac.SubMoveRoutine);
            ac.SubMoveRoutine = null;
            print("距离太远");
            currentTarget = null;
            currentMoveCoroutine = null;
            //currentMainRoutineType = MainRoutineType.None;
            isAction = false;
            yield break;
        }

        if (ac.SubMoveRoutine != null)
        {
            currentMoveCoroutine = null;
            //currentMainRoutineType = MainRoutineType.None;
            isAction = false;
            yield break;
        }

        //全部改成射线检测
        if(ac.SubMoveRoutine == null)
        {
            ac.anim.SetFloat("forward",1);
            ac.SubMoveRoutine = StartCoroutine(DoCombo());
        }
        yield return new WaitUntil(()=>ac.SubMoveRoutine == null);
        currentMoveCoroutine = null;
        //currentMainRoutineType = MainRoutineType.None;
        isAction = false;
        
    }
    
    protected IEnumerator DoCombo()
    {
        ac.anim.Play("combo1");
        yield return null;
        yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);

        if (CheckDistanceXDoubleDirection(6, 2) && actionMode == ActionMode.AttackMode)
        {
            ac.anim.Play("combo2");
            // ac.TurnMove(CheckNearsetTargetDoubleDirection(6,2));
            yield return null;
        }
        else
        {
            yield return null;
            yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            ac.anim.Play("idle");
            ac.anim.SetFloat("forward",0);
            ac.SubMoveRoutine = null;
            yield break;
        }

        yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);

        if (actionMode != ActionMode.AttackMode)
        {
            yield return new WaitUntil(()=>ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
            ac.anim.Play("idle");
            ac.anim.SetFloat("forward",0);
            ac.SubMoveRoutine = null;
            yield break;
        }

        ac.anim.Play("combo3");

        yield return null;

        yield return new WaitUntil(() => ac.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        ac.anim.Play("idle");
        ac.anim.SetFloat("forward",0);
        ac.SubMoveRoutine = null;
        //currentMainRoutineType = MainRoutineType.None;
    }
    
    
}
