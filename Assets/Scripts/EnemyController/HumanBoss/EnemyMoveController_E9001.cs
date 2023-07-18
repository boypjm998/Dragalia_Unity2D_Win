using System.Collections;
using System.Collections.Generic;
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
    
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerHumanoid>();
        _statusManager.OnHPChange += TurnMoveToTarget;
        _statusManager.OnHPDecrease += AddTotalDamage;
    }

    void TurnMoveToTarget()
    {
        ac.TurnMove(_behavior.targetPlayer);
    }

    public void AddTotalDamage(int dmg)
    {
        damageTotal += dmg;
    }

    public IEnumerator ReadInformation()
    {
        yield return null;
        UI_DialogDisplayer.Instance.EnqueueDialogShared(10101,90011,null);

        yield return new WaitForSeconds(4f);
        
        UI_DialogDisplayer.Instance.EnqueueDialogShared(10101,90012,null);
        var UIElements = GameObject.Find("UI");
        var prefab = Instantiate(projectile1, UIElements.transform);

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

        tmp.text = BasicCalculation.GetTraningHintString(playerInput);

    }


}
