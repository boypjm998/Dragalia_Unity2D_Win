using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_LevelSelection : MonoBehaviour
{
    public static UI_LevelSelection Instance
    {
        get;
        protected set;
    }

    [Serializable] public class SelectionMenuInfo
    {
        public GameObject menuPrefab;
        [Tooltip("当面板为地图上的首级菜单时，为地图ID。否则为加载出页面Prefab后的后缀（选单）ID。按钮点击事件需要在其id前加1010（必须）。")]public long menuID;
    }

    

    private GameObject contentGameobject;
    [SerializeField] List<SelectionMenuInfo> selectionMenuInfo;
    // Start is called before the first frame update
    public List<SelectionMenuInfo> menuItems => selectionMenuInfo;

    public void Reload(int id)
    {
        //删除子物体中Scroll View/Viewport/Content下的第一个子物体
        if(contentGameobject.transform.childCount > 0)
            Destroy(contentGameobject.transform.GetChild(0).gameObject);
        //查找selectionMenuInfo中id为id的menuPrefab
        foreach (var VARIABLE in selectionMenuInfo)
        {
            if (VARIABLE.menuID == id)
            {
                //实例化menuPrefab
                GameObject menu = Instantiate(VARIABLE.menuPrefab, contentGameobject.transform);
                //设置menu的位置
                //menu.transform.localPosition = VARIABLE.menuPosition;
                print(menu.name);
                AddNewLevels(menu);
                return;
            }
        }
        print("找不到"+id+"对应的menuPrefab");
        //MenuUIManager.Instance.map.ReloadDotsOf(GlobalController.lastQuestSpot);
        
    }

    public int GetDotCountInfo(int menuID)
    {
        var matchedMenu = selectionMenuInfo.Find(x => x.menuID == menuID);
        if (matchedMenu == null)
            return 0;
        var prefab = matchedMenu.menuPrefab;
        var visitedSpots = GlobalController.Instance.gameOptions.visitedQuest;

        //var levelEnterScript = prefab.GetComponent<UI_LevelEnterButton>();
        var subMenuScript = prefab.GetComponent<UI_LevelSubSelection>();
        
        List<string> unviewedLevels = GetUnviewedLevels(prefab);

        // 去重
        unviewedLevels = unviewedLevels.Except(visitedSpots).ToList();

        // 返回未查看的关卡数
        return unviewedLevels.Count;

    }

    public static void AddNewLevels(GameObject menu)
    {
        UI_LevelEnterButton levelEnterButton = menu.GetComponent<UI_LevelEnterButton>();

        if (levelEnterButton == null)
        {
            print("NULL");
            return;
        }
            
        
        var list = levelEnterButton.GetActiveLevelCount();
        
        print(list);

        if (GlobalController.Instance.gameOptions.visitedQuest.Count > 0 &&
            list.Count > 0)
        {
            GlobalController.Instance.gameOptions.visitedQuest = 
                GlobalController.Instance.gameOptions.visitedQuest.Union(list).ToList();
            GlobalController.Instance.InvokeRefreshMapSpotInfo();
        }else if (GlobalController.Instance.gameOptions.visitedQuest.Count == 0)
        {
            GlobalController.Instance.gameOptions.visitedQuest.AddRange(list);
            GlobalController.Instance.InvokeRefreshMapSpotInfo();
        }

    }
    public static List<string> GetUnviewedLevels(GameObject menuPrefab)
    {
        List<string> unviewedLevels = new List<string>();

        // 获取UI_LevelEnterButton组件
        UI_LevelEnterButton levelEnterButton = menuPrefab.GetComponent<UI_LevelEnterButton>();
        if (levelEnterButton != null)
        {
            // 添加未查看的关卡
            unviewedLevels.AddRange(levelEnterButton.GetActiveLevelCount());
        }

        // 获取UI_LevelSubSelection组件
        UI_LevelSubSelection levelSubSelection = menuPrefab.GetComponent<UI_LevelSubSelection>();
        if (levelSubSelection != null)
        {
            // 遍历所有子菜单
            foreach (SelectionMenuInfo subMenu in levelSubSelection.GetAllSubMenu())
            {
                // 递归获取子菜单中的未查看的关卡
                unviewedLevels.AddRange(GetUnviewedLevels(subMenu.menuPrefab));
            }
        }

        return unviewedLevels;
    }
    
    void Awake()
    {
        Instance = this;
        contentGameobject = transform.Find("Scroll View/Viewport/Content").gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if(contentGameobject.transform.childCount <= 0)
            Reload(GlobalController.lastQuestSpot);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    public void AddPanel(SelectionMenuInfo selectionMenuInfo)
    {
        this.selectionMenuInfo.Add(selectionMenuInfo);
    }
}
