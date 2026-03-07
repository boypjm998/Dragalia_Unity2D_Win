using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCameraController : MonoBehaviour
{
    public static StageCameraController Instance;
    public static float cameraSize = 8f;
    public static float maxCameraSize = 11f;
    
    public float currentMainCameraSize;
    public float cameraSizeOverall;
    public Vector2 startPosition = Vector2.zero;
    
    protected Tweener cameraTweener;
    protected bool isTweening = false;

    protected CinemachineVirtualCamera cmMain;
    protected CinemachineVirtualCamera cmOverall;
    
    [SerializeField] protected List<CinemachineVirtualCamera> otherCms = new List<CinemachineVirtualCamera>();
    public static GameObject MainCameraGameObject
    {
        get => mainCameraGameObject;
        //set => mainCameraGameObject = value;
    }

    public float MainCameraSize
    {
        get => mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
    }
    public Transform MainCameraFollowObject
    {
        get => mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>().Follow;
    }

    private static GameObject overallCameraGameObject;

    private static GameObject mainCameraGameObject;
    // Start is called before the first frame update
    public bool zoomLock = false;
    private static int currentCamera = 1;
    public float MinCameraSize { get; private set; } = 8;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    IEnumerator Start()
    {
        mainCameraGameObject = gameObject;
        yield return new WaitUntil(() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        overallCameraGameObject = GameObject.Find("OverallCamera");
        cmMain = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        cmOverall = overallCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        AddOtherCamera();
        currentMainCameraSize = MinCameraSize;
        cameraSizeOverall = cmOverall.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        // if (testFlag == true && currentCamera==1)
        // {
        //     currentCamera = 2;
        //     SwitchOverallCamera();
        // }
        // if (testFlag == false && currentCamera==2)
        // {
        //     currentCamera = 1;
        //     SwitchMainCamera();
        // }
    }

    public static void SwitchOverallCamera()
    {
        //mainCameraGameObject = GameObject.Find("Main Camera");
        //overallCameraGameObject = GameObject.Find("OverallCamera");
        TargetAimer ta = BattleStageManager.Instance.GetPlayer().GetComponentInChildren<TargetAimer>();
        ta.enabled = false;
        
        
        Instance.cmMain.Priority = 0;
        Instance.cmOverall.Priority = 10;

        foreach (var cm in Instance.otherCms)
        {
            cm.Priority = 0;
        }
        
        CineMachineOperator.Instance.StopCameraShake();
        overallCameraGameObject.GetComponentInChildren<CineMachineOperator>()?.SetInstance();
        
        
        
    }
    public static void SwitchMainCamera()
    {
        //mainCameraGameObject = GameObject.Find("Main Camera");
        //overallCameraGameObject = GameObject.Find("OverallCamera");
        TargetAimer ta = BattleStageManager.Instance.GetPlayer().GetComponentInChildren<TargetAimer>();
        ta.ResetOffset();
        ta.enabled = true;
        
        Instance.cmOverall.Priority = 0;
        Instance.cmMain.Priority = 10;

        foreach (var cm in Instance.otherCms)
        {
            cm.Priority = 0;
        }
        
        CineMachineOperator.Instance.StopCameraShake();
        mainCameraGameObject.GetComponentInChildren<CineMachineOperator>()?.SetInstance();
    }

    public static GameObject GetCurrentCamera()
    {
        if (currentCamera == 1)
        {
            return mainCameraGameObject;
        }
        else
        {
            return overallCameraGameObject;
        }
    }

    public static void SwitchMainCameraFollowObject(GameObject target,bool resize = true)
    {
        CinemachineVirtualCamera camera;
        if(Instance != null && Instance.cmMain != null)
        {
            camera = Instance.cmMain;
        }else{
            camera = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        }
        
        
        camera.Follow = target.transform;

        if (resize)
        {
            if (target == BattleStageManager.Instance.GetPlayer())
            {
                Instance.ResizeCameraForce((int)Instance.currentMainCameraSize);
            }else
            {
                Instance.ResizeCameraForce((int)Instance.MinCameraSize);
            }
        }
        
        
    }

    public static void SwitchOverallCameraFollowObject(GameObject target)
    {
        var camera = overallCameraGameObject.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        if(target == null)
            camera.Follow = null;
        else
        {
            camera.Follow = target.transform;
        }
        
        
        
    }

    public static void SetMainCameraSize(int size)
    {
        var camera = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        camera.m_Lens.OrthographicSize = size;
        //Instance.currentMainCameraSize = size;
    }

    
    public void SetMinCameraSize(int size)
    {
        MinCameraSize = size;
        if(currentMainCameraSize < MinCameraSize)
        {
            MainCameraZoomAuto(size);
        }
    }

    private void MainCameraZoomAuto(int endValue)
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;

        if (camera.Follow != BattleStageManager.Instance.GetPlayer().transform)
        {
            Debug.Log("Camera not following player");
            return;
        }
        
        var newSize = camera.m_Lens.OrthographicSize;

        newSize = endValue;

        var minSize = MinCameraSize;
        var maxSize = Mathf.Min(maxCameraSize, cameraSizeOverall);

        if (newSize <= minSize)
        {
            newSize = MinCameraSize;
        }
        else if (newSize > maxSize)
        {
            newSize = cmMain.m_Lens.OrthographicSize;
        }
        
        
        currentMainCameraSize = newSize;
        isTweening = true;
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, newSize,
            0.3f * Mathf.Abs(newSize - cmMain.m_Lens.OrthographicSize)).
            OnComplete(()=>isTweening = false);
    }
    

    public void MainCameraZoom(int increment)
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;

        if (isTweening || zoomLock)
        {
            return;
        }

        if (camera.Priority <= 0)
        {
            return;
        }

        if (camera.Follow != BattleStageManager.Instance.GetPlayer().transform)
        {
            Debug.Log("Camera not following player");
            return;
        }
        
        var newSize = camera.m_Lens.OrthographicSize;
        
        if (increment > 0)
            newSize = cmMain.m_Lens.OrthographicSize + 1;
        else if(increment < 0)
            newSize = cmMain.m_Lens.OrthographicSize - 1;
        else newSize = cmMain.m_Lens.OrthographicSize;
        
        
        var minSize = MinCameraSize;
        var maxSize = Mathf.Min(maxCameraSize, cameraSizeOverall);

        if (newSize <= minSize)
        {
            newSize = MinCameraSize;
        }
        else if (newSize > maxSize)
        {
            newSize = cmMain.m_Lens.OrthographicSize;
        }
        
        
        currentMainCameraSize = newSize;
        isTweening = true;
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, newSize,
            0.3f).OnComplete(()=>isTweening = false);
        
    }

    private void ResizeCameraForce(int newSize = 8)
    {
        if (newSize != MinCameraSize)
        {
            newSize = (int)currentMainCameraSize;
        }

        var camera = cmMain;
        isTweening = true;
        
        var tweenTime = Mathf.Clamp(Mathf.Abs(currentMainCameraSize - newSize) * 0.3f,0.3f,0.75f);
        
        cameraTweener?.Kill();
        
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, newSize,
            tweenTime).OnComplete(()=>isTweening = false);
        
        currentMainCameraSize = newSize;
    }
    

    public void ToShapeshiftingView()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9

        var endValue = Mathf.Max(MinCameraSize + 1, cmMain.m_Lens.OrthographicSize);
        
        if(isTweening)
            return;
        
        isTweening = true;
        currentMainCameraSize = endValue;
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, endValue,
            0.5f).OnComplete(()=>isTweening = false);
        
    }
    
    public void DoViewTween(float endValue, float duration = 0.5f)
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        
        
        if(isTweening)
            return;
        
        isTweening = true;
        currentMainCameraSize = endValue;
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, endValue,
            duration).OnComplete(()=>isTweening = false);
    }
    public void ToNormalView()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        var endValue = Mathf.Max(MinCameraSize, cmMain.m_Lens.OrthographicSize);
        
        if(isTweening)
            return;
        
        isTweening = true;
        currentMainCameraSize = endValue;
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, endValue,
            0.5f).OnComplete(()=>isTweening = false);
    }

    protected void AddOtherCamera()
    {
        var root = SceneManager.GetActiveScene().GetRootGameObjects();
        var otherCameraGameObject = root.FirstOrDefault(x => x.name == "OtherCamera");
        
        
        for (int i = 1; i < otherCameraGameObject.transform.childCount - 2; i++)
        {
            otherCms.Add(otherCameraGameObject.transform.GetChild(i).GetComponentInChildren<CinemachineVirtualCamera>());
        }
    }

    public static void SwitchToOtherCamera(int id,bool followPlayer = false)
    {
        //mainCameraGameObject = GameObject.Find("Main Camera");
        //overallCameraGameObject = GameObject.Find("OverallCamera");
        TargetAimer ta = BattleStageManager.Instance.GetPlayer().GetComponentInChildren<TargetAimer>();
        ta.enabled = false;
        
        Instance.cmMain.Priority = 0;
        Instance.cmOverall.Priority = 0;
        CinemachineVirtualCamera targetCamera = null;
        
        for (int i = 0; i < Instance.otherCms.Count; i++)
        {
            if(i == id)
            {
                Instance.otherCms[i].Priority = 10;
                targetCamera = Instance.otherCms[i];
                if(followPlayer)
                {
                    Instance.otherCms[i].Follow = BattleStageManager.Instance.GetPlayer().transform;
                }
                else
                {
                    Instance.otherCms[i].Follow = null;
                }
                break;
            }
            else
            {
                Instance.otherCms[i].Priority = 0;
            }
        }
        
        CineMachineOperator.Instance.StopCameraShake();
        targetCamera.gameObject.GetComponent<CineMachineOperator>()?.SetInstance();
    }

}
