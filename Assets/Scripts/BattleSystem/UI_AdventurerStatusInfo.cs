using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_AdventurerStatusInfo : MonoBehaviour
{
    private PlayerStatusManager statusManager;

    private Coroutine HurtRoutine = null;
    private Slider _slider;
    private Image HPBarImage;
    private float currentHP;
    private float maxHP;
    [SerializeField] private Color HPover50Color;
    [SerializeField] private Color HPover30Color;
    [SerializeField] private Color HPbelow30Color;
    
    
    [SerializeField] private Image statusImage;

    [SerializeField] private TextMeshProUGUI HPText;

    [SerializeField] private Sprite HurtImageSprite;

    [SerializeField] private Sprite normalImageSprite;

    // Start is called before the first frame update
    void Start()
    {
        statusManager = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        _slider = GetComponentInChildren<Slider>();
        statusImage = transform.Find("CharacterIcon").GetComponent<Image>();
        HPText = GetComponentInChildren<TextMeshProUGUI>();
        HPBarImage = transform.Find("HPbar").Find("Fill").GetComponent<Image>();
        currentHP = statusManager.currentHp;
        maxHP = statusManager.maxHP;
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
        if (_slider.value >= 0.5)
        {
            HPBarImage.color = HPover50Color;
        }else if (_slider.value < 0.3)
        {
            HPBarImage.color = HPbelow30Color;
        }
        else
        {
            HPBarImage.color = HPover30Color;
        }
    }

    public void OnHPChange()
    {
        
        if (currentHP > statusManager.currentHp)
        {
            if (HurtRoutine == null)
            {
                HurtRoutine = StartCoroutine("ChangeImage");
            }
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
