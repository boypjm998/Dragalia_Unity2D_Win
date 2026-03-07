using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogImageSwapper : MonoBehaviour
{

    private Image _image;
    // Start is called before the first frame update
    private Dictionary<string, Sprite> _loadedSprites = new Dictionary<string, Sprite>();



    void Start()
    {
        _image = GetComponent<Image>();
        
    }

    public void LoadFaceExpression(int speakerID, int expID)
    {
        var path = $"UI/InBattle/DialogIcon/Texture2D/dialog_icon_{speakerID}_{expID}";

        if (_loadedSprites.ContainsKey(path))
        {
            _image.sprite = _loadedSprites[path];
            return;
        }
        
        var sprite = Resources.Load<Sprite>
            (path);
        _loadedSprites.Add(path, sprite);
        
        _image.sprite = sprite;
    }
    
    public void UnloadFaceExpression()
    {
        if (_loadedSprites.Count >= 20)
        {
            foreach (var key in _loadedSprites.Keys)
            {
                Resources.UnloadAsset(_loadedSprites[key]);
            }
            _loadedSprites.Clear();
        }
    }

    // Update is called once per frame
    
}
