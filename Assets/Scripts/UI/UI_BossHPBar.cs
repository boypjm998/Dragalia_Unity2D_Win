using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHPBar : MonoBehaviour
{
    private Slider redGauge;

    private Slider yellowGauge;

    public StatusManager bossStat;
    
    private Coroutine HPChangeRoutine = null;

    protected float currentHP = 0;
    
    protected float maxHP = 0;

    [SerializeField] private Color FullHPColor;
    [SerializeField] private Color NormalHPColor;

    private Image HPBarImage;


    private void Awake()
    {
        yellowGauge = transform.Find("GaugeBack").GetComponent<Slider>();
        //yellowGauge.value = 1;
        redGauge = transform.Find("GaugeFront").GetComponent<Slider>();
        //redGauge.value = 1;
        HPBarImage = redGauge.GetComponentInChildren<Image>();
        HPBarImage.color = FullHPColor;
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHP > 0)
        {
            ExcuteHPBar();
        }
    }

    void ExcuteHPBar()
    {
        if (currentHP != bossStat.currentHp || maxHP != bossStat.maxHP)
        {
            OnHPChange();
            currentHP = bossStat.currentHp;
            maxHP = bossStat.maxHP;
        }
    }

    public void SetTarget(StatusManager target)
    {
        bossStat = target;
        currentHP = bossStat.currentHp;
        maxHP = bossStat.maxHP;
    }
    
    public void OnHPChange()
    {
        if (bossStat.currentHp > bossStat.maxHP)
        {
            bossStat.currentHp = bossStat.maxHP;
        }

        if ((int)currentHP > bossStat.currentHp)
        {
            if (HPChangeRoutine != null)
            {
                //StopCoroutine(HPChangeRoutine);
                //HPChangeRoutine = null;
            }
            else
            {
                HPChangeRoutine = StartCoroutine("HPFillAnimation");
            }

            
        }
        else
        {
            currentHP = bossStat.currentHp;
        }

        GetHPValue();
    }
    
    private void GetHPValue()
    {
        redGauge.value = (float)bossStat.currentHp / (float)(bossStat.maxHP);
        
        if (bossStat.currentHp >= bossStat.maxHP)
        {
            HPBarImage.color = FullHPColor;
        }
        else
        {
            HPBarImage.color = NormalHPColor;
        }
    }

    IEnumerator HPFillAnimation()
    {
        var distance = redGauge.value - yellowGauge.value;
        if (distance < 0.01f)
        {
            yield return new WaitForSeconds(0.3f);
            yellowGauge.value -= 0.02f;
        }

        while (distance < 0.01f)
        {
            yellowGauge.value -= 0.01f;
            yield return null;
            distance = redGauge.value - yellowGauge.value;
        }

        yellowGauge.value = redGauge.value;
        if(redGauge.value <= 0)
            yellowGauge.value = 0;
        HPChangeRoutine = null;

    }

}
