using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


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
    public float delayingDuration = 0.8f;

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


public class GamePadInput
{
    public static float sensitivity;

    public static void InitSensitivity()
    {
        int sensLevel = GlobalController.Instance.gameOptions.gamepadSettings[1];
        if (sensLevel <= 1)
        {
            sensitivity = 0.75f;
        }else if (sensLevel == 2)
        {
            sensitivity = 0.45f;
        }else if (sensLevel >= 3)
        {
            sensitivity = 0.1f;
        }
        Debug.Log(sensitivity);

    }
    
    public static bool GetButton(string actionName)
    {
        if (!GlobalController.Instance.gamepadEnable)
            return false;
        
        
        var action = GlobalController.gamepadMap.FindAction(actionName);
        if (action.type == InputActionType.Button)
        {
            return action.IsPressed();
        }
        else
        {
            //Debug.LogError("Action type is not button!");
            return false;
        }
    }
    
    public static bool GetAxis(string actionName, int direction)
    {
        if (!GlobalController.Instance.gamepadEnable)
            return false;
        
        var action = GlobalController.gamepadMap.FindAction(actionName);

        if (action.type == InputActionType.Value)
        {
            if (direction > 0)
            {
                float val = action.ReadValue<float>();
                //Debug.Log("Val:"+val);
                return val >= sensitivity;
            }
            else
            {
                float val = action.ReadValue<float>();//Debug.Log("Val:"+val);
                return val <= -sensitivity;
            }
        }
        else
        {
            Debug.LogError("Action type is not button!");
            return false;
        }
    }

}
