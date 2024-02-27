using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class MapObjectTrigger_100003 : MonoBehaviour
{
    
    [SerializeField] private bool isOn = false;
    [SerializeField] List<GameObject> relatedObjects = new();
    [SerializeField] private float cooldown = 3f;
    private Collider2D _collider2D;
    private Action<GameObject,bool> onTriggeredAction;
    
    
    private GameObject fx_on;
    private GameObject fx_off;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        fx_off = transform.GetChild(0).gameObject;
        fx_on = transform.GetChild(1).gameObject;
        
        SetTriggerAction((relatedObject,flag) =>
        {
            DOVirtual.DelayedCall(0.5f,
                () =>
                {
                    relatedObject.SetActive(!relatedObject.activeSelf);
                },
            false);
            if (flag)
            {
                StageCameraController.SwitchMainCameraFollowObject(relatedObject);
            }

            
        });
        
        
        
        
    }

    private void Start()
    {
        SetOn(isOn);
    }

    private void SetOn(bool flag)
    {
        fx_on.SetActive(flag);
        fx_off.SetActive(!flag);
    }


    public void SetTriggerAction(Action<GameObject,bool> action)
    {
        onTriggeredAction = action;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        
        var atk = other.GetComponent<AttackFromPlayer>();
        
        if(atk == null)
            return;
        
        if(atk.attackType != BasicCalculation.AttackType.DASH)
            return;
        
        
        if(StageCameraController.Instance.MainCameraFollowObject.gameObject != BattleStageManager.Instance.GetPlayer())
            return;
        
        BattleStageManager.Instance.GetPlayer().GetComponent<ActorController>().SetHitSensor(false);


        _collider2D.enabled = false;

        int count = relatedObjects.Count;

        int index = 0;
        
        for (int i = 0; i < count; i ++)
        {
            var relatedObject = relatedObjects[i];
            if (index < count - 1)
            {
                print(index);
                DOVirtual.DelayedCall((index+0.5f), ()=>
                {
                    onTriggeredAction(relatedObject,true);
                },false);
            }
            else
            {
                print(index + "Return");
                DOVirtual.DelayedCall((index+0.5f), ()=>
                {
                    onTriggeredAction(relatedObject,true);
                },false);
                
            }

            index++;

            if (index >= count)
            {
                DOVirtual.DelayedCall((index+0.5f), ()=>
                {
                    BattleStageManager.Instance.GetPlayer().GetComponent<ActorController>().SetHitSensor(true);
                    StageCameraController.SwitchMainCameraFollowObject(BattleStageManager.Instance.GetPlayer());
                },false);
            }

        }

        var hitEff = atk.hitConnectEffect;

        if (hitEff != null)
        {
            Instantiate(hitEff, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        CineMachineOperator.Instance.CamaraShake(atk.hitShakeIntensity, .1f);

        isOn = !isOn;
        SetOn(isOn);
        
        DOVirtual.DelayedCall(cooldown*count/3, () => _collider2D.enabled = true,false);
    }
    
    
    
}
