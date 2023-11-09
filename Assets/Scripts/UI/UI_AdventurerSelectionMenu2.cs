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
        //Find({id}).childCount == 1
        if (upperMenu.currentSelectedCharaID == id && ContentTransform.Find($"{id}").childCount != 2)
        {
            GlobalController.currentCharacterID = id;
            RedirectSelectionArrow(id);
            print(GlobalController.currentCharacterID);
        }
        upperMenu.ChangeCurrentSelectedCharacter(id);
    }
    
    private void RedirectSelectionArrow(int id)
    {
        //id = id - 1;
        print(id);
        for(int i = 0 ; i < ContentTransform.childCount; i++)
        {
            var child = ContentTransform.GetChild(i);
            if (child.name == id.ToString() && child.childCount != 2)
            {
                print(child.Find("Light").gameObject);
                child.Find("Light").gameObject.SetActive(true);
            }
            else
            {
                //print(ContentTransform.GetChild(i).Find("Light").gameObject);
                child.Find("Light")?.gameObject.SetActive(false);
            }
        }
    }
}
