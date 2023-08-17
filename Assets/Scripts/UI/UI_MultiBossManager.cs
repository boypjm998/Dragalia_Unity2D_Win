using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_MultiBossManager : MonoBehaviour
{
    [SerializeField] protected List<UI_BossStatus> _bossStatusList = new();
    public static UI_MultiBossManager Instance { get;protected set; }

    public float bossStatusShiftSensitivity = 3;
    
    protected Coroutine _shiftingBossStatusCoroutine;
    protected float bossStatusShiftTimer;
    
    public GameObject debugBoss2;
    public bool debug;

    private void Awake()
    {
        Instance = this;
        _bossStatusList.Add(GetComponentInChildren<UI_BossStatus>());
        bossStatusShiftTimer = bossStatusShiftSensitivity;
    }

    private void Update()
    {
        if (bossStatusShiftTimer > 0)
        {
            bossStatusShiftTimer -= Time.deltaTime;
        }

        if (debug == true)
        {
            debug = false;
            AddNewBoss(debugBoss2);
        }
    }

    public void AddNewBoss(GameObject bossObject,int bossIndex = 0)
    {
        string languageCheck = "";

        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            languageCheck = "EN";
        }


        var newBossStatusUI = Resources.Load<GameObject>($"StageComponent/BossStatusBar{languageCheck}");
        var newBossStatus = Instantiate(newBossStatusUI, transform);
        newBossStatus.name = "BossStatusBar" + _bossStatusList.Count;
        _bossStatusList.Add(newBossStatus.GetComponent<UI_BossStatus>());
        _bossStatusList[^1].SetBoss(bossObject,bossIndex);

        Invoke("CheckBossNumber", 0.1f);
        

    }

    public void RemoveDeadBoss()
    {
        for(int i = _bossStatusList.Count-1; i >= 0; i--)
        {
            if (_bossStatusList[i].bossStat.currentHp <= 0 && _bossStatusList.Count>1)
            {
                _bossStatusList[i].bossStat.OnTakeDirectDamage = null;
                _bossStatusList[i].bossStat.OnHPBelow0 = null;
                _bossStatusList[i].visible = false;
                _bossStatusList.RemoveAt(i);
                break;
            }
        }
        Invoke("CheckBossNumber", 0.1f);
    }
    
    protected void RemoveBossStatusBar(UI_BossStatus bossStatus)
    {
        _bossStatusList.Remove(bossStatus);
        if(_shiftingBossStatusCoroutine != null)
            StopCoroutine(_shiftingBossStatusCoroutine);
        Destroy(bossStatus.gameObject);
    }

    public void CheckBossNumber()
    {
        if (_bossStatusList.Count > 1)
        {
            for (int i = 0; i < _bossStatusList.Count; i++)
            {
                _bossStatusList[i].visible = false;
                _bossStatusList[i].bossStat.OnTakeDirectDamage += ShiftBossStatus;
                _bossStatusList[i].bossStat.OnHPBelow0 += RemoveDeadBoss;
            }
            
            _bossStatusList[^1].visible = true;
        }
        else
        {
            _bossStatusList[0].visible = true;
            _bossStatusList[0].bossStat.OnTakeDirectDamage = null;
            
        }
    }

    public void ShiftBossStatus(StatusManager statusManager)
    {
        if (_shiftingBossStatusCoroutine != null)
        {
            StopCoroutine(_shiftingBossStatusCoroutine);
        }
        _shiftingBossStatusCoroutine = StartCoroutine(WaitForShiftingBossStatus(statusManager));
    }

    protected IEnumerator WaitForShiftingBossStatus(StatusManager statusManager)
    {
        print(statusManager.gameObject.name);
        yield return new WaitForSeconds(bossStatusShiftSensitivity);

        if (_bossStatusList.Count <= 1)
        {
            _shiftingBossStatusCoroutine = null;
            bossStatusShiftTimer = bossStatusShiftSensitivity;
            yield break;
        }
        
        if(statusManager == null)
        {
            _shiftingBossStatusCoroutine = null;
            bossStatusShiftTimer = bossStatusShiftSensitivity;
            yield break;
        }


        foreach (var bossStatus in _bossStatusList)
        {
            if(statusManager.gameObject.GetInstanceID() == bossStatus.bossStat.gameObject.GetInstanceID())
                bossStatus.visible = true;
            else
                bossStatus.visible = false;
        }
        _shiftingBossStatusCoroutine = null;
        bossStatusShiftTimer = bossStatusShiftSensitivity;

    }


}
