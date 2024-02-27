using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using LitJson;
using TMPro;
using Unity.Profiling;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StorySceneManager : MonoBehaviour
{
    GlobalController _globalController;
    [SerializeField] TextAsset storyData;
    [SerializeField] private TextAsset prologueStoryData;
    [SerializeField] private Queue<StoryCommand> _storyCommands;
    private StoryComponent.DialogMain.Portrait currentSpeakingChara;
    private bool characterIsSpeaking = false;
    private Coroutine currentCoroutine;
    private bool taskRunning = false;
    private bool skip = true;
    private bool moveNext = false;
    public bool autoMode = false;
    public bool paused = false;
    public GameObject skipMenu;
    public float autoNextTime = 1f;
    protected float autoTime = 0;
    public bool started = false;
    
    private string storyStr = "main_story_001";
    public float[] voiceData;


    private string storyJumpInfo;
    private AssetBundle effectsBundle;
    public bool isDebug;
    public string currentStoryID;
    
    // Story Components
    
    public StoryComponent storyComponent = new();
    private StoryBackgroundSprites storyBackgroundSprites;
    private AudioBundlesTest audioBundlesTest;

    [SerializeField] private StoryCommand currentCommand;

    private void LateUpdate()
    {
        if(started == false)
            return;

        moveNext = false;
        try
        {
            if (_globalController.loadingEnd == false || paused)
            {
                if(!isDebug) 
                    return;
            }

           
        }
        catch
        {
            _globalController = GlobalController.Instance;
        }



        if (autoMode)
        {
            print(currentCoroutine == null? "currentCoroutine null":"currentCoroutine not null");
            
            
            if (currentCoroutine == null && characterIsSpeaking == false && !taskRunning &&
                storyComponent.dialog.voiceSource.isPlaying == false)
            {
                autoTime+= Time.deltaTime;
                if (autoTime >= autoNextTime)
                {
                    moveNext = true;
                    autoTime = 0;
                }

                
            }

            
        }
    }

    public void ClickEventInStory()
    {
        //print("clicked");
        try
        {
            if (_globalController.loadingEnd == false || paused)
                return;
        }
        catch
        {
            _globalController = GlobalController.Instance;
        }
        
        
        
        if(currentCoroutine!=null)
            skip = true;
        else
        {
            moveNext = true;
        }
        

        
    }

    private void Awake()
    {
        if (isDebug)
        {
            GlobalController.questID = currentStoryID;
            if (currentStoryID != "100001")
                storyStr = "content";
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {

        

        
        //storyJsonData = storyData.ToString();
        //storyComponent = new StoryComponent();
        storyComponent.Init();
        audioBundlesTest = GetComponent<AudioBundlesTest>();
        storyBackgroundSprites = 
            storyComponent.backGround.gameObject.GetComponent<StoryBackgroundSprites>();
        InitScene();
        
        //TODO: 设置剧情ID
        
        ReadCommands();
        
        yield return new WaitUntil(() => started || isDebug);
        
        _globalController = FindObjectOfType<GlobalController>();
        
        var effectsBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/story/eff");
        effectsBundleRequest.completed += operation =>
        {
            var effectsBundle = effectsBundleRequest.assetBundle;
            _globalController.loadedBundles.Add("story/eff", effectsBundle);
        };
        
        yield return new WaitUntil(()=>effectsBundleRequest.isDone);
        effectsBundle = effectsBundleRequest.assetBundle;
        
        
        StartCoroutine(StartStory());
    }

    

    void InitScene()
    {
        storyComponent.dialog.dialogBody.SetActive(false);
        storyComponent.dialog.blackScreen.gameObject.SetActive(false);
        storyComponent.dialog.blackScreenText.text = "";
        storyComponent.dialog.dialogText.text = "";
        storyComponent.dialog.speakerName.text = "";
        storyComponent.dialog.skipButton.gameObject.SetActive(false);
        storyComponent.dialog.storyRecordButton.gameObject.SetActive(false);
        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            VARIABLE.rootGameObject.SetActive(false);
        }
        storyComponent.backGround.blackOut.gameObject.SetActive(true);
        if(GlobalController.Instance.GetBundle("118effbundle") == null)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/118effbundle");
            bundleRequest.completed += operation =>
            {
                var bundle = bundleRequest.assetBundle;
                GlobalController.Instance.loadedBundles.Add("118effbundle", bundle);
            };
        }
        
    }

    public void SetStoryDataTextAsset(TextAsset textAsset)
    {
        storyData = textAsset;
        storyStr = "content";
    }

    private void ReadCommands()
    {
        _storyCommands = new Queue<StoryCommand>();
        //将stroyData的"main_story_001"读取出来，将其中的"command_list"列表加入到_storyCommands中
        if (GlobalController.questID == "100001")
        {
            storyData = prologueStoryData;
        }
        

        print(storyData.text);
        JsonData storyJsonData = JsonMapper.ToObject(storyData.text);
        JsonData commandList = storyJsonData[storyStr]["command_list"];
        for (int i = 0; i < commandList.Count; i++)
        {
            StoryCommand storyCommand = new StoryCommand();
            storyCommand.commandType = (StoryCommand.CommandType) Enum.Parse(typeof(StoryCommand.CommandType), commandList[i]["command"].ToString());
            storyCommand.args = new List<string>();
            for (int j = 0; j < commandList[i]["args"].Count; j++)
            {
                storyCommand.args.Add(commandList[i]["args"][j].ToString());
            }
            _storyCommands.Enqueue(storyCommand);
        }
        string storyContent = storyJsonData[storyStr]["content"].ToString();
        skipMenu.transform.Find("Borders/Banner2/Text").
            GetComponentInChildren<TextMeshProUGUI>().text = storyContent;

        //todo: 读取剧情跳转信息
        
        storyJumpInfo = (storyJsonData[storyStr]["jump_arg"].ToString());
        var returnMenuButton = skipMenu.transform.Find("ReturnMenuButton").GetComponent<Button>();

        if (storyJumpInfo == "100001")
        {
            returnMenuButton.onClick.AddListener(() =>
            {
                ToBattle();
            });
        }
        else
        {
            returnMenuButton.onClick.RemoveAllListeners();
            returnMenuButton.onClick.AddListener(() =>
            {
                ToStoryBattle(storyJumpInfo);
            });
        }

    }



    IEnumerator StartStory()
    {
        StoryCommand currentCommand;
        moveNext = true;
        while (_storyCommands.Count > 0)
        {
            
            yield return new WaitUntil(() => moveNext && !taskRunning);
            moveNext = false;
            currentCommand = _storyCommands.Dequeue();
            this.currentCommand = currentCommand;
            DoCommand(currentCommand.commandType, currentCommand.args.ToArray(), currentCommand.end);

        }
        
        if(storyJumpInfo == "100001")
            ToBattle();
        else
            ToStoryBattle(storyJumpInfo);
        //_globalController.EnterPrologue();
        
    }

    protected IEnumerator TestCommands()
    {
        BlackOut();
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreen(1f);
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreenPrintAll
            ("所有世界被泽诺斯毁灭\n\n但圣城却在巫女的守护之下幸免于难\n\n王子一同闯入要塞与泽诺斯展开决战\n\n经历了众多的牺牲，泽诺斯被击败\n\n巫女们用世间众人的创世之力重新创造了世界\n\n但重新创造的世界却不知为何开始崩溃"+
            "\n\n原来这个世界中还留有泽诺斯的核心\n\n而能够到达那里的则只有同为泽诺斯一部分的王子\n\n为了切断泽诺斯与世界的关联\n\n王子只身前往泽诺斯的藏身之处");
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreenPrint();
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreenPrint();
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreenPrint();
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreenPrint();
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        BlackScreenPrint();
        yield return new WaitUntil(() => taskRunning == false && moveNext);
        

    }
    
    

    private void DoCommand(StoryCommand.CommandType commandType,string[] args,int end)
    {
        if(currentCoroutine != null)
            return;
        
        //print("CurrentCommand: " + commandType + " );
        
        
        switch (commandType)
        {
            case StoryCommand.CommandType.ADD_CHARA:
            {
                if(args.Length == 1)
                    AddCharacter(args[0]);
                else if(args.Length == 2)
                    AddCharacter(args[0],ObjectExtensions.ParseInvariantFloat(args[1]));
                else if(args.Length == 3)
                    AddCharacter(args[0],ObjectExtensions.ParseInvariantFloat(args[1]),ObjectExtensions.ParseInvariantFloat(args[2]));
                else if(args.Length == 4)
                    AddCharacter(args[0],ObjectExtensions.ParseInvariantFloat(args[1]),ObjectExtensions.ParseInvariantFloat(args[2]),int.Parse(args[3]));
                else if(args.Length == 5)
                    AddCharacter(args[0],ObjectExtensions.ParseInvariantFloat(args[1]),ObjectExtensions.ParseInvariantFloat(args[2]),int.Parse(args[3]),int.Parse(args[4]));
                else if (args.Length == 6)
                {
                    AddCharacter(args[0],ObjectExtensions.ParseInvariantFloat(args[1]),ObjectExtensions.ParseInvariantFloat(args[2]),int.Parse(args[3]),int.Parse(args[4]),args[5]);
                }
                else if (args.Length == 7)
                {
                    AddCharacter(args[0],ObjectExtensions.ParseInvariantFloat(args[1]),ObjectExtensions.ParseInvariantFloat(args[2]),
                        int.Parse(args[3]),int.Parse(args[4]),args[5],int.Parse(args[6]));
                }
                else
                {
                    Debug.LogError("ADD_CHARA命令参数错误");
                    return;
                }
                    break;
            }
            case StoryCommand.CommandType.BLACKOUT:
                BlackOut();
                break;
            case StoryCommand.CommandType.BLACKIN:
                BlackIn();
                break;
            case StoryCommand.CommandType.BLACKSCREEN:
                BlackScreen(ObjectExtensions.ParseInvariantFloat(args[0]));
                break;
            case StoryCommand.CommandType.BLACKSCREEN_FADE:
                BlackScreenFade(ObjectExtensions.ParseInvariantFloat(args[0]));
                break;
            case StoryCommand.CommandType.BLACKSCREEN_PRINT:
            {
                if(args.Length == 1)
                    BlackScreenPrintAll(args[0]);
                else
                    BlackScreenPrint();
            }
                break;
            case StoryCommand.CommandType.BLACKSCREEN_PRINTS:
                BlackScreenPrints(int.Parse(args[0]));
                break;
            case StoryCommand.CommandType.BUTTON_VISIBILITY:
                ButtonVisiblity(Convert.ToInt32(args[0]));
                break;
            case StoryCommand.CommandType.CHARA_FADE:
                CharaFade();
                break;
            case StoryCommand.CommandType.CHARA_FADE_ANIM:
                CharacterPortraitBlackInOut(args);
                break;
            case StoryCommand.CommandType.CLEAR_DIALOG:
            {
                if(args.Length > 0)
                    ClearDialog(false);
                else 
                    ClearDialog();
                break;
            }
                
            case StoryCommand.CommandType.DIALOG_FADEIN:
                DialogFadeIn();
                break;
            case StoryCommand.CommandType.DIALOG_FADEOUT:
                DialogFadeOut();
                break;
            case StoryCommand.CommandType.DESTROY_EFFECTS:
                DestroyEffects(args[0]);
                break;
            case StoryCommand.CommandType.DESTROY_PORTRAIT:
                DestroyPortrait(args[0]);
                break;
            case StoryCommand.CommandType.EFFECTS:
                Effects(args);
                break;
            case StoryCommand.CommandType.FADE_BGM:
                FadeBGM(int.Parse(args[0]),ObjectExtensions.ParseInvariantFloat(args[1]));
                break;
            case StoryCommand.CommandType.SET_CHARA:
                SetCharacter(args);
                break;
            case StoryCommand.CommandType.SET_CHARA_FACE:
                SetCharacterFacialExpression(args);
                break;
            case StoryCommand.CommandType.SET_CHARA_SIBLING:
                SetCharacterSlibingOrder(args[0],int.Parse(args[1]));
                break;
            case StoryCommand.CommandType.SET_SPEAKER:
                SetSpeakerName(args[0]);
                break;
            case StoryCommand.CommandType.SET_BACKGROUND:
                if (args.Length == 2)
                {
                    SetBackground(args[0],ObjectExtensions.ParseInvariantFloat(args[1]));
                }else if(args.Length == 4)
                {
                    SetBackgroundImageColor(ObjectExtensions.ParseInvariantFloat(args[0]),
                        ObjectExtensions.ParseInvariantFloat(args[1]),ObjectExtensions.ParseInvariantFloat(args[2]),
                        ObjectExtensions.ParseInvariantFloat(args[3]));
                }
                break;
            case StoryCommand.CommandType.SWITCH_SPEAKER:
                SetCurrentSpeakingCharacter(args[0]);
                break;
            case StoryCommand.CommandType.PLAY_BGM:
                PlayBGM(args);
                break;
            case StoryCommand.CommandType.PLAY_SE:
                PlaySE(args);
                break;
            case StoryCommand.CommandType.PLAY_VOICE:
                PlayVoice(args);
                break;
            case StoryCommand.CommandType.PORTRAIT_MOVE:
                PortraitMove(args);
                break;
            case StoryCommand.CommandType.PRINT_DIALOG:
                PrintDialog(args,end);
                break;
            case StoryCommand.CommandType.SCREEN_SHAKE:
                ScreenShake(args);
                break;
            case StoryCommand.CommandType.TRANSFORM_BLACKSCREEN:
                BlackScreenAnimation(args);
                break;
            case StoryCommand.CommandType.WAIT:
                Wait(ObjectExtensions.ParseInvariantFloat(args[0]));
                break;



            default:
                break;
            
        }
    }

    private IEnumerator BlackScreenTextOutput()
    {
        yield return null;
        TextMeshProUGUI textMeshProUGUI = transform.Find("Dialog/BlackScreen").GetComponentInChildren<TextMeshProUGUI>();
        string Mytext = textMeshProUGUI.text;
        //print text to textMeshProUGUI.text, but only display the first line
        textMeshProUGUI.text = Mytext; 
        //Display the first line
        //textMeshProUGUI.maxVisibleLines = 3;
        
        
        TMP_WordInfo info = textMeshProUGUI.textInfo.wordInfo[1];//第几行显示
        for (int i = 0; i < info.characterCount; ++i)
        {
            int charIndex = info.firstCharacterIndex + i;
            int meshIndex = textMeshProUGUI.textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = textMeshProUGUI.textInfo.characterInfo[charIndex].vertexIndex;
   
            Color32[] vertexColors = textMeshProUGUI.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0] = Color.red;
            vertexColors[vertexIndex + 1] = Color.red;
            vertexColors[vertexIndex + 2] = Color.red;
            vertexColors[vertexIndex + 3] = Color.red;
        }
 
        textMeshProUGUI.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        
    }
    
    # region BlackOut
    private void BlackOut()
    {
        currentCoroutine = StartCoroutine(BlackOutRoutine());
    }
    
    private void BlackIn()
    {
        currentCoroutine = StartCoroutine(BlackInRoutine());
    }

    private IEnumerator BlackOutRoutine()
    {
        taskRunning = true;
        storyComponent.backGround.blackOut.DOFade(0, 2f).OnComplete(
            () =>
            {
                storyComponent.backGround.blackOut.gameObject.SetActive(false);
                taskRunning = false;
            });
        yield return new WaitUntil(() => taskRunning == false);
        moveNext = true;
        currentCoroutine = null;
    }
    
    private IEnumerator BlackInRoutine()
    {
        taskRunning = true;
        storyComponent.backGround.blackOut.gameObject.SetActive(true);
        storyComponent.backGround.blackOut.DOFade(1, 2f).OnComplete(
            () =>
            {
                //storyComponent.backGround.blackOut.gameObject.SetActive(false);
                taskRunning = false;
            });
        yield return new WaitUntil(() => taskRunning == false);
        moveNext = true;
        currentCoroutine = null;
    }
    #endregion

    #region BlackScreen

    private void BlackScreenFade(float fadeTime)
    {
        currentCoroutine = StartCoroutine(BlackScreenFadeRoutine(fadeTime));
    }
    
    private void BlackScreen(float fadeTime)
    {
        currentCoroutine = StartCoroutine(BlackScreenRoutine(fadeTime));
    }
    private IEnumerator BlackScreenFadeRoutine(float fadeTime)
    {
        skip = false;
        taskRunning = true;
        storyComponent.dialog.blackScreenText.text = "";
        storyComponent.dialog.voiceSource.Stop();
        var tweenerCore = storyComponent.dialog.blackScreen.DOFade(0, fadeTime).OnComplete(
            () =>
            {
                storyComponent.dialog.blackScreen.gameObject.SetActive(false);
                taskRunning = false;
            });
        yield return new WaitUntil(() => taskRunning == false || skip);
        tweenerCore.Kill();
        storyComponent.dialog.blackScreen.color = new Color(0, 0, 0, 0);
        storyComponent.dialog.blackScreen.gameObject.SetActive(false);
        skip = false;
        taskRunning = false;
        currentCoroutine = null;
        moveNext = true;
    }
    
    private IEnumerator BlackScreenRoutine(float fadeTime)
    {
        skip = false;
        taskRunning = true;
        storyComponent.dialog.blackScreen.gameObject.SetActive(true);
        storyComponent.dialog.blackScreen.color = new Color(0, 0, 0, 0f);
        
        var tweenerCore = storyComponent.dialog.blackScreen.DOFade(0.6f, fadeTime).OnComplete(
            () =>
            {
                taskRunning = false;
            });
        yield return new WaitUntil(() => taskRunning == false || skip);
        tweenerCore.Kill();
        skip = false;
        taskRunning = false;
        storyComponent.dialog.blackScreen.color = new Color(0, 0, 0, 0.6f);
        currentCoroutine = null;
        moveNext = true;
    }
    
    private void BlackScreenPrintAll(string textAll)
    {
        taskRunning = true;
        var tmp = storyComponent.dialog.blackScreenText;
        tmp.text = textAll;
        //tmp.color = new Color(1, 1, 1, 0);
        tmp.maxVisibleLines = 0;
        taskRunning = false;
        moveNext = true;
    }

    private void BlackScreenPrint()
    {
        currentCoroutine = StartCoroutine(BlackScreenPrintRoutine());
    }
    
    private void BlackScreenPrints(int lines)
    {
        currentCoroutine = StartCoroutine(BlackScreenPrintsRoutine(lines));
    }

    private IEnumerator BlackScreenPrintRoutine()
    {
        taskRunning = true;
        
        var tmp = storyComponent.dialog.blackScreenText;
        var maxLine = tmp.maxVisibleLines;
        if (maxLine == 0)
        {
            maxLine = 1;
        }
        else
        {
            maxLine += 2; // 2->lines * 2
            //tmp.maxVisibleLines = maxLine + 2;
        }
        var info = tmp.textInfo.lineInfo[maxLine-1];//第几行显示,wordInfo
        
        //获取第maxLine行的文本
        var originText = tmp.text;
        //将第tmp的第3行前加上<alpha=#00>
        var newText = tmp.text.Insert(info.firstCharacterIndex, "<alpha=#00>");

        tmp.text = newText;
        tmp.maxVisibleLines = maxLine;
        
        
        skip = false;
        var newAlpha = 0;
        while (newAlpha < 254 && skip == false)
        {
            newAlpha += 5;
            //将tmp.text从info.firstCharacterIndex开始的11个字符替换为"123456789XY"
            newText = tmp.text.Replace(tmp.text.Substring(info.firstCharacterIndex, 11),
                $"<alpha=#{newAlpha.ToString("X2")}>");
            tmp.text = newText;
            yield return new WaitForSeconds(0.02f);
        }


        tmp.text = originText;
        skip = false;
        taskRunning = false;
        currentCoroutine = null;
    }
    
    private IEnumerator BlackScreenPrintsRoutine(int lines = 1)
    {
        taskRunning = true;
        
        var tmp = storyComponent.dialog.blackScreenText;
        var maxLine = tmp.maxVisibleLines;
        if (maxLine == 0)
        {
            maxLine = 1 + (lines - 1) * 2;
        }
        else
        {
            maxLine += 2 * lines; // 2->lines * 2
        }
        var info = tmp.textInfo.lineInfo[ maxLine-(1 + (lines-1) * 2) ]; //第几行显示,wordInfo
        
        //获取第maxLine行的文本
        var originText = tmp.text;
        //将第tmp的第3行前加上<alpha=#00>
        var newText = tmp.text.Insert(info.firstCharacterIndex, "<alpha=#00>");

        tmp.text = newText;
        tmp.maxVisibleLines = maxLine;
        
        
        skip = false;
        var newAlpha = 0;
        while (newAlpha < 254 && skip == false)
        {
            newAlpha += 5;
            //将tmp.text从info.firstCharacterIndex开始的11个字符替换为"123456789XY"
            newText = tmp.text.Replace(tmp.text.Substring(info.firstCharacterIndex, 11),
                $"<alpha=#{newAlpha.ToString("X2")}>");
            tmp.text = newText;
            yield return new WaitForSeconds(0.02f);
        }


        tmp.text = originText;
        skip = false;
        taskRunning = false;
        currentCoroutine = null;
    }


    #endregion

    #region Visiblity
    
    private void ButtonVisiblity(int visible)
    {
        taskRunning = true;
        if (visible == 0)
        {
            storyComponent.dialog.skipButton.gameObject.SetActive(false);
            storyComponent.dialog.storyRecordButton.gameObject.SetActive(false);
        }
        else
        {
            storyComponent.dialog.skipButton.gameObject.SetActive(true);
            storyComponent.dialog.storyRecordButton.gameObject.SetActive(true);
        }
        taskRunning = false;
        moveNext = true;
    }
    
    private void DialogFadeIn()
    {
        currentCoroutine = StartCoroutine(DialogFadeInRoutine());
    }
    
    private IEnumerator DialogFadeInRoutine()
    {
        taskRunning = true;
        skip = false;
        
        storyComponent.dialog.dialogBody.gameObject.SetActive(true);
        var canvasGroup = storyComponent.dialog.dialogBody.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        var tweenerCore = canvasGroup.DOFade(1, 0.3f).OnComplete(
            () =>
            {
                taskRunning = false;
            }).OnKill(
            () =>
            {
                canvasGroup.alpha = 1;
                taskRunning = false;
                skip = false;
            });
            
        yield return new WaitUntil(() => taskRunning == false || skip);
        //If tween is not complete
        if (tweenerCore.IsComplete() == false)
        {
            tweenerCore.Kill();
        }
        currentCoroutine = null;
        moveNext = true;
    }
    
    private void DialogFadeOut()
    {
        currentCoroutine = StartCoroutine(DialogFadeOutRoutine());
    }
    
    private IEnumerator DialogFadeOutRoutine()
    {
        taskRunning = true;
        skip = false;
        storyComponent.dialog.voiceSource.Stop();
        
        
        storyComponent.dialog.dialogText.text = "";
        storyComponent.dialog.speakerName.text = "";
        
        
        var canvasGroup = storyComponent.dialog.dialogBody.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        var tweenerCore = canvasGroup.DOFade(0, 0.3f).OnComplete(
            () =>
            {
                storyComponent.dialog.dialogBody.gameObject.SetActive(false);
                taskRunning = false;
            }).OnKill(
            () =>
            {
                canvasGroup.alpha = 0;
                storyComponent.dialog.dialogBody.gameObject.SetActive(false);
                taskRunning = false;
                skip = false;
            });
            
        yield return new WaitUntil(() => taskRunning == false || skip);
        //If tween is not complete
        if (tweenerCore.IsComplete() == false)
        {
            tweenerCore.Kill();
        }
        currentCoroutine = null;
        moveNext = true;
    }

    #endregion

    #region Set Arts

    private void DestroyPortrait(string id)
    {
        StoryComponent.DialogMain.Portrait portrait = null;
        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            if (VARIABLE.portraitParts.speakerID == id)
            {
                portrait = VARIABLE;
                break;
            }
        }
        
        if(portrait != null)
        {
            storyComponent.dialog.Portraits.Remove(portrait);
            Destroy(portrait.rootGameObject);
        }
        
        
        
        moveNext = true;
    }

    private void SetSpeakerName(string name)
    {
        taskRunning = true;
        storyComponent.dialog.speakerName.text = name;
        taskRunning = false;
        moveNext = true;
    }

    /// <summary>
    /// Set a single character
    /// </summary>
    /// <param name="portrait_id"></param>
    private void SetCharacter(string[] args)
    {
        taskRunning = true;
        bool found = false;
        int faceID, mouthID;
        if (args.Length == 1)
        {
            faceID = -1;
            mouthID = -1;
        }
        else if(args.Length == 2)
        {
            faceID = int.Parse(args[1]);
            mouthID = -1;
        }
        else if(args.Length > 2)
        {
            faceID = int.Parse(args[1]);
            mouthID = int.Parse(args[2]);
        }
        else return;
        print("faceID"+faceID);
        print("mouthID"+mouthID);

        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            if (VARIABLE.portraitParts.speakerID == args[0])
            {
                found = true;
                try
                {
                    if (currentSpeakingChara != null)
                    {
                        if (currentSpeakingChara?.portraitParts.blinkingAnimationRoutine != null)
                        {
                            StopCoroutine(currentSpeakingChara.portraitParts.blinkingAnimationRoutine);
                        }

                        
                    }
                }
                catch
                {
                    Debug.LogWarning("NotFoundError");
                }

                
                currentSpeakingChara = VARIABLE;
                VARIABLE.rootGameObject.SetActive(true);
                VARIABLE.rootGameObject.transform.localPosition = new Vector3(0, -108, 0);
                if (args.Length == 5)
                {
                    VARIABLE.rootGameObject.transform.localPosition = 
                        new Vector3(int.Parse(args[3]), int.Parse(args[4]), 0);
                }



                if (faceID != -1)
                {
                    VARIABLE.faceImage.sprite = VARIABLE.portraitParts.GetFaceSprite(faceID);
                    VARIABLE.portraitParts.currentBaseFaceIndex = faceID;
                }

                if (mouthID != -1)
                {
                    VARIABLE.mouthImage.sprite = VARIABLE.portraitParts.GetMouthSprite(mouthID);
                    VARIABLE.portraitParts.currentBaseMouthIndex = mouthID;
                }

                
            }
            else
            {
                VARIABLE.rootGameObject.SetActive(false);
            }
        }
        if (!found)
        {
            //处理异常
            InstantiateCharacter(args[0]);
            SetCharacter(args);
            return;
        }

        if (args.Length == 6)
        {
            if (args[5] == "1")
            {
                currentSpeakingChara.rootGameObject.transform.DOLocalMoveY
                    (-138, 0.25f).SetEase(Ease.InOutSine).OnComplete(
                    () =>
                    {
                        currentSpeakingChara.rootGameObject.transform.
                            DOLocalMoveY(-108, 0.25f).SetEase(Ease.InOutSine);
                    });
            }else if (args[5] == "0")
            {
                //左右摇摆一次
                currentSpeakingChara.rootGameObject.transform.DOLocalMoveX
                    (currentSpeakingChara.rootGameObject.transform.localPosition.x + 20, 0.2f).SetEase(Ease.InOutSine).OnComplete(
                    () =>
                    {
                        currentSpeakingChara.rootGameObject.transform.
                            DOLocalMoveX(currentSpeakingChara.rootGameObject.transform.localPosition.x - 20, 0.4f).SetEase(Ease.InOutSine).OnComplete(
                                () =>
                                {
                                    //回到原位
                                    currentSpeakingChara.rootGameObject.transform.
                                        DOLocalMoveX(currentSpeakingChara.rootGameObject.transform.localPosition.x, 0.2f).SetEase(Ease.InOutSine);
                                });
                    });
            }else if (args[5] == "2")
            {
                currentSpeakingChara.rootGameObject.transform.DOLocalMoveY
                    (-68, 0.15f).SetEase(Ease.InOutSine).OnComplete(
                    () =>
                    {
                        currentSpeakingChara.rootGameObject.transform.
                            DOLocalMoveY(-108, 0.2f).SetEase(Ease.InOutSine);
                    });
            }


        }




        taskRunning = false;
        moveNext = true;
    }

    private void SetCharacterSlibingOrder(string id, int order)
    {
        var character = GetCharacter(id);
        if (order >= 0)
        {
            character.rootGameObject.transform.SetSiblingIndex(order);
        }else if(order == -1)
            character.rootGameObject.transform.SetAsLastSibling();

        moveNext = true;
        
    }

    /// <summary>
    /// </summary>
    /// <param name="args[0]">Character id</param>
    /// <param name="args[1]">faceID</param>
    /// <param name="args[2]">mouthID</param>
    /// <param name="args[3]">StopFacialAnimation</param>
    private void SetCharacterFacialExpression(string[] args)
    {
        
        taskRunning = true;
        
        bool found = false;
        int faceID, mouthID;
        if (args.Length == 1)
        {
            faceID = -1;
            mouthID = -1;
        }else if(args.Length == 2)
        {
            faceID = int.Parse(args[1]);
            mouthID = -1;
        }
        else
        {
            faceID = int.Parse(args[1]);
            mouthID = int.Parse(args[2]);
        }

        foreach (var p in storyComponent.dialog.Portraits)
        {
            if (p.portraitParts.speakerID == (args[0]))
            {
                if (faceID != -1)
                {
                    p.faceImage.sprite = p.portraitParts.GetFaceSprite(faceID);
                    p.portraitParts.currentBaseFaceIndex = faceID;
                }

                if (mouthID != -1)
                {
                    p.mouthImage.sprite = p.portraitParts.GetMouthSprite(mouthID);
                    p.portraitParts.currentBaseMouthIndex = mouthID;
                }
                if (args.Length == 4 && int.Parse(args[3]) == 1)
                {
                    if(p.portraitParts.blinkingAnimationRoutine != null)
                        StopCoroutine(p.portraitParts.blinkingAnimationRoutine);
                    if (p.portraitParts.speakingAnimationRoutine != null)
                        StopCoroutine(p.portraitParts.speakingAnimationRoutine);
                }
            }

                
        }
        
        taskRunning = false;
        moveNext = true;

        

    }

    private void AddCharacter(string id, float positionX = 0, float positionY = -108, int faceID = -1, int mouthID = -1, string originID = "", int black = 0)
    {
        taskRunning = true;
        bool found = false;
        var position = new Vector2(positionX, positionY);
        
        

        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            if (VARIABLE.portraitParts.speakerID == id)
            {
                found = true;
                try
                {
                    if (currentSpeakingChara != null)
                    {
                        if (currentSpeakingChara?.portraitParts.blinkingAnimationRoutine != null)
                            StopCoroutine(currentSpeakingChara.portraitParts.blinkingAnimationRoutine);
                    }
                }
                catch
                {
                    Debug.LogWarning("NotFoundError");
                }


                
                currentSpeakingChara = VARIABLE;
                VARIABLE.rootGameObject.SetActive(true);
                VARIABLE.rootGameObject.transform.localPosition = position;



                if (faceID != -1)
                {
                    VARIABLE.faceImage.sprite = VARIABLE.portraitParts.GetFaceSprite(faceID);
                    VARIABLE.portraitParts.currentBaseFaceIndex = faceID;
                }

                if (mouthID != -1)
                {
                    VARIABLE.mouthImage.sprite = VARIABLE.portraitParts.GetMouthSprite(mouthID);
                    VARIABLE.portraitParts.currentBaseMouthIndex = mouthID;
                }

                if (black != 0)
                {
                    VARIABLE.faceImage.color = new Color(0,0,0,0);
                    VARIABLE.mouthImage.color = new Color(0,0,0,0);
                    VARIABLE.baseImage.color = new Color(0,0,0,0);
                }


            }
            else
            {
                //VARIABLE.rootGameObject.SetActive(false);
            }
        }
        if (!found)
        {
            //处理异常
            if (originID != "")
            {
                InstantiateCharacter(originID,id);
            }
            else
            {
                InstantiateCharacter(id);
            }

            
            AddCharacter(id, positionX, positionY, faceID, mouthID, originID, black);
            return;
        }

        


        taskRunning = false;
        moveNext = true;
    }
    
    private void SetCurrentSpeakingCharacter(string id)
    {
        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            if (VARIABLE.portraitParts.speakerID == id)
            {
                try
                {
                    if (currentSpeakingChara != null)
                    {
                        if (currentSpeakingChara?.portraitParts.blinkingAnimationRoutine != null)
                            StopCoroutine(currentSpeakingChara.portraitParts.blinkingAnimationRoutine);
                    }
                }
                catch
                {
                    Debug.LogWarning("NotFoundError");
                }
                currentSpeakingChara = VARIABLE;
                VARIABLE.rootGameObject.SetActive(true);
                //VARIABLE.rootGameObject.transform.localPosition = new Vector3(0, -108, 0);
            }
            else
            {
                //VARIABLE.rootGameObject.SetActive(false);
            }
        }
        moveNext = true;
    }


    private void InstantiateCharacter(string id, string newID = "")
    {
        //Load assetbundle from streamingAssets/storyPortrait/{id}
        AssetBundle bundle;
        try
        {
            bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/storyportrait/" + id);
            _globalController.loadedBundles.Add("storyportrait/" + id, bundle);
        }
        catch
        {
            bundle = _globalController.loadedBundles["storyportrait/" + id];
        }


        //Load PortraitBase from assetbundle
        var portraitBase = bundle.LoadAsset<GameObject>("PortraitBase");
        //Instantiate PortraitBase
        var dialogBodyMask = transform.Find("Dialog/Mask");
        var portrait = Instantiate(portraitBase, dialogBodyMask);

        portrait.GetComponent<RectTransform>().localPosition = new Vector3(0, -108, 0);
        portrait.name = "PortraitBase";
        //Add portrait to storyComponent.dialog.Portraits
        var portraitParts = portrait.GetComponent<PortraitParts>();

        portraitParts.speakerID = id;
        if(newID != "")
            portraitParts.speakerID = newID;

        StoryComponent.DialogMain.Portrait newPortrait = new StoryComponent.DialogMain.Portrait();
        newPortrait.Init(portrait);
        storyComponent.dialog.Portraits.Add(newPortrait);
    }



    private void SetBackground(string name,float posY = 0)
    {
        taskRunning = true;
        storyComponent.backGround.image.sprite = storyBackgroundSprites.GetSprite(name);
        
        storyComponent.backGround.image.transform.localPosition = new Vector3(0, posY, 0);
        
        storyComponent.backGround.image.color = Color.white;
        
        taskRunning = false;
        moveNext = true;
    }

    private void SetBackgroundImageColor(float r, float g, float b, float duration = 0.5f)
    {
        taskRunning = true;
        
        storyComponent.backGround.image.DOColor(new Color(r, g, b),duration);
        
        taskRunning = false;
        moveNext = true;
    }

    private void CharaFade()
    {
        //隐藏所有Portrait
        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            VARIABLE.rootGameObject.SetActive(false);
        }
        moveNext = true;
    }

    #endregion

    #region Audio

    private void PlayBGM(string[] args)
    {
        taskRunning = true;
        storyComponent.dialog.bgmSource.Stop();
        storyComponent.dialog.bgmSource.clip = audioBundlesTest.GetBGM(args[0]);
        if(args.Length > 1)
            storyComponent.dialog.bgmSource.volume = ObjectExtensions.ParseInvariantFloat(args[1]);
        
        storyComponent.dialog.bgmSource.Play();
        taskRunning = false;
        moveNext = true;
    }

    private void FadeBGM(int fadeIn, float time)
    {
        storyComponent.dialog.bgmSource.DOFade(fadeIn, time);
        moveNext = true;
    }

    private void PlaySE(string[] args)
    {
        taskRunning = true;
        storyComponent.dialog.SESource.Stop();
        storyComponent.dialog.SESource.clip = audioBundlesTest.GetSE(args[0]);
        if(args.Length > 1)
            storyComponent.dialog.SESource.volume = ObjectExtensions.ParseInvariantFloat(args[1]);
        
        storyComponent.dialog.SESource.Play();
        taskRunning = false;
        moveNext = true;
    }

    private void PlayVoice(string[] args)
    {
        taskRunning = true;
        storyComponent.dialog.voiceSource.Stop();
        
        
        storyComponent.dialog.voiceSource.clip = audioBundlesTest.GetVoice(args[0]);
        if(args.Length > 1)
            storyComponent.dialog.voiceSource.volume = ObjectExtensions.ParseInvariantFloat(args[1]);

        if (args.Length > 3)
        {
            if(currentSpeakingChara.portraitParts.speakingAnimationRoutine != null)
                StopCoroutine(currentSpeakingChara.portraitParts.speakingAnimationRoutine);

            if (currentSpeakingChara.portraitParts.blinkingAnimationRoutine != null)
            {
                StopCoroutine(currentSpeakingChara.portraitParts.blinkingAnimationRoutine);
                currentSpeakingChara.faceImage.sprite = 
                    currentSpeakingChara.portraitParts.
                        GetFaceSprite(currentSpeakingChara.portraitParts.currentBaseFaceIndex);
            }

            
            
            storyComponent.dialog.voiceSource.Play();
            //TestWaveDetect();
            
            currentSpeakingChara.portraitParts.speakingAnimationRoutine = StartCoroutine
            (CharacterSpeakAnimation(int.Parse(args[2]), int.Parse(args[3]),
                currentSpeakingChara.portraitParts, args[0]));
            
            print("mouthOpen:"+args[2]+" mouthClose"+args[3]);

            var currentFaceIndex = currentSpeakingChara.portraitParts.currentBaseFaceIndex;
            var totalFaceIndex = currentSpeakingChara.portraitParts.GetEyeSpriteTotalCount();
            
            var blinkFaceIndex = 
                currentFaceIndex>=totalFaceIndex?totalFaceIndex:currentFaceIndex+1;

            if (args.Length >= 5)
            {
                currentFaceIndex = int.Parse(args[4]);
                blinkFaceIndex = 
                    currentFaceIndex>=totalFaceIndex?totalFaceIndex:currentFaceIndex+1;
            }
            if (args.Length >= 6)
            {
                blinkFaceIndex = int.Parse(args[5]);
            }

            currentSpeakingChara.portraitParts.blinkingAnimationRoutine = 
                StartCoroutine
            (CharacterBlinkAnimation(currentFaceIndex,
                currentFaceIndex>=totalFaceIndex-1?totalFaceIndex-1:blinkFaceIndex, currentSpeakingChara.portraitParts));
            
            taskRunning = false;
            moveNext = true;
            return;
        }

        storyComponent.dialog.voiceSource.Play();
        taskRunning = false;
        moveNext = true;
    }
    
    private IEnumerator CharacterSpeakAnimation(int mouthClose, int mouthOpen, PortraitParts currentSpeakingCharaPortraitParts,string clipName)
    {
        List<float> start_times = new List<float>();
        List<float> end_times = new List<float>();

        audioBundlesTest.GetVoiceInfo(clipName, ref start_times, ref end_times);
        end_times.Add(storyComponent.dialog.voiceSource.clip.length);
        //print(start_times.ToString());

        foreach (var startTime in start_times)
        {
            Invoke("StartTalk", startTime);
        }
        foreach (var startTime in end_times)
        {
            Invoke("EndTalk", startTime);
        }

        while (storyComponent.dialog.voiceSource.isPlaying)
        {
            if (storyComponent.dialog.voiceSource.isPlaying && characterIsSpeaking == false)
            {
                currentSpeakingChara.mouthImage.sprite = currentSpeakingChara.portraitParts.GetMouthSprite(mouthClose);
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            
            
            currentSpeakingChara.mouthImage.sprite = currentSpeakingChara.portraitParts.GetMouthSprite(mouthOpen);
            yield return new WaitForSeconds(0.2f);

            if (storyComponent.dialog.voiceSource.isPlaying == false)
            {
                currentSpeakingChara.mouthImage.sprite = currentSpeakingChara.portraitParts.GetMouthSprite(mouthClose);
                break;
            }
            
            currentSpeakingChara.mouthImage.sprite = currentSpeakingChara.portraitParts.GetMouthSprite(mouthClose);
            yield return new WaitForSeconds(0.1f);
        }
        CancelInvoke();
        characterIsSpeaking = false;
        currentSpeakingCharaPortraitParts.speakingAnimationRoutine = null;
        currentSpeakingChara.mouthImage.sprite = currentSpeakingChara.portraitParts.GetMouthSprite(mouthClose);
    }

    private IEnumerator CharacterBlinkAnimation(int eyeOpen, int eyeClose, PortraitParts currentSpeakingCharaPortraitParts)
    {
        var tempCharacter = currentSpeakingChara;
        yield return new WaitUntil(()=>!storyComponent.dialog.voiceSource.isPlaying);
        //print("I start blinking!");
        while (currentSpeakingChara == tempCharacter &&
               currentSpeakingChara.rootGameObject.activeSelf)
        {
            print("EyeClose:"+eyeClose+" EyeOpen:"+eyeOpen);
            currentSpeakingChara.faceImage.sprite = currentSpeakingChara.portraitParts.GetFaceSprite(eyeClose);
            yield return new WaitForSeconds(0.2f);
            if(currentSpeakingChara != tempCharacter)
                break;
            currentSpeakingChara.faceImage.sprite = currentSpeakingChara.portraitParts.GetFaceSprite(eyeOpen);
            yield return new WaitForSeconds(2.5f+Random.Range(0,0.5f));
            
        }
        currentSpeakingChara.faceImage.sprite = currentSpeakingChara.portraitParts.GetFaceSprite(eyeOpen);
        currentSpeakingCharaPortraitParts.blinkingAnimationRoutine = null;
    }


    #endregion

    #region Effects

    private void DestroyEffects(string name)
    {
        //将name补0成三位数的形式，如"1"转为"001"，"10"转为"010"
        if (name.Length == 1)
        {
            name = "fx_insty_00" + name;
        }
        else if (name.Length == 2)
        {
            name = "fx_insty_0" + name;
        }
        else
        {
            name = "fx_insty_" + name;
        }
        moveNext = true;

        for (int i = storyComponent.dialog.effectRoot.childCount - 1; i >= 0; i--)
        {
            if (name == storyComponent.dialog.effectRoot.GetChild(i).name)
            {
                Destroy(storyComponent.dialog.effectRoot.GetChild(i).gameObject);
                return;
            }
        }
    }

    private void Effects(string[] args)
    {
        //从assetBundle: effectsBundle中加载名为args[0]的特效。
        //如果args长度只为1，那么加载到(0,0)位置，否则加载到(args[1],args[2])位置
        
        //将args[0]凑成三位数的形式，如"1"转为"001"，"10"转为"010"
        if (args[0].Length == 1)
        {
            args[0] = "00" + args[0];
        }
        else if (args[0].Length == 2)
        {
            args[0] = "0" + args[0];
        }
        //args前拼接"fx_insty_"
        args[0] = "fx_insty_" + args[0];
        

        var effect = effectsBundle.LoadAsset<GameObject>(args[0]);
        var effectInstance = Instantiate(effect, storyComponent.dialog.effectRoot);
        if (args.Length >= 3)
        {
            effectInstance.transform.localPosition = new Vector3(ObjectExtensions.ParseInvariantFloat(args[1]), ObjectExtensions.ParseInvariantFloat(args[2]), 0);
        }
        effectInstance.name = args[0];

        moveNext = true;

    }




    void ScreenShake(string[] args)
    {
        
        currentCoroutine = 
            StartCoroutine(ScreenShakeRoutine(ObjectExtensions.ParseInvariantFloat(args[0]), ObjectExtensions.ParseInvariantFloat(args[1])));
                
        
    }
    
    IEnumerator ScreenShakeRoutine(float duration, float strength)
    {
        taskRunning = true;
        skip = false;
        //Shake the currentSpeakingChara's GameObject and the background vertically
        var tweenerCore = currentSpeakingChara.rootGameObject.transform.DOShakePosition(duration, strength, 10, 90, false, true).OnComplete(
            () =>
            {
                taskRunning = false;
            }).OnKill(
            () =>
            {
                taskRunning = false;
                skip = false;
            });
        var tweenerCore2 = storyComponent.backGround.image.transform.DOShakePosition(duration, strength, 10, 90, false, true);
        
        
        
        yield return new WaitUntil(() => taskRunning == false);
        //If tween is not complete
        if (tweenerCore.IsComplete() == false)
        {
            tweenerCore.Kill();
            tweenerCore2.Kill();
        }
        currentCoroutine = null;
        moveNext = true;
    }

    private void BlackScreenAnimation(string[] args)
    {
        taskRunning = true;
        
        var canvasGroup =
            storyComponent.dialog.slideBlackScreen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        
        
        var fadeIn = int.Parse(args[0]);//0 for fade in, 1 for fade out
        var direction = args[1];
        var duration = ObjectExtensions.ParseInvariantFloat(args[2]);

        Tweener tweener;
        
        if (fadeIn == 0)
        {
            switch (direction)
            {
                case "UP":
                    storyComponent.dialog.slideBlackScreen.transform.localPosition = new Vector2(0, -2048);
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveY(0, duration);
                    break;
                case "DOWN":
                    storyComponent.dialog.slideBlackScreen.transform.localPosition = new Vector2(0, 2048);
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveY(0, duration);
                    break;
                case "LEFT":
                    storyComponent.dialog.slideBlackScreen.transform.localPosition = new Vector2(-2048, 0);
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveX(0, duration);
                    break;
                case "RIGHT":
                    storyComponent.dialog.slideBlackScreen.transform.localPosition = new Vector2(2048, 0);
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveX(0, duration);
                    break;
                default:
                    Debug.LogError("BlackScreenAnimation Error");
                    return;
            }
        }else if (fadeIn == 1)
        {
            storyComponent.dialog.slideBlackScreen.transform.localPosition = new Vector2(0, 0);
            switch (direction)
            {
                case "UP":
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveY(-2048, duration);
                    break;
                case "DOWN":
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveY(2048, duration);
                    break;
                case "LEFT":
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveX(2048, duration);
                    break;
                case "RIGHT":
                    tweener = storyComponent.dialog.slideBlackScreen.transform.DOLocalMoveX(-2048, duration);
                    break;
                default:
                    Debug.LogError("BlackScreenAnimation Error");
                    return;
            }
        }
        else
        {
            Debug.LogError("BlackScreenAnimation Error");
            return;
        }
        tweener.OnComplete(() =>
        {
            taskRunning = false;
            moveNext = true;
            if(fadeIn == 1)
                canvasGroup.alpha = 0;
        }).OnKill(() =>
        {
            taskRunning = false;
            moveNext = true;
            if(fadeIn == 1)
                canvasGroup.alpha = 0;
        });


    }






    private void PortraitMove(string[] args)
    {
        switch (args[0])
        {
            case "MOVE":
            {
                if (args.Length < 4)
                    Debug.LogError("Portrait Move Command Error");
                if (args.Length == 4)
                {
                    currentCoroutine =
                        StartCoroutine(PortraitSimpleMoveRoutine(new Vector2(ObjectExtensions.ParseInvariantFloat(args[1]), ObjectExtensions.ParseInvariantFloat(args[2])),
                            ObjectExtensions.ParseInvariantFloat(args[3])));
                }else if (args.Length == 5)
                {
                    //Parse the args[4] to get the easing enum type
                    var easingType = (DG.Tweening.Ease) Enum.Parse(typeof(DG.Tweening.Ease), args[4]);
                    currentCoroutine =
                        StartCoroutine(PortraitSimpleMoveRoutine(new Vector2(ObjectExtensions.ParseInvariantFloat(args[1]), ObjectExtensions.ParseInvariantFloat(args[2])),
                            ObjectExtensions.ParseInvariantFloat(args[3]), easingType));
                }

                break;
            }
            case "SHAKE":
                if(args.Length != 3)
                    Debug.LogError("Portrait Shake Command Error");
                currentCoroutine = 
                    StartCoroutine(PortraitShakeRoutine(ObjectExtensions.ParseInvariantFloat(args[1]), ObjectExtensions.ParseInvariantFloat(args[2])));
                break;
            case "JUMP":
                if(args.Length != 6)
                    Debug.LogError("Portrait Jump Command Error");
                currentCoroutine = 
                    StartCoroutine(PortraitJumpRoutine(args[1], ObjectExtensions.ParseInvariantFloat(args[2]), int.Parse(args[3]), ObjectExtensions.ParseInvariantFloat(args[4]), args[5]));
                break;
            case "WALK":
                if(args.Length != 5)
                    Debug.LogError("Portrait Walk Command Error");
                currentCoroutine = 
                    StartCoroutine(PortraitWalkRoutine(ObjectExtensions.ParseInvariantFloat(args[1]), ObjectExtensions.ParseInvariantFloat(args[2]), ObjectExtensions.ParseInvariantFloat(args[3]), ObjectExtensions.ParseInvariantFloat(args[4])));
                break;
            case "ZOOM":
                if(args.Length != 5)
                    Debug.LogError("Portrait Zoom Command Error");
                currentCoroutine = 
                    StartCoroutine(PortraitZoomRoutine(ObjectExtensions.ParseInvariantFloat(args[1]), int.Parse(args[2]), ObjectExtensions.ParseInvariantFloat(args[3]), args[4]));
                break;
            default:
                break;
        }
    }
    
    IEnumerator PortraitSimpleMoveRoutine(Vector2 moveVector, float duration,Ease ease = Ease.Linear)
    {
        taskRunning = true;
        var tweenerCore = currentSpeakingChara.rootGameObject.transform.DOLocalMove(moveVector, duration).OnComplete(
            () =>
            {
                taskRunning = false;
            }).OnKill(
            () =>
            {
                taskRunning = false;
                skip = false;
            });

        tweenerCore.SetEase(ease);
            
        yield return new WaitUntil(() => taskRunning == false);
        //If tween is not complete
        if (tweenerCore.IsComplete() == false)
        {
            tweenerCore.Kill();
        }
        currentCoroutine = null;
        moveNext = true;
    }

    IEnumerator PortraitWalkRoutine(float offsetY, float from, float to, float totalTime)
    {
        taskRunning = true;
        
        var transform = currentSpeakingChara.rootGameObject.transform;
        
        transform.localPosition = new Vector3(from, -108, 0);
        // 创建一个序列
        Sequence s = DOTween.Sequence();
        // 添加移动动画
        s.Append(transform.DOLocalMoveX(to, totalTime).SetEase(Ease.InOutSine));
        
        var loops = (int) ((totalTime / 0.5f)) * 2;

        // 添加摇摆效果
        s.Join(transform.DOLocalMoveY(-108 + offsetY, totalTime / 2).
            SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));
        // 动画结束后停止所有动画
        s.OnComplete(() =>
        {
            DOTween.Clear();
            taskRunning = false;
        }).OnKill(() =>
        {
            taskRunning = false;
        });
        
        yield return new WaitUntil( ()=>taskRunning == false );
        
        currentCoroutine = null;
        moveNext = true;
    }

    IEnumerator PortraitJumpRoutine(string direction, float distance, int loopTimes, float totalTime, string easeType)
    {
        taskRunning = true;
        Ease ease = (Ease)Enum.Parse(typeof(Ease), easeType);
        //Ease.InOutSine;

        // 计算每次循环的持续时间
        float duration = totalTime / loopTimes;

        // 计算移动距离
        var transform = currentSpeakingChara.rootGameObject.transform;

        Tweener _tweener = null;
        if (direction == "up")
        {
            _tweener = transform.DOLocalMoveY(transform.localPosition.y + distance, duration)
                .SetEase(ease) // 设置缓动函数
                .SetLoops(loopTimes * 2, LoopType.Yoyo).OnComplete(() =>
                {
                    taskRunning = false;
                });
        }else if (direction == "down")
        {
            _tweener = transform.DOLocalMoveY(transform.localPosition.y - distance, duration)
                .SetEase(ease) // 设置缓动函数
                .SetLoops(loopTimes * 2, LoopType.Yoyo).OnComplete(() => { taskRunning = false; });
        }else if (direction == "left")
        {
            _tweener = transform.DOLocalMoveX(transform.localPosition.x - distance, duration)
                .SetEase(ease) // 设置缓动函数
                .SetLoops(loopTimes * 2, LoopType.Yoyo).OnComplete(() => { taskRunning = false; });
        }else if (direction == "right")
        {
            _tweener = transform.DOLocalMoveX(transform.localPosition.x + distance, duration)
                .SetEase(ease) // 设置缓动函数
                .SetLoops(loopTimes * 2, LoopType.Yoyo).OnComplete(() => { taskRunning = false; });
        }
        else
        {
            Debug.LogError("Portrait Jump Command Error");
        }
        
        _tweener.OnKill(() =>
        {
            _tweener.Complete();
            taskRunning = false;
        });


        yield return new WaitUntil(() => taskRunning == false);
        
        currentCoroutine = null;
        moveNext = true;
    }
    IEnumerator PortraitShakeRoutine(float distance, float duration)
    {
        taskRunning = true;
        var tweenerCore = currentSpeakingChara.rootGameObject.transform.DOShakePosition(duration, distance).OnComplete(
            () =>
            {
                taskRunning = false;
            }).OnKill(
            () =>
            {
                taskRunning = false;
                skip = false;
            });
            
        yield return new WaitUntil(() => taskRunning == false);
        //If tween is not complete
        if (tweenerCore.IsComplete() == false)
        {
            tweenerCore.Kill();
        }
        currentCoroutine = null;
        moveNext = true;
    }

    IEnumerator PortraitZoomRoutine(float maxScale, int loopTimes, float totalTime, string easeType)
    {
        taskRunning = true;
        Ease ease = (Ease)Enum.Parse(typeof(Ease), easeType);
        
        // 计算每次循环的持续时间
        float duration = totalTime / loopTimes;
        
        var transform = currentSpeakingChara.rootGameObject.transform;
        
        Tweener _tweener = transform.DOScale(maxScale, duration)
            .SetEase(ease) // 设置缓动函数
            .SetLoops(loopTimes * 2, LoopType.Yoyo).OnComplete(() => { taskRunning = false; });
        
        _tweener.OnKill(() =>
        {
            _tweener.Complete();
            taskRunning = false;
        });
        
        yield return new WaitUntil(() => taskRunning == false);
        
        currentCoroutine = null;
        moveNext = true;

    }
    
    private void CharacterPortraitBlackInOut(string[] args)
    {
        //args1: chara id
        //args2: in or out
        //args3: duration
        //args4: movenext or not
        
        taskRunning = true;

        var character = GetCharacter(args[0]);
        if (character == null)
        {
            Debug.LogError("Character Not Found");
            return;
        }

        Tweener _tweener = null;
        //var canvasGroup = character.rootGameObject.GetComponent<CanvasGroup>();
        character.faceImage.color = Color.clear;
        character.mouthImage.color = Color.clear;
        character.baseImage.color = Color.black;


        if (args[1] == "IN")
        {
            character.baseImage.color = new Color(0, 0, 0, 0);
            _tweener = character.baseImage.DOFade(1, ObjectExtensions.ParseInvariantFloat(args[2]));
            _tweener.OnComplete(() =>
            {
                taskRunning = false;
                if (ObjectExtensions.ParseInvariantFloat(args[3]) == 0)
                {
                    moveNext = true;
                }
                character.baseImage.color = Color.black;
                character.faceImage.color = Color.clear;
                character.mouthImage.color = Color.clear;
            });
        }else if (args[1] == "OUT")
        {
            character.baseImage.color = new Color(0, 0, 0, 1);
            _tweener = character.baseImage.DOColor(Color.white, ObjectExtensions.ParseInvariantFloat(args[2]));
            
            _tweener.OnComplete(() =>
            {
                taskRunning = false;
                if (ObjectExtensions.ParseInvariantFloat(args[3]) == 0)
                {
                    moveNext = true;
                }
                character.baseImage.color = Color.white;
                character.faceImage.color = Color.white;
                character.mouthImage.color = Color.white;
            });
        }
        else
        {
            Debug.LogError("Character Portrait Black In Out Command Error");
        }

        if (ObjectExtensions.ParseInvariantFloat(args[3]) != 0)
        {
            moveNext = true;
            taskRunning = false;
        }

    }


    #endregion

    #region Print

    private void ClearDialog(bool clearSpeakerName = true)
    {
        taskRunning = true;
        storyComponent.dialog.arrowImg.gameObject.SetActive(false);
        storyComponent.dialog.dialogText.text = "";
        if(clearSpeakerName)
            storyComponent.dialog.speakerName.text = "";
        storyComponent.dialog.voiceSource.Stop();
        taskRunning = false;
        moveNext = true;
    }
    
    private void PrintDialog(string[] args,int end)
    {
        if(args.Length == 2) 
            currentCoroutine = StartCoroutine(PrintDialogRoutine(args[0], ObjectExtensions.ParseInvariantFloat(args[1]),end));
    }
    
    private IEnumerator PrintDialogRoutine(string dialog,float voiceTime, int end)
    {
        taskRunning = true;
        skip = false;
        storyComponent.dialog.arrowImg.gameObject.SetActive(false);
        
        string text = storyComponent.dialog.dialogText.text;
        //获得当前的文本总字数
        //2023-9-10
        int textLength = dialog.Length;
        //print("字符数："+textLength);
        if(GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN)
            voiceTime = textLength * 0.08f + 0.02f;
        else if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
            voiceTime = textLength * 0.03f + 0.02f;
        
        
        var tween = DOTween.To(()=>text, x=>text = x, dialog, voiceTime).SetEase(Ease.Linear).OnUpdate(
            () =>
            {
                storyComponent.dialog.dialogText.text = text;
            }).OnComplete(
            () =>
            {
                storyComponent.dialog.dialogText.text = dialog;
                
                storyComponent.dialog.arrowImg.gameObject.SetActive(true);
                taskRunning = false;
            }).OnKill(
            () =>
            {
                storyComponent.dialog.dialogText.text = dialog;
                
                storyComponent.dialog.arrowImg.gameObject.SetActive(true);
                taskRunning = false;
                skip = false;
            });
        
        yield return new WaitUntil(() => taskRunning == false || skip == true);
        //If tween is not complete
        if (tween.IsComplete() == false)
        {
            tween.Kill();
        }
        currentCoroutine = null;

    }
    

    #endregion
    
    private void Wait(float waitTime)
    {
        currentCoroutine = StartCoroutine(WaitRoutine(waitTime));
    }
    
    private IEnumerator WaitRoutine(float waitTime)
    {
        //taskRunning = true;
        yield return new WaitForSecondsRealtime(waitTime);
        //taskRunning = false;
        currentCoroutine = null;
        moveNext = true;
    }

    
    
    
    public string GetLevelName()
    {
        return storyStr;
    }

    private void StartTalk()
    {
        characterIsSpeaking = true;
    }
    private void EndTalk()
    {
        characterIsSpeaking = false;
    }
    
    public void SwapAutoMode()
    {
        if(paused)
            return;
        
        
        autoMode = !autoMode;
        autoTime = 0;
        if(autoMode)
            storyComponent.dialog.storyRecordButton.GetComponent<Animation>().Play();
        else
        {
            storyComponent.dialog.storyRecordButton.GetComponent<Animation>().Stop();
            storyComponent.dialog.storyRecordButton.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    private StoryComponent.DialogMain.Portrait GetCharacter(string id)
    {
        foreach (var VARIABLE in storyComponent.dialog.Portraits)
        {
            if (VARIABLE.portraitParts.speakerID == id)
            {
                return VARIABLE;
            }
        }

        return null;
    }

    public void SetGaumePaused()
    {
        if (paused || !_globalController.loadingEnd)
        {
            return;
        }
        paused = true;
        skipMenu?.SetActive(true);
    }
    
    public void SetGameContinue()
    {
        if (!paused || !_globalController.loadingEnd)
        {
            return;
        }
        paused = false;
        moveNext = false;
        skipMenu?.SetActive(false);
        
    }



    public void ToBattle()
    {
        _globalController.EnterPrologue();
    }

    public void ToStoryBattle(string levelName)
    {
        _globalController.EnterNormalStoryBattle(levelName);
    }








}

[Serializable]
public class StoryCommand
{
    public enum CommandType
    {
        ADD_CHARA, //添加人物
        
        BLACKOUT, //黑屏淡出
        BLACKIN, //黑屏
        
        BLACKSCREEN_FADE, //半透明黑屏淡出
        BLACKSCREEN, //半透明黑屏
        BLACKSCREEN_PRINT,//输出全屏文本
        BLACKSCREEN_PRINTS,//输出多行全屏文本
        BUTTON_VISIBILITY,//按钮可见性
        
        CLEAR_DIALOG,//清空对话框
        CHARA_FADE,//人物立绘淡入淡出
        CHARA_FADE_ANIM,//人物立绘淡出动画
        
        
        DIALOG_FADEIN,//对话框淡入
        DIALOG_FADEOUT,//对话框淡出
        
        DESTROY_PORTRAIT,//回收人物立绘
        DESTROY_EFFECTS,//回收特效
        
        EFFECTS,//特效
        
        FADE_BGM,
        
        PLAY_BGM,//播放背景音乐
        PLAY_VOICE,//播放语音
        PLAY_SE,//播放音效
        PORTRAIT_MOVE,//人物立绘运动
        PORTRAIT_MOVE_GROUP,//多个人物立绘运动
        PRINT_DIALOG,//输出对话
        
        RENAME_SPEAKER,//重命名人物ID
        
        SCREEN_SHAKE,//屏幕震动
        SET_BACKGROUND,//设置背景图片
        SET_CHARA,//设置人物图片
        SET_CHARA_FACE,//设置人物静止图片
        SET_CHARA_SIBLING,//设置人物层级
        SET_SPEAKER,//设置人物名字
        SET_CHARA_ANIM,//人物淡入动画
        SWITCH_SPEAKER,//切换当前人物
        
        TRANSFORM_BLACKSCREEN,//幻灯片转场
        
        WAIT//等待
        
    }
    public CommandType commandType;
    public List<string> args;
    public int end;
}

[Serializable]
public class StoryComponent
{
    [Serializable]
    public class BackGround
    {
        public GameObject gameObject;
        public Image image;
        public Image blackOut;
        public void Init()
        {
            gameObject = GameObject.Find("Background");
            image = gameObject.GetComponent<Image>();
            blackOut = gameObject.transform.parent.Find("BlackOut").GetComponent<Image>();
        }
    }

    [Serializable]
    public class DialogMain
    {
        [Serializable]
        public class Portrait
        {
            public GameObject rootGameObject;
            public Image baseImage;
            public Vector2 startPosition;
            public Image mouthImage;
            public Image faceImage;
            public PortraitParts portraitParts;

            public void Init(GameObject root)
            {
                rootGameObject = root;
                portraitParts = root.GetComponent<PortraitParts>();
                baseImage = root.transform.Find("Base")?.GetComponent<Image>();
                mouthImage = root.transform.Find("Mouth")?.GetComponent<Image>();
                faceImage = root.transform.Find("Face")?.GetComponent<Image>();
                startPosition = root.transform.localPosition;
            }

        }

        public List<Portrait> Portraits;
        public Image blackScreen;
        public TextMeshProUGUI blackScreenText;
        public Transform slideBlackScreen;
        public Transform effectRoot;
        public AudioSource voiceSource;
        public AudioSource bgmSource;
        public AudioSource SESource;
        public GameObject dialogBody;
        public TextMeshProUGUI dialogText;
        public TextMeshProUGUI speakerName;
        public Image arrowImg;
        public Button skipButton;
        public Button storyRecordButton;
        
        
        public void Init()
        {
            Portraits = new List<Portrait>();
            blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();
            blackScreenText = blackScreen.GetComponentInChildren<TextMeshProUGUI>();
            voiceSource = GameObject.Find("Voice").GetComponent<AudioSource>();
            bgmSource = GameObject.Find("BGM").GetComponent<AudioSource>();
            dialogBody = GameObject.Find("StoryUI")
                .transform.Find("Dialog/DialogBody").gameObject;
            dialogText = dialogBody.transform.Find("Content").GetComponentInChildren<TextMeshProUGUI>();
            speakerName = dialogBody.transform.Find("Banner").GetComponentInChildren<TextMeshProUGUI>();
            arrowImg = dialogBody.transform.Find("Arrow").GetComponent<Image>();
            skipButton = GameObject.Find("SkipButton").GetComponent<Button>();
            storyRecordButton = GameObject.Find("RecordButton").GetComponent<Button>();
            

            var dialogBodyMask = GameObject.Find("Dialog").transform.Find("Mask");
            for(int i = 0; i < dialogBodyMask.childCount; i++)
            {
                Portrait portrait = new Portrait();
                portrait.Init(dialogBodyMask.GetChild(i).gameObject);
                Portraits.Add(portrait);
            }

        }
        
    }
    
    public BackGround backGround = new();
    public DialogMain dialog = new();

    public void Init()
    {
        //backGround = new BackGround();
        backGround.Init();
        //dialog = new DialogMain();
        dialog.Init();
        
    }

    

}

/*
 * 所有世界被泽诺斯毁灭
 * 但圣城却在巫女的守护之下幸免于难
 * 王子一同闯入要塞与泽诺斯展开决战
 * 经历了众多的牺牲，泽诺斯被击败
 * 巫女们用世间众人的创世之力重新创造了世界
 * 但重新创造的世界却不知为何开始崩溃
 * 原来这个世界中还留有泽诺斯的核心
 * 而能够到达那里的则只有同为泽诺斯一部分的王子
 * 为了切断泽诺斯与世界的关联
 * 王子只身前往泽诺斯的藏身之处
 * 但在前往泽诺斯的途中
 * 王子遇到了一位名为艾莉的少女
 * 艾莉是一位来自异世界的少女
 * 她的世界也因为泽诺斯的崩溃而被毁灭
 * 艾莉也是为了切断泽诺斯与世界的关联
 * 才来到了这个世界
 * 艾莉和王子一同前往泽诺斯的藏身之处
 * 但是王子的身体却开始变得异常的虚弱
 * 艾莉也因为王子的虚弱而受到了伤害
 * 两人一同到达了泽诺斯的藏身之处
 * 但是泽诺斯的核心却已经被巫女们封印
 * 泽诺斯的核心已经无法再被操纵
 * 但是泽诺斯的核心却还是在不断的崩溃
 * 艾莉和王子一同决定将泽诺斯的核心破坏
 * 但是王子的身体却已经无法再维持
 * 艾莉只能将王子的身体封印
*/