using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionZoneUnlimited : MonoBehaviour
{
    
    protected List<StatusManager> StatusManagers = new();

    public bool playerEnable = true;
    public bool enemyEnable = true;

    public delegate void OnEnterZone(StatusManager statusManager);
    public event OnEnterZone onEnterZone;
    

    [Serializable]
    public class BuffZoneInfo
    {
        [Range(1,999)]public int conditionID = 1;
        public float effect;
        
        [Range(1,100)]public int maxStack = 1;
        public int spID = -1;
        public bool dispellable = true;
    }
    public List<BuffZoneInfo> BuffZoneInfos;
    public float field_duration;
    
    private void Awake()
    {
        if(field_duration > 0)
            Invoke("DisableZone",field_duration);
        
    }

    public void DisableZone()
    {
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject,0.1f);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(StatusManagers == null)
            return;
        
            //如果是Character层，return
            if (col.gameObject.tag == "Player" && playerEnable)
            {
                var statusManager = col.GetComponentInParent<StatusManager>();
                
                //print(col.gameObject.name);
                if(statusManager == null)
                    return;
                if (!StatusManagers.Contains(statusManager))
                {
                    StatusManagers.Add(statusManager);
                    int i = 0;
                    BattleCondition condition;
                    foreach (var buffZone in BuffZoneInfos)
                    {
                        condition = new TimerBuff(buffZone.conditionID, buffZone.effect, -1, buffZone.maxStack,
                            buffZone.spID);
                        if (!buffZone.dispellable)
                            condition.dispellable = false;
                        statusManager.ObtainTimerBuff(condition,i==0);
                        i = 1;
                    }
                    
                }

            }
            
            if (col.gameObject.tag == "Enemy" && enemyEnable)
            {
                var statusManager = col.GetComponentInParent<StatusManager>();
                
                if (!StatusManagers.Contains(statusManager))
                {
                    StatusManagers.Add(statusManager);
                    int i = 0;
                    BattleCondition condition;
                    foreach (var buffZone in BuffZoneInfos)
                    {
                        condition = new TimerBuff(buffZone.conditionID, buffZone.effect, -1, buffZone.maxStack,
                            buffZone.spID);
                        if (!buffZone.dispellable)
                            condition.dispellable = false;
                        statusManager.ObtainTimerBuff(condition,i==0);
                        i = 1;
                    }
                    
                }

            }
        
        
    }

    private void OnDestroy()
    {
        // var Stats = FindObjectsOfType<StatusManager>();
        // foreach (var statusManager in Stats)
        // {
        //     foreach (var buffZone in BuffZoneInfos)
        //     {
        //         statusManager.RemoveSpecificTimerbuff(buffZone.conditionID, buffZone.spID);
        //     }
        // }
        
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(StatusManagers == null)
            return;
        
        
            if (col.gameObject.tag == "Player" && playerEnable)
            {
                var statusManager = col.GetComponentInParent<StatusManager>();
                if(statusManager == null)
                    return;
                
                if (StatusManagers.Contains(statusManager))
                {
                    StatusManagers.Remove(statusManager);
                    foreach (var buffZone in BuffZoneInfos)
                    {
                        statusManager.RemoveSpecificTimerbuff(buffZone.conditionID, buffZone.spID);
                    }
                    
                }

            }
            
            if (col.gameObject.tag == "Enemy" && enemyEnable)
            {
                var statusManager = col.GetComponentInParent<StatusManager>();
                if(statusManager == null)
                    return;
                
                if (StatusManagers.Contains(statusManager))
                {
                    StatusManagers.Remove(statusManager);
                    foreach (var buffZone in BuffZoneInfos)
                    {
                        statusManager.RemoveSpecificTimerbuff(buffZone.conditionID, buffZone.spID);
                    }
                    
                }

            }
        
        
    }
}
