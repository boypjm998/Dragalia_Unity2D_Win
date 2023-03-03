using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class UI_AdventurerStatusInfo : MonoBehaviour
{
    private StatusManager statusManager;
    

    protected Coroutine HurtRoutine = null;
    protected Slider _slider;
    private Image HPBarImage;
    protected float currentHP;
    protected float maxHP;
    protected UI_FullScreenEffect fullScreenEffect;
    
    
    [SerializeField] private Color HPFullColor;
    [SerializeField] private Color HPOver30Color;
    [SerializeField] private Color HPBelow30Color;
    
    
    [SerializeField] private Image statusImage;

    [SerializeField] private TextMeshProUGUI HPText;

    [SerializeField] private Sprite HurtImageSprite;

    [SerializeField] private Sprite normalImageSprite;

    // Start is called before the first frame update
    void Start()
    {
        fullScreenEffect = GameObject.Find("UI").transform.Find("FullScreenEffect").
            GetChild(0).GetComponent<UI_FullScreenEffect>();
        var characterID = GlobalController.currentCharacterID;
        statusManager = GameObject.Find("PlayerHandle").GetComponent<StatusManager>();
        _slider = GetComponentInChildren<Slider>();
        statusImage = transform.Find("CharacterIcon").GetComponent<Image>();
        HPText = GetComponentInChildren<TextMeshProUGUI>();
        HPBarImage = transform.Find("HPbar").Find("Fill").GetComponent<Image>();
        currentHP = statusManager.currentHp;
        maxHP = statusManager.maxHP;
        GetHPValue();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP != statusManager.currentHp || maxHP != statusManager.maxHP)
        {
            OnHPChange();
            currentHP = statusManager.currentHp;
            maxHP = statusManager.maxHP;
        }
    }

    private void GetHPValue()
    {
        _slider.value = (float)statusManager.currentHp / (float)(statusManager.maxHP);
        HPText.text = statusManager.currentHp.ToString();
        if (_slider.value >= 1)
        {
            HPBarImage.color = HPFullColor;
            fullScreenEffect.Disable();
        }else if (_slider.value < 0.3)
        {
            HPBarImage.color = HPBelow30Color;
            fullScreenEffect.Enable();
        }
        else
        {
            fullScreenEffect.Disable();
            HPBarImage.color = HPOver30Color;
        }
    }

    public void OnHPChange()
    {
        if (statusManager.currentHp > statusManager.maxHP)
        {
            statusManager.currentHp = statusManager.maxHP;
        }

        if ((int)currentHP > statusManager.currentHp)
        {
            if (HurtRoutine == null)
            {
                HurtRoutine = StartCoroutine("ChangeImage");
            }
        }
        else
        {
            currentHP = statusManager.currentHp;
        }

        GetHPValue();
    }

    private IEnumerator ChangeImage()
    {
        statusImage.sprite = HurtImageSprite;
        yield return new WaitForSeconds(0.5f);
        statusImage.sprite = normalImageSprite;
        HurtRoutine = null;
    }


}
