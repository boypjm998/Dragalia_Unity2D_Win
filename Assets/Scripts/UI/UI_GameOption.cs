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

    private bool isSettingKey = false;

    private void Awake()
    {
        keySettingButton = new Button[11];
        keyText = new TextMeshProUGUI[11];

        var keyBoardSetting = transform.Find("KeyboardSettings");
        int i = 0;
        foreach (Transform item in keyBoardSetting)
        {
            keySettingButton[i] = item.Find("Button").GetComponent<Button>();
            keyText[i] = item.Find("Key").GetComponent<TextMeshProUGUI>();
            i++;
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnEnable()
    {
        keyText[0].text = GlobalController.keyLeft.ToUpper();
        keyText[1].text = GlobalController.keyRight.ToUpper();
        keyText[2].text = GlobalController.keyDown.ToUpper();
        keyText[3].text = GlobalController.keyAttack.ToUpper();
        keyText[4].text = GlobalController.keyJump.ToUpper();
        keyText[5].text = GlobalController.keyRoll.ToUpper();
        keyText[6].text = GlobalController.keySpecial.ToUpper();
        keyText[7].text = GlobalController.keySkill1.ToUpper();
        keyText[8].text = GlobalController.keySkill2.ToUpper();
        keyText[9].text = GlobalController.keySkill3.ToUpper();
        keyText[10].text = GlobalController.keySkill4.ToUpper();
        
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


    IEnumerator SetKeyRoutine(int keyID)
    {
        isSettingKey = true;
        
        for (int i = 0; i < 11; i++)
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
            if (Input.GetKeyDown(key) && (int)key > 27 && (int)key < 315)
            {
                newKey = key.ToString();
            }
        }

        if (newKey == "")
        {
            newKey = keyText[keyID].text;
        }

        keyText[keyID].text = newKey.ToUpper();

        for (int i = 0; i < 11; i++)
        {
            if (keyID != i && keyText[i].text == newKey.ToUpper())
            {
                keyText[i].text = string.Empty;
            }
        }

        for (int i = 0; i < 11; i++)
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
        GlobalController.keyLeft = keyText[0].text.ToLower();
        GlobalController.keyRight = keyText[1].text.ToLower();
        GlobalController.keyDown = keyText[2].text.ToLower();
        GlobalController.keyAttack = keyText[3].text.ToLower();
        GlobalController.keyJump = keyText[4].text.ToLower();
        GlobalController.keyRoll = keyText[5].text.ToLower();
        GlobalController.keySpecial = keyText[6].text.ToLower();
        GlobalController.keySkill1 = keyText[7].text.ToLower();
        GlobalController.keySkill2 = keyText[8].text.ToLower();
        GlobalController.keySkill3 = keyText[9].text.ToLower();
        GlobalController.keySkill4 = keyText[10].text.ToLower();
    }


}
