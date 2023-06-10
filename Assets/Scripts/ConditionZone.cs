using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Zone ID + 9000 is Special Status ID, don't use it in other scripts
[RequireComponent(typeof(Collider2D))]
public class ConditionZone : MonoBehaviour
{
    //public bool isEnemy = false;
    public static int debuffZoneNum;
    public static int buffZoneNum;
    public static ConditionZone[] instances = new ConditionZone[6];
    [Tooltip("If zone type equals to 0/1, this zone is a buffzone/debuffzone.")]
    public int zoneType;
    protected int zoneID;
    private bool isEnabled = false;
    

    protected List<StatusManager> StatusManagers = new();

    [Serializable]
    public class BuffZoneInfo
    {
        [Range(1,999)]public int conditionID;
        public float effect;
        
        [Range(1,100)]public int maxStack = 100;
        public bool dispellable = true;
    }

    public List<BuffZoneInfo> BuffZoneInfos;
    public float field_duration;

    private void Awake()
    {
        if(field_duration > 0)
            Destroy(gameObject,field_duration);
        
    }

    private void OnEnable()
    {
        isEnabled = true;
        if (zoneType == 0)
        {
            //print(buffZoneNum);
            buffZoneNum = buffZoneNum + 1;
            if (buffZoneNum > 4)
            {
                Destroy(instances[0].gameObject);
            }
        }
        if (zoneType == 1)
        {
            debuffZoneNum = debuffZoneNum + 1;
            if (buffZoneNum > 2)
            {
                Destroy(instances[4].gameObject);
            }
        }
        
        
        
        
        if(zoneType == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (instances[i] == null)
                {
                    instances[i] = this;
                    zoneID = i;
                    break;
                }
            }
            // instances[buffZoneNum - 1] = this;
            // zoneID = buffZoneNum - 1;
        }
        if (zoneType == 1)
        {
            for (int i = 4; i < 6; i++)
            {
                if (instances[i] == null)
                {
                    instances[i] = this;
                    zoneID = i;
                    break;
                }
            }
            // instances[debuffZoneNum + 3] = this;
            // zoneID = debuffZoneNum + 3;
        }
    }

    private void OnDisable()
    {
        isEnabled = false;
        instances[zoneID] = null;
        zoneID = 0;
        if (zoneType == 0)
        {
            buffZoneNum = buffZoneNum - 1;
        }
        if (zoneType == 1)
        {
            debuffZoneNum = debuffZoneNum - 1;
        }
    }

    


    private void OnTriggerEnter2D(Collider2D col)
    {
        if(StatusManagers == null)
            return;
        if (zoneType == 0)
        {
            //如果是Character层，return
            if (col.gameObject.tag == "Player")
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
                            9000 + zoneID);
                        if (!buffZone.dispellable)
                            condition.dispellable = false;
                        statusManager.ObtainTimerBuff(condition,i==0);
                        i = 1;
                    }
                    
                }

            }
        }
        if (zoneType == 1)
        {
            //如果是Character层，return
            if (col.gameObject.tag == "Enemy")
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
                            9000 + zoneID);
                        if (!buffZone.dispellable)
                            condition.dispellable = false;
                        statusManager.ObtainTimerBuff(condition,i==0);
                        i = 1;
                    }
                    
                }

            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(StatusManagers == null)
            return;
        
        if (zoneType == 0)
        {
            if (col.gameObject.tag == "Player")
            {
                var statusManager = col.GetComponentInParent<StatusManager>();
                if(statusManager == null)
                    return;
                
                if (StatusManagers.Contains(statusManager))
                {
                    StatusManagers.Remove(statusManager);
                    foreach (var buffZone in BuffZoneInfos)
                    {
                        statusManager.RemoveSpecificTimerbuff(buffZone.conditionID, 9000 + zoneID);
                    }
                    
                }

            }
        }
        if (zoneType == 1)
        {
            if (col.gameObject.tag == "Enemy")
            {
                var statusManager = col.GetComponentInParent<StatusManager>();
                if(statusManager == null)
                    return;
                
                if (StatusManagers.Contains(statusManager))
                {
                    StatusManagers.Remove(statusManager);
                    foreach (var buffZone in BuffZoneInfos)
                    {
                        statusManager.RemoveSpecificTimerbuff(buffZone.conditionID, 9000 + zoneID);
                    }
                    
                }

            }
        }
    }

    private void OnDestroy()
    {
        var Stats = FindObjectsOfType<StatusManager>();
        foreach (var statusManager in Stats)
        {
            foreach (var buffZone in BuffZoneInfos)
            {
                statusManager.RemoveSpecificTimerbuff(buffZone.conditionID, 9000 + zoneID);
            }
        }
        

        if(isEnabled==false)
            return;
        if(zoneType == 0)
        {
            buffZoneNum = buffZoneNum - 1;
            instances[zoneID] = null;
        }
        if (zoneType == 1)
        {
            debuffZoneNum = debuffZoneNum - 1;
            instances[zoneID] = null;
        }
    }
}
