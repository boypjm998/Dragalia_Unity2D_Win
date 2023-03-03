using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuButton : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalController.currentGameState == GlobalController.GameState.Inbattle)
        {
            _button.interactable = true;
        }
        else _button.interactable = false;
    }
}
