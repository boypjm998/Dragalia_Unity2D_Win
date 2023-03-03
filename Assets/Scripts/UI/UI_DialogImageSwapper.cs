using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogImageSwapper : MonoBehaviour
{

    private Image _image;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        
       
    }

    public void LoadFaceExpression(int speakerID, int expID)
    {
        var path = $"UI/InBattle/DialogIcon/Texture2D/dialog_icon_{speakerID}_{expID}";
        //print(path);
        var sprite = Resources.Load<Sprite>
            (path);
        //print(sprite.name);
        _image.sprite = sprite;
    }

    // Update is called once per frame
    
}
