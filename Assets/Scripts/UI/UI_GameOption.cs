using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOption : MonoBehaviour
{
    private Button[] keySettingButton;
    private TextMeshProUGUI[] keyText;

    private string defaultKeyLeft = "a";
    private string defaultKeyRight = "d";
    private string defaultKeyDown = "s";
    private string defaultKeySpecial = "w";
    private string defaultKeyAttack = "j";
    private string defaultKeyJump = "k";
    private string defaultKeyRoll = "l";
    private string defaultKeySkill1 = "u";
    private string defaultKeySkill2 = "i";
    private string defaultKeySkill3 = "o";
    private string defaultKeySkill4 = "h";
    private string defaultKeyEscape = "escape";

    private bool isSettingKey = false;

    private Toggle fullScrrenToggle;
    
    
    

    private void Awake()
    {
        keySettingButton = new Button[12];
        keyText = new TextMeshProUGUI[12];

        var keyBoardSetting = transform.Find("KeyboardSettings");
        int i = 0;
        foreach (Transform item in keyBoardSetting)
        {
            keySettingButton[i] = item.Find("Button").GetComponent<Button>();
            keyText[i] = item.Find("Key").GetComponent<TextMeshProUGUI>();
            i++;
        }
        
        fullScrrenToggle = transform.Find("FullScreenSetting/Panel/Toggle").GetComponent<Toggle>();

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnEnable()
    {
        keyText[0].text = GlobalController.keyLeft.ToString();
        keyText[1].text = GlobalController.keyRight.ToString();
        keyText[2].text = GlobalController.keyDown.ToString();
        keyText[3].text = GlobalController.keyAttack.ToString();
        keyText[4].text = GlobalController.keyJump.ToString();
        keyText[5].text = GlobalController.keyRoll.ToString();
        keyText[6].text = GlobalController.keySpecial.ToString();
        keyText[7].text = GlobalController.keySkill1.ToString();
        keyText[8].text = GlobalController.keySkill2.ToString();
        keyText[9].text = GlobalController.keySkill3.ToString();
        keyText[10].text = GlobalController.keySkill4.ToString();
        keyText[11].text = GlobalController.keyEscape.ToString();
        
        //检测当前是否全屏
        if (Screen.fullScreen)
        {
            fullScrrenToggle.isOn = true;
        }
        else
        {
            fullScrrenToggle.isOn = false;
        }
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SettingKey(int keyID)
    {
        if(!isSettingKey)
            StartCoroutine(SetKeyRoutine(keyID));
    }

    public void SettingToggle()
    {
        if (!isSettingKey)
           SettingFullScreenRoutine();
        
        

    }

    void SettingFullScreenRoutine()
    {
        isSettingKey = true;
        
        //如果当前为Unity编辑器模式，下述代码跳过
        #if !UNITY_EDITOR

        if (fullScrrenToggle.isOn)
        {
            //全屏
            Screen.fullScreen = true;
        }
        else
        {
            //将屏幕设为全屏
            Screen.fullScreen = false;
        }
        
        #endif
        
        isSettingKey = false;
        

    }


    IEnumerator SetKeyRoutine(int keyID)
    {
        isSettingKey = true;
        
        for (int i = 0; i < 12; i++)
        {
            
            keySettingButton[i].interactable = false;
                
            
            if(i == keyID)
            {
                keySettingButton[i].interactable = true;
                keyText[i].color = Color.white;
                keySettingButton[i].GetComponent<Image>().color = Color.red;
            }
            
        }

        yield return new WaitUntil(() => Input.anyKeyDown);
        

        var newKey = "";

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && ((int)key > 27 && (int)key < 315))
            {
                newKey = key.ToString();
            }
        }

        if (newKey == "")
        {
            newKey = keyText[keyID].text;
        }

        keyText[keyID].text = newKey;

        for (int i = 0; i < 12; i++)
        {
            if (keyID != i && keyText[i].text == newKey)
            {
                keyText[i].text = string.Empty;
            }
        }

        for (int i = 0; i < 12; i++)
        {
            if (keyID == i)
            {
                keySettingButton[i].GetComponent<Image>().color = Color.white;
                keyText[i].color = Color.black;
            }
            keySettingButton[i].interactable = true;

        }

        ReloadKeySetting();
        isSettingKey = false;


    }

    void ReloadKeySetting()
    {
        GlobalController.keyLeft = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[0].text);
        GlobalController.keyRight = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[1].text);
        GlobalController.keyDown = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[2].text);
        GlobalController.keyAttack = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[3].text);
        GlobalController.keyJump = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[4].text);
        GlobalController.keyRoll = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[5].text);
        GlobalController.keySpecial = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[6].text);
        GlobalController.keySkill1 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[7].text);
        GlobalController.keySkill2 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[8].text);
        GlobalController.keySkill3 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[9].text);
        GlobalController.keySkill4 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[10].text);
        GlobalController.keyEscape = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[11].text);

        // GlobalController.keyRight = keyText[1].text.ToLower();
        // GlobalController.keyDown = keyText[2].text.ToLower();
        // GlobalController.keyAttack = keyText[3].text.ToLower();
        // GlobalController.keyJump = keyText[4].text.ToLower();
        // GlobalController.keyRoll = keyText[5].text.ToLower();
        // GlobalController.keySpecial = keyText[6].text.ToLower();
        // GlobalController.keySkill1 = keyText[7].text.ToLower();
        // GlobalController.keySkill2 = keyText[8].text.ToLower();
        // GlobalController.keySkill3 = keyText[9].text.ToLower();
        // GlobalController.keySkill4 = keyText[10].text.ToLower();
        // GlobalController.keyEscape = keyText[11].text.ToLower();
        
        GlobalController.Instance.WritePlayerSettingsToFile();
        
    }


}
