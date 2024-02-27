using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using GameMechanics;
using TMPro;
using UnityEngine;

/// <summary>
/// Training Hint!!!
/// </summary>
public class EnemyMoveController_E9001 : EnemyMoveManager
{
    public int damageTotal;
    public bool resetDamage;
    public float dps;
    private float currentTime = 0.1f;

    private int dmgStandardAttack;
    private int dmgSkill;
    private int dmgForce;
    private int dmgDMode;
    private int dmgDOT;
    private int dmgOther;


    private int dmgSkill1;
    private int dmgSkill2;
    private int dmgSkill3;
    private int dmgSkill4;

    [SerializeField] private GameObject DPS_PauseMenu;
    [SerializeField] private GameObject DPS_PauseMenuEN;
    
    
    
    
    
    [SerializeField] private bool printDamagePercentage;
    
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerHumanoid>();
        _statusManager.OnHPChange += TurnMoveToTarget;
        _statusManager.OnHPDecrease += AddTotalDamage;
        BattleStageManager.Instance.OnGameStart += ResetTime;
    }

    void TurnMoveToTarget()
    {
        ac.TurnMove(_behavior.targetPlayer);
    }

    public int[] GetDPSData()
    {
        return new[] { dmgStandardAttack, dmgSkill1, dmgSkill2, dmgSkill3, dmgSkill4, dmgForce, dmgDMode, dmgOther, dmgDOT };
        
    }

    private void ResetTime()
    {
        currentTime = 0.1f;
        BattleStageManager.Instance.OnGameStart -= ResetTime;
    }
    public float GetTime()
    {
        return currentTime;
    }

    public GameObject InstantiateNewPrefabMenu()
    {
        var prefab = GlobalController.Instance.GameLanguage == GlobalController.Language.EN ? DPS_PauseMenuEN : DPS_PauseMenu;
        
        var ui = Instantiate(prefab, GameObject.Find("UI").transform);
        ui.GetComponent<UI_DPS_PauseMenu>().dmgSource = this;
        return ui;
    }

    public void AddTotalDamage(int dmg, AttackBase attackBase)
    {
        damageTotal += dmg;

        if (attackBase == null)
        {
            dmgDOT += dmg;
        }
        else
        {
            var atkPlayer = attackBase as AttackFromPlayer;
            PlayerStatusManager ps;

            try
            {
                if ((atkPlayer.ac as ActorController).dc.isSpecial == false)
                {
                    if (atkPlayer.attackType == BasicCalculation.AttackType.SKILL)
                    {
                        dmgDMode += dmg;
                    }
                    else if (atkPlayer.attackType == BasicCalculation.AttackType.STANDARD)
                    {
                        dmgDMode += dmg;
                    }
                    else return;
                }
            }
            catch
            {
            }



            if (atkPlayer.skill_id > 0)
            {
                switch (atkPlayer.skill_id)
                {
                    case 1:
                        dmgSkill1 += dmg;
                        break;
                    case 2:
                        dmgSkill2 += dmg;
                        break;
                    case 3:
                        dmgSkill3 += dmg;
                        break;
                    case 4:
                        dmgSkill4 += dmg;
                        break;
                }
                dmgSkill += (dmgSkill1 + dmgSkill2 + dmgSkill3 + dmgSkill4);
                return;
            }









            switch (attackBase.attackType)
            {
                case BasicCalculation.AttackType.STANDARD:
                {
                    dmgStandardAttack += dmg;
                    break;
                }
                // case BasicCalculation.AttackType.SKILL:
                // {
                //     dmgSkill += dmg;
                //     break;
                // }
                case BasicCalculation.AttackType.DSKILL:
                {
                    dmgDMode += dmg;
                    break;
                }
                case BasicCalculation.AttackType.DSTANDARD:
                {
                    dmgDMode += dmg;
                    break;
                }
                case BasicCalculation.AttackType.FORCE:
                {
                    dmgForce += dmg;
                    break;
                }
                default:
                {
                    dmgOther += dmg;
                    break;
                }

            }




        }
        
    }

    private void PrintPercentagesAndSort()
    {
        // 创建一个字典来存储变量的名称和值
        Dictionary<string, int> variables = new Dictionary<string, int>()
        {
            { "dmgStandardAttack", dmgStandardAttack },
            { "dmgSkill", dmgSkill },
            { "dmgForce", dmgForce },
            { "dmgDMode", dmgDMode },
            { "dmgDOT", dmgDOT },
            { "dmgOther", dmgOther }
        };

        // 计算总和
        int total = damageTotal;

        // 计算每个变量的百分比，并按百分比从高到低排序
        var sortedVariables = variables.Select(x => new 
            {
                Name = x.Key,
                Value = x.Value,
                Percentage = (double)x.Value / total * 100
            })
            .OrderByDescending(x => x.Percentage)
            .ToList();

        // 打印出每个变量的名称和百分比
        foreach (var variable in sortedVariables)
        {
            print($"{variable.Name}: {variable.Percentage}%");
        }
    }







    private void Update()
    {
        if (resetDamage)
        {
            damageTotal = 0;
            resetDamage = false;
            currentTime = 0.1f;
        }

        

        dps = damageTotal / currentTime;
        currentTime += Time.deltaTime;
    }


    public IEnumerator ReadInformation()
    {
        yield return null;
        UI_DialogDisplayer.Instance.EnqueueDialogShared(10101,90011,null);
        
        

        yield return new WaitForSeconds(4f);
        
        UI_DialogDisplayer.Instance.EnqueueDialogShared(10101,90012,null);
        var UIElements = GameObject.Find("UI");

        GameObject prefab;

        if (GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
        {
            prefab = Instantiate(projectile1, UIElements.transform);
        }else if(GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            prefab = Instantiate(projectile2, UIElements.transform);
        }
        else
        {
            yield break;
        }
        
        

        //TODO: 检查特殊行动
        SetHintTxt(prefab);
        var canvasGroup = prefab.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1, 0.5f);
        
        yield return new WaitForSeconds(4f);

        UI_DialogDisplayer.Instance.EnqueueDialogShared(10101,90013,null);
    }


    public void SetHintTxt(GameObject prefab)
    {
        var tmp = prefab.GetComponentInChildren<TextMeshProUGUI>();
        
        
        var playerInput = FindObjectOfType<PlayerInput>();

        tmp.text = BasicCalculation.GetTraningHintString(playerInput,GlobalController.Instance.GameLanguage);

    }


}
