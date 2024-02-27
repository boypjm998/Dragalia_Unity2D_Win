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

    public Vector2 startPosition = Vector2.zero;
    
    protected Tweener cameraTweener;

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
    public bool testFlag = false;
    private static int currentCamera = 1;

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

    public static void SwitchMainCameraFollowObject(GameObject target)
    {
        CinemachineVirtualCamera camera;
        if(Instance != null && Instance.cmMain != null)
        {
            camera = Instance.cmMain;
        }else{
            camera = mainCameraGameObject.GetComponentInChildren<CinemachineVirtualCamera>();
        }
        
        
        camera.Follow = target.transform;

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
    }

    public void ToShapeshiftingView()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, 9,
            0.5f);
    }
    
    public void DoViewTween(float endValue, float duration = 0.5f)
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, endValue,
            duration);
    }
    public void ToNormalView()
    {
        if(GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        var camera = cmMain;
        //0.5s内将摄像机的m_Lens.OrthographicSize变为9
        cameraTweener = DOTween.To(() => camera.m_Lens.OrthographicSize,
            x => camera.m_Lens.OrthographicSize = x, 8,
            0.5f);
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
