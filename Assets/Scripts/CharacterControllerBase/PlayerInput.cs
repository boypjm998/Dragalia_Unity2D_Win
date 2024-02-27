using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using GameMechanics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public class PlayerInput : MonoBehaviour
{
    public delegate void OnPressSignal();
    public static event OnPressSignal OnPressAttack;
    
    // Start is called before the first frame update
    //variables
    private PlayerStatusManager stat;
    private ActorController ac;
    
    public bool quicklandingEnabled = false;

    [Header("Key Settings")] 
    [SerializeField]private float accTime = 0.1f;
    public bool standardAttackContinious = true;
    public bool allowForceStrike = false;
    
    public KeyCode keyRight = KeyCode.D;
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyDown = KeyCode.S;
    public KeyCode keyUp = KeyCode.Space;
    public KeyCode keyAttack = KeyCode.J;
    public KeyCode keyJump = KeyCode.K;
    public KeyCode keyRoll = KeyCode.L;
    public KeyCode keySkill1 = KeyCode.U;
    public KeyCode keySkill2 = KeyCode.I;
    public KeyCode keySkill3 = KeyCode.O;
    public KeyCode keySkill4 = KeyCode.H;
    public KeyCode keyEsc = KeyCode.Escape;
    public Dictionary<string, InputBinding> gamepadButtonDict = new();

    public MyInputMoudle buttonRight = new MyInputMoudle();
    public MyInputMoudle buttonLeft = new MyInputMoudle();
    public MyInputMoudle buttonDown = new MyInputMoudle();
    public MyInputMoudle buttonUp = new MyInputMoudle();
    public MyInputMoudle buttonAttack = new MyInputMoudle();
    public MyInputMoudle buttonJump = new MyInputMoudle();
    public MyInputMoudle buttonRoll = new MyInputMoudle();
    public MyInputMoudle buttonSkill1 = new MyInputMoudle();
    public MyInputMoudle buttonSkill2 = new MyInputMoudle();
    public MyInputMoudle buttonSkill3 = new MyInputMoudle();
    public MyInputMoudle buttonSkill4 = new MyInputMoudle();
    public MyInputMoudle buttonEsc = new MyInputMoudle();


    [Header("Output Signal")]
    public float DRight;
    public float isMove;
    public float velocityDRight;
    public float targetDRight;

    protected float targetDUp;
    protected float velocityDUp;
    public float DUp;

    //once-trigger signal
    public bool jump;

    private bool lastJump;
    public bool wjump;
    [SerializeField]
    private bool lastwJump;
    public int jumptime = 2;
    public bool roll;
    public bool stdAtk;
    public bool isSkill = false;
    public bool hurt = false;
    private bool lastStdAtk;

    public bool[] skill = new bool[4];



    [Header("Others")]
    //允许信号作用
    public bool moveEnabled = true;
    public bool jumpEnabled = true;
    public bool rollEnabled = true;
    public bool attackEnabled = true;
    
    public bool directionLock = false;

    //允许信号进入
    public bool inputMoveEnabled = true;
    public bool inputAttackEnabled = true;
    public bool inputJumpEnabled = true;
    public bool inputRollEnabled = true;

    private int pointerIsOnClickableUINum = 0;



    private void Awake()
    {
        stat = transform.GetComponent<PlayerStatusManager>();
        ac = transform.GetComponent<ActorController>();
        for (int i = 0; i < 4; i++)
        {
            skill[i] = false;
        }
        
        LoadKeySetting();
        GamePadInput.InitSensitivity();
        
    }


    private bool ControlEnable(KeyCode keyCode)
    {
        // if (keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1)
        // {
        //     if (EventSystem.current == null)
        //     {
        //         
        //     }
        //
        //     if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.CompareTag("ClickableUI"))
        //     {
        //         return false;
        //     }
        // }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if(ControlEnable(keyEsc))
            buttonEsc.Tick(Input.GetKey(keyEsc) || GamePadInput.GetButton("Escape"));
        
        if(BattleStageManager.Instance.isGamePaused)
            return;
        
        //
        if(hurt)
            return;
        
        // buttonLeft.Tick(CheckSwipeLeft());
        // buttonRight.Tick(CheckSwipeRight());

        if (keyLeft != KeyCode.None)
        {
            buttonLeft.Tick(Input.GetKey(keyLeft) || GamePadInput.GetAxis("Move",-1));
        }
        if (keyRight != KeyCode.None)
        {
            buttonRight.Tick(Input.GetKey(keyRight) || GamePadInput.GetAxis("Move",1));
        }
        if (keyAttack != KeyCode.None)
        {
            buttonAttack.Tick(Input.GetKey(keyAttack) || GamePadInput.GetButton("Attack"));
        }
        if (keyJump != KeyCode.None)
        {
            buttonJump.Tick(Input.GetKey(keyJump) || GamePadInput.GetButton("Jump"));
        }
        if (keyRoll != KeyCode.None)
        {
            buttonRoll.Tick(Input.GetKey(keyRoll) || GamePadInput.GetButton("Dodge"));
        }
        if (keyDown != KeyCode.None)
        {
            buttonDown.Tick(Input.GetKey(keyDown) || GamePadInput.GetButton("Down"));
        }
        if (keyUp != KeyCode.None)
        {
            buttonUp.Tick(Input.GetKey(keyUp) || GamePadInput.GetButton("Special"));
        }
        if (keySkill1 != KeyCode.None)
        {
            buttonSkill1.Tick(Input.GetKey(keySkill1) || GamePadInput.GetButton("Skill1"));
        }
        if (keySkill2 != KeyCode.None)
        {
            buttonSkill2.Tick(Input.GetKey(keySkill2)|| GamePadInput.GetButton("Skill2"));
        }
        if (keySkill3 != KeyCode.None)
        {
            buttonSkill3.Tick(Input.GetKey(keySkill3)|| GamePadInput.GetButton("Skill3"));
        }
        if (keySkill4 != KeyCode.None)
        {
            buttonSkill4.Tick(Input.GetKey(keySkill4)|| GamePadInput.GetButton("Skill4"));
        }

        //print(buttonSkill4.OnPressed);
        
        
        

        //print(buttonDown.IsPressing && buttonDown.isExtending);

        
        
        checkMovement();    
        checkJump();
        checkRoll();
        checkStdAttack();
        
        //PlayerInput.CheckSkill() -> ActorController.CheckSkill() -> ActorController.UseSkill(id)
        CheckSpecialMove();
        CheckSkill1();
        CheckSkill2();
        CheckSkill3();
        CheckSkill4();
        
    }


    #region ActionInputCheck

    

    bool checkMovement()
    {
        


        targetDRight = (buttonRight.IsPressing ? 1.0f : 0) - (buttonLeft.IsPressing ? 1.0f : 0);
        targetDUp = (buttonJump.IsPressing ? 1.0f : 0) - (buttonDown.IsPressing ? 1.0f : 0);
        //DRight = Mathf.SmoothDamp(DRight, targetDRight, ref velocityDRight, accTime);
        DRight = Mathf.SmoothDamp(DRight, targetDRight, ref velocityDRight, accTime);
        DUp = Mathf.SmoothDamp(DUp, targetDUp, ref velocityDUp, accTime);

        if (inputMoveEnabled == false)
        {
            targetDRight = 0;
            DRight = 0;
            targetDUp = 0;
            DUp = 0;
            return false;
        }
        
        

        if (DRight < 0.25f && DRight > -0.25f){ 
            isMove = 0;
            return false;
        }
        else { 
            isMove = DRight;
            return true;
        }
        
        
    }

  
    bool[] checkJump()
    {

        wjump = (buttonJump.OnPressed && jumptime == 1);
        if (wjump)
        {
            jumptime--;
        }

        bool[] rtn = new bool[2];


        if (inputJumpEnabled == false)
        {
            rtn[0] = false;
            rtn[1] = false;
            jump = false;
            wjump = false;
            return rtn;
        }

        jump = (buttonJump.IsPressing && jumptime == 2);
        if (jump)
        {
            jumptime--;
        }

        rtn[0] = jump;
        rtn[1] = wjump;

        return rtn;
    }

    bool checkRoll()
    {
        if (inputRollEnabled == false)
        {
            roll = false;
            return false;
        }
        roll = buttonRoll.IsPressing;

        return roll;
    }

    bool checkStdAttack()
    {
        if (inputAttackEnabled == false)
        {
            stdAtk = false;
            return false;
        }

        if (standardAttackContinious)
        {
            stdAtk = buttonAttack.IsPressing;
        }
        else if(!allowForceStrike)
        {
            stdAtk = buttonAttack.OnPressed;
        }

        if (allowForceStrike)
        {
            if (buttonAttack.OnReleased)
                stdAtk = true;
            //stdAtk = buttonAttack.OnReleased;
        }


        return stdAtk;
    }

    void CheckSpecialMove()
    {
        
    }

    void CheckSkill1()
    {
        

        if (stat.CheckSkillSPEnough(0) && buttonSkill1.OnPressed)
        {
            skill[0] = true;
        }
        else {
            skill[0] = false;
        }
        

    }

    void CheckSkill2()
    {


        if (stat.CheckSkillSPEnough(1) && buttonSkill2.OnPressed)
        {
            skill[1] = true;
        }
        else
        {
            skill[1] = false;
        }


    }
    
    void CheckSkill3()
    {


        if (stat.CheckSkillSPEnough(2) && buttonSkill3.OnPressed)
        {
            skill[2] = true;
            
        }
        else
        {
            skill[2] = false;
        }


    }
    
    void CheckSkill4()
    {


        if (stat.CheckSkillSPEnough(3) && buttonSkill4.OnPressed)
        {
            skill[3] = true;
            
        }
        else
        {
            skill[3] = false;
        }


    }





    #endregion

    #region Get/SetActionEnableState
    public bool GetMoveEnabled()
    {
        return moveEnabled; 
    }
    public bool GetAttackEnabled()
    {
        return attackEnabled;
    }

    public bool GetJumpEnabled()
    {
        return jumpEnabled;
    }
    public bool GetRollEnabled()
    {
        return rollEnabled;
    }

    //set方法
    public void SetMoveEnabled()
    {
        moveEnabled = true;
    }
    public void SetAttackEnabled()
    {
        attackEnabled = true;
    }

    public void SetJumpEnabled()
    {
        jumpEnabled = true;
    }
    public void SetRollEnabled()
    {
        rollEnabled = true;
    }


    public void SetMoveDisabled()
    {
        moveEnabled = false;
    }
    public void SetAttackDisabled()
    {
        attackEnabled = false;
    }

    public void SetJumpDisabled()
    {
        jumpEnabled = false;
    }
    public void SetRollDisabled()
    {
        rollEnabled = false;
    }
    #endregion
    public bool CheckCharacterClipState(Animator animator, int LayerId, string[] states)
    {
        //string currstate = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        
        //print(currstate);
        foreach (string s in states)
        {
            if (info.IsName(s))
                return true;
        }
        return false;
    }

    #region Get/SetSignalInputEnableState
    public void SetInputDisabled(string s)
    {

        if (s.Equals("roll"))
        {
            inputRollEnabled = false;
        }
        else if (s.Equals("jump"))
        {
            inputJumpEnabled = false;
        }
        else if (s.Equals("attack"))
        {
            inputAttackEnabled = false;
        }
        else if (s.Equals("move"))
        {
            inputMoveEnabled = false;
        }
        StartCoroutine(InputEnableAgain(s,0.3f));

    }
    public void SetInputRoll(int signal)
    {
        if (signal == 0)
            inputRollEnabled = false;
        else inputRollEnabled = true;

    }
    public void SetInputJump(int signal)
    {
        if (signal == 0)
            inputJumpEnabled = false;
        else inputJumpEnabled = true;

    }
    public void SetInputAttack(int signal)
    {
        if (signal == 0)
            inputAttackEnabled = false;
        else inputAttackEnabled = true;

    }
    public void SetInputMove(int signal)
    {
        if (signal == 0)
            inputMoveEnabled = false;
        else inputMoveEnabled = true;

    }



    public void SetInputEnabled(string s)
    {

        if (s.Equals("roll"))
        {
            inputRollEnabled = true;
        }
        else if (s.Equals("jump"))
        {
            inputJumpEnabled = true;
        }
        else if (s.Equals("attack"))
        {
            inputAttackEnabled = true;
        }
        else if (s.Equals("move"))
        {
            inputMoveEnabled = true;
        }

    }

    private IEnumerator InputEnableAgain(string type, float time)
    {
        //if(type.Equals("roll") && inputRollEnabled)

        yield return new WaitForSeconds(time);
        SetInputEnabled(type);
    }
    #endregion

    public void LockDirection(int flag)

    {
        if (flag == 1)
            directionLock = true;
        else directionLock = false;
    }

    public void DisableAllInput()
    {
        inputAttackEnabled = false;
        inputJumpEnabled = false;
        inputMoveEnabled = false;
        inputRollEnabled = false;
    }
    public void EnableAllInput()
    {
        inputAttackEnabled = true;
        inputJumpEnabled = true;
        inputMoveEnabled = true;
        inputRollEnabled = true;
    }

    private void LoadKeySetting()
    {
        keyAttack = GlobalController.keyAttack;
        keySkill1 = GlobalController.keySkill1;
        keySkill2 = GlobalController.keySkill2;
        keySkill3 = GlobalController.keySkill3;
        keySkill4 = GlobalController.keySkill4;
        keyLeft = GlobalController.keyLeft;
        keyRight = GlobalController.keyRight;
        keyUp = GlobalController.keySpecial;
        keyDown = GlobalController.keyDown;
        keyRoll = GlobalController.keyRoll;
        keyJump = GlobalController.keyJump;
        
        if(Input.GetJoystickNames().Length == 0)
            return;
        
        GlobalController.Instance.inputActionAsset.Enable();
        GlobalController.gamepadMap = GlobalController.Instance.inputActionAsset.FindActionMap("Player");
        GlobalController.gamepadMap.Enable();
        
        var actionMap = GlobalController.Instance.inputActionAsset.FindActionMap("Player");
        var actions = actionMap.actions;

        var moveL = actions[0].bindings[1];
        var moveR = actions[0].bindings[2];
        var moveD = actions[1].bindings[0];
        var attack = actions[2].bindings[0];
        var jump = actions[3].bindings[0];
        var roll = actions[4].bindings[0];
        var special = actions[5].bindings[0];
        var skill1 = actions[6].bindings[0];
        var skill2 = actions[7].bindings[0];
        var skill3 = actions[8].bindings[0];
        var skill4 = actions[9].bindings[0];
        var escape = actions[10].bindings[0];
        
        gamepadButtonDict = new()
        {
            {"MoveL",moveL},
            {"MoveR",moveL},
            {"MoveD",moveD},
            {"Attack",attack},{"Jump",jump},{"Dodge",roll},{"Special",special},
            {"Skill1",skill1},{"Skill2",skill2},{"Skill3",skill3},{"Skill4",skill4},
            {"Escape",escape}
        };

        

    }
    /// <summary>
    /// 设置按键
    /// </summary>
    /// <param name="keys">0:Attack / 1-4:Skills / 5:Left / 6:Right / 7:Special / 8:Down / 9:Roll / 10:Jump</param>
    public void SetKeySetting(KeyCode[] keys)
    {
        if(keys.Length != 12)
        {
            return;
        }
        
        keyAttack = keys[0];
        keySkill1 = keys[1];
        keySkill2 = keys[2];
        keySkill3 = keys[3];
        keySkill4 = keys[4];
        keyLeft = keys[5];
        keyRight = keys[6];
        keyUp = keys[7]; //special
        keyDown = keys[8];
        keyRoll =  keys[9];
        keyJump =  keys[10];
        keyEsc =  keys[11];

        // keySkill1 = keys[1];
        // keySkill2 = keys[2];
        // keySkill3 = keys[3];
        // keySkill4 = keys[4];
        // keyLeft = keys[5];
        // keyRight = keys[6];
        // keyUp = keys[7]; //special
        // keyDown = keys[8];
        // keyRoll = keys[9];
        // keyJump = keys[10];
    }

    public void InvokeAttackSignal()
    {
        OnPressAttack?.Invoke();
    }

    public void DisableAndIdle(bool idle = true)
    {
        inputAttackEnabled = false;
        enabled = false;
        ac.OnHurtExit();
        ac.anim.SetFloat("forward", 0);
        ac.anim.SetBool("attack", false);
        ac.anim.SetBool("roll", false);
        roll = false;
        stdAtk = false;
        for(int i = 0; i < skill.Length; i++)
        {
            skill[i] = false;
        }
        if(idle)
            ac.anim.Play("idle");
        DRight = 0;
    }
    
    public void EnableAndIdle()
    {
        inputAttackEnabled = true;
        enabled = true;
        ac.OnHurtExit();
        ac.anim.SetFloat("forward", 0);
        ac.anim.SetBool("attack", false);
        ac.anim.SetBool("roll", false);
        ac.anim.Play("idle");
        DRight = 0;
    }

    public static string GetGamepadInputKeyPath(string name)
    {
        var actionMap = GlobalController.Instance.inputActionAsset.FindActionMap("Player");
        var actions = actionMap.actions;

        var moveL = actions[0].bindings[1];
        var moveR = actions[0].bindings[2];
        var moveD = actions[1].bindings[0];
        var attack = actions[2].bindings[0];
        var jump = actions[3].bindings[0];
        var roll = actions[4].bindings[0];
        var special = actions[5].bindings[0];
        var skill1 = actions[6].bindings[0];
        var skill2 = actions[7].bindings[0];
        var skill3 = actions[8].bindings[0];
        var skill4 = actions[9].bindings[0];
        var escape = actions[10].bindings[0];
        
        Dictionary<string,InputBinding> gamepadButtonDict = new()
        {
            {"MoveL",moveL},
            {"MoveR",moveR},
            {"MoveD",moveD},
            {"Attack",attack},{"Jump",jump},{"Dodge",roll},{"Special",special},
            {"Skill1",skill1},{"Skill2",skill2},{"Skill3",skill3},{"Skill4",skill4},
            {"Escape",escape}
        };
        var binding = gamepadButtonDict[name];

        return UI_GameOption.SimplifyInputActionName(binding.path);

    }


}


