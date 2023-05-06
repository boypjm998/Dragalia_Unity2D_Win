using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
public class EnemyAttackHintBar : MonoBehaviour
{
    public float warningTime;
    public float attackLastTime = 0.5f;
    public float timeToSendEvadeSignal;
    [SerializeField]protected float height = -1;

    //protected Tweener _tweener;
    protected GameObject Fill;
    protected GameObject MaxFill;

    protected EnemyController ac;
    public bool interruptable = true;
    public float warningTimeLeft { get { return GetWarningTimeLeft(); } }
    
    protected Tweener _tweener;
    protected List<NpcController> npcInRange = new();
    [SerializeField] protected bool autoDestruct = false;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if(interruptable)
            ac.OnAttackInterrupt -= DestroySelf;
        CancelInvoke();
        foreach (var npc in npcInRange)
        {
            npc?.DeleteIgnoreList(GetComponent<Collider2D>());
        }
    }

    protected virtual void Start()
    {
        if (ac == null)
        {
            interruptable = false;
            Debug.LogWarning("HintBar cannot find enemy source.");
            return;
        }

        if(interruptable)
            ac.OnAttackInterrupt += DestroySelf;

        if (timeToSendEvadeSignal != 0)
        {
            Invoke(nameof(BroadCastEvadeSignalInTrigger),timeToSendEvadeSignal);
        }

        if (autoDestruct)
        {
            Destroy(gameObject, warningTime + attackLastTime);
        }
    }

    public void SetSource(EnemyController controller)
    {
        ac = controller;
    }

    public void HideAllUI()
    {
       //把所有子物体都隐藏
        for(int i = 0; i < transform.childCount ; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

    }
    
    public void ShowAllUI()
    {
        //把所有子物体都显示
        for(int i = 0; i < transform.childCount ; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    protected virtual float GetWarningTimeLeft()
    {
        if(_tweener == null)
            return 0.5f;

        return _tweener.ElapsedPercentage() * warningTime;
    }

    protected virtual void SetWarningSignalTime()
    {
        timeToSendEvadeSignal = warningTime - 0.5f;
    }

    protected virtual void BroadCastEvadeSignalInTrigger()
    {
        foreach (var npc in npcInRange)
        {
            npc.ReceiveEvadeSignal(height,attackLastTime,GetComponent<Collider2D>());
        }
    }
    
    protected bool BroadCastEvadeSignalInTriggerWithCallback()
    {
        //TODO:测试用，逻辑有问题
        var flags = new List<bool>();
        foreach (var npc in npcInRange)
        {
            flags.Add(npc.ReceiveEvadeSignal(height,attackLastTime,GetComponent<Collider2D>()));
        }

        if (npcInRange.Count == 0)
            return true;

        return (flags[0]);
    }
    
    

    protected void OnTriggerEnter2D(Collider2D col)
    {
        NpcController npc = null;
        if ((npc = col.GetComponentInParent<NpcController>()) != null)
        {
            if(!npcInRange.Contains(npc))
                npcInRange.Add(npc);
        }
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        NpcController npc = null;
        if ((npc = col.GetComponentInParent<NpcController>()) != null)
        {
            //npcInRange.Remove(npc);
        }
    }
}
