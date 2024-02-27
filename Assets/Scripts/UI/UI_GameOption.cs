using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class UI_GameOption : MonoBehaviour
{
    private Button[] keySettingButton;
    private TextMeshProUGUI[] keyText;
    private CanvasGroup keySettingCanvasGroup;
    
    public InputActionAssetData inputActionAssetData;


    private Button[] gamepadSettingButton;
    private TextMeshProUGUI[] gamepadText;
    private CanvasGroup gamepadSettingCanvasGroup;

    public InputActionReference m_inputActionReference;
    private InputActionRebindingExtensions.RebindingOperation m_rebindingOperation;
    

    // private string defaultKeyLeft = "a";
    // private string defaultKeyRight = "d";
    // private string defaultKeyDown = "s";
    // private string defaultKeySpecial = "w";
    // private string defaultKeyAttack = "j";
    // private string defaultKeyJump = "k";
    // private string defaultKeyRoll = "l";
    // private string defaultKeySkill1 = "u";
    // private string defaultKeySkill2 = "i";
    // private string defaultKeySkill3 = "o";
    // private string defaultKeySkill4 = "h";
    // private string defaultKeyEscape = "escape";

    public static bool isSettingKey = false;
    
    [SerializeField] private TextMeshProUGUI notFoundGamepadText;

    [SerializeField] private Toggle fullScrrenToggle;

    [SerializeField] private Toggle[] fontSizeToggles;
    [SerializeField] private Toggle[] musicToggles;
    [SerializeField] private Toggle[] voiceToggles;
    [SerializeField] private Toggle[] soundEffectToggles;
    [SerializeField] private Toggle[] controlTypeSwitchToggles;
    [SerializeField] private Toggle[] skillhintSwitchToggles;
    [SerializeField] private Toggle[] gamepadSensitivityToggles;





    private void Awake()
    {
        keySettingButton = new Button[12];
        keyText = new TextMeshProUGUI[12];

        var keyBoardSetting = transform.Find("KeyboardSettings");
        keySettingCanvasGroup = keyBoardSetting.GetComponent<CanvasGroup>();
        int i = 0;
        foreach (Transform item in keyBoardSetting)
        {
            keySettingButton[i] = item.Find("Button").GetComponent<Button>();
            keyText[i] = item.Find("Key").GetComponent<TextMeshProUGUI>();
            i++;
        }


        var gamePadSetting = transform.Find("GamepadSettings");
        gamepadSettingCanvasGroup = gamePadSetting.GetComponent<CanvasGroup>();
        gamepadSettingButton = new Button[12];
        gamepadText = new TextMeshProUGUI[12];
        i = 0;
        foreach (Transform item in gamePadSetting)
        {
            gamepadSettingButton[i] = item.Find("Button").GetComponent<Button>();
            gamepadText[i] = item.Find("Key").GetComponent<TextMeshProUGUI>();
            i++;
        }

    }
    
    

    private void OnDisable()
    {
        GlobalController.Instance.WriteGameOptionToFile();
        StopAllCoroutines();
    }

    void OnEnable()
    {
        DisplayKeySettingText();
        DisplayGamepadSettingText();
        ReloadDamageFontSettings();
        ReloadSoundSettings();
        ReloadGamepadSettings();
        //TestLoadInputActionAssetData();
        
        //ReloadGamepadSettings();

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
    private void Update()
    {
        if (Gamepad.current == null)
        {
            //print("No Gamepad Found");
            controlTypeSwitchToggles[0].isOn = true;
            skillhintSwitchToggles[1].isOn = true;
            notFoundGamepadText.enabled = true;
        }
        else notFoundGamepadText.enabled = false;

    }


    public void SettingKey(int keyID)
    {
        if (!isSettingKey)
            StartCoroutine(SetKeyRoutine(keyID));
    }

    public void SettingGamepad(int keyID)
    {
        if (!isSettingKey)
            StartCoroutine(SetGamepadRoutine(keyID));
    }

    public void SettingToggle()
    {
        if (!isSettingKey)
            SettingFullScreenRoutine();

    }

    void SettingFullScreenRoutine()
    {
        if (isSettingKey)
            return;


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


    private void ResetPanels()
    {
        for (int i = 0; i < 12; i++)
        {
            
            keySettingButton[i].image.color = Color.white;
            keyText[i].color = Color.black;
            keySettingButton[i].interactable = true;

        }
        for (int i = 0; i < 12; i++)
        {
            
            gamepadSettingButton[i].image.color = Color.white;
            gamepadText[i].color = Color.black;
            gamepadSettingButton[i].interactable = true;

        }
    }
    IEnumerator SetKeyRoutine(int keyID)
    {
        isSettingKey = true;

        for (int i = 0; i < 12; i++)
        {

            keySettingButton[i].interactable = false;


            if (i == keyID)
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
            if (Input.GetKeyDown(key) &&
                (((int)key > 27 && (int)key < 315) || key == KeyCode.Mouse0 || key == KeyCode.Mouse1) || key == KeyCode.Escape)
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

    // private void TestRebind()
    // {
    //     m_rebindingOperation = m_inputActionReference.action.PerformInteractiveRebinding()
    //         .WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f).
    //         OnComplete(operation => m_rebindingOperation?.Dispose());
    // }

    IEnumerator SetGamepadRoutine(int keyID)
    {
        GlobalController.Instance.inputActionAsset.Disable();
        var actionMap = GlobalController.gamepadMap;
        
        
        var actions = actionMap.actions;

        var moveL = actions[0].bindings[1];
        var moveR = actions[0].bindings[2];
        var moveD = actions[1].bindings[0];
        var attack = actions[2].bindings[0];
        var jump = actions[3].bindings[0];
        var roll = actions[4].bindings[0];
        var special = actions[5].bindings[0];
        var skill1 = actions[6].bindings[0];
        var skill2 = actions[7].bindings[0];
        var skill3 = actions[8].bindings[0];
        var skill4 = actions[9].bindings[0];
        var escape = actions[10].bindings[0];

        var actionBindings = new List<InputBinding>()
        {
            moveL, moveR, moveD, attack, jump, roll, special, skill1, skill2, skill3, skill4, escape
        };
        
        isSettingKey = true;
        string newPath = "";
        
        for (int i = 0; i < 12; i++)
        {

            gamepadSettingButton[i].interactable = false;


            if (i == keyID)
            {
                gamepadSettingButton[i].interactable = true;
                gamepadText[i].color = Color.white;
                gamepadSettingButton[i].image.color = Color.red;
            }

        }
        
        string newJsonData;
        
        while (true)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is ButtonControl button && button.isPressed)
                {
                    print(button.name);
                    print(button.path);
                    newPath = ConvertToGamepadPath(button);
                    print(newPath);
                    break;
                }
            }

            
            
            if (!string.IsNullOrEmpty(newPath) && !IsKeyUsed(actionMap, newPath))
            {
                // int actionIndex = -1;
                // for (int i = 0; i < actions.Count; i++)
                // {
                //     if (actions[i].bindings.Contains(actionBindings[keyID]))
                //     {
                //         actionIndex = i;
                //         break;
                //     }
                // }
                // if (actionIndex != -1)
                // {
                //     int bindingIndex = -1;
                //     for (int i = 0; i < actions[actionIndex].bindings.Count; i++)
                //     {
                //         if (actions[actionIndex].bindings[i].Equals(actionBindings[keyID]))
                //         {
                //             bindingIndex = i;
                //             break;
                //         }
                //     }
                //     if (bindingIndex != -1)
                //     {
                //         var newBinding = new InputBinding();
                //         newBinding.path = newPath;
                //
                //         // 在指定的动作和键位绑定索引处应用新的键位绑定
                //         actions[actionIndex].ApplyBindingOverride(bindingIndex, newBinding);
                //
                //         // 将新的键位绑定保存到actionBindings列表中，以便之后应用到GlobalController.gamepadMap
                //         actionBindings[keyID] = newBinding;
                //
                //         // 应用新的键位绑定到GlobalController.gamepadMap
                //         actionMap.ApplyBindingOverrides(actionBindings);
                //
                //         GlobalController.gamepadMap = actionMap;
                //         GlobalController.Instance.inputActionAsset.Enable();
                //         
                //         // 打印新的键位绑定的路径，以验证键位绑定已经被成功修改
                //         print(newBinding.path);
                //         break;
                //     }
                // }
                newJsonData = TestLoadInputActionAssetData(keyID,newPath);
                break;
                
            }

            yield return null;
        }

        
        yield return null;

        //GlobalController.Instance.inputActionAsset = null;
        ReloadInputActionAssetData(newJsonData);
        
        
        for (int i = 0; i < 12; i++)
        {
            if (keyID == i)
            {
                gamepadSettingButton[i].image.color = Color.white;
                gamepadText[i].color = Color.black;
            }

            gamepadSettingButton[i].interactable = true;

        }
        
        ReloadGamepadInfo(actionMap);
        isSettingKey = false;
    }
    
    private bool IsKeyUsed(InputActionMap actionMap, string newKeyPath)
    {
        var allControls = Gamepad.current.allControls;
        bool isGamepadControl = allControls.Any(control => control.path == newKeyPath);

        if (!isGamepadControl)
            return false;
        
        
        
        foreach (var action in actionMap.actions)
        {
            foreach (var binding in action.bindings)
            {
                if (binding.path == newKeyPath)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ReloadGamepadInfo(InputActionMap map)
    {
        GlobalController.gamepadMap = map;
        //print(map.bindings[^1].path);
        DisplayGamepadSettingText();
    }

    private void ReloadKeySetting()
    {
        


        GlobalController.keyLeft = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[0].text==""? "None":keyText[0].text);
        GlobalController.keyRight = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[1].text==""? "None":keyText[1].text);
        GlobalController.keyDown = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[2].text==""? "None":keyText[2].text);
        GlobalController.keyAttack = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[3].text==""? "None":keyText[3].text);
        GlobalController.keyJump = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[4].text==""? "None":keyText[4].text);
        GlobalController.keyRoll = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[5].text==""? "None":keyText[5].text);
        GlobalController.keySpecial = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[6].text==""? "None":keyText[6].text);
        GlobalController.keySkill1 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[7].text==""? "None":keyText[7].text);
        GlobalController.keySkill2 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[8].text==""? "None":keyText[8].text);
        GlobalController.keySkill3 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[9].text==""? "None":keyText[9].text);
        GlobalController.keySkill4 = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[10].text==""? "None":keyText[10].text);
        GlobalController.keyEscape = (KeyCode)Enum.Parse(typeof(KeyCode),keyText[11].text==""? "None":keyText[11].text);
        
        
        GlobalController.Instance.WritePlayerSettingsToFile();
        
    }

    public void SetSmallFontSize(bool flag)
    {
        if(isSettingKey)
            return;
        
        
        if (flag)
        {
            GlobalController.Instance.ChangeFontSizeOfDamageNum(1);
            //GlobalController.Instance.WriteGameOptionToFile();
        }
        //GlobalController.Instance.WriteGameOptionToFile();
    }
    
    public void SetMidFontSize(bool flag)
    {
        if(isSettingKey)
            return;
        
        if (flag)
        {
            GlobalController.Instance.ChangeFontSizeOfDamageNum(2);
            //GlobalController.Instance.WriteGameOptionToFile();
        }
        //GlobalController.Instance.WriteGameOptionToFile();
    }
    
    public void SetBigFontSize(bool flag)
    {
        if(isSettingKey)
            return;
        
        if (flag)
        {
            GlobalController.Instance.ChangeFontSizeOfDamageNum(3);
            //GlobalController.Instance.WriteGameOptionToFile();
        }
        
    }

    public void SetBGM(bool flag)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeSoundMute(0,!flag);
    }
    public void SetVoice(bool flag)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeSoundMute(1,!flag);
    }
    public void SetSE(bool flag)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeSoundMute(2,!flag);
    }

    public void SetLayoutHint(bool keyboard)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeLayoutHintKeyboardOrGamepad(keyboard);
        //ReloadGamepadSettings();
    }
    
    public void SetGamepadSensitivityLow(bool flag)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeGamepadSensitivity(1);
        
    }
    
    public void SetGamepadSensitivityMid(bool flag)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeGamepadSensitivity(2);
        
    }

    public void SetGamepadSensitivityHigh(bool flag)
    {
        if(isSettingKey)
            return;
        
        GlobalController.Instance.ChangeGamepadSensitivity(3);
        
    }

    public void DisplayGamepadControlPanel(bool flag)
    {
        if(isSettingKey)
            return;
        
        
         string[] joystickNames = Input.GetJoystickNames();
        
         if (Gamepad.current == null)
         {
             //todo: 提示没有手柄
             
             return;
         }
        
        if (flag)
        {
            keySettingCanvasGroup.alpha = 0;
            keySettingCanvasGroup.interactable = false;
            keySettingCanvasGroup.blocksRaycasts = false;
            
            gamepadSettingCanvasGroup.alpha = 1;
            gamepadSettingCanvasGroup.interactable = true;
            gamepadSettingCanvasGroup.blocksRaycasts = true;

            
        }
    }
    
    public void DisplayKeyControlPanel(bool flag)
    {
        if(isSettingKey)
            return;
        
        if (flag)
        {
            keySettingCanvasGroup.alpha = 1;
            keySettingCanvasGroup.interactable = true;
            keySettingCanvasGroup.blocksRaycasts = true;
            
            gamepadSettingCanvasGroup.alpha = 0;
            gamepadSettingCanvasGroup.interactable = false;
            gamepadSettingCanvasGroup.blocksRaycasts = false;
        }
    }

    private void DisplayKeySettingText()
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
    }

    private void DisplayGamepadSettingText()
    {
        //var  = GlobalController.gamepadMap;
        InputActionMap actionMap = new InputActionMap();
        actionMap = GlobalController.Instance.inputActionAsset.FindActionMap("Player");
        // print(GlobalController.Instance.
        //     inputActionAsset.FindActionMap("Player").
        //     actions[10].bindings[0].path);
        // string[] actionNames = new string[]
        // {
        //     "Move","Move","Down","Attack","Jump",
        //     "Roll","Special",
        //     "Skill1","Skill2","Skill3","Skill4",
        //     "Escape"
        // };
        var actions = actionMap.actions;

        var moveL = actions[0].bindings[1];
        var moveR = actions[0].bindings[2];
        var moveD = actions[1].bindings[0];
        var attack = actions[2].bindings[0];
        var jump = actions[3].bindings[0];
        var roll = actions[4].bindings[0];
        var special = actions[5].bindings[0];
        var skill1 = actions[6].bindings[0];
        var skill2 = actions[7].bindings[0];
        var skill3 = actions[8].bindings[0];
        var skill4 = actions[9].bindings[0];
        var escape = actions[10].bindings[0];
        
        
        gamepadText[0].text = SimplifyInputActionName(moveL.path);
        gamepadText[1].text = SimplifyInputActionName(moveR.path);
        gamepadText[2].text = SimplifyInputActionName(moveD.path);
        gamepadText[3].text = SimplifyInputActionName(attack.path);
        gamepadText[4].text = SimplifyInputActionName(jump.path);
        gamepadText[5].text = SimplifyInputActionName(roll.path);
        gamepadText[6].text = SimplifyInputActionName(special.path);
        gamepadText[7].text = SimplifyInputActionName(skill1.path);
        gamepadText[8].text = SimplifyInputActionName(skill2.path);
        gamepadText[9].text = SimplifyInputActionName(skill3.path);
        gamepadText[10].text = SimplifyInputActionName(skill4.path);
        gamepadText[11].text = SimplifyInputActionName(escape.path);

        

    }

    private void ReloadDamageFontSettings()
    {
        
        if(GlobalController.Instance.gameOptions.damage_font_size == 1)
        {
            print("size:1");
            fontSizeToggles[0].isOn = true;
            //fontSizeToggles[0].onValueChanged.Invoke(true);
        }
        else if(GlobalController.Instance.gameOptions.damage_font_size == 2)
        {
            print("size:2");
            fontSizeToggles[1].isOn = true;
            //fontSizeToggles[0].onValueChanged.Invoke(true);
        }
        else if(GlobalController.Instance.gameOptions.damage_font_size == 3)
        {
            print("size:3");
            fontSizeToggles[2].isOn = true;
            //fontSizeToggles[0].onValueChanged.Invoke(true);
        }

    }

    private void ReloadSoundSettings()
    {
        var bgmToggle = GlobalController.Instance.gameOptions.soundSettings[0];
        var voiceToggle = GlobalController.Instance.gameOptions.soundSettings[1];
        var seToggle = GlobalController.Instance.gameOptions.soundSettings[2];
        
        musicToggles[bgmToggle].isOn = true;
        voiceToggles[voiceToggle].isOn = true;
        soundEffectToggles[seToggle].isOn = true;

    }

    private void ReloadGamepadSettings()
    {
        var layoutToggle = GlobalController.Instance.gameOptions.gamepadSettings[0];
        
        skillhintSwitchToggles[layoutToggle].isOn = true;
        
        
        
        if(GlobalController.Instance.gameOptions.gamepadSettings[1] == 1)
        {
            gamepadSensitivityToggles[0].isOn = true;
            //fontSizeToggles[0].onValueChanged.Invoke(true);
        }
        else if(GlobalController.Instance.gameOptions.gamepadSettings[1] == 2)
        {
            gamepadSensitivityToggles[1].isOn = true;
            //fontSizeToggles[0].onValueChanged.Invoke(true);
        }
        else if(GlobalController.Instance.gameOptions.gamepadSettings[1] == 3)
        {
            gamepadSensitivityToggles[2].isOn = true;
            //fontSizeToggles[0].onValueChanged.Invoke(true);
        }

        if (Gamepad.current == null)
        {
            SetLayoutHint(true);
            DisplayKeyControlPanel(true);
            notFoundGamepadText.enabled = true;
        }
        else notFoundGamepadText.enabled = false;
        
    }

    

    public static string SimplifyInputActionName(string bindingPath)
    {
        var currentGamepad = Gamepad.current;
        
        if (currentGamepad != null)
        {
            string prefix = "<Gamepad>";
            if (currentGamepad is XInputController)
            {
                prefix = "<XInputController>"; //Xbox
            }else if (currentGamepad is DualShockGamepad)
            {
                prefix = "<DualShockGamepad>"; //PS
            }else if (currentGamepad is SwitchProControllerHID)
            {
                prefix = "<SwitchProControllerHID>"; //Switch
            }
            //将prefix替换掉<Gamepad>
            bindingPath = bindingPath.Replace("<Gamepad>", prefix);
        }
        
        //把humanReadableString中替换成只有大写字母和符号
        var humanReaable = InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.UseShortNames);
        print(humanReaable);
        //找到]的位置
        var index = humanReaable.IndexOf("[");
        
        //返回值只有]后面的字符串
        return humanReaable.Substring(0,index-1);

        //return bindingPath.Replace("[Gamepad]", "");
        
        
        
    }
    

    private string TestLoadInputActionAssetData(int keyID, string newpath)
    {
        var path = Application.streamingAssetsPath + "/savedata/GamepadSettings.json";
        StreamReader sr = new StreamReader(path);
        var str = sr.ReadToEnd();
        sr.Close();
        
        inputActionAssetData = JsonUtility.
            FromJson<InputActionAssetData>(str);
        
        //newpath = InputControlPath.
        
        inputActionAssetData.maps[0].bindings[keyID+1].path = newpath;
        
        var outputPath = Application.streamingAssetsPath + "/savedata/GamepadSettings.json";
        StreamWriter sw = new StreamWriter(outputPath);
        var newStr = JsonUtility.ToJson(inputActionAssetData,true);
        sw.Write(newStr);
        
        sw.Close();

        return newStr;

    }

    private void ReloadInputActionAssetData(string newStr)
    {
        print(newStr);
        
        GlobalController.Instance.inputActionAsset.Disable();

        GlobalController.Instance.inputActionAsset = null;
        
        GlobalController.Instance.inputActionAsset = 
            ScriptableObject.CreateInstance<InputActionAsset>();
        
        GlobalController.Instance.inputActionAsset.LoadFromJson(newStr);
        
        
        GlobalController.gamepadMap?.Dispose();
        GlobalController.gamepadMap = null;
        


        GlobalController.gamepadMap = 
            GlobalController.Instance.inputActionAsset.FindActionMap("Player");
        //GlobalController.gamepadMap.Enable();
        //GlobalController.Instance.inputActionAsset.Enable();
        //GlobalController.gamepadMap.Enable();
        DisplayGamepadSettingText();
    }

    private string ConvertToGamepadPath(ButtonControl btn)
    {
        string path = btn.name;
        path = char.ToLowerInvariant(path[0]) + path.Substring(1);

        string effectivePath =
            btn.path.Substring(btn.path.IndexOf('/', 1));
        
        //print(effectivePath);
        
        switch (path)
        {
            case "X":
                return "<Gamepad>/buttonWest";
            case "Y":
                return "<Gamepad>/buttonNorth";
            case "A":
                return "<Gamepad>/buttonSouth";
            case "B":
                return "<Gamepad>/buttonEast";
            
            case "triangle":
                return "<Gamepad>/buttonNorth";
            case "circle":
                return "<Gamepad>/buttonEast";
            case "cross":
                return "<Gamepad>/buttonSouth";
            case "square":
                return "<Gamepad>/buttonWest";

        }

        return "<Gamepad>" + effectivePath;


    }

}

[Serializable]
public class InputActionAssetData
{
    public string name;
    public ActionMapData[] maps;
    public ControlSchemeData[] controlSchemes;

    [Serializable]
    public class ActionMapData
    {
        public string name;
        public string id;
        public List<ActionData> actions;
        public List<BindingData> bindings;
        
        [Serializable]
        public class ActionData
        {
            public string name;
            public string type;
            public string id;
            public string expectedControlType;
            public string processors;
            public string interactions;
            public bool initialStateCheck;

        }

        [Serializable]
        public class BindingData
        {
            public string name;
            public string id;
            public string path;
            public string interactions;
            public string processors;
            public string groups;
            public string action;
            public bool isComposite;
            public bool isPartOfComposite;
        }
    }
    
        
    [Serializable]
    public class ControlSchemeData
    {
        public string name;
        public string bindingGroup;
        
        [Serializable]
        public class DeviceData
        {
            public string devicePath;
            public bool isOptional;
            public bool isOR;
        }
        
    }

}
