using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    
    private Transform cameraTransform;
    private GameObject UIFXContainer;
    private int clickEffCD = 0;
    private bool gameIsEnded = true;
    public static bool gameIsStarted = false;
    public Dictionary<string, AssetBundle> loadedBundles;
    
    
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private GameObject clickEff;
    [SerializeField] private int currentCharacterID = 1;
    [SerializeField] private int questID = -1;
    public bool loadingEnd = true;
    

    private void Awake()
    {
        
        var other = FindObjectsOfType<GlobalController>();
        if (other.Length > 1)
        {
            this.enabled = false;
            Destroy(gameObject);
        }
        this.loadedBundles = new Dictionary<string, AssetBundle>();
        UIFXContainer = GameObject.Find("UIFXContainer");
    }

    void Start()
    {
        if (GetBundle("Iconsmall") == null)
        {
            var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/Iconsmall");
            loadedBundles.Add("Iconsmall",ab);
        }
        cameraTransform = GameObject.Find("Main Camera").transform;
        
    }

    private void Update()
    {
        
        transform.position = cameraTransform.position;
        
        
        if (Input.GetMouseButtonDown(0) && clickEffCD<0)
        {
            clickEffCD = 15;
            Instantiate(clickEff, Camera.main.ScreenToWorldPoint(Input.mousePosition)
                , Quaternion.identity,UIFXContainer.transform);
            
        }

        clickEffCD--;
    }

    public void TestReturnMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }


    // Update is called once per frame
    public void TestEnterLevel()
    {
        StartCoroutine(LoadBattleScene());
    }
    
    IEnumerator LoadBattleScene(){
        //异步加载场景

        loadingEnd = false;
        var loadingScreen = Instantiate(LoadingScreen, Vector3.zero, Quaternion.identity, transform);
            
        var anim = loadingScreen.GetComponent<Animator>();
        //yield return new WaitForSecondsRealtime(2f);


        var needLoad = SearchPlayerRelatedAssets(1);
        
        AssetBundleCreateRequest ar = null;
        AssetBundle assetBundle,assetBundle2;
        if (!loadedBundles.ContainsKey(needLoad[0]))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, needLoad[0]));
            yield return ar;
            loadedBundles.Add(needLoad[0],ar.assetBundle);
            assetBundle = ar.assetBundle;
        }
        else
        {
            assetBundle = loadedBundles[needLoad[0]];
        }
        //load UI_dependencies
        if (!loadedBundles.ContainsKey("ui_general"))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "ui_general"));
            yield return ar;
            loadedBundles.Add("ui_general",ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }
        else
        {
            //assetBundle2 = loadedBundles["ui_general"];
        }
        //Load Voices
        if (!loadedBundles.ContainsKey("voice_c005"))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "voice_c005"));
            yield return ar;
            loadedBundles.Add("voice_c005",ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }
        else
        {
            //assetBundle2 = loadedBundles["voice_c005"];
        }
        if (!loadedBundles.ContainsKey(needLoad[1]))
        {
            ar =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, needLoad[1]));
            yield return ar;
            loadedBundles.Add(needLoad[1],ar.assetBundle);
            //assetBundle2 = ar.assetBundle;
        }


        AsyncOperation ao = SceneManager.LoadSceneAsync("BattleScene");
        DontDestroyOnLoad(this.gameObject);
        yield return ao;

        
        cameraTransform = GameObject.Find("Main Camera").transform;
        //loadingScreen.transform.position = Camera.main.transform.position;
        loadingScreen.transform.position = cameraTransform.transform.position;
        UIFXContainer = GameObject.Find("UIFXContainer");
        //var assetBundle = ar.assetBundle;
        var plr = assetBundle.LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = 
            Instantiate(plr, new Vector3(-15f, -6.5f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        FindObjectOfType<BattleStageManager>().InitPlayer(plrclone);
        plrclone.GetComponentInChildren<TargetAimer>().enabled = false;

        yield return null;
        //StageCameraController.SwitchOverallCamera();

        //Time.timeScale = 0;
        anim.SetBool("loaded",true);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        loadingEnd = true;
        //Time.timeScale = 1;
        var mainCamera = GameObject.Find("Main Camera");
        var cinemachineVirtualCamera = mainCamera.GetComponentInChildren<CinemachineVirtualCamera>();
        plrclone.GetComponentInChildren<TargetAimer>().enabled = true;

        yield return new WaitForSeconds(2f);
        cinemachineVirtualCamera.Follow = plrclone.transform;
        //yield return null;
        //StageCameraController.SwitchMainCamera();
    }

    IEnumerator LoadMainMenu()
    {
        loadingEnd = false;
        FindObjectOfType<TargetAimer>().enabled = false;
        yield return null;

        var loadingScreen = Instantiate(LoadingScreen, Camera.main.transform.position, Quaternion.identity, transform);
        var anim = loadingScreen.GetComponent<Animator>();
        
        yield return new WaitForSecondsRealtime(2);

        AsyncOperation ao = SceneManager.LoadSceneAsync("MainMenu");
        DontDestroyOnLoad(this.gameObject);
        yield return ao;
        cameraTransform = GameObject.Find("Main Camera").transform;
        
        
        //AssetBundle.UnloadAllAssetBundles(true);
        var bundles = AssetBundle.GetAllLoadedAssetBundles();
        print(bundles.Count());
        foreach (var ab in bundles)
        {
            if (ab.name == "iconsmall")
            {
                continue;
            }

            loadedBundles.Remove(ab.name);
            var async = ab.UnloadAsync(true);
            yield return async;
            
        }
        
        

        yield return null;
        
        //Resources.UnloadUnusedAssets();
        
        loadingScreen.transform.position = Vector3.zero;
        //loadedBundles.Clear();

        //print(FindObjectOfType<UI_AdventurerSelectionMenu>().
            //iconBundle.LoadAssetWithSubAssets<Sprite>("Iconsmall")[0].name);
        
        
        Time.timeScale = 1;
        cameraTransform = Camera.main.transform;
        UIFXContainer = GameObject.Find("UIFXContainer");
        
        anim.SetBool("loaded",true);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("End"));
        Destroy(loadingScreen);
        this.enabled = true;
        print(loadedBundles.ContainsKey("Iconsmall"));
        loadingEnd = true;

    }

    protected virtual void LoadPlayer(int characterID)
    {
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "c001"));
        var plr = assetBundle.LoadAsset<GameObject>("PlayerHandle");
        var plrlayer = GameObject.Find("Player");
        var plrclone = Instantiate(plr, new Vector3(4.5f, -6.5f, 0), transform.rotation, plrlayer.transform);
        plrclone.name = "PlayerHandle";
        
    }

    public AssetBundle GetBundle(string name)
    {
        if (loadedBundles.ContainsKey(name))
        {
            return loadedBundles[name];
        }
        Debug.LogWarning("ErrorWhenLoadingBundle");
        return null;
    }

    string[] SearchPlayerRelatedAssets(int id)
    {
        var needLoad = new string[3];
        needLoad[0] = BasicCalculation.ConvertID("c", id);
        needLoad[1] = BasicCalculation.ConvertID("voice_c", id);
        needLoad[2] = BasicCalculation.ConvertID("ui_c", id);
        return needLoad;
    }

}
