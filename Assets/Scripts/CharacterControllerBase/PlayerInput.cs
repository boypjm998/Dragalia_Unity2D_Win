using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public delegate void OnPressSignal();
    public static event OnPressSignal OnPressAttack;
    
    // Start is called before the first frame update
    //variables
    private PlayerStatusManager stat;
    private ActorController ac;

    [Header("Key Settings")] 
    [SerializeField]private float accTime = 0.1f;
    public bool standardAttackContinious = true;
    public string keyRight = "d";
    public string keyLeft = "a";
    public string keyDown = "s";
    public string keyUp = "w";
    public string keyAttack = "j";
    public string keyJump = "k";
    public string keyRoll = "l";
    public string keySkill1 = "u";
    public string keySkill2 = "i";
    public string keySkill3 = "o";
    public string keySkill4 = "h";
    //public string keyMenu = "Escape";

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



    private void Awake()
    {
        stat = transform.GetComponent<PlayerStatusManager>();
        ac = transform.GetComponent<ActorController>();
        for (int i = 0; i < 4; i++)
        {
            skill[i] = false;
        }
        //DisableAllInput();
        LoadKeySetting();
    }
    
    
    

    // Update is called once per frame
    void Update()
    {
        
        buttonEsc.Tick(Input.GetKey(KeyCode.Escape));
        
        if(BattleStageManager.Instance.isGamePaused)
            return;
        
        
        if(keyLeft!=String.Empty)
            buttonLeft.Tick(Input.GetKey(keyLeft));
        if(keyRight!=String.Empty)
            buttonRight.Tick(Input.GetKey(keyRight));
        if(keyAttack!=String.Empty)
            buttonAttack.Tick(Input.GetKey(keyAttack));
        if(keyJump!=String.Empty)
            buttonJump.Tick(Input.GetKey(keyJump));
        if(keyRoll!=String.Empty)
            buttonRoll.Tick(Input.GetKey(keyRoll));
        if(keyDown!=String.Empty)
            buttonDown.Tick(Input.GetKey(keyDown));
        if(keyUp!=String.Empty)
            buttonUp.Tick(Input.GetKey(keyUp));
        if(keySkill1!=String.Empty)
            buttonSkill1.Tick(Input.GetKey(keySkill1));
        if(keySkill2!=String.Empty)
            buttonSkill2.Tick(Input.GetKey(keySkill2));
        if(keySkill3!=String.Empty)
            buttonSkill3.Tick(Input.GetKey(keySkill3));
        if(keySkill4!=String.Empty)
            buttonSkill4.Tick(Input.GetKey(keySkill4));
        
        
        

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
        //DRight = Mathf.SmoothDamp(DRight, targetDRight, ref velocityDRight, accTime);
        DRight = Mathf.SmoothDamp(DRight, targetDRight, ref velocityDRight, accTime);

        if (inputMoveEnabled == false)
        {
            targetDRight = 0;
            DRight = 0;
            return false;
        }
        
        //if (buttonLeft.IsPressing && !buttonRight.IsPressing)
        //{
        //    ac.SetFaceDir(-1);
        //}
        //if (!buttonLeft.IsPressing && buttonRight.IsPressing)
        //{
        //    ac.SetFaceDir(1);
        //}

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
        else
        {
            stdAtk = buttonAttack.OnPressed;
        }


        return stdAtk;
    }

    void CheckSpecialMove()
    {
        
    }

    void CheckSkill1()
    {
        

        if (stat.currentSP[0] >= stat.requiredSP[0] && buttonSkill1.OnPressed)
        {
            skill[0] = true;
        }
        else {
            skill[0] = false;
        }
        

    }

    void CheckSkill2()
    {


        if (stat.currentSP[1] >= stat.requiredSP[1] && buttonSkill2.OnPressed)
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


        if (stat.currentSP[2] >= stat.requiredSP[2] && buttonSkill3.OnPressed)
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


        if (stat.currentSP[3] >= stat.requiredSP[3] && buttonSkill4.OnPressed)
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
    }
    /// <summary>
    /// 设置按键
    /// </summary>
    /// <param name="keys">0:Attack / 1-4:Skills / 5:Left / 6:Right / 7:Special / 8:Down / 9:Roll / 10:Jump</param>
    public void SetKeySetting(string[] keys)
    {
        if(keys.Length != 11)
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
        keyRoll = keys[9];
        keyJump = keys[10];
    }

    public void InvokeAttackSignal()
    {
        OnPressAttack?.Invoke();
    }

    public void DisableAndIdle()
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

    
}


