using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
public class UI_BattleInfoCaster : MonoBehaviour
{
    public static UI_BattleInfoCaster Instance { get; protected set; }

    protected GlobalController _globalController;
    protected GameObject BossSkillBanner;

    protected TextMeshProUGUI _text;
    protected Image _banner;
    protected Tweener _tweenerText;
    protected Tweener _tweenerImage;
    [SerializeField] private bool test;
    protected Color tweenColor;

    protected GameObject DialogDisplayer;
    protected Coroutine displayRoutine;

    protected JsonData BossSkillNameData;
    protected JsonData BossVoiceTextData;

    private void Awake()
    {
        Instance = this;
        BossSkillBanner = transform.GetChild(0).gameObject;
        DialogDisplayer = transform.Find("DialogDisplayer").gameObject;
        _text = BossSkillBanner.GetComponentInChildren<TextMeshProUGUI>();
        _banner = BossSkillBanner.GetComponentInChildren<Image>();
        //_banner.gameObject.SetActive(false);
        //_text.gameObject.SetActive(false);
        _tweenerImage = null;
        _tweenerText = null;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        _globalController = GlobalController.Instance;
        BossSkillNameData = ReadBattleInfoData("LevelInformation/BossSkillInfo.json");
        //gameObject.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintSkillName(string actionName)
    {
        string txt;
        string title;
        //title为actionName第一个_前的字符串
        title = actionName.Substring(0, actionName.IndexOf("_"));
        //print(title);
        
        switch (_globalController.GameLanguage)
        {
            case GlobalController.Language.JP:
            {
                txt = BossSkillNameData[title][actionName]["JP"].ToString();
                PrintSkillName_ZH(txt);
                break;
            }
            case GlobalController.Language.ZHCN:
            {
                txt = BossSkillNameData[title][actionName]["ZHCN"].ToString();
                PrintSkillName_ZH(txt);
                break;
            }
            case GlobalController.Language.EN:
            {
                txt = BossSkillNameData[title][actionName]["EN"].ToString();
                PrintSkillName_ZH(txt);
                break;
            }
                

        }

        
    }

    protected void PrintSkillName_ZH(string str)
    {
        

        if (displayRoutine != null)
        {
            StopCoroutine(displayRoutine);
        }
        _text.text = str;
        displayRoutine = StartCoroutine(SkillBannerAnimation());

    }

    

    protected IEnumerator SkillBannerAnimation()
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
    
    protected JsonData ReadBattleInfoData(string name)
    {
        string path = Application.streamingAssetsPath + "/"+ name;
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        return JsonMapper.ToObject(str);
    }

}
