using System.Collections.Generic;
using System.IO;
using LitJson;
using Newtonsoft.Json;
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
    private void Awake()
    {
        InitParts();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void InitParts()
    {
        var bundle = GlobalController.Instance.GetBundle($"storyportrait/{speakerID}");
        if (bundle == null)
        {
            print("bundle is null");
            bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/storyportrait/{speakerID}");
            GlobalController.Instance.loadedBundles.Add($"storyportrait/{speakerID}", bundle);
        }

        print(bundle.name);
        
        var data = bundle.LoadAsset<TextAsset>("data.json");

        if (data == null)
        {
            print("bundle is null");
            return;
        }

        
        
        var rootData = JsonConvert.DeserializeObject<StoryPortraitData>(data.text);
        print(rootData);
        var partsData = rootData.partsData;
        print(data.text);
        print(partsData.mouthParts);

        if (_mouthSprites.Count == 0)
        {
            foreach (var part in partsData.mouthParts)
            {
                //取part中以/为分隔符的最后一个字符串
                var partName = part.Split('/')[^1];
                var sprite = bundle.LoadAsset<Sprite>(partName);
                _mouthSprites.Add(sprite);
            }
        }
        
        if (_faceSprites.Count == 0)
        {
            foreach (var part in partsData.faceParts)
            {
                //取part中以/为分隔符的最后一个字符串
                var partName = part.Split('/')[^1];
                var sprite = bundle.LoadAsset<Sprite>(partName);
                _faceSprites.Add(sprite);
            }
        }


    }

    public int GetEyeSpriteTotalCount()
    {
        return _faceSprites.Count;
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



public class StoryPortraitData
{
    public class StoryPortraitOffset
    {
        public int x;
        public int y;
        public float size_x;
        public float size_y;
    }

    public class StoryPortraitParts
    {
        public List<string> faceParts;
        public List<string> mouthParts;
    }
    public StoryPortraitOffset offset = new();
    public StoryPortraitParts partsData = new();
}