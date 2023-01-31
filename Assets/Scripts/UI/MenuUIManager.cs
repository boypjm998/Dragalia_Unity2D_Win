using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    private Coroutine animRoutine = null;
    private Sequence _sequence;
    public static int highestMenu = 0;
    public int lastDisabledUIGroup;
    private Dictionary<string, UISortingGroup> UIDict;
    private GlobalController _globalController;

    private void Awake()
    {
        
    }

    private void Start()
    {
        InitAllChildrenElements();
        _globalController = FindObjectOfType<GlobalController>();
        

        
    }

    public void InitAllChildrenElements()
    {
        UIDict = new Dictionary<string, UISortingGroup>();
        var uiSortingGroups = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in uiSortingGroups)
        {
            UIDict.Add(ui.gameObject.name,ui);
        }
        foreach (var ui in uiSortingGroups)
        {
            //if(!ui.isActive)
             //ui.gameObject.SetActive(false);
        }
        
    }




    public static IEnumerator HideGUI(GameObject obj,Vector3 localPosition, float animStartTime,float animDuration)
    {

        ActiveGUI(obj,false);

        yield return new WaitForSecondsRealtime(animStartTime);

        obj.transform.DOLocalMove(obj.transform.localPosition - localPosition, animDuration);
        
        yield return new WaitForSecondsRealtime(animDuration);
        
        //_sequence.Append(obj.transform.DOLocalMove(localPosition, 0.5f));
        obj.SetActive(false);

    }

    private IEnumerator HideGUI(GameObject obj, float animStartTime, float animDuration)
    {
        ActiveGUI(obj,false);
        obj.transform.localScale = new Vector3(1,1,1);

        yield return new WaitForSecondsRealtime(animStartTime);

        obj.transform.DOScale(new Vector3(.1f, .1f, .1f), animDuration);
        
        yield return new WaitForSecondsRealtime(animDuration);
        
        //_sequence.Append(obj.transform.DOLocalMove(localPosition, 0.5f));
        obj.SetActive(false);
    }

    public static IEnumerator DisplayGUI(GameObject obj,Vector3 localPosition, float animStartTime,float animDuration)
    {
        obj.SetActive(true);
        
        yield return new WaitForSecondsRealtime(animStartTime);

        print(localPosition);
        obj.transform.DOLocalMove(obj.transform.localPosition - localPosition, animDuration);

        yield return new WaitForSecondsRealtime(animDuration);
        
        ActiveGUI(obj,true);
        //

    }
    
    private IEnumerator DisplayGUI(GameObject obj, float animStartTime,float animDuration)
    {
        obj.SetActive(true);
        obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        yield return new WaitForSecondsRealtime(animStartTime);

        obj.transform.DOScale(Vector3.one, animDuration);

        yield return new WaitForSecondsRealtime(animDuration);
        
        ActiveGUI(obj,true);
        //

    }

    public static void ActiveGUI(GameObject obj, bool flag)
    {
        var buttons = obj.GetComponentsInChildren<Button>();
        
        foreach (var button in buttons)
        {
            button.interactable = flag;
            
        }
    }

    public static void SetMaxMenuLevel(int id)
    {
        highestMenu = id;
    }
    public static int GetMaxMenuLevel()
    {
        return highestMenu;
    }

    private void Hide(UISortingGroup _parent, float startTime,float duration)
    {
        if(!_parent.isActive)
            return;

        _parent.isActive = false;

        if (_parent._hideDirection == UISortingGroup.HideDirection.Zoom)
        {
            StartCoroutine
            (HideGUI
                (_parent.gameObject, startTime, duration));
        }
        else
        {
            StartCoroutine
            (HideGUI
                (_parent.gameObject, _parent._hideLocalPosition, startTime, duration));
        }

        


    }
    public void Display(UISortingGroup _parent, float startTime,float duration)
    {
        if(_parent.isActive)
            return;

        //var _parent = obj.GetComponent<UISortingGroup>();
        _parent.isActive = true;

        if (_parent._hideDirection == UISortingGroup.HideDirection.Zoom)
        {
            _parent.transform.localPosition = _parent.initialPosition;
            StartCoroutine
            (DisplayGUI
                (_parent.gameObject, startTime, duration));
        }
        else
        {
            StartCoroutine
            (DisplayGUI
                (_parent.gameObject, -_parent._hideLocalPosition, startTime, duration));
        }


    }

    private void HideAll(int level, float startTime, float duration)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.LayerID == level || level<0)
            {
                Hide(ui,startTime,duration);
            }
        }
        
    }
    
    private void DisplayAll(int level, float startTime, float duration)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.LayerID == level)
            {
                Display(ui,startTime,duration);
            }
        }
        
    }

    private void DisableAll(int level)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.LayerID == level || level<0)
            {
                ActiveGUI(ui.gameObject,false);
            }
        }
    }
    private void EnableAll(int level)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.LayerID == level || level<0)
            {
                ActiveGUI(ui.gameObject,true);
            }
        }
    }

    private void DisableOne(string name)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.name == name)
            {
                ActiveGUI(ui.gameObject,false);
            }
        }
    }
    private void EnableOne(string name)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.name == name)
            {
                ActiveGUI(ui.gameObject,true);
            }
        }
    }

    public void MenuButtonClickEvent(string btnName)
    {
        switch (btnName)
        {
            case "StartButton":
            {
                Hide(UIDict["MenuButton_01"],0,0.3f);
                Display(UIDict["LevelSelection"],0.3f,0.3f);
                Display(UIDict["MenuButton_02"],0.3f,0.3f);
                SetMaxMenuLevel(1);
                break;
            }
            case "ReturnButton":
            {
                var currentMaxMenuLevel = GetMaxMenuLevel();
                HideAll(currentMaxMenuLevel,0,0.3f);
                SetMaxMenuLevel(currentMaxMenuLevel--);
                if (currentMaxMenuLevel == 0)
                {
                    Display(UIDict["MenuButton_01"],0.3f,0.3f);
                }

                break;
            }
            case "LevelSelectionButton":
            {
                DisableAll(GetMaxMenuLevel());
                _globalController.TestEnterLevel();
                break;
            }
            case "CharacterSelectionButton":
            {
                Hide(UIDict["MenuButton_01"],0,0.3f);
                Display(UIDict["CharacterSelection"],0.3f,0.3f);
                Display(UIDict["CharacterInfoMenu"],0.3f,0.3f);
                Display(UIDict["MenuButton_03"],0.3f,0.3f);
                SetMaxMenuLevel(1);
                break;
            }
            case "CloseInfoMenu":
            {
                Hide(UIDict["DetailedInfoMenu"],0f,0.3f);
                EnableOne("CharacterSelection");
                EnableOne("CharacterInfoMenu");
                EnableOne("MenuButton_03");
                SetMaxMenuLevel(1);
                break;
            }
            case "OpenInfoMenu":
            {
                DisableAll(GetMaxMenuLevel());
                Display(UIDict["DetailedInfoMenu"],0f,0.3f);
                SetMaxMenuLevel(2);
                break;
            }
            default: break;
        }
    }

}
