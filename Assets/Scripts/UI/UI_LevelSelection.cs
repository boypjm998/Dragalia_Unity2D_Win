using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LevelSelection : MonoBehaviour
{
    
    [Serializable] public class SelectionMenuInfo
    {
        public GameObject menuPrefab;
        public int menuID;
    }

    private GameObject contentGameobject;
    [SerializeField] List<SelectionMenuInfo> selectionMenuInfo;
    // Start is called before the first frame update
    
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
                break;
            }
        }
        
        
    }
    
    
    void Awake()
    {
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
}
