using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public PlayerInput pi;
    public PlayerStatusManager _statusManager;
    public ActorController ac;
    public Animator dAnim;
    public Rigidbody2D rigid;
    

    public float moveSpeed;
    public float gravityScale;
    public bool isFlying = false;
    
    
    public Vector4 HitSensorProperty = new Vector4(0, 2.2f, 1.5f, 5);
    public Coroutine comboStageResetRoutine;
    
    //Properties
    public float[] requiredDSP = new float[4];
    public float[] currentDSP = new float[4];

    public float MaxDModeGauge => maxDModeGauge;
    public float ReqDModeGauge => requiredDModeGauge;
    
    [SerializeField] protected float maxDModeGauge = 100;
    [SerializeField] protected float requiredDModeGauge = 50;
    [SerializeField] protected float depletionRatePerSec = 10f;
    
    
    public bool isSpecial = false;
    public bool canTransformBack = true;
    
    /*TODO: 龙化需要注意的地方：
     异常状态期间不可以龙化。
     当龙化时，需要解除所有的debuff，但是需要保留所有的buff,龙化时免疫部分debuff。
     龙化后的攻击需要重新计算伤害。
     龙化后的技能槽获取需要视情况而定。
     龙化后需要重置角色信息栏UI
     龙化后需要重置角色ActorController的动画状态机，开启龙化动画状态机。
     龙化需要调整碰撞体积和重力等等
     
     
     */

    protected virtual void Awake()
    {
        pi = transform.GetComponentInParent<PlayerInput>();
        _statusManager = transform.GetComponentInParent<PlayerStatusManager>();
        ac = transform.parent.parent.GetComponent<ActorController>();
        rigid = ac.rigid;
        dAnim = GetComponentInChildren<Animator>();
        _statusManager.OnReviveOrDeath += DModeForcePurge;
    }

    protected virtual void Move()
    {
        
    }

    protected virtual void Update()
    {
        if(!ac.DModeIsOn)
            return;
        
        //ac.checkFaceDir();
        CheckShapeShifting();
        DModeGaugeTick(Time.deltaTime);
        CheckSkill();
        CheckRoll();
        CheckStandardAttack();
    }

    protected void DModeGaugeTick(float deltaTime)
    {
        if(_statusManager.isShapeshifting == false)
            return;
        if(pi.isSkill)
            return;
        _statusManager.DepleteDP(depletionRatePerSec * deltaTime);
        if(_statusManager.DModeGauge <= 0 && canTransformBack)
            DModePurged();
    }

    /// <summary>
    /// 同Actorcontroller的ClearEnergizedOrInspired
    /// </summary>
    protected void ClearEnergizedOrInspired()
    {
        if (_statusManager.Inspired)
        {
            _statusManager.Inspired = false;
            var buff = _statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.Inspiration);
            if (buff.Count > 0)
                _statusManager.RemoveConditionWithLog(buff[0]);
        }


        if (_statusManager.Energized)
        {
            _statusManager.Energized = false;
            var buff = _statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.Energy);
            if (buff.Count > 0)
                _statusManager.RemoveConditionWithLog(buff[0]);
        }
    }

    protected virtual void CheckSkill()
    {
    }

    protected virtual void UseSkill(int skillID)
    {
    }

    protected virtual void CheckShapeShifting()
    {
        
        if (pi.buttonUp.OnPressed)
        {
            if (pi.hurt == false && pi.isSkill == false && pi.attackEnabled &&
                dAnim.GetCurrentAnimatorStateInfo(0).IsName("transform")==false)
            {
                
                DModePurged();
                //pi.moveEnabled = false;
                //pi.isSkill = true;
            }
        }
    }

    protected virtual void CheckStandardAttack()
    {
        if (pi.stdAtk && pi.attackEnabled)
        {
            dAnim.SetBool("attack",true);
        }
    }

    protected virtual void CheckRoll()
    {
        if (pi.roll && pi.rollEnabled)
        {
            dAnim.SetBool("roll",true);
            if (dAnim.GetCurrentAnimatorStateInfo(0).IsName("transform") &&
                dAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
            {
                //dAnim.SetBool("roll",false);
                dAnim.Play("roll");
            }

        }
        
        
    }

    /// <summary>
    /// 进入龙化状态
    /// </summary>
    protected virtual void EnterShapeShifting()
    {
        ac.dodging = true;
        ac.anim.enabled = false;
        pi.moveEnabled = false;
        pi.inputRollEnabled = true;
        pi.rollEnabled = false;
        //pi.isSkill = false;
        //rigid.gravityScale = gravityScale;
        
        _statusManager.knockbackRes = 999;
        _statusManager.ReliefAllAfflication();
        _statusManager.ReliefAllDebuff();
        
        //重置碰撞体
        var hitsensorCol = ac.HitSensor as BoxCollider2D;
        hitsensorCol.offset = new Vector3(HitSensorProperty.x,
            HitSensorProperty.y);
        hitsensorCol.size = new Vector3(HitSensorProperty.z,
            HitSensorProperty.w);
        
        ac.SetGravityScale(gravityScale);
        
        //重置UI状态(通过事件已经实现)
        
        

        //TODO: 如果是特殊龙化，需要在这里添加特殊效果

        //为龙化技能充能
        if (isSpecial)
        {
            for (int i = 0; i < requiredDSP.Length; i++)
            {
                currentDSP[i] = requiredDSP[i];
            }
            

        }


        //TODO: 如果不是特殊龙化，需要处理龙化的血量单独计算的问题。
        
    }

    protected Vector2 NormalizedSpeed(Vector2 originSpeed)
    {
        if(originSpeed.magnitude > 1)
            return originSpeed.normalized;
        else
            return originSpeed;
    }

    protected virtual void OnShapeShiftingTransformed()
    {
        pi.moveEnabled = true;
        ac.dodging = false;
        pi.isSkill = false;
        pi.moveEnabled = true;
    }

    protected void DisableInputMove()
    {
        pi.inputMoveEnabled = false;
    }
    
    protected void EnableInputMove()
    {
        pi.inputMoveEnabled = true;
    }

    protected void EnableInputAttack()
    {
        pi.inputAttackEnabled = true;
    }
    
    protected void SetRollEnabled()
    {
        pi.rollEnabled = true;
    }
    
    protected void SetRollDisabled()
    {
        pi.rollEnabled = false;
    }

    protected void SetAttackEnabled()
    {
        pi.attackEnabled = true;
    }
    
    protected void SetAttackDisabled()
    {
        pi.attackEnabled = false;
    }

    protected void ClearBoolSignal(string varname)
    {
        dAnim.SetBool(varname, false);
    }
    
    protected void ClearFloatSignal(string varname)
    {
        dAnim.SetFloat(varname, 0);
    }

    protected void SetMoveEnabled()
    {
        pi.moveEnabled = true;
    }

    protected void SetMoveDisabled()
    {
        pi.moveEnabled = false;
    }

    protected void FaceDirectionAutoFix(int eventID)
    {
        ac.FaceDirectionAutoFix(eventID);
    }


    protected void RollMove()
    {
        if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
        {
            ac.SetFaceDir(-1);
        }
        else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
        {
            ac.SetFaceDir(1);
        }

        StartCoroutine(HorizontalMove(moveSpeed, 0.5f, "roll"));
    }

    public IEnumerator HorizontalMove(float speed, float time, string move)
    {

        while (time > 0)
        {
            print("speed"+speed);
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            rigid.position = new Vector3(rigid.position.x + ac.facedir * speed * Time.fixedDeltaTime,
                rigid.position.y);
            //transform.position = new Vector2(transform.position.x+transform.right.x * speed * Time.fixedDeltaTime,transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            if (dAnim.GetCurrentAnimatorStateInfo(0).IsName(move) == false)
            {
                //print("interrupt");
                if (Mathf.Abs(rigid.velocity.x) > moveSpeed && ac.hurt==false)
                    rigid.velocity = new Vector2(moveSpeed, 0);
                //pi.SetMoveEnabled();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }


    }
    public void CheckFaceDir()
    {
        switch (pi.buttonLeft.IsPressing)
        {
            //if (pi.DRight > 0.01f) 
            case false when pi.buttonRight.IsPressing:
                //rigid.transform.eulerAngles = new Vector3(0, 0, 0);
                ac.SetFaceDir(1);
                break;
            //else if (pi.DRight < -0.01f)
            case true when !pi.buttonRight.IsPressing:
                //rigid.transform.eulerAngles = new Vector3(0, 180, 0);
                ac.SetFaceDir(-1);
                break;
        }
    }

    protected virtual void CheckCombo()
    {
        
    }
    
    protected virtual IEnumerator ResetComboStage(float time)
    {
        yield return new WaitForSeconds(time);
        
        comboStageResetRoutine = null;
    }


    /// <summary>
    /// 解除龙化状态
    /// </summary>
    public virtual void DModePurged()
    {
        if(ac.DModeIsOn == false)
            return;
        
        _statusManager.InvokeShapeshiftingExit();
        _statusManager.knockbackRes = 0;
        print("Called DModePurged");
        
        //重置碰撞体
        var hitsensorCol = ac.HitSensor as BoxCollider2D;
        hitsensorCol.offset = new Vector3(PlayerStatusManager.NormalHitSensorProperty.x,
            PlayerStatusManager.NormalHitSensorProperty.y);
        hitsensorCol.size = new Vector3(PlayerStatusManager.NormalHitSensorProperty.z,
            PlayerStatusManager.NormalHitSensorProperty.w);

        Instantiate(BattleEffectManager.Instance.shapeShiftPurgeFXPrefab,
            transform.position, Quaternion.identity,
            BattleStageManager.Instance.RangedAttackFXLayer.transform);
        
        ac.ResetGravityScale();
        _statusManager.shapeshiftingCDTimer = _statusManager.shapeshiftingCD;
        ac.transform.Find("Model").gameObject.SetActive(true);
        pi.hurt = false;
        pi.isSkill = false;
        ac.DModeIsOn = false;
        ac.anim.enabled = true;

        if (_statusManager.remainReviveTimes > 0 && _statusManager.currentHp > 0)
        {
            ac.anim.Play("idle");
        
            if(ac.HitSensor.enabled)
                ac.InvokeIFrameForSeconds(1f);
            ac.OnHurtExit();
            pi.moveEnabled = true;
        }
        transform.parent.gameObject.SetActive(false);
    }

    public void DModeForcePurge()
    {
        if(ac.DModeIsOn == false)
            return;
        
        //如果该游戏物体或者其父物体被禁用，那么就不执行
        if (gameObject.activeInHierarchy == false)
            return;

        canTransformBack = true;
        _statusManager.InvokeShapeshiftingExit();
        _statusManager.knockbackRes = 0;
        print("Called DModePurged");
        
        //重置碰撞体
        var hitsensorCol = ac.HitSensor as BoxCollider2D;
        hitsensorCol.offset = new Vector3(PlayerStatusManager.NormalHitSensorProperty.x,
            PlayerStatusManager.NormalHitSensorProperty.y);
        hitsensorCol.size = new Vector3(PlayerStatusManager.NormalHitSensorProperty.z,
            PlayerStatusManager.NormalHitSensorProperty.w);

        Instantiate(BattleEffectManager.Instance.shapeShiftPurgeFXPrefab,
            transform.position, Quaternion.identity,
            BattleStageManager.Instance.RangedAttackFXLayer.transform);
        
        ac.ResetGravityScale();
        _statusManager.shapeshiftingCDTimer = _statusManager.shapeshiftingCD;
        ac.transform.Find("Model").gameObject.SetActive(true);
        pi.hurt = false;
        pi.isSkill = false;
        ac.DModeIsOn = false;
        ac.anim.enabled = true;

        if (_statusManager.remainReviveTimes > 0)
        {
            ac.anim.Play("idle");
            ac.OnHurtExit();
            pi.moveEnabled = true;
        }
        transform.parent.gameObject.SetActive(false);
    }



    protected virtual void OnDModeRollEnter()
    {
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.moveEnabled = false;
        
        pi.roll = false;
        dAnim.SetBool("roll", false);

        ac.dodging = true;
    }
    
    protected virtual void OnDModeRollExit()
    {
        pi.attackEnabled = true;
        pi.jumpEnabled = true;
        pi.moveEnabled = true;
        //pi.rollEnabled = true;
        ac.dodging = false;
        dAnim.SetBool("roll", false);
        pi.SetInputEnabled("move");
        
    }

    

    protected void OnDModeIdleEnter()
    {
        if (comboStageResetRoutine != null)
        {
            StopCoroutine(comboStageResetRoutine);
            comboStageResetRoutine = null;
            print("interrupted");
        }

        comboStageResetRoutine = StartCoroutine(ResetComboStage(0.5f));
    }

    protected virtual void OnDModeStandardAttackEnter()
    {
        
        ac.ta.FaceDirectionAutofixWithMarking();
        pi.stdAtk = false;
        pi.moveEnabled = false;
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.inputAttackEnabled = false;
        pi.rollEnabled = false;
          
        if(comboStageResetRoutine!=null)
            StopCoroutine(comboStageResetRoutine);
        comboStageResetRoutine = null;
          
        ClearBoolSignal("attack");
        
        CheckCombo();

    }

    protected virtual void OnDModeStandardAttackExit()
    {
        pi.jumpEnabled = true;
        pi.moveEnabled = true;
        pi.attackEnabled = true;
        pi.jumpEnabled = true;
        pi.inputAttackEnabled = true;
        pi.inputRollEnabled = true;
        pi.rollEnabled = true;
    }

    protected virtual void OnDModeSkillEnter()
    {
        ac.ta.FaceDirectionAutofixWithMarking();
        
        canTransformBack = false;
        pi.isSkill = true;
        pi.rollEnabled = false;
        pi.inputRollEnabled = false;
        pi.directionLock = false;
        ac.dodging = true;
        

        ac.ActionDisable((int)ActorController.PlayerActionType.MOVE);
        ac.ActionDisable((int)ActorController.PlayerActionType.JUMP);
        ac.ActionDisable((int)ActorController.PlayerActionType.ROLL);
        ac.ActionDisable((int)ActorController.PlayerActionType.ATTACK);
        //pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        //print("skillEnter");
    }

    protected virtual void OnDModeSkillExit()
    {
        ClearEnergizedOrInspired();
        canTransformBack = true;
        pi.isSkill = false;
        ac.dodging = false;
        pi.rollEnabled = true;
        pi.inputRollEnabled = true;
        ac.ActionEnable((int)ActorController.PlayerActionType.MOVE); //move
        ac.ActionEnable((int)ActorController.PlayerActionType.JUMP); //jump
        ac.ActionEnable((int)ActorController.PlayerActionType.ROLL); //roll
        ac.ActionEnable((int)ActorController.PlayerActionType.ATTACK);
        pi.SetInputEnabled("move");
        //2023.6.19
        try
        {
            dAnim.SetFloat("forward", 0);
        }
        catch
        {
        }

    }

    protected void SkillCancleFrame()
    {
        pi.isSkill = false;
        pi.rollEnabled = true;
        pi.attackEnabled = true;
    }

    public bool CheckTransformCondition()
    {
        if(!_statusManager)
            _statusManager = GetComponentInParent<PlayerStatusManager>();
        
        if (requiredDModeGauge <= _statusManager.DModeGauge)
            return true;
        return false;
    }

}
