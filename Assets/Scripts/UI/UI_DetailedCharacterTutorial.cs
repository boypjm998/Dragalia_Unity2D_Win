using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_DetailedCharacterTutorial : UI_Tutorial_PauseMenu
{
    [SerializeField] private Transform _tutorialContentParent;
    [SerializeField] private TextMeshProUGUI _title;
    /// <summary>
    /// 页面信息
    /// </summary>
    [FormerlySerializedAs("_tutorialData")]
    [SerializeField] private TextAsset _tutorialPageData;
    [SerializeField] private TextAsset _tutorialDictData;
    /// <summary>
    /// 前缀
    /// </summary>
    [SerializeField] private string _tutorialResBundleName;

    [SerializeField] private float _fontSize;
    [SerializeField] private TMP_FontAsset _fontAssetZHCN;
    [SerializeField] private TMP_FontAsset _fontAssetEN;

    private List<GameObject> _pageInstances = new();

    //private AssetBundle _tutorialResBundle;
    
    /// <summary>
    /// 每一页的数据
    /// </summary>
    private List<CharacterTutorialAssetData> _tutorialAssetData = new();
    [FormerlySerializedAs("_tutorialAssetDict")]
    public List<CharacterTutorialAssetManager> tutorialAssetDict = new();
    
    private void Awake()
    {
        _battleSceneUIManager = transform.parent.GetComponent<BattleSceneUIManager>();
        //pagesParent = transform.Find("TutorialPages");
        //currentPageObject = pagesParent.GetChild(1).gameObject;
        leftPageButton = transform.Find("Panel/PrevPage").gameObject;
        rightPageButton = transform.Find("Panel/NextPage").gameObject;
        Instance = this;
        tutorialAssetDict =
            JsonConvert.DeserializeObject<List<CharacterTutorialAssetManager>>(_tutorialDictData.text);
    }

    private void Start()
    {

        var assetDict = tutorialAssetDict.
            Find(x => x.charaID == GlobalController.currentCharacterID).resourceList;

        var resList = tutorialAssetDict.
            Find(x =>
                x.charaID == GlobalController.currentCharacterID).resourceList;
        
        // var bundlePathList = tutorialAssetDict.
        //     Find(x =>
        //         x.charaID == GlobalController.currentCharacterID).assetBundleList;
        //
        // //异步加载AssetBundle
        // foreach (var bundlePath in bundlePathList)
        // {
        //     var truePath = $"{_tutorialResBundleName}/{bundlePath}";
        //     var bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, truePath));
        //     yield return bundle;
        //     print(bundle.name);
        //     GlobalController.Instance.loadedBundles.Add(truePath, bundle);
        // }


        var tutorialPageList = JsonConvert.
            DeserializeObject<List<CharacterTutorialAssetData>>(_tutorialPageData.text);

        //遍历resList，找到tutorialPageData中对应的ID，然后将对应的数据加入到_tutorialAssetData中
        foreach (var resID in resList)
        {
            _tutorialAssetData.Add(
             tutorialPageList.
                Find(x => x.id == resID));
            
        }
        
        if(_tutorialAssetData.Count == 0)
            return;
        
        InitializePages();
        currentPageObject = _pageInstances[0].gameObject;
        DisplayPage(1);
        if(currentMaxPages > 1)
            rightPageButton.SetActive(true);
        else
            rightPageButton.SetActive(false);
    }

    private void InitializePages()
    {
        int index = 1;

        var controls = new string[]
        {
            PlayerInput.GetInputKeyPath("MoveL"),//0
            PlayerInput.GetInputKeyPath("MoveR"),//1
            PlayerInput.GetInputKeyPath("MoveU"),//2
            PlayerInput.GetInputKeyPath("MoveD"),//3
            PlayerInput.GetInputKeyPath("Attack"),//4
            PlayerInput.GetInputKeyPath("Jump"),//5
            PlayerInput.GetInputKeyPath("Dodge"),//6
            PlayerInput.GetInputKeyPath("Skill1"),//7
            PlayerInput.GetInputKeyPath("Skill2"),//8
            PlayerInput.GetInputKeyPath("Skill3"),//9
            PlayerInput.GetInputKeyPath("Skill4"),//10
            PlayerInput.GetInputKeyPath("Special"),//11
            PlayerInput.GetInputKeyPath("Escape"),
            PlayerInput.GetInputKeyPath("ZoomIn"),
            PlayerInput.GetInputKeyPath("ZoomOut")
        };




        foreach(var pageData in _tutorialAssetData)
        {
            
            var rectTransform = new GameObject($"Page{index}").AddComponent<RectTransform>();
            
            rectTransform.SetParent(_tutorialContentParent);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;

            foreach (var elementData in pageData.assetInfoList)
            {
                var childRectTransform = 
                    new GameObject().AddComponent<RectTransform>();
                childRectTransform.SetParent(rectTransform);
                childRectTransform.localScale = Vector3.one;

                var assetType = elementData.assetType;
                var width = elementData.width;
                var height = elementData.height;
                var positionX = elementData.posX;
                var positionY = elementData.posY;
                
                childRectTransform.anchoredPosition = new Vector2(positionX, positionY);
                childRectTransform.sizeDelta = new Vector2(width, height);

            if(assetType == CharacterTutorialAssetInfo.AssetType.Text)
            {
                var tmpUGUI = childRectTransform.gameObject.AddComponent<TextMeshProUGUI>();
                //tmpUGUI.text = pageData.assetInfo;
                tmpUGUI.font = GlobalController.Instance.GameLanguage == GlobalController.Language.ZHCN ?
                    _fontAssetZHCN : _fontAssetEN;
                
                tmpUGUI.fontSizeMax = _fontSize;
                tmpUGUI.fontSizeMin = (int)(_fontSize * 0.75f);
                tmpUGUI.color = Color.black;
                tmpUGUI.alignment = TextAlignmentOptions.Center;
                tmpUGUI.text = String.Format(elementData.assetInfo,controls
                    );
                
                tmpUGUI.enableAutoSizing = true;

            }
            else if(assetType == CharacterTutorialAssetInfo.AssetType.Image)
            {
                var img = childRectTransform.gameObject.AddComponent<Image>();
                img.raycastTarget = false;
                //elementData.assetInfo中，位于/前面的是bundlePath，后面的是assetName
                var bundlePath = elementData.assetInfo.Split('/')[0];
                var assetName = elementData.assetInfo.Split('/')[1];
                var bundle = GlobalController.Instance.GetBundle(_tutorialResBundleName + $"/{bundlePath}");
                print(bundle.name);
                var sprite = bundle.LoadAsset<Sprite>(assetName);
                img.sprite = sprite;
            }
            else
            {
                var videoPlayer = rectTransform.gameObject.AddComponent<VideoPlayer>();
                
                var bundlePath = elementData.assetInfo.Split('/')[0];
                var assetName = elementData.assetInfo.Split('/')[1];
                
                print(bundlePath);
                print(assetName);
                
                var bundle = GlobalController.Instance.GetBundle(_tutorialResBundleName + $"/{bundlePath}");
                var videoClip = bundle.LoadAsset<VideoClip>(assetName);
                
                videoPlayer.clip = videoClip;
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                videoPlayer.aspectRatio = VideoAspectRatio.FitOutside;
                videoPlayer.isLooping = true;
                videoPlayer.playOnAwake = true;
                
                videoPlayer.targetTexture = new RenderTexture(width, height, 32);
                videoPlayer.targetTexture.useDynamicScale = true;
                videoPlayer.targetTexture.graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                videoPlayer.targetTexture.depthStencilFormat = GraphicsFormat.D32_SFloat_S8_UInt;
                
                var rawImage = childRectTransform.gameObject.AddComponent<RawImage>();
                rawImage.texture = videoPlayer.targetTexture;
                //rawImage.rectTransform.sizeDelta = new Vector2(width, height);

                videoPlayer.Play();
            }
                
                
                
                
                
                
                
                
            }
            
            var title = Instantiate(_title.gameObject, _title.transform.position,
                Quaternion.identity, rectTransform);
            
            title.GetComponent<TextMeshProUGUI>().text = pageData.title;
            title.GetComponent<RectTransform>().anchoredPosition =
                _title.transform.parent.GetComponent<RectTransform>().anchoredPosition;
            title.transform.localScale = Vector3.one;
            
            index++;
            
            _pageInstances.Add(rectTransform.gameObject);
            rectTransform.gameObject.SetActive(false);
            
        }
        
        _pageInstances[0].SetActive(true);
        currentMaxPages = _pageInstances.Count;

    }
    
    protected override void DisplayPage(int targetPageID)
    {
        if(currentPage!=0)
            _pageInstances[currentPage - 1].gameObject.SetActive(false);
        _pageInstances[(targetPageID) - 1].gameObject.SetActive(true);
        currentPageObject = _pageInstances[(targetPageID) - 1].gameObject;
        currentPage = targetPageID;
        //FormatTutorialText(currentPage);
        if (currentPage == 1)
        {
            leftPageButton.SetActive(false);
            if (currentMaxPages > 1)
            {
                rightPageButton.SetActive(true);
            }
            else
            {
                rightPageButton.SetActive(false);
            }
        }
        else if (currentPage == currentMaxPages)
        {
            rightPageButton.SetActive(false);
            if (currentMaxPages > 1)
            {
                leftPageButton.SetActive(true);
            }
            else
            {
                leftPageButton.SetActive(false);
            }
        }
        else
        {
            leftPageButton.SetActive(true);
            rightPageButton.SetActive(true);
        }

        
    }
}

[Serializable]
public class CharacterTutorialAssetData
{
    public int id;
    public string title;
    public List<CharacterTutorialAssetInfo> assetInfoList = new();
}

[Serializable]
public class CharacterTutorialAssetInfo
{
    public enum AssetType
    {
        Image,
        Text,
        Video
    }
    
    public AssetType assetType;
    /// <summary>
    /// If assetType is Image, this is the path to the image in the asset bundle.
    /// If assetType is Text, this is the text content.
    /// </summary>
    public string assetInfo = "";
    public int width;
    public int height;
    public int posX;
    public int posY;


}

[Serializable]
public class CharacterTutorialAssetManager
{
    public int charaID;
    public List<int> resourceList = new();
    public List<string> assetBundleList = new();
}