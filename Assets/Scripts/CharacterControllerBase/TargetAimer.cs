using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using GameMechanics;

public class TargetAimer : MonoBehaviour
{
    public float aimSizeX;
    public float aimSizeY;
    [SerializeField]
    private GameObject CameraFollowTarget; //玩家目标
    [SerializeField]
    public GameObject EnemyWatched; //视线偏向的敌人目标
    private PolygonCollider2D TargetSearchScale; //锁定范围触发器
    public List<GameObject> ReachableEnemies => EnemyInRange;
    
    
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera; //摄像头
    [SerializeField] 
    private CinemachineCameraOffset cinemachineCameraOffset; //摄像头偏移量
    
    private bool TargetFixed;
    private GameObject mainCamera;

    [SerializeField]
    private List<GameObject> EnemyInRange; //在警戒范围的敌人列表
    [SerializeField]
    List<GameObject> ObjectReachable; //在攻击范围内的地方

    [SerializeField]
    private float cameraMoveSpeed;//摄像头移动速度
    public float lookAheadDistanceX;//视线X方向最大距离
    public float lookAheadDistanceY;//视线Y方向最大距离
    //private Coroutine cameraMoveRoutine;
    private float maxCameraMoveSpeed;//摄像头移动最大速度
    private float velocityCameraMoveSpeed;
    [SerializeField]
    private bool stopFlagX;//摄像头X方向停止位移flag
    [SerializeField]
    private bool stopFlagY;//摄像头Y方向停止位移flag


    public bool TestButton;


    private void InitCameraFollowAttributes()
    {
        //cameraMoveSpeed = 0f;
        maxCameraMoveSpeed = 6.0f;
        lookAheadDistanceX = 14.0f;
        lookAheadDistanceY = 0f;
        cinemachineCameraOffset.m_Offset = new Vector3(0, 0, 20);
        stopFlagX = true;
        stopFlagY = true;
        aimSizeX = Mathf.Abs(TargetSearchScale.points[0].x);
        aimSizeY = Mathf.Abs(TargetSearchScale.points[0].y);
    }


