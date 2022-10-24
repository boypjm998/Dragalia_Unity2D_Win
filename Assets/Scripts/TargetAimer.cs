using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TargetAimer : MonoBehaviour
{
    public float aimSizeX;
    public float aimSizeY;
    [SerializeField]
    private GameObject CameraFollowTarget; //���Ŀ��
    [SerializeField]
    public GameObject EnemyWatched; //����ƫ��ĵ���Ŀ��
    private PolygonCollider2D TargetSearchScale; //������Χ������
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera; //����ͷ
    [SerializeField] 
    private CinemachineCameraOffset cinemachineCameraOffset; //����ͷƫ����
    
    private bool TargetFixed;

    [SerializeField]
    private List<GameObject> EnemyInRange; //�ھ��䷶Χ�ĵ����б�
    [SerializeField]
    List<GameObject> ObjectReachable; //�ڹ�����Χ�ڵĵط�

    [SerializeField]
    private float cameraMoveSpeed;//����ͷ�ƶ��ٶ�
    public float lookAheadDistanceX;//����X����������
    public float lookAheadDistanceY;//����Y����������
    //private Coroutine cameraMoveRoutine;
    private float maxCameraMoveSpeed;//����ͷ�ƶ�����ٶ�
    private float velocityCameraMoveSpeed;
    [SerializeField]
    private bool stopFlagX;//����ͷX����ֹͣλ��flag
    [SerializeField]
    private bool stopFlagY;//����ͷY����ֹͣλ��flag


    public bool TestButton;


    private void InitCameraFollowAttributes()
    {
        //cameraMoveSpeed = 0f;
        maxCameraMoveSpeed = 6.0f;
        lookAheadDistanceX = 14.0f;
        lookAheadDistanceY = 7.0f;
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
        InitCameraFollowAttributes();
        
    }

    private void Update()
    {
      
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        
        if (stopFlagX == false || stopFlagY == false)
        {
            maxCameraMoveSpeed = 6.0f;
        }
        else {
            maxCameraMoveSpeed = 0;
            
        }
        //����ͷ����
        cameraMoveSpeed = Mathf.SmoothDamp(cameraMoveSpeed, maxCameraMoveSpeed, ref velocityCameraMoveSpeed, 1.0f);

        if (EnemyWatched != null)
        {
            float offsetX =  CheckTargetXDistance();//��ȡ����ͷ����ƫ����
            float offsetY =  CheckTargetYDistance();
            int moveDirX; 
            int moveDirY;
            //print("����ͷ����ƫ����:"+offsetX);

            if (offsetX > 0)
                moveDirX = 1;
            else moveDirX = -1;

            if (offsetY > 0)
                moveDirY = 1;
            else moveDirY = -1;
            //�������ͷ������ƫ����С��Ŀ��ƫ�������ƶ�����ͷ
            if (Mathf.Abs(offsetX) > 0.1f || Mathf.Abs(offsetY) > 0.1f)
            {

                //if( Mathf.Abs(lookAheadDistanceX - Mathf.Abs(cinemachineCameraOffset.m_Offset.x) + CameraFollowTarget.transform.position.x) > Mathf.Abs(EnemyWatched.transform.position.x - CameraFollowTarget.transform.position.x))
                
                //���˲������߷�Χ�ڣ��ƶ�����ͷ
                if (Mathf.Abs(cinemachineCameraOffset.m_Offset.x) != Mathf.Abs(EnemyWatched.transform.position.x - CameraFollowTarget.transform.position.x) - lookAheadDistanceX)
                {
                    cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x + cameraMoveSpeed * Time.deltaTime * moveDirX,
                    cinemachineCameraOffset.m_Offset.y, 0);
                    stopFlagX = false;

                }
                else {
                    //print("����");
                    stopFlagX = true;
                }

                if (Mathf.Abs(cinemachineCameraOffset.m_Offset.y) < Mathf.Abs(EnemyWatched.transform.position.y - CameraFollowTarget.transform.position.y) - lookAheadDistanceY)
                {
                    cinemachineCameraOffset.m_Offset = new Vector3(
                    cinemachineCameraOffset.m_Offset.x,
                    cinemachineCameraOffset.m_Offset.y + cameraMoveSpeed * Time.deltaTime * moveDirY, 0);
                    stopFlagY = false;

                }
                else {
                    stopFlagY = true;
                }
                
                if (stopFlagY && stopFlagX)
                    cameraMoveSpeed = 0.1f;
                
            }


        }
        else {
            
            //������ʧ����ͷ��λ��ͬ��
            stopFlagY = false;
            stopFlagX = false;
            if (Mathf.Abs(cinemachineCameraOffset.m_Offset.x) < 0.1f)
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




            if (Mathf.Abs(cinemachineCameraOffset.m_Offset.y) < 0.1f)
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





    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        if (!collision.CompareTag("Enemy"))
            return;
        if (EnemyWatched != null )
        {

            if (collision.GetComponent<Transform>().gameObject == EnemyWatched)
            {
                Debug.Log("Unlocked:" + EnemyWatched);
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
            maxCameraMoveSpeed = 6;
        }

    }


    private float CheckTargetXDistance()
    {
        float offsetX;
        float distanceX;
        
        offsetX = (EnemyWatched.transform.position.x - CameraFollowTarget.transform.position.x)- cinemachineCameraOffset.m_Offset.x;
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

        offsetY = (EnemyWatched.transform.position.y - CameraFollowTarget.transform.position.y) - cinemachineCameraOffset.m_Offset.y;
        distanceY = Mathf.Abs(offsetY);

        if (distanceY > lookAheadDistanceY)
        {
            if (offsetY > 0)
                return distanceY - lookAheadDistanceY;
            else return lookAheadDistanceX - distanceY;
        }
        else return 0;
    }

    private float DistanceFromPlayerToTarget(Transform target)
    {
        
        Transform playerTransform = CameraFollowTarget.transform;

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

    public Transform GetNearestTargetInRangeDirection(int dir, float sizeX, float sizeY, LayerMask targetLayers)
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
            Physics2D.OverlapAreaAll(transform.parent.position + new Vector3(sizeX*rightModifier, sizeY), transform.parent.position + new Vector3(-sizeX*leftModifier, -sizeY),
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


    //���߼�⣬��ȡ����ܹ������ĵ���
    public Transform GetNearestReachableTarget(float range, LayerMask targetLayers)
    {
        
        if (EnemyInRange.Count == 0)
            return null;
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
            return ObjectReachable[0].transform;
        }
        else {
            //EnemyInRange.Sort(SortDistance);
            return null;
        }

        //return EnemyInRange[0].transform;
    }

    //ͨ���������л�Ŀ��
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
                    stopFlagY = false;

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
                    stopFlagY = false;
                    break;
                }
            }
        }
        
    }

}

