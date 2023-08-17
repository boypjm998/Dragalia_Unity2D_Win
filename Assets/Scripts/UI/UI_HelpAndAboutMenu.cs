using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HelpAndAboutMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TextMeshProUGUI contentTMP;
    [SerializeField] private TextAsset localizedTextInfo;
    [SerializeField] private GameObject buttonGameObject;
    [SerializeField] private GameObject buttonSample;
    private HelpMenuInfo _helpMenuInfoList;

    [Serializable]
    public class HelpMenuTextInfo
    {
        public string title;
        public string content;
    }
    [Serializable]
    public class HelpMenuInfo
    {
        public List<HelpMenuTextInfo> help_list = new();
    }


    private void OnEnable()
    {
        ReloadPanel(0);
    }

    void Awake()
    {
        _helpMenuInfoList = JsonUtility.FromJson<HelpMenuInfo>(localizedTextInfo.text);
        for (int i = 0; i < buttonGameObject.transform.childCount; i++)
        {
            buttonGameObject.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text =
                _helpMenuInfoList.help_list[i].title;
        }
        
        if(_helpMenuInfoList.help_list.Count > buttonGameObject.transform.childCount)
            for (int i = buttonGameObject.transform.childCount; i < _helpMenuInfoList.help_list.Count; i++)
            {
                GameObject newButton = Instantiate(buttonSample, buttonGameObject.transform);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text =
                    _helpMenuInfoList.help_list[i].title;
                var btn = newButton.GetComponent<Button>();
                //btn.onClick = new Button.ButtonClickedEvent();
                int index = i;
                btn.onClick.AddListener(()=>ReloadPanel(index));
            }
        
    }

    public void ReloadPanel(int id)
    {
        print("ReloadPanel"+id);
        titleTMP.text = _helpMenuInfoList.help_list[id].title;
        contentTMP.text = _helpMenuInfoList.help_list[id].content;
    }
}
