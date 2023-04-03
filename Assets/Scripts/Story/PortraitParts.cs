using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitParts : MonoBehaviour
{
    [SerializeField] private List<Sprite> _faceSprites;
    [SerializeField] private List<Sprite> _mouthSprites;
    public string speakerID;
    public Coroutine blinkingAnimationRoutine;
    public Coroutine speakingAnimationRoutine;
    public int currentBaseFaceIndex;
    public int currentBaseMouthIndex;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Sprite GetMouthSprite(int mouthIndex)
    {
        return _mouthSprites[mouthIndex];
    }

    public Sprite GetFaceSprite(int faceIndex)
    {
        return _faceSprites[faceIndex];
    }

    public int GetCurrentFaceIndex()
    {
        return _faceSprites.IndexOf(transform.GetChild(1).GetComponent<Image>().sprite);
    }

    public int GetCurrentFaceIndexBase()
    {
        var currentIndex = _faceSprites.IndexOf(transform.GetChild(1).GetComponent<Image>().sprite);
        return currentIndex / 2 * 2;
    }

    public int GetCurrentMouthIndex()
    {
        return _mouthSprites.IndexOf(transform.GetChild(2).GetComponent<Image>().sprite);
    }
}