    // Start is called before the first frame update
    private void Awake()
    {
        ObjectReachable = new List<GameObject>();
        TargetSearchScale = GetComponent<PolygonCollider2D>();
        EnemyInRange = new List<GameObject>();
        EnemyWatched = null;
        //cameraMoveRoutine = null;
        
        //print(cinemachineVirtualCamera);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil
            (() => GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        
        mainCamera = GameObject.Find("Main Camera");
        cinemachineVirtualCamera = mainCamera.GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineCameraOffset = cinemachineVirtualCamera.GetComponent<CinemachineCameraOffset>();
        CameraFollowTarget = transform.parent.gameObject;
        cinemachineVirtualCamera.transform.position = CameraFollowTarget.transform.position;
        cinemachineVirtualCamera.Follow = CameraFollowTarget.transform;
        InitCameraFollowAttributes();
        
    }

    private void Update()
    {
      
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if(mainCamera==null)
           return;
        
        if (stopFlagX == false || stopFlagY == false)
        {
            maxCameraMoveSpeed = 6.0f;
        }
        else {
            maxCameraMoveSpeed = 0;
            
        }

        
        //摄像头缓动
        cameraMoveSpeed = Mathf.SmoothDamp(cameraMoveSpeed, maxCameraMoveSpeed, ref velocityCameraMoveSpeed, 1.0f);

        if (EnemyWatched != null)
        {
            float offsetX =  CheckTargetXDistance();//获取摄像头所需偏移量
            float offsetY =  CheckTargetYDistance();
            int moveDirX; 
            int moveDirY;
            
            //print("有敌人");
            

            if (offsetX > 0)
                moveDirX = 1;
            else moveDirX = -1;

            if (offsetY > 0)
                moveDirY = 1;
            else moveDirY = -1;
            //如果摄像头的中心偏移量小于目标偏移量，移动摄像头
            if (Mathf.Abs(offsetX) > 1f || Mathf.Abs(offsetY) > 1f)
            {
                
                //if( Mathf.Abs(lookAheadDistanceX - Mathf.Abs(cinemachineCameraOffset.m_Offset.x) + CameraFollowTarget.transform.position.x) > Mathf.Abs(EnemyWatched.transform.position.x - CameraFollowTarget.transform.position.x))
                
                
                if(Mathf.Abs(mainCamera.transform.position.x  - EnemyWatched.transform.position.x) - lookAheadDistanceX > 1f)
                //if (Mathf.Abs(cinemachineCameraOffset.m_Offset.x) - (Mathf.Abs(EnemyWatched.transform.position.x - CameraFollowTarget.transform.position.x) - lookAheadDistanceX) > 0.1f)
                {
                    cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x + cameraMoveSpeed * Time.deltaTime * moveDirX,
                    cinemachineCameraOffset.m_Offset.y, 0);
                    stopFlagX = false;

                }
                else {
                    
                    stopFlagX = true;
                }
                
                if(Mathf.Abs(mainCamera.transform.position.y  - EnemyWatched.transform.position.y) - lookAheadDistanceY > 1f)
                //if (Mathf.Abs(cinemachineCameraOffset.m_Offset.y) - (Mathf.Abs(EnemyWatched.transform.position.y - CameraFollowTarget.transform.position.y) - lookAheadDistanceY) > 0.1f)
                {
                    //print(Mathf.Abs(mainCamera.transform.position.y  - EnemyWatched.transform.position.y));
                    cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x,
                    cinemachineCameraOffset.m_Offset.y + cameraMoveSpeed * Time.deltaTime * moveDirY, 0);
                    stopFlagY = false;
                    //print("StopFlagY is set to false , offsetY:"+offsetY);

                }
                else {
                    stopFlagY = true;
                    //print("StopFlagY is set to true , offsetY:"+offsetY);
                }
                
                if (stopFlagY && stopFlagX)
                    cameraMoveSpeed = 0.1f;
                
            }


        }
        else {
            //print("镜头归位");
            //敌人消失，镜头归位，同上
            stopFlagY = false;
            stopFlagX = false;
            if (Mathf.Abs(cinemachineCameraOffset.m_Offset.x) < 0.3f)
            {
                stopFlagX = true;
                cinemachineCameraOffset.m_Offset = new Vector3(0, cinemachineCameraOffset.m_Offset.y, 0);
            }
            else if (cinemachineCameraOffset.m_Offset.x > 0)
            {
                cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x - cameraMoveSpeed * Time.deltaTime,
                    cinemachineCameraOffset.m_Offset.y, 0);
            }
            else {
                cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x + cameraMoveSpeed * Time.deltaTime,
                    cinemachineCameraOffset.m_Offset.y, 0);
            }




            if (Mathf.Abs(cinemachineCameraOffset.m_Offset.y) < 0.3f)
            {
                stopFlagY = true;
                cinemachineCameraOffset.m_Offset = new Vector3(cinemachineCameraOffset.m_Offset.x, 0, 0);
            }
            else if (cinemachineCameraOffset.m_Offset.y > 0)
            {
                cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x,
                    cinemachineCameraOffset.m_Offset.y - cameraMoveSpeed * Time.deltaTime, 0);
            }
            else
            {
                cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x,
                    cinemachineCameraOffset.m_Offset.y + cameraMoveSpeed * Time.deltaTime, 0);
            }

            if (stopFlagY && stopFlagX)
                cameraMoveSpeed = 0.1f;
            //cinemachineCameraOffset.m_Offset = new Vector3(0, 0, 0);
        }

        if (cinemachineCameraOffset.m_Offset.y > lookAheadDistanceY)
        {
            cinemachineCameraOffset.m_Offset = new
                Vector3(cinemachineCameraOffset.m_Offset.x,
                    lookAheadDistanceY, 0);
            stopFlagY = true;
        }else if (cinemachineCameraOffset.m_Offset.y < -lookAheadDistanceY)
        {
            cinemachineCameraOffset.m_Offset = new
                Vector3(cinemachineCameraOffset.m_Offset.x,
                    -lookAheadDistanceY, 0);
            stopFlagY = true;
        }
        
        if (cinemachineCameraOffset.m_Offset.x > lookAheadDistanceX)
        {
            cinemachineCameraOffset.m_Offset = new
                Vector3(lookAheadDistanceX,cinemachineCameraOffset.m_Offset.y,
                    0);
            stopFlagX = true;
        }else if (cinemachineCameraOffset.m_Offset.x < -lookAheadDistanceX)
        {
            cinemachineCameraOffset.m_Offset = new
                Vector3(-lookAheadDistanceX,cinemachineCameraOffset.m_Offset.y,
                    0);
            stopFlagX = true;
        }



    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(!enabled)
            //return;
        
        
        if (collision.CompareTag("Enemy") && collision.GetComponent<Transform>().gameObject != EnemyWatched)
        {
            EnemyInRange.Add(collision.GetComponent<Transform>().gameObject);
            
          
            //Debug.Log("InRange:" + collision.GetComponent<Transform>().gameObject);
        }
        
        if (EnemyWatched == null && collision.CompareTag("Enemy"))
        {
            EnemyInRange.Sort(SortDistanceDescendent);
            EnemyWatched = collision.GetComponent<Transform>().gameObject;
            //EnemyLocked = EnemyInRange[0];
            //Debug.Log("Locked:" + EnemyWatched);
        }
        EnemyInRange.Sort(SortDistanceDescendent);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if(!enabled)
            //return;
        if (!collision.CompareTag("Enemy"))
            return;
        if (EnemyWatched != null )
        {

            if (collision.GetComponent<Transform>().gameObject == EnemyWatched)
            {
                //Debug.Log("Unlocked:" + EnemyWatched);
                EnemyInRange.Remove(EnemyWatched);
                EnemyWatched = null;
                //EnemyInRange.RemoveAt(0);
            }
            else {
                EnemyInRange.Remove(collision.GetComponent<Transform>().gameObject);
            }
            
            EnemyInRange.Sort(SortDistanceDescendent);
        }
        if (EnemyInRange.Count > 0)
        {
            EnemyInRange.Sort(SortDistanceDescendent);
            EnemyWatched = EnemyInRange[0];
            stopFlagX = false;
            stopFlagY = false;
            print("StopFlagY is set to false");
            maxCameraMoveSpeed = 6;
        }

    }


    private float CheckTargetXDistance()
    {
        float offsetX;
        float distanceX;
        
        offsetX = (EnemyWatched.transform.position.x - mainCamera.transform.position.x);
        //offsetX = mainCamera.transform.position.x  - EnemyWatched.transform.position.x;
        distanceX = Mathf.Abs(offsetX);

        if (distanceX > lookAheadDistanceX)
        {
            if (offsetX > 0)
                return distanceX - lookAheadDistanceX;
            else return lookAheadDistanceX - distanceX;
        }
        else return 0;
    }

    private float CheckTargetYDistance()
    {
        float offsetY;
        float distanceY;

        offsetY = (EnemyWatched.transform.position.y - mainCamera.transform.position.y);
        //offsetY = mainCamera.transform.position.y  - EnemyWatched.transform.position.y;
        distanceY = Mathf.Abs(offsetY);

        if (distanceY > lookAheadDistanceY)
        {
            if (offsetY > 0)
                return distanceY - lookAheadDistanceY;
            else return lookAheadDistanceY - distanceY;
        }
        else return 0;
    }

    private float DistanceFromPlayerToTarget(Transform target)
    {
        Transform playerTransform;
        try
        {
            playerTransform = CameraFollowTarget.transform;
        }
        catch
        {
            playerTransform = FindObjectOfType<PlayerInput>().transform;
        }

        

        return Mathf.Sqrt(Mathf.Pow(playerTransform.position.x - target.position.x, 2) + Mathf.Pow(playerTransform.position.y - target.position.y, 2)); ;
    }
    private int SortDistance(GameObject a, GameObject b)

    {
        float disA = DistanceFromPlayerToTarget(a.transform);
        float disB = DistanceFromPlayerToTarget(b.transform);
        if (disA > disB)
        {
            return 1;
        }
        else if (disA < disB)
        { return -1; }
        else return 0;


    }

    private int SortDistanceDescendent(GameObject a, GameObject b)

    {
        float disA = DistanceFromPlayerToTarget(a.transform);
        float disB = DistanceFromPlayerToTarget(b.transform);
        if (disA > disB)
        {
            return -1;
        }
        else if (disA < disB)
        { return 1; }
        else return 0;


    }

    public Transform GetNearestTarget()
    {
        if (EnemyInRange.Count == 0)
            return null;
        EnemyInRange.Sort(SortDistance);
        return EnemyInRange[0].transform;
    }

    public Transform GetNearestTargetInRange(float sizeX, float sizeY, LayerMask targetLayers)
    {
        Collider2D[] AttackRangeAreaInfo = 
            Physics2D.OverlapAreaAll(transform.parent.position + new Vector3(sizeX, sizeY), transform.parent.position + new Vector3(-sizeX, -sizeY),
            targetLayers);

        if (AttackRangeAreaInfo.Length == 0)
            return null;

        Transform target = AttackRangeAreaInfo[0].transform;
        float minDistance = Vector3.Distance(transform.parent.position, target.position);

        for (int i = 1; i < AttackRangeAreaInfo.Length; i++)
        {
            float tempDistance = Vector3.Distance(transform.parent.position, AttackRangeAreaInfo[i].transform.position);
            if (tempDistance < minDistance)
            {
                target = AttackRangeAreaInfo[i].transform;
                minDistance = Vector3.Distance(transform.parent.position, target.position);
            }
        }




        return target;
    }

    public Transform GetNearestTargetInRangeDirection(int dir, float sizeX, float sizeY, LayerMask targetLayers,float shootStartPointX=0)
    {
        float leftModifier = 0;
        float rightModifier = 0;
        if (dir == 1)
        {
            leftModifier = 0;
            rightModifier = 1;
        }
        else {
            leftModifier = 1;
            rightModifier = 0;
        }
        Collider2D[] AttackRangeAreaInfo =
            Physics2D.OverlapAreaAll(transform.parent.position + new Vector3(sizeX*rightModifier, sizeY), transform.parent.position + new Vector3(-sizeX*leftModifier + shootStartPointX*dir, -sizeY),
            targetLayers);
        
        if (AttackRangeAreaInfo.Length == 0)
            return null;
        
        AttackRangeAreaInfo = MarkingCheck(AttackRangeAreaInfo.ToList()).ToArray();
        
        
        
        // print("Marked:"+AttackRangeAreaInfo.Length + " Target:"+
        //       AttackRangeAreaInfo[0].transform.parent.name);
        


        Transform target = AttackRangeAreaInfo[0].transform;
        float minDistance = Vector3.Distance(transform.parent.position, target.position);

        for (int i = 1; i < AttackRangeAreaInfo.Length; i++)
        {
            float tempDistance = Vector3.Distance(transform.parent.position, AttackRangeAreaInfo[i].transform.position);
            if (tempDistance < minDistance)
            {
                target = AttackRangeAreaInfo[i].transform;
                minDistance = Vector3.Distance(transform.parent.position, target.position);
            }
            
        }




        return target;
    }

    


    //射线检测，获取最近能攻击到的敌人
    public Transform GetNearestReachableTarget(float range, LayerMask targetLayers)
    {
        //1/29修改
        //if (EnemyInRange.Count == 0)
        //    return null;
        ObjectReachable.Clear();
        RaycastHit2D hitinfo = Physics2D.Raycast(transform.parent.position, new Vector2(1, 0), range, targetLayers);
        RaycastHit2D hitinfoback = Physics2D.Raycast(transform.parent.position, new Vector2(-1,0), range, targetLayers);
        if (hitinfo.collider != null)
        {
            
            if (hitinfo.collider.CompareTag("Enemy"))
            {
                //Debug.Log("Enter");
                ObjectReachable.Add(hitinfo.collider.GetComponent<Transform>().gameObject);
            }
                
            
        }
        if (hitinfoback.collider != null)
        {
            if (hitinfoback.collider.CompareTag("Enemy"))
            {
                //Debug.Log("EnterBack");
                ObjectReachable.Add(hitinfoback.collider.GetComponent<Transform>().gameObject);
            }
        }
        //Debug.Log(ObjectReachable.Count);
        if (ObjectReachable.Count > 0)
        {
            ObjectReachable.Sort(SortDistance);
            //TODO:2023 / 7 / 2 修改
            ObjectReachable = MarkingCheck(ObjectReachable);
            return ObjectReachable[0].transform;
        }
        else {
            //EnemyInRange.Sort(SortDistance);
            return null;
        }

        //return EnemyInRange[0].transform;
    }

    //通过攻击来切换目标
    public void TargetSwapByAttack()
    {
        if (EnemyInRange.Count==0)
            return;
        int facedir = CameraFollowTarget.GetComponent<ActorController>().facedir;
        EnemyInRange.Sort(SortDistanceDescendent);
        maxCameraMoveSpeed = 6.0f;
        if (facedir == 1)
        {
            foreach (GameObject nextTarget in EnemyInRange)
            {
                if (nextTarget.transform.position.x > CameraFollowTarget.transform.position.x)
                {
                    EnemyWatched = nextTarget;
                    stopFlagX = false;
                    //stopFlagY = false;

                    break;
                }
            }
        }
        else if (facedir == -1)
        {
            foreach (GameObject nextTarget in EnemyInRange)
            {
                if (nextTarget.transform.position.x < CameraFollowTarget.transform.position.x)
                {
                    //Debug.Log("Trans!");
                    EnemyWatched = nextTarget;
                    stopFlagX = false;
                    //stopFlagY = false;
                    break;
                }
            }
        }
        
    }

    public int TargetAutoFix_Linear(float range)
    {
        RaycastHit2D hitinfol = 
            Physics2D.Raycast
                (transform.parent.position, new Vector2(1, 0), range, LayerMask.GetMask("Enemies"));
        RaycastHit2D hitinfor = 
            Physics2D.Raycast
                (transform.parent.position, new Vector2(-1,0), range, LayerMask.GetMask("Enemies"));

        bool left = false;
        bool right = false;
        if (hitinfol.collider != null)
        {
            
            if (hitinfol.collider.CompareTag("Enemy"))
            {
                left = true;
            }
                
            
        }
        if (hitinfor.collider != null)
        {
            if (hitinfor.collider.CompareTag("Enemy"))
            {
                right = true;
            }
        }

        ActorController ac = GetComponentInParent<ActorController>();
        if (left && !right)
        {
            ac.SetFaceDir(1);
            return 1;
        }else if (!left && right)
        {
            ac.SetFaceDir(-1);
            return -1;
        }
        else return 0;
        

    }

    public bool HasMarking(Transform transform)
    {
        var status = transform.GetComponentInParent<StatusManager>();
        if (status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Taunt) > 0)
            return true;
        return false;
    }

    private List<GameObject> MarkingCheck(List<GameObject> list)
    {
        //如果list中有被嘲讽的目标，就只返回被嘲讽的目标
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject obj in list)
        {
            if (HasMarking(obj.transform))
            {
                result.Add(obj);
            }
        }

        if (result.Count > 0)
        {
            return result;
        }

        return list;
    }
    
    //将MarkingCheck改为泛型方法

    /// <summary>
    /// 返回被嘲讽的目标，可以为空。
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private List<T> MarkingCheck<T>(List<T> list) where T : Component
    {
        //如果list中有被嘲讽的目标，就只返回被嘲讽的目标
        List<T> result = new List<T>();
        foreach (T obj in list)
        {
            if (HasMarking(obj.transform))
            {
                result.Add(obj);
            }
        }
        

        if (result.Count > 0)
        {
            return result;
        }

        return list;
    }
    
    private List<GameObject> MarkingCheck(List<GameObject> list, out bool markingExist)
    {
        //如果list中有被嘲讽的目标，就只返回被嘲讽的目标
        List<GameObject> result = new List<GameObject>();
        foreach (var obj in list)
        {
            if (HasMarking(obj.transform))
            {
                result.Add(obj);
            }
        }

        if (result.Count > 0)
        {
            markingExist = true;
            return result;
        }

        markingExist = false;
        return list;
    }

    public void FaceDirectionAutofixWithMarking()
    {
        bool markingExist;
        var enemyInRange = MarkingCheck(EnemyInRange,out markingExist);
        if(markingExist == false)
            return;
        else
        {
            enemyInRange.Sort(SortDistance);
            if (enemyInRange[0].transform.position.x > transform.parent.position.x)
                GetComponentInParent<ActorController>().SetFaceDir(1);
            else
                GetComponentInParent<ActorController>().SetFaceDir(-1);
        }
    }

}

