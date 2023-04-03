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
    private UI_DialogImageSwapper _swapper;
    private Image portraitIcon;
    private TextMeshProUGUI tmp;
    private GameObject balloons;
    private Queue<DialogFormat> dialogQueue;
    private Coroutine displayRoutine;
    private JsonData questDialogInfoData;


    // Start is called before the first frame update
    void Start()
    {
        dialogQueue = new Queue<DialogFormat>();
        portraitIcon = transform.Find("Icon").GetComponent<Image>();
        _swapper = transform.Find("Icon").GetComponent<UI_DialogImageSwapper>();
        balloons = transform.Find("Balloons").gameObject;
        questDialogInfoData = 
            BasicCalculation.ReadJsonData("LevelInformation/QuestDialogInfo.json");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogQueue.Count > 0 && displayRoutine == null)
        {
            displayRoutine = StartCoroutine(DisplayDialog());
        }
    }

    public void EnqueueDialog(int speakerID,int clipID, AudioSource audioSource, AudioClip clip)
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


    private IEnumerator DisplayDialog()
    {
        var dialog = dialogQueue.Dequeue();
        if (dialog.audioSource != null)
        {
            dialog.audioSource.PlayOneShot(dialog.clip);
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
        
        yield return new WaitForSeconds(0.5f + dialog.lastTime_10/10f);
        
        DOTween.To(() => image.color, x => image.color = x,
            new Color(1,1,1,0), 
            0.5f);
        DOTween.To(() => tmp.color, x => tmp.color = x,
            Color.clear, 
            0.5f);
        DOTween.To(() => portraitIcon.color, x => portraitIcon.color = x,
            new Color(1,1,1,0), 
            0.5f);

        yield return new WaitForSeconds(0.5f);
        balloon.SetActive(false);
        displayRoutine = null;
    }


    [Serializable]
    class DialogFormat
    {
        public int speakerID;
        public string text;
        public int ballonType;
        public int faceExpression;
        public int lastTime_10;
        public AudioSource audioSource;
        public AudioClip clip;
    }
}
