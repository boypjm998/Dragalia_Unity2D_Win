using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConditionBar : MonoBehaviour
{
    [SerializeField] protected GameObject buffIconPrefab;
    [SerializeField] protected GameObject debuffIconPrefab;
    [SerializeField] protected GameObject AfflictionPrefab;

    [SerializeField] protected StatusManager targetStat;

    [SerializeField] protected List<int> conditionIDList;

    [SerializeField] protected float startPosition = -350;

    [SerializeField] protected float distanceBetweenBuff = 50;
    
    [SerializeField] protected float maxCapacity = 14;

    [SerializeField] private int seperateBound; //index between buff and debuff

    private int conditionCount;
    
    // Start is called before the first frame update
    private void Awake()
    {
        conditionIDList = new List<int>();
    }

    private void Start()
    {
        //yield return new WaitUntil(()=>GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        targetStat = GameObject.Find("PlayerHandle").GetComponentInChildren<StatusManager>();
        conditionCount = targetStat.conditionList.Count;



    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    protected void TestInstIcon()
    {
        var sprite = targetStat.conditionList[0].GetIcon();
        GameObject icon = Instantiate(buffIconPrefab, transform.position, Quaternion.identity, transform);
        Image iconImg = icon.transform.Find("Icon").GetComponent<Image>();
        iconImg.sprite = sprite;
    }

  

    public void OnConditionAdd(BattleCondition buff)
    {
        
        
        if (!conditionIDList.Contains(buff.buffID))
        {
            InstantiateIcon(buff);
            StartCoroutine(SortBar());
            //print("sort");
        }
        var icon = FindBuffInBar(buff.buffID);
            
        icon.OnConditionAdd();
        
        

    }
    public void OnConditionRemove(int buffID)
    {
        if (conditionIDList.Contains(buffID))
        {
            var icon = FindBuffInBar(buffID);
            
            int buffnum = icon.OnConditionRemove();

            if (buffnum <= 0)
            {
                conditionIDList.Remove(buffID);
                Destroy(icon.gameObject);
                if(GlobalController.currentGameState == GlobalController.GameState.Inbattle)
                    StartCoroutine(SortBar());
            }

            //Remember to clear the icon if stack is 0.
        }
    }

    protected void InstantiateIcon(BattleCondition buff)
    {
        var sprite = buff.GetIcon();
        bool isBuff = false;

        if (buff.buffID<=200)
        {
            isBuff = true;
        }

        GameObject icon;
        if (buff.buffID < 400)
        {
            icon = 
                Instantiate(isBuff?buffIconPrefab:debuffIconPrefab, 
                    transform.position,
                    Quaternion.identity, transform);
        }
        else
        {
            icon = 
                Instantiate(AfflictionPrefab, 
                    transform.position,
                    Quaternion.identity, transform);
        }


        icon.GetComponent<RectTransform>().localPosition =
            new Vector3(startPosition + conditionIDList.Count * distanceBetweenBuff, 0);
        
        Image iconImg = icon.transform.Find("Icon").GetComponent<Image>();
        iconImg.sprite = sprite;

        var condIcon = icon.GetComponent<ConditionIcon>();
        condIcon.order = conditionIDList.Count;
        condIcon.conditionInfo = buff;
        condIcon._statusManager = targetStat;
        
        conditionIDList.Add(buff.buffID);
        
    }
    protected void RemoveIcon(BattleCondition buff)
    {
        int index = conditionIDList.IndexOf(buff.buffID);
        if (index >= 0)
        {
            
        }

    }

    protected ConditionIcon FindBuffInBar(int buffID)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<ConditionIcon>();
            if (child.conditionInfo.buffID == buffID)
            {
                return child;
            }
        }

        return null;
    }

    protected IEnumerator SortBar()
    {
        yield return null;//Destroy等一会
 
        
        var childs = transform.GetComponentsInChildren<ConditionIcon>();
        List<ConditionIcon> childList = 
            new List<ConditionIcon>(childs.ToList());
        
        for (int i = 0; i < transform.childCount; i++)
        {
            childs[i].order = i;
        }

        var newOrderList = SortConditionPriorty(childList);
   
        for (int i = 0; i < childs.Length; i++)
        {
            
            //childs[i].order = newOrder;
            childs[i].order = newOrderList[i];
            
            childs[i].GetComponent<RectTransform>().localPosition =
                new Vector3(startPosition + (childs[i].order) * distanceBetweenBuff, 0);
            //}
            
        }
        

        
        
    }

    private int CompareConditionPriority(ConditionIcon a, ConditionIcon b)
    {
        int typeA = GetConditionType(a.conditionInfo.buffID);
        int typeB = GetConditionType(b.conditionInfo.buffID);

        if (typeA > typeB)
        {
            return 1;
        }else if (typeA < typeB)
        {
            return -1;
        }
        else if (typeA == typeB)
        {
            if (a.order >= b.order)
                return 1;
            else if (a.order < b.order)
                return -1;
            else return 0;
        }
        else return 0;

        

    }

    private int GetConditionType(int buffID)
    {
        if (buffID <= 200)
        {
           return 1;
        }else if (buffID <= 400)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    int[] SortConditionPriorty(List<ConditionIcon> conds)
    {
        
        var output = new int[conds.Count];
        var m = 0;

        for (int i = 0; i < conds.Count; i++)
        {
            if (GetConditionType(conds[i].conditionInfo.buffID) != 1) continue;
            output[i] = m;
            m++;
        }
        for (int i = 0; i < conds.Count; i++)
        {
            if (GetConditionType(conds[i].conditionInfo.buffID) != 2) continue;
            output[i] = m;
            m++;
        }
        for (int i = 0; i < conds.Count; i++)
        {
            if (GetConditionType(conds[i].conditionInfo.buffID) != 3) continue;
            output[i] = m;
            m++;
        }
        
        return output;

    }

}