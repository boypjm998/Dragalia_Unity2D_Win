using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleInfoCaster : MonoBehaviour
{
    private GameObject BossSkillBanner;

    private TextMeshProUGUI _text;
    private Image _banner;
    private Tweener _tweenerText;
    private Tweener _tweenerImage;
    [SerializeField] private bool test;
    private Color tweenColor;

    private GameObject DialogDisplayer;
    private Coroutine displayRoutine;

    private void Awake()
    {
        BossSkillBanner = transform.Find("BossSkillBanner").gameObject;
        DialogDisplayer = transform.Find("DialogDisplayer").gameObject;
        _text = BossSkillBanner.GetComponentInChildren<TextMeshProUGUI>();
        _banner = BossSkillBanner.GetComponentInChildren<Image>();
        //_banner.gameObject.SetActive(false);
        //_text.gameObject.SetActive(false);
        _tweenerImage = null;
        _tweenerText = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintSkillName_ZH(string str)
    {
        

        if (displayRoutine != null)
        {
            StopCoroutine(displayRoutine);
        }
        _text.text = str;
        displayRoutine = StartCoroutine(SkillBannerAnimation());

    }

    

    private IEnumerator SkillBannerAnimation()
    {
        //_banner.gameObject.SetActive(true);
        //_text.gameObject.SetActive(true);
       
        
        var alpha = 0f;
        

        _banner.color = new Color(_banner.color.r, _banner.color.g, _banner.color.b, 0);
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0);

        while (alpha<1)
        {
            alpha += .04f;
            _banner.color = new Color(_banner.color.r, _banner.color.g, _banner.color.b, alpha);
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(2f);
        
        while (alpha>0)
        {
            alpha -= .04f;
            _banner.color = new Color(_banner.color.r, _banner.color.g, _banner.color.b, alpha);
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);
            yield return new WaitForSeconds(0.02f);
        }

        //yield return new WaitUntil(() => finishedTween >= 2);
        
        //_banner.gameObject.SetActive(false);
        //_text.gameObject.SetActive(false);
        displayRoutine = null;

    }

    void TweenFinished()
    {
        //finishedTween++;
    }

}
