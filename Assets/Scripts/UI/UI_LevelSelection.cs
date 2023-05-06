using System;
using System.Collections;
using System.Collections.Generic;
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
                print(VARIABLE.menuID);
                return;
            }
        }
        print("找不到"+id+"对应的menuPrefab");
        
        
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
