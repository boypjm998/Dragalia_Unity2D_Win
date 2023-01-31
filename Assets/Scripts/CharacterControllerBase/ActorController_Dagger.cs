using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController_Dagger : ActorController
{
    public int combo = 0;
    //public bool anim.SetBool("attack",false);
    protected Coroutine movementRoutine = null;
    protected Coroutine comboResetRoutine = null;
    protected float comboStageResetTime = 0f;
   
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    
    void FixedUpdate()
    {
        Move();

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Roll()
    {
        
        anim.SetBool("roll",true);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("roll"))
        {
            return;
        }

        if (movementRoutine != null)
        {
            
            StopCoroutine(movementRoutine);
            
        }
        movementRoutine = StartCoroutine("RollRoutine");


    }

    public override void StdAtk()
    {
        anim.SetBool("attack",true);
        if (movementRoutine == null)
        {
            
            switch (combo)
            {
                case 0:
                    movementRoutine = StartCoroutine(Combo1());
                    break;
                case 1:
                    movementRoutine = StartCoroutine(Combo2());
                    break;
                case 2:
                    movementRoutine = StartCoroutine(Combo3());
                    break;
                case 3:
                    movementRoutine = StartCoroutine(Combo4());
                    break;
                case 4:
                    movementRoutine = StartCoroutine(Combo5());
                    break;
            }
        }

        



    }

    #region Combo1-Combo5
    protected virtual IEnumerator Combo1()
    {
        EnterCombo();
        combo = 1;

        anim.Play("combo1");
        bool event1 = false;
        bool event2 = false;
        
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<0.5f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && !event1)
            {
                event1 = true;
                print("执行事件1");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.15f && !event2)
            {
                event2 = true;
                pi.SetInputAttack(1);
            }

            yield return null;
        }
        //anim.SetBool("attack",false);
        ActionEnable((int)PlayerActionType.ATTACK);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<BasicCalculation.GetLastAnimationNormalizedTime(anim))
        {
            if(anim.GetBool("attack") && combo==1)
            {
                //combo = 1;
                anim.SetBool("attack",false);
                movementRoutine = StartCoroutine("Combo2");
                yield break;
            }

            yield return null;
        }
        comboResetRoutine = StartCoroutine(ResetCombo());
        ExitCombo();
        
    }
    protected virtual IEnumerator Combo2()
    {
        EnterCombo();
        combo = 2;
        
        anim.Play("combo2");
        bool event1 = false;
        bool event2 = false;
        
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<0.5f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && !event1)
            {
                event1 = true;
                print("执行事件2");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f && !event2)
            {
                event2 = true;
                pi.SetInputAttack(1);
            }
            
            yield return null;
        }
        //anim.SetBool("attack",false);
        ActionEnable((int)PlayerActionType.ATTACK);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<BasicCalculation.GetLastAnimationNormalizedTime(anim))
        {
            if(anim.GetBool("attack") && combo==2)
            {
                movementRoutine = StartCoroutine("Combo3");
                //combo = 2;
                yield break;
            }

            yield return null;
        }
        
        ExitCombo();
        comboResetRoutine = StartCoroutine(ResetCombo());
    }

    protected virtual IEnumerator Combo3()
    {
        EnterCombo();
        combo = 3;
        
        
        anim.Play("combo3");
        bool event1 = false;
        bool event2 = false;
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<0.5f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && !event1)
            {
                event1 = true;
                print("执行事件3");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f && !event2)
            {
                event2 = true;
                pi.SetInputAttack(1);
            }
            
            yield return null;
        }
        //anim.SetBool("attack",false);
        ActionEnable(4);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<BasicCalculation.GetLastAnimationNormalizedTime(anim))
        {
            if(anim.GetBool("attack") && combo==3)
            {
                //combo = 3;
                movementRoutine = StartCoroutine("Combo4");
                yield break;
            }

            yield return null;
        }
        
        ExitCombo();
        comboResetRoutine = StartCoroutine(ResetCombo());
    }
    
    protected virtual IEnumerator Combo4()
    {
        EnterCombo();
        combo = 4;
        
        
        anim.Play("combo4");
        bool event1 = false;
        bool event2 = false;
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<0.5f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && !event1)
            {
                event1 = true;
                print("执行事件4");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f && !event2)
            {
                event2 = true;
                pi.SetInputAttack(1);
            }
            
            yield return null;
        }
        //anim.SetBool("attack",false);
        ActionEnable((int)PlayerActionType.ATTACK);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<BasicCalculation.GetLastAnimationNormalizedTime(anim))
        {
            if(anim.GetBool("attack") && combo==4)
            {
                //combo = 4;
                movementRoutine = StartCoroutine("Combo5");
                yield break;
            }

            yield return null;
        }

        
        ExitCombo();
        comboResetRoutine = StartCoroutine(ResetCombo());
        
    }
    
    protected virtual IEnumerator Combo5()
    {
        //print("BeforeEnter");
        EnterCombo();
        combo = 0;

        anim.Play("combo5");
        bool event1 = false;
        bool event2 = false;
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<0.6f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && !event1)
            {
                event1 = true;
                print("执行事件5");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && !event2)
            {
                event2 = true;
                pi.SetInputAttack(1);
            }
            
            yield return null;
        }
        //anim.SetBool("attack",false);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime<BasicCalculation.GetLastAnimationNormalizedTime(anim))
        {
            if(anim.GetBool("attack"))
            {
                //combo = 0;
                anim.speed = 2;
                //yield break;
            }

            yield return null;
        }

        anim.speed = 1;
        if(anim.GetBool("attack") && combo==0)
        {
            //combo = 0;
            movementRoutine = StartCoroutine("Combo1");
            yield break;
        }
        ExitCombo();
        combo = 0;
    }
    
    #endregion

    protected IEnumerator RollRoutine()
    {
        onRollEnter();
        anim.Play("roll",-1,0);
        bool event1 = false;
        bool event2 = false;
        yield return null;

        anim.SetBool("roll",false);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (1f/12f) && !event1)
            {
                event1 = true;
                EventRoll();
            }

            yield return null;
        }
        //anim.SetBool("roll",false);
        ActionEnable((int)PlayerActionType.ATTACK);
        pi.SetInputRoll(1);
        ActionEnable((int)PlayerActionType.ROLL);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < (17f/18f))
        {
            yield return null;
        }

        if (anim.GetBool("roll"))
        {
            movementRoutine = StartCoroutine("RollRoutine");
            yield break;
        }
        onRollExit();
        movementRoutine = null;
    }

    public override void EventRoll()
    {
        base.EventRoll();
        
    }

    public override void onRollEnter()
    {
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.moveEnabled = false;
        
        //pi.LockDirection(1);
        dodging = true;
        combo = 0;
        pi.SetInputRoll(0);
        voiceController?.PlayDodgeVoice();

        
    }

    public override void onRollExit()
    {
        base.onRollExit();
        ActionEnable((int)PlayerActionType.ROLL);

        /*if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }*/

        
    }

    public override void OnStandardAttackEnter()
    {
        print("OnStandAttackEnter");
        EnterCombo();
    }

    public override void OnStandardAttackExit()
    {
        pi.SetInputMove(1);
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.MOVE);
        
        //anim.SetBool("attack",false);
        //combo = 0;
        pi.SetInputAttack(1);
    }

    protected virtual void EnterCombo()
    {
        if (comboResetRoutine != null)
        {
            StopCoroutine(comboResetRoutine);
            comboResetRoutine = null;
        }

        anim.SetBool("attack",false);
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.ROLL);
        ActionEnable((int)PlayerActionType.ATTACK);
        pi.SetInputMove(0);
        pi.SetInputAttack(0);
        
    }

    protected virtual void ExitCombo()
    {
        
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.MOVE);
        pi.SetInputMove(1);
        pi.SetInputAttack(1);
        combo = 0;
        movementRoutine = null;
    }

    protected IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboStageResetTime);
        combo = 0;
    }

    protected override void CheckSignal()
    {
        base.CheckSignal();
        //combo = 0;
    }
}
