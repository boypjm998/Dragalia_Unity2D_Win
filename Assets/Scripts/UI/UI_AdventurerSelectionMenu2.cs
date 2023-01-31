using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AdventurerSelectionMenu2 : MonoBehaviour
{
    private UI_AdventurerSelectionMenu upperMenu;

    private void Awake()
    {
        upperMenu = FindObjectOfType<UI_AdventurerSelectionMenu>();
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
        upperMenu.ChangeCurrentSelectedCharacter(id);
        
    }
}
