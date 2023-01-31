using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInputMoudle
{

    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool isExtending = false;
    public bool isDelaying = false;

    private bool curState = false;
    private bool lastState = false;

    private MyTimer exTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    private float extendingDuration = 0.2f;
    public float delayingDuration = 0.6f;

    public void Tick(bool input)
    {
        curState = input;
        IsPressing = curState;
        OnPressed = false;
        OnReleased = false;
        isExtending = false;
        isDelaying = false;

        exTimer.Tick();
        delayTimer.Tick();




        if (curState != lastState)
        {
            if (curState)
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
            }
            else {
                OnReleased = true;
                StartTimer(exTimer, 0.2f);
            }
        }

        lastState = curState;

        if (exTimer.state == MyTimer.STATE.RUN)
        {
            isExtending = true;
        }

        if (delayTimer.state == MyTimer.STATE.RUN)
        {
            isDelaying = true;
        }


    }


    private void StartTimer(MyTimer timer, float duration)
    {
        timer.durtime = duration;
        timer.RunTimer();
    }
    
}
