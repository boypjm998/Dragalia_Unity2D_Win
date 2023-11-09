using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameMechanics;
public class UI_DialogDisplayer : MonoBehaviour
{
    protected UI_DialogImageSwapper _swapper;
    protected Image portraitIcon;
    protected TextMeshProUGUI tmp;
    protected GameObject balloons;
    protected Queue<DialogFormat> dialogQueue;
    protected Coroutine displayRoutine;
    protected JsonData questDialogInfoData;
    protected JsonData questDialogInfoDataBasic;
    protected int currentPrority = 0;
    protected string currentStoryID;

    public static UI_DialogDisplayer Instance { get; private set; }



    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogQueue = new Queue<DialogFormat>();
        portraitIcon = transform.Find("Icon").GetComponent<Image>();
        _swapper = transform.Find("Icon").GetComponent<UI_DialogImageSwapper>();
        balloons = transform.Find("Balloons").gameObject;
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            questDialogInfoData = 
                BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfo_EN.json");
        }
        else
        {
            questDialogInfoData = 
                BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfo.json");
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (dialogQueue.Count > 0)
        {
            if (displayRoutine == null)
            {
                displayRoutine = StartCoroutine(DisplayDialog());
            }else if(dialogQueue.Peek().priority > currentPrority)
            {
                StopCoroutine(displayRoutine);
                displayRoutine = StartCoroutine(DisplayDialog());
            }
        }
    }
    
    
    public void LoadBasicStoryInfo(string storyID)
    {
        currentStoryID = storyID;
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            questDialogInfoDataBasic = 
                BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfoBasic_EN.json");
        }
        else
        {
            questDialogInfoDataBasic = 
                BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfoBasic.json");
        }
    }
    
    
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speakerID"></param>
    /// <param name="clipID">用来寻找声音来源的</param>
    /// <param name="audioSource"></param>
    /// <param name="clip"></param>
    public virtual void EnqueueDialog(int speakerID,int clipID, AudioSource audioSource, AudioClip clip)
    {
        if(clipID < 0)
            return;
        var dialogInfo = questDialogInfoData[$"SPEAKER_{speakerID}"][$"DIALOG_CLIP_{clipID}"];
        DialogFormat dialogFormat = new DialogFormat();
        dialogFormat.text = dialogInfo["TEXT"].ToString();
        dialogFormat.ballonType = Convert.ToInt32(dialogInfo["BALLOON"].ToString());
        dialogFormat.faceExpression = Convert.ToInt32(dialogInfo["FACE"].ToString());
        dialogFormat.lastTime_10 = Convert.ToInt32(dialogInfo["TIME"].ToString());
        dialogFormat.speakerID = speakerID;
        dialogFormat.audioSource = audioSource;
        dialogFormat.clip = clip;
        dialogQueue.Enqueue(dialogFormat);
    }

    public void EnqueueDialogShared(int speakerID, int clipID, AudioClip clip)
    {
        if(clipID < 0)
            return;
        var dialogInfo = questDialogInfoData[$"SPEAKER_{speakerID}"][$"DIALOG_CLIP_{clipID}"];
        DialogFormat dialogFormat = new DialogFormat();
        dialogFormat.text = dialogInfo["TEXT"].ToString();
        dialogFormat.ballonType = Convert.ToInt32(dialogInfo["BALLOON"].ToString());
        dialogFormat.faceExpression = Convert.ToInt32(dialogInfo["FACE"].ToString());
        dialogFormat.lastTime_10 = Convert.ToInt32(dialogInfo["TIME"].ToString());
        dialogFormat.speakerID = speakerID;
        dialogFormat.audioSource = BattleEffectManager.Instance.sharedVoiceSource;
        dialogFormat.clip = clip;
        dialogQueue.Enqueue(dialogFormat);
    }

    public void EnqueueDialogSharedInBasicStory(int speakerID, int clipID, AudioClip clip)
    {
        if(clipID < 0)
            return;
        var dialogInfo = questDialogInfoDataBasic[$"QUEST_{currentStoryID}"]["dialog_list"][clipID];
        DialogFormat dialogFormat = new DialogFormat();
        dialogFormat.text = dialogInfo["TEXT"].ToString();
        dialogFormat.ballonType = Convert.ToInt32(dialogInfo["BALLOON"].ToString());
        dialogFormat.faceExpression = Convert.ToInt32(dialogInfo["FACE"].ToString());
        dialogFormat.lastTime_10 = Convert.ToInt32(dialogInfo["TIME"].ToString());
        dialogFormat.speakerID = speakerID;
        dialogFormat.audioSource = BattleEffectManager.Instance.sharedVoiceSource;
        dialogFormat.clip = clip;
        dialogQueue.Enqueue(dialogFormat);
    }


    public void EnqueueDialog(int speakerID, string dialogInfoName, AudioSource audioSource, AudioClip clip)
    {
        var dialogInfo = questDialogInfoData[dialogInfoName];
        DialogFormat dialogFormat = new DialogFormat();
        dialogFormat.text = dialogInfo["TEXT"].ToString();
        dialogFormat.ballonType = Convert.ToInt32(dialogInfo["BALLOON"].ToString());
        dialogFormat.faceExpression = Convert.ToInt32(dialogInfo["FACE"].ToString());
        dialogFormat.lastTime_10 = Convert.ToInt32(dialogInfo["TIME"].ToString());
        dialogFormat.priority = Convert.ToInt32(dialogInfo["PRIORITY"].ToString());
        dialogFormat.speakerID = speakerID;
        dialogFormat.audioSource = audioSource;
        dialogFormat.clip = clip;
        dialogQueue.Enqueue(dialogFormat);
    }
    
    


    private IEnumerator DisplayDialog()
    {
        var dialog = dialogQueue.Dequeue();
        if (dialog.audioSource != null)
        {
            dialog.audioSource.Stop();
            dialog.audioSource.clip = dialog.clip;
            dialog.audioSource.Play();
            currentPrority = dialog.priority;
            //dialog.audioSource.PlayOneShot(dialog.clip);
        }
        GameObject balloon;
        if (dialog.ballonType == 1)
        {
            balloon = balloons.transform.GetChild(0).gameObject;
        }else if (dialog.ballonType == 2)
        {
            balloon = balloons.transform.GetChild(1).gameObject;
        }
        else
        {
            Debug.LogWarning("NotFountBallonType");
            yield break;
        }

        balloon.SetActive(true);
        _swapper.LoadFaceExpression(dialog.speakerID,dialog.faceExpression);
        tmp = balloon.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = dialog.text;

        var image = balloon.GetComponent<Image>();

        image.color = new Color(1, 1, 1, 0);
        tmp.color = Color.clear;
        portraitIcon.color = new Color(1, 1, 1, 0);
        
        DOTween.To(() => image.color, x => image.color = x,
            Color.white, 
            0.5f);
        DOTween.To(() => tmp.color, x => tmp.color = x,
            Color.black, 
            0.5f);
        DOTween.To(() => portraitIcon.color, x => portraitIcon.color = x,
            Color.white, 
            0.5f);
        
        yield return new WaitForSeconds(0.3f + dialog.lastTime_10/10f);
        
        DOTween.To(() => image.color, x => image.color = x,
            new Color(1,1,1,0), 
            0.5f);
        DOTween.To(() => tmp.color, x => tmp.color = x,
            Color.clear, 
            0.5f);
        DOTween.To(() => portraitIcon.color, x => portraitIcon.color = x,
            new Color(1,1,1,0), 
            0.5f);

        yield return new WaitForSeconds(0.3f);
        balloon.SetActive(false);
        displayRoutine = null;
    }


    [Serializable]
    protected class DialogFormat
    {
        public int speakerID;
        public string text;
        public int ballonType;
        public int faceExpression;
        public int lastTime_10;
        public int priority;
        public AudioSource audioSource;
        public AudioClip clip;
    }
}
