using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class MyTimer
{
    public enum STATE
    {
        IDLE,
        RUN,
        FINISHED
    }

    public STATE state = STATE.IDLE;
    public float durtime = 1f;
    public float elapseTime = 0;

    public void Tick()
    {
        switch (state)
        {
            case STATE.IDLE:
                break;

            case STATE.RUN:
                elapseTime += Time.deltaTime;
                if (elapseTime >= durtime)
                {
                    state = STATE.FINISHED;
                }
                break;
            
            case STATE.FINISHED:
                break;

            default:
                Debug.Log("ERROR");
                break;
      
        }

    }

    public void RunTimer()
    {
        elapseTime = 0;
        state = STATE.RUN;
    }

}
