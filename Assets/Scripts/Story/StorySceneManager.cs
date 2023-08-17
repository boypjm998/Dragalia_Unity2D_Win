using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private Queue<StoryCommand> _storyCommands;
    private StoryComponent.DialogMain.Portrait currentSpeakingChara;
    private bool characterIsSpeaking = false;
    private Coroutine currentCoroutine;
    private bool taskRunning = false;
    private bool skip;
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
    
    // Story Components
    
    public StoryComponent storyComponent;
    private StoryBackgroundSprites storyBackgroundSprites;
    private AudioBundlesTest audioBundlesTest;

    private void LateUpdate()
    {
        if(started == false)
            return;

        moveNext = false;
        try
        {
            if (_globalController.loadingEnd == false || paused)
                return;
        }
        catch
        {
            _globalController = GlobalController.Instance;
        }



        if (autoMode)
        {
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
        print("clicked");
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


    // Start is called before the first frame update
    IEnumerator Start()
    {
        
        
        //storyJsonData = storyData.ToString();
        storyComponent = new StoryComponent();
        storyComponent.Init();
        audioBundlesTest = GetComponent<AudioBundlesTest>();
        storyBackgroundSprites = 
            storyComponent.backGround.gameObject.GetComponent<StoryBackgroundSprites>();
        InitScene();
        
        //TODO: 设置剧情ID
        
        ReadCommands();
        
        yield return new WaitUntil(() => started);
        
        _globalController = FindObjectOfType<GlobalController>();
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
    }

    private void ReadCommands()
    {
        _storyCommands = new Queue<StoryCommand>();
        //将stroyData的"main_story_001"读取出来，将其中的"command_list"列表加入到_storyCommands中
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
        switch (commandType)
        {
            case StoryCommand.CommandType.BLACKOUT:
                BlackOut();
                break;
            case StoryCommand.CommandType.BLACKIN:
                BlackIn();
                break;
            case StoryCommand.CommandType.BLACKSCREEN:
                BlackScreen(float.Parse(args[0]));
                break;
            case StoryCommand.CommandType.BLACKSCREEN_FADE:
                BlackScreenFade(float.Parse(args[0]));
                break;
            case StoryCommand.CommandType.BLACKSCREEN_PRINT:
            {
                if(args.Length == 1)
                    BlackScreenPrintAll(args[0]);
                else
                    BlackScreenPrint();
            }
                break;
            case StoryCommand.CommandType.BUTTON_VISIBILITY:
                ButtonVisiblity(Convert.ToInt32(args[0]));
                break;
            case StoryCommand.CommandType.CHARA_FADE:
                CharaFade();
                break;
            case StoryCommand.CommandType.CLEAR_DIALOG:
                ClearDialog();
                break;
            case StoryCommand.CommandType.DIALOG_FADEIN:
                DialogFadeIn();
                break;
            case StoryCommand.CommandType.DIALOG_FADEOUT:
                DialogFadeOut();
                break;
            case StoryCommand.CommandType.SET_CHARA:
                SetCharacter(args);
                break;
            case StoryCommand.CommandType.SET_SPEAKER:
                SetSpeakerName(args[0]);
                break;
            case StoryCommand.CommandType.SET_BACKGROUND:
                SetBackground(args[0]);
                break;
            case StoryCommand.CommandType.PLAY_BGM:
                PlayBGM(args);
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
            case StoryCommand.CommandType.WAIT:
                Wait(float.Parse(args[0]));
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

    private IEnumerator BlackScreenPrintRoutine()
    {
        taskRunning = true;
        
        var tmp = storyComponent.dialog.blackScreenText;
        var maxLine = tmp.maxVisibleLines;
        if (maxLine == 0)
        {
            maxLine = 1;
            //tmp.maxVisibleLines = 1;
        }
        else
        {
            maxLine += 2;
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
                            StopCoroutine(currentSpeakingChara.portraitParts.blinkingAnimationRoutine);
                    }
                }
                catch
                {
                    Debug.LogWarning("NotFoundError");
                }






                currentSpeakingChara = VARIABLE;
                VARIABLE.rootGameObject.SetActive(true);
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
        taskRunning = false;
        moveNext = true;
    }


    private void InstantiateCharacter(string id)
    {
        //Load assetbundle from streamingAssets/storyPortrait/{id}
        var bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/storyportrait/" + id);
        _globalController.loadedBundles.Add("storyportrait/"+id,bundle);
        //Load PortraitBase from assetbundle
        var portraitBase = bundle.LoadAsset<GameObject>("PortraitBase");
        //Instantiate PortraitBase
        var dialogBodyMask = transform.Find("Dialog/Mask");
        var portrait = Instantiate(portraitBase, dialogBodyMask);
        portrait.name = "PortraitBase";
        //Add portrait to storyComponent.dialog.Portraits
        var portraitParts = portrait.GetComponent<PortraitParts>();

        portraitParts.speakerID = id;
        
        StoryComponent.DialogMain.Portrait newPortrait = new StoryComponent.DialogMain.Portrait();
        newPortrait.Init(portrait);
        storyComponent.dialog.Portraits.Add(newPortrait);
    }



    private void SetBackground(string name)
    {
        taskRunning = true;
        storyComponent.backGround.image.sprite = storyBackgroundSprites.GetSprite(name);
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
            storyComponent.dialog.bgmSource.volume = float.Parse(args[1]);
        
        storyComponent.dialog.bgmSource.Play();
        taskRunning = false;
        moveNext = true;
    }
    
    private void PlayVoice(string[] args)
    {
        taskRunning = true;
        storyComponent.dialog.voiceSource.Stop();
        
        
        
        
        storyComponent.dialog.voiceSource.clip = audioBundlesTest.GetVoice(args[0]);
        if(args.Length > 1)
            storyComponent.dialog.voiceSource.volume = float.Parse(args[1]);

        if (args.Length > 3)
        {
            if(currentSpeakingChara.portraitParts.speakingAnimationRoutine != null)
                StopCoroutine(currentSpeakingChara.portraitParts.speakingAnimationRoutine);
            
            storyComponent.dialog.voiceSource.Play();
            //TestWaveDetect();
            
            currentSpeakingChara.portraitParts.speakingAnimationRoutine = StartCoroutine
            (CharacterSpeakAnimation(int.Parse(args[2]), int.Parse(args[3]),
                currentSpeakingChara.portraitParts, args[0]));

            var currentFaceIndex = currentSpeakingChara.portraitParts.currentBaseFaceIndex;
            
            currentSpeakingChara.portraitParts.blinkingAnimationRoutine = 
                StartCoroutine
            (CharacterBlinkAnimation(currentFaceIndex,
                currentFaceIndex>=9?9:currentFaceIndex+1, currentSpeakingChara.portraitParts));
            
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
        print(start_times.ToString());

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
            currentSpeakingChara.faceImage.sprite = currentSpeakingChara.portraitParts.GetFaceSprite(eyeClose);
            yield return new WaitForSeconds(0.2f);
            currentSpeakingChara.faceImage.sprite = currentSpeakingChara.portraitParts.GetFaceSprite(eyeOpen);
            yield return new WaitForSeconds(2.5f+Random.Range(0,0.5f));
            
        }
        currentSpeakingChara.faceImage.sprite = currentSpeakingChara.portraitParts.GetFaceSprite(eyeOpen);
        currentSpeakingCharaPortraitParts.blinkingAnimationRoutine = null;
    }


    #endregion

    #region Effects

    void ScreenShake(string[] args)
    {
        
        currentCoroutine = 
            StartCoroutine(ScreenShakeRoutine(float.Parse(args[0]), float.Parse(args[1])));
                
        
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
    void PortraitMove(string[] args)
    {
        switch (args[0])
        {
            case "MOVE":
                if(args.Length != 4)
                    Debug.LogError("Portrait Move Command Error");
                currentCoroutine = 
                    StartCoroutine(PortraitSimpleMoveRoutine(new Vector2(float.Parse(args[1]), float.Parse(args[2])), float.Parse(args[3])));
                break;
            case "SHAKE":
                if(args.Length != 3)
                    Debug.LogError("Portrait Shake Command Error");
                currentCoroutine = 
                    StartCoroutine(PortraitShakeRoutine(float.Parse(args[1]), float.Parse(args[2])));
                break;
            default:
                break;
        }
    }
    
    IEnumerator PortraitSimpleMoveRoutine(Vector2 moveVector, float duration)
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
            
        yield return new WaitUntil(() => taskRunning == false);
        //If tween is not complete
        if (tweenerCore.IsComplete() == false)
        {
            tweenerCore.Kill();
        }
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


    #endregion

    #region Print

    private void ClearDialog()
    {
        taskRunning = true;
        storyComponent.dialog.arrowImg.gameObject.SetActive(false);
        storyComponent.dialog.dialogText.text = "";
        storyComponent.dialog.speakerName.text = "";
        storyComponent.dialog.voiceSource.Stop();
        taskRunning = false;
        moveNext = true;
    }
    
    private void PrintDialog(string[] args,int end)
    {
        if(args.Length == 2) 
            currentCoroutine = StartCoroutine(PrintDialogRoutine(args[0], float.Parse(args[1]),end));
    }
    
    private IEnumerator PrintDialogRoutine(string dialog,float voiceTime, int end)
    {
        taskRunning = true;
        skip = false;
        storyComponent.dialog.arrowImg.gameObject.SetActive(false);
        
        string text = storyComponent.dialog.dialogText.text;
        
        
        var tween = DOTween.To(()=>text, x=>text = x, dialog, voiceTime).OnUpdate(
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
    

    void TestWaveDetect()
    {
        
        float length = storyComponent.dialog.voiceSource.clip.length;
        //获取声音的采样率
        int frequency = storyComponent.dialog.voiceSource.clip.frequency;
        print(frequency);
        float[] samples = new float[(int)length * frequency];
        
        storyComponent.dialog.voiceSource.clip.GetData(samples, 0);
        //将声音的音频数据samples的长度压缩到length * 100，存在新数组newData中
        float[] newData = new float[(int)length * 10];
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = samples[i * (frequency/500)];
        }
        
        
        for (int i = 0; i < newData.Length; i++)
        {
            var volume = Mathf.Abs(newData[i]);
            if (volume <= 0.05f)
            {
                print(i / 20f + "s, " + "没声音");
            }
            else
            {
                print(i / 20f + "s, " + "有声音");
            }

        }
        voiceData = newData;

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
        _globalController.TestEnterLevel(levelName);
    }








}

[Serializable]
public class StoryCommand
{
    public enum CommandType
    {
        BLACKOUT, //黑屏淡出
        BLACKIN, //黑屏
        
        BLACKSCREEN_FADE, //半透明黑屏淡出
        BLACKSCREEN, //半透明黑屏
        BLACKSCREEN_PRINT,//输出全屏文本
        BUTTON_VISIBILITY,//按钮可见性
        
        CLEAR_DIALOG,//清空对话框
        CHARA_FADE,//人物立绘淡入淡出
        
        DIALOG_FADEIN,//对话框淡入
        DIALOG_FADEOUT,//对话框淡出
        
        LOAD_PORTRAIT,//加载人物立绘
        
        PLAY_BGM,//播放背景音乐
        PLAY_VOICE,//播放语音
        PORTRAIT_MOVE,//人物立绘运动
        PRINT_DIALOG,//输出对话
        
        SCREEN_SHAKE,//屏幕震动
        SET_BACKGROUND,//设置背景图片
        SET_CHARA,//设置人物图片
        SET_SPEAKER,//设置人物名字
        
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
        public AudioSource voiceSource;
        public AudioSource bgmSource;
        public GameObject dialogBody;
        public TextMeshProUGUI dialogText;
        public TextMeshProUGUI speakerName;
        public Image arrowImg;
        public Button skipButton;
        public Button storyRecordButton;
        public AudioClip voiceClip;
        public AudioClip bgmClip;
        
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
            voiceClip = null;
            bgmClip = null;

            var dialogBodyMask = GameObject.Find("Dialog").transform.Find("Mask");
            for(int i = 0; i < dialogBodyMask.childCount; i++)
            {
                Portrait portrait = new Portrait();
                portrait.Init(dialogBodyMask.GetChild(i).gameObject);
                Portraits.Add(portrait);
            }

        }
        
    }
    
    public BackGround backGround;
    public DialogMain dialog;

    public void Init()
    {
        backGround = new BackGround();
        backGround.Init();
        dialog = new DialogMain();
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