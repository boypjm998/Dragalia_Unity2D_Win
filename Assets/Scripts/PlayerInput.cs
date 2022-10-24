using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    //variables
    private StatusManager stat;

    [Header("Key Settings")]
    public string keyRight = "d";
    public string keyLeft = "a";
    public string keyDown = "s";
    public string keyAttack = "j";
    public string keyJump = "k";
    public string keyRoll = "l";
    public string keySkill1 = "u";
    public string keySkill2 = "i";
    public string keySkill3 = "o";
    public string keySkill4 = "h";

    public MyInputMoudle buttonRight = new MyInputMoudle();
    public MyInputMoudle buttonLeft = new MyInputMoudle();
    public MyInputMoudle buttonDown = new MyInputMoudle();
    public MyInputMoudle buttonAttack = new MyInputMoudle();
    public MyInputMoudle buttonJump = new MyInputMoudle();
    public MyInputMoudle buttonRoll = new MyInputMoudle();
    public MyInputMoudle buttonSkill1 = new MyInputMoudle();
    public MyInputMoudle buttonSkill2 = new MyInputMoudle();
    public MyInputMoudle buttonSkill3 = new MyInputMoudle();
    public MyInputMoudle buttonSkill4 = new MyInputMoudle();


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
    //是否允许信号作用
    public bool moveEnabled = true;
    public bool jumpEnabled = true;
    public bool rollEnabled = true;
    public bool attackEnabled = true;
    public bool directionLock = false;

    //是否允许信号输入
    public bool inputMoveEnabled = true;
    public bool inputAttackEnabled = true;
    public bool inputJumpEnabled = true;
    public bool inputRollEnabled = true;



    private void Awake()
    {
        stat = transform.GetComponent<StatusManager>();
        for (int i = 0; i < 4; i++)
        {
            skill[i] = false;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        buttonLeft.Tick(Input.GetKey(keyLeft));
        buttonRight.Tick(Input.GetKey(keyRight));
        buttonAttack.Tick(Input.GetKey(keyAttack));
        buttonJump.Tick(Input.GetKey(keyJump));
        buttonRoll.Tick(Input.GetKey(keyRoll));
        buttonDown.Tick(Input.GetKey(keyDown));
        buttonSkill1.Tick(Input.GetKey(keySkill1));


        //print(buttonDown.IsPressing && buttonDown.isExtending);

        checkMovement();    
        checkJump();
        checkRoll();
        checkStdAttack();

        CheckSkill1();

    }


    #region ActionInputCheck
    bool checkMovement()
    {
        targetDRight = (buttonRight.IsPressing ? 1.0f : 0) - (buttonLeft.IsPressing ? 1.0f : 0);
        DRight = Mathf.SmoothDamp(DRight, targetDRight, ref velocityDRight, 0.15f);

        if (inputMoveEnabled == false)
        {
            targetDRight = 0;
            DRight = 0;
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

        stdAtk = buttonAttack.IsPressing;
        return stdAtk;
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
        string currstate = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
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

}


