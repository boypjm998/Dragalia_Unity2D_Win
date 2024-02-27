using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class UI_HB004_Legend_01 : MonoBehaviour
{
    [SerializeField] private GameObject particleEffects;
    [SerializeField] private GameObject icon_skl;
    [SerializeField] private GameObject icon_dfs;
    [SerializeField] private GameObject icon_std;
    [SerializeField] private GameObject icon_oth;
    
    public StatusManager attachedStatus;
    public event Action OnTimer;
    
    
    private Tuple<BasicCalculation.AttackType,StatusManager> lastReceivedAttack;
    private UI_RingSlider _ringSlider;
    private bool receivedDamageInPeriod = false;
    private bool isActive = false;
    public bool IsActive => isActive;
    public int CurrentBuff { get; private set; }

    private void Awake()
    {
        attachedStatus = GetComponentInParent<StatusManager>();
        BattleStageManager.Instance.OnFieldAbilityAdd += CheckField;
        BattleStageManager.Instance.OnFieldAbilityRemove += CheckField;
        attachedStatus.OnBuffEventDelegate += DisplayEffects;
        attachedStatus.OnTakeDirectDamageFrom += EventOnHurt;
        _ringSlider = GetComponentInChildren<UI_RingSlider>();
        // attachedStatus.OnBuffDispelledEventDelegate += DisplayEffects;
        // attachedStatus.OnBuffExpiredEventDelegate += DisplayEffects;
    }

    private void OnDestroy()
    {
        BattleStageManager.Instance.OnFieldAbilityAdd -= CheckField;
        BattleStageManager.Instance.OnFieldAbilityRemove -= CheckField;
        attachedStatus.OnBuffEventDelegate -= DisplayEffects;
        attachedStatus.OnTakeDirectDamageFrom -= EventOnHurt;
    }

    private void Update()
    {
        if (_ringSlider.currentValue <= 0)
        {
            OnTimer?.Invoke();
            _ringSlider.currentValue = _ringSlider.maxValue;
        }
    }


    private void CheckField(int id)
    {
        if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20151))
        {
            particleEffects.SetActive(true);
            icon_skl.transform.parent.gameObject.SetActive(true);
            isActive = true;
            _ringSlider.currentValue = _ringSlider.maxValue;
            CurrentBuff = 1;
        }
        else
        {
            particleEffects.SetActive(false);
            icon_skl.transform.parent.gameObject.SetActive(false);
            isActive = false;
            CurrentBuff = 0;
        }
    }

    private void EventOnHurt(StatusManager src, StatusManager attacker, AttackBase atk, float dmg)
    {
        receivedDamageInPeriod = true;
        lastReceivedAttack = new Tuple<BasicCalculation.AttackType, StatusManager>(atk.attackType,attacker);
    }

    public Tuple<int,StatusManager> CheckReceivedAttack(bool reset = true)
    {
        if (receivedDamageInPeriod == false)
            return new Tuple<int, StatusManager>(-1,null);

        receivedDamageInPeriod = !reset;
        return new Tuple<int, StatusManager>((int)lastReceivedAttack.Item1,lastReceivedAttack.Item2);
    }


    private void DisplayEffects(BattleCondition buff)
    {
        switch (buff.buffID)
        {
            case (int)(BasicCalculation.BattleCondition.SkillShield):
            {
                icon_skl.SetActive(true);
                icon_std.SetActive(false);
                icon_dfs.SetActive(false);
                icon_oth.SetActive(false);
                CurrentBuff = 3;
                break;
            }
            case (int)(BasicCalculation.BattleCondition.StandardAttackShield):
            {
                icon_skl.SetActive(false);
                icon_std.SetActive(true);
                icon_dfs.SetActive(false);
                icon_oth.SetActive(false);
                CurrentBuff = 1;
                break;
            }
            case (int)(BasicCalculation.BattleCondition.DashForceShield):
            {
                icon_skl.SetActive(false);
                icon_std.SetActive(false);
                icon_dfs.SetActive(true);
                icon_oth.SetActive(false);
                CurrentBuff = 2;
                break;
            }
            case (int)(BasicCalculation.BattleCondition.OtherShield):
            {
                icon_skl.SetActive(false);
                icon_std.SetActive(false);
                icon_dfs.SetActive(false);
                icon_oth.SetActive(true);
                CurrentBuff = 4;
                break;
            }
        }
        
    }
    
    
}
