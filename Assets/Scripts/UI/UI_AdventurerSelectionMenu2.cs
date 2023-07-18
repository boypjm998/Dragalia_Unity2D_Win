using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AdventurerSelectionMenu2 : MonoBehaviour
{
    [SerializeField] private UI_AdventurerSelectionMenu upperMenu;

    private Transform ContentTransform;

    private void Awake()
    {
        //upperMenu = FindObjectOfType<UI_AdventurerSelectionMenu>();
        if (upperMenu == null)
        {
            upperMenu = GameObject.Find("UI").transform.Find("CharacterInfoMenu").GetComponent<UI_AdventurerSelectionMenu>();
        }
        ContentTransform = transform.Find("Scroll View/Viewport/Content");
    }

    private void Start()
    {
        RedirectSelectionArrow(GlobalController.currentCharacterID);
    }

    // Start is called before the first frame update
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public void ChooseCharacter(int id)
    {
        if (upperMenu.currentSelectedCharaID == id && ContentTransform.GetChild(id-1).childCount==1)
        {
            GlobalController.currentCharacterID = id;
            RedirectSelectionArrow(id);
        }
        upperMenu.ChangeCurrentSelectedCharacter(id);
    }
    
    private void RedirectSelectionArrow(int id)
    {
        id = id - 1;
        print(id);
        for(int i = 0 ; i < ContentTransform.childCount; i++)
        {
            print(ContentTransform.GetChild(i).GetChild(0).name);
            if (i == id && ContentTransform.GetChild(i).childCount == 1)
            {
                print(ContentTransform.GetChild(i).Find("Light").gameObject);
                ContentTransform.GetChild(i).Find("Light").gameObject.SetActive(true);
            }
            else
            {
                //print(ContentTransform.GetChild(i).Find("Light").gameObject);
                ContentTransform.GetChild(i).Find("Light")?.gameObject.SetActive(false);
            }
        }
    }
}
