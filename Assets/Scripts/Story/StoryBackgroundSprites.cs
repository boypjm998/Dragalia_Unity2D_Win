using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryBackgroundSprites : MonoBehaviour
{
    [SerializeField] private List<Sprite> _backgroundSprites;
    // Start is called before the first frame update
   
    public Sprite GetSprite(string name)
    {
        foreach (Sprite sprite in _backgroundSprites)
        {
            if (sprite.name == name)
            {
                return sprite;
            }
        }
        return null;
    }
}
