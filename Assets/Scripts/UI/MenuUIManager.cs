using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public int GUIAnimCount { get; protected set; } = 0;
    //private Coroutine animRoutine = null;
    private Sequence _sequence;
    public static int highestMenu = 0;
    //public int lastDisabledUIGroup;
    private Dictionary<string, UISortingGroup> UIDict;
    private GlobalController _globalController;
    public Stack<int> menuLevelStack;
    
    public enum UIState
    {
        Active,
        Inactive,
        Hiding
    }

    private void Awake()
    {
        menuLevelStack = new Stack<int>();
        menuLevelStack.Push(0);
        InitAllChildrenElements();
    }

    private void Start()
    {
        
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
            if(!ui.isActive)
             ui.gameObject.SetActive(false);
        }
        
    }




    public IEnumerator HideGUI(GameObject obj,Vector3 localPosition, float animStartTime,float animDuration)
    {
        GUIAnimCount++;

        ActiveGUI(obj,false);

        yield return new WaitForSecondsRealtime(animStartTime);

        //obj.transform.localPosition = obj.GetComponent<UISortingGroup>().initialLocalPosition;
        obj.transform.DOLocalMove(obj.transform.localPosition - localPosition, animDuration);
        
        yield return new WaitForSecondsRealtime(animDuration);
        
        //_sequence.Append(obj.transform.DOLocalMove(localPosition, 0.5f));
        obj.SetActive(false);

        GUIAnimCount--;
    }

    private IEnumerator HideGUI(GameObject obj, float animStartTime, float animDuration)
    {
        GUIAnimCount++;
        
        ActiveGUI(obj,false);
        obj.transform.localScale = new Vector3(1,1,1);

        yield return new WaitForSecondsRealtime(animStartTime);

        obj.transform.DOScale(new Vector3(.1f, .1f, .1f), animDuration);
        
        yield return new WaitForSecondsRealtime(animDuration);
        
        //_sequence.Append(obj.transform.DOLocalMove(localPosition, 0.5f));
        obj.SetActive(false);

        GUIAnimCount--;
    }
    private IEnumerator HideGUI(CanvasGroup canv, float animStartTime, float animDuration)
    {
        GUIAnimCount++;
        ActiveGUI(canv.gameObject,false);
        canv.alpha = 1;

        yield return new WaitForSecondsRealtime(animStartTime);

        canv.DOFade(0, animDuration);
        
        yield return new WaitForSecondsRealtime(animDuration);
        
        //_sequence.Append(obj.transform.DOLocalMove(localPosition, 0.5f));
        canv.gameObject.SetActive(false);
        GUIAnimCount--;
    }

    public IEnumerator DisplayGUI(GameObject obj,Vector3 localPosition, float animStartTime,float animDuration)
    {
        GUIAnimCount++;
        
        
        obj.SetActive(true);
        
        yield return new WaitForSecondsRealtime(animStartTime);

        //obj.transform.localPosition = obj.GetComponent<UISortingGroup>()._hideLocalPosition;
        obj.transform.DOLocalMove(obj.transform.localPosition - localPosition, animDuration);

        yield return new WaitForSecondsRealtime(animDuration);
        
        ActiveGUI(obj,true);
        GUIAnimCount--;

    }
    
    private IEnumerator DisplayGUI(GameObject obj, float animStartTime,float animDuration)
    {
        
        GUIAnimCount++;
        obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        yield return new WaitForSecondsRealtime(animStartTime); 
        
        obj.SetActive(true);
        obj.transform.DOScale(Vector3.one, animDuration);

        yield return new WaitForSecondsRealtime(animDuration);
        
        ActiveGUI(obj,true);
        GUIAnimCount--;

    }
    
    private IEnumerator DisplayGUI(CanvasGroup canv, float animStartTime,float animDuration)
    {
        GUIAnimCount++;
        canv.alpha = 0;
        //obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        yield return new WaitForSecondsRealtime(animStartTime); 
        
        canv.gameObject.SetActive(true);
        canv.DOFade(1, animDuration);

        yield return new WaitForSecondsRealtime(animDuration);
        
        ActiveGUI(canv.gameObject,true);
        GUIAnimCount--;
        //177

    }

    public static void ActiveGUI(GameObject obj, bool flag)
    {
        var canvasGroup = obj.GetComponent<CanvasGroup>();

        
        //print($"{(flag?"active":"disable")}_group {obj.name}");
        canvasGroup.interactable = flag; 
        return;
        

        // var buttons = obj.GetComponentsInChildren<Button>();
        //
        // foreach (var button in buttons)
        // {
        //     button.interactable = flag;
        //     
        // }
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
        else if (_parent._hideDirection == UISortingGroup.HideDirection.Alpha)
        {
            StartCoroutine(HideGUI(_parent.GetComponent<CanvasGroup>(), startTime, duration));
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
        else if (_parent._hideDirection == UISortingGroup.HideDirection.Alpha)
        {
            StartCoroutine(DisplayGUI(_parent.GetComponent<CanvasGroup>(), startTime, duration));
        }
        else
        {
            StartCoroutine
            (DisplayGUI
                (_parent.gameObject, -_parent._hideLocalPosition, startTime, duration));
        }


    }

    public void DisplayInstant(UISortingGroup _parent)
    {
        if(_parent.isActive)
            return;

        //var _parent = obj.GetComponent<UISortingGroup>();
        _parent.isActive = true;

        if (_parent._hideDirection == UISortingGroup.HideDirection.Zoom)
        {
            //throw new NotImplementedException();
            _parent.transform.localPosition = _parent.initialPosition;
            _parent.gameObject.SetActive(true);
            _parent.transform.localScale = Vector3.one;
            ActiveGUI(_parent.gameObject,true);
            
        }
        else if (_parent._hideDirection == UISortingGroup.HideDirection.Alpha)
        {
            var canv = _parent.GetComponent<CanvasGroup>();
            canv.alpha = 1;
            canv.gameObject.SetActive(true);
            ActiveGUI(canv.gameObject,true);
            //StartCoroutine(DisplayGUI(_parent.GetComponent<CanvasGroup>(), startTime, duration));
        }
        else
        {
            throw new NotImplementedException();
            //StartCoroutine
            //(DisplayGUI
            //(_parent.gameObject, -_parent._hideLocalPosition, startTime, duration));
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
    
    private void HideAllWithID(int sortingGroupID, float startTime, float duration)
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.sortingGroupID == sortingGroupID)
            {
                Hide(ui,startTime,duration);
            }
        }
        
    }

    private UISortingGroup InstantiateGUI(string name, float startTime)
    {
        UISortingGroup uiSortingGroup;
        if (!UIDict.ContainsKey(name))
        {
            var ab = _globalController.GetBundle("iconsmall");
            var asset = ab.LoadAsset<GameObject>(name);
            var obj = Instantiate(asset, transform);
            obj.SetActive(false);
            obj.name = name;
            uiSortingGroup = obj.GetComponent<UISortingGroup>();
            UIDict.Add(name,uiSortingGroup);
        }
        else
        {
            uiSortingGroup = UIDict[name];
        }

        return uiSortingGroup;
    }
    

    private void DisableAll()
    {
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            ActiveGUI(ui.gameObject,false);
            
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

    private void EnableOne(string name, float waitTime)
    {
        StartCoroutine(WaitGUI(waitTime));
        var all = FindObjectsOfType<UISortingGroup>();
        foreach (var ui in all)
        {
            if (ui.name == name)
            {
                ActiveGUI(ui.gameObject,true);
            }
        }
    }

    private IEnumerator WaitGUI(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
    }

    private IEnumerator ReloadLevelSelectionMenuRoutine(int menuID)
    {
        GUIAnimCount++;
        var menuObj = UIDict["LevelSelectionMenu"].gameObject;
        Hide(menuObj.GetComponent<UISortingGroup>(), 0, 0.15f);
        yield return new WaitForSecondsRealtime(0.15f);
        menuObj.GetComponent<UI_LevelSelection>().Reload(menuID);
        Display(menuObj.GetComponent<UISortingGroup>(), 0, 0.15f);
        yield return new WaitForSecondsRealtime(0.15f);
        GUIAnimCount--;
    }


    public void EnterLevel(int questID)
    {
        DisableAll();
        
        string questIDStr = questID.ToString();
        //如果questIDStr长度不足6位，就在前面补0
        questIDStr = questIDStr.PadLeft(6,'0');
        
        _globalController.TestEnterLevel(questIDStr);
    }

    public void ToNextUIState(int toState,bool animation = true)
    {
        SwitchUIState(toState,animation);
    }
    public void ToPreviousUIState(bool animation = true)
    {
        menuLevelStack.Pop();
        //打印出全部的栈
        foreach (var i in menuLevelStack)
        {
            print(i);
        }
        
        SwitchUIState(menuLevelStack.Peek(),animation,true);
    }
    
    public void SwitchUIState(int toState,bool animation,bool pop = false)
    {
        print("SwitchUIStateToState:"+toState);
        string[] activeList = {};
        string[] inactiveList = {};
        /*
         * 0: 主菜单
         * 101: 地图
         * 1010: 关卡选单
         * 1010(XXXYZ): XXX为地图ID，Y为关卡ID，Z为难度ID
         * 102: 角色选择
         * 103: 设置
         * 1021: 角色详细信息
         */
        
        //如果toState的前四位是1010，return
        if (toState.ToString().Length == 9 && toState.ToString()[..4] == "1010")
        {
            SwitchLevelSelectionMenu(toState,pop);
            return;
        }
        
        switch (toState)
        {
            case 0:
            {
                activeList = new[] {"MenuButton_01"};
                break;
            }
            case 101:
            {
                activeList = new[] {"MapView","MenuButton_02","MenuButton_04"};
                break;
            }
            case 1010:
            {
                activeList = new[] {"LevelSelection","MenuButton_02"};
                inactiveList = new[] {"MapView"};
                break;
            }
            case 102:
            {
                activeList = new[] {"CharacterSelection","CharacterInfoMenu","MenuButton_03"};
                break;
            }
            case 1021:
            {
                activeList = new[] {"DetailedInfoMenu"};
                inactiveList = new[] {"CharacterSelection","CharacterInfoMenu","MenuButton_03"};
                break;
            }
            case 103:
            {
                activeList = new[] { "GameOption", "MenuButton_03" };
                break;
            }

        }
        
        if(animation)
            DOUIState(activeList, inactiveList, animation);
        else
        {
            DOUIState(activeList,inactiveList, false);
        }

        if(!pop)
            menuLevelStack.Push(toState);

    }

    private void SwitchLevelSelectionMenu(int menuID,bool pop)
    {
        StartCoroutine(ReloadLevelSelectionMenuRoutine(menuID));
        if(!pop)
            menuLevelStack.Push(menuID);
    }


    private void DOUIState(string[] actives, string[] inactives, bool animation)
    {
        UISortingGroup selectedUI;
        float animTime = 0;
        var activeList = actives.ToList();
        var inactiveList = inactives.ToList();

        foreach (var hide in UIDict.Keys)
        {
            // 如果hide已经在activeList或者inactiveList数组中，跳过
            if (activeList.Contains(hide) || inactiveList.Contains(hide))
            {
                continue;
            }
            selectedUI = UIDict[hide];
            if (selectedUI.gameObject.activeSelf)
            {
                if (animation)
                {
                    animTime = selectedUI.hideTime;
                    Hide(UIDict[hide],0,animTime);
                }
                else
                {
                    Hide(UIDict[hide],0,0);
                }


            }
            
        }

        
        
        foreach (var active in activeList)
        {
            //print(active);
            // 如果字典中没有active，实例化并显示

            if (!UIDict.ContainsKey(active))
            {
                selectedUI = InstantiateGUI(active,animTime);
                Display(selectedUI,animTime,animation?selectedUI.hideTime:0);
            }
            else
            {
                selectedUI = UIDict[active];
                if (selectedUI.gameObject.activeSelf)
                {
                    EnableOne(active,animTime);
                }

                else
                {
                    if(animation)
                        Display(selectedUI,animTime,selectedUI.hideTime);
                    else Display(selectedUI,animTime,0);
                }
            }
            
            foreach (var inactive in inactives)
            {
                if(UIDict[inactive].gameObject.activeSelf)
                    DisableOne(inactive);
                else
                {
                    DisplayInstant(UIDict[inactive]);
                    //Display(UIDict[inactive],0,0);
                    //DisableOne(inactive);
                    Debug.LogWarning($"Inactive UI {inactive} is hidden.");
                }
            }


        }
        
        
        
    }

    private void DOUIStateInstantly(string[] actives, string[] inactives)
    {
        foreach (var key in UIDict.Keys)
        {
            if (actives.Contains(key))
            {
                Display(UIDict[key],0,0);
                continue;
                
            }

            if (inactives.Contains(key))
            {
                DisableOne(key);
                continue;    
            }
            
            Hide(UIDict[key],0,0);
        }
    }


    private void Update()
    {
        //print(menuLevelStack.Count);
    }
}
