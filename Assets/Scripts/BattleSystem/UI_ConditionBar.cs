using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_ConditionBar : MonoBehaviour
{
    [SerializeField] private GameObject buffIconPrefab;

    [SerializeField] private StatusManager targetStat;

    [SerializeField] private List<int> conditionIDList;

    [SerializeField] private float startPosition = -350;

    [SerializeField] private float distanceBetweenBuff = 50;
    
    [SerializeField] private float maxCapacity = 14;

    [SerializeField] private int seperateBound; //index between buff and debuff

    private int conditionCount;
    
    // Start is called before the first frame update
    private void Awake()
    {
        conditionIDList = new List<int>();
    }

    private void Start()
    {
        
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
                //print(icon.gameObject.name);
                conditionIDList.Remove(buffID);
                Destroy(icon.gameObject);
                //Remove Icon
                StartCoroutine(SortBar());
            }

            //Remember to clear the icon if stack is 0.
        }
    }

    private void InstantiateIcon(BattleCondition buff)
    {
        var sprite = buff.GetIcon();
        GameObject icon = 
            Instantiate(buffIconPrefab, 
                transform.position,
                Quaternion.identity, transform);
        icon.GetComponent<RectTransform>().localPosition =
            new Vector3(startPosition + conditionIDList.Count * distanceBetweenBuff, 0);
        
        Image iconImg = icon.transform.Find("Icon").GetComponent<Image>();
        iconImg.sprite = sprite;

        icon.GetComponent<ConditionIcon>().order = conditionIDList.Count;
        icon.GetComponent<ConditionIcon>().conditionInfo = buff;
        icon.GetComponent<ConditionIcon>()._statusManager = targetStat;
        
        conditionIDList.Add(buff.buffID);
        
    }
    private void RemoveIcon(BattleCondition buff)
    {
        int index = conditionIDList.IndexOf(buff.buffID);
        if (index >= 0)
        {
            
        }

    }

    private ConditionIcon FindBuffInBar(int buffID)
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

    IEnumerator SortBar()
    {
        yield return null;
        for (int i = 0; i < transform.childCount; i++)
        {
            
            var child = transform.GetChild(i).GetComponent<ConditionIcon>();
            print(child.order);
            if (child.order != i)
            {
                //print(child.order);
                child.order = i;
                child.GetComponent<RectTransform>().localPosition =
                    new Vector3(startPosition + (child.order) * distanceBetweenBuff, 0);
            }
            
        }
    }


}