using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

/// <summary>
/// lilith的饴晶穿梭
/// </summary>
public class Projectile_H002_1 : MonoBehaviour, IEnemySealedContainer
{
    [SerializeField] private bool redIsSafe = true;
    [SerializeField] private int redZone = -1;

    [Tooltip("横向激光攻击的预制体，只有特效，没有伤害")]
    [SerializeField] private GameObject laserAttackPrefabHorizontal;
    [Tooltip("纵向激光攻击的预制体，只有特效，没有伤害")]
    [SerializeField] private GameObject laserAttackPrefabVertical;
    [Tooltip("激光攻击的预制体，没有特效，只有伤害")]
    [SerializeField] private List<GameObject> laserAttackColliderPrefab;
    
    /// <summary>
    /// 当RedIsSafe为true时，红色糖果的起始位置为最终的安全位置，否则为紫色糖果的初始位置。
    /// 此时红色和紫色糖果都会出现在屏幕的左/右同侧，否则红色和紫色糖果会出现在屏幕的上/下同侧。
    /// </summary>
    public bool RedIsSafe { get; set; } = true;

    /// <summary>
    /// 红色糖果的象限，1代表第一象限，2代表第二象限，3代表第三象限，4代表第四象限。
    /// </summary>
    public int RedZone { get; private set; } = -1;
    
    /// <summary>
    /// 紫色糖果的象限，1代表第一象限，2代表第二象限，3代表第三象限，4代表第四象限。
    /// 不能通过外部设置，只能由RedZone和RedIsSafe共同确定。
    /// </summary>
    public int PurpleZone { get; private set; } = -1;
    
    /// <summary>
    /// 随机种子，用于确定红色糖果的象限。
    /// </summary>
    public int RandomSeed
    {
        get => RedZone;
        set
        {
            if (value < 1 || value > 4)
            {
                RedZone = Random.Range(1, 5);
            }else
            {
                RedZone = value;
            }
        }
    }
    
    
    [SerializeField]
    [Tooltip("红色糖果的父物体，飞行到终点时，会发射横向激光")]
    private Transform redCandiesParent;
    [SerializeField]
    [Tooltip("紫色糖果的父物体，飞行到终点时，会发射纵向激光")]
    private Transform purpleCandiesParent;

    private Transform enemySource;

    /// <summary>
    /// 糖果的初始位置，根据糖果的象限不同，需要对其X和Y坐标进行调整。
    /// </summary>
    private static Dictionary<string, Vector2> _startPositionDictionary =
        new Dictionary<string, Vector2>()
        {
            {"FirstUpper", new Vector2(9, 6)},//-16
            {"SecondUpper", new Vector2(13, 4)},//-14
            {"ThirdUpper", new Vector2(16, 1)},//-11
            
            {"FirstLower", new Vector2(9, -16)},
            {"SecondLower", new Vector2(13, -14)},
            {"ThirdLower", new Vector2(16, -11)}
        };
    
    /// <summary>
    /// 糖果的终点位置，根据糖果的象限不同，需要对其X和Y坐标进行调整。
    /// </summary>
    private static Dictionary<string,Vector2> _endPositionDictionary =
        new Dictionary<string, Vector2>()
        {
            
            //紫色
            {"UpperHorizontalLeft", new Vector2(6, 5)},
            {"UpperHorizontalCenter", new Vector2(12, 5)},
            {"UpperHorizontalRight", new Vector2(18, 5)},
            
            {"LowerHorizontalLeft", new Vector2(6, -11)},
            {"LowerHorizontalCenter", new Vector2(12, -11)},
            {"LowerHorizontalRight", new Vector2(18, -11)},
            
            //红色
            {"UpperVerticalUp", new Vector2(12, 9)},
            {"UpperVerticalCenter", new Vector2(12, 5)},
            {"UpperVerticalDown", new Vector2(12, 1)},
                
            {"LowerVerticalUp", new Vector2(12, -15)},
            {"LowerVerticalCenter", new Vector2(12, -11)},
            {"LowerVerticalDown", new Vector2(12, -7)}
        };
    
    private static List<Vector2> _laserPositionList = new List<Vector2>()
    {
        new Vector2(12, 14),//第一象限
        new Vector2(-12, 14),//第二象限
        new Vector2(-12, -2),//第三象限
        new Vector2(12, -2),//第四象限
    };

    private List<Vector2> _redCandiesEndPositionList = new List<Vector2>();
    private List<Vector2> _purpleCandiesEndPositionList = new List<Vector2>();

    private IEnumerator Start()
    {
        InitAttacks();
        InitZoneData();

        yield return null;
        InitStartPositions();
        InitEndPositions();
        
        yield return new WaitForSeconds(1.5f);
        
        LaunchCandies(2f);
        Invoke("LaserAttack",1.9f);

        yield return new WaitForSeconds(3);
        
        Destroy(gameObject);
        
    }
    
    public void SetEnemySource(GameObject enemySource)
    {
        this.enemySource = enemySource.transform;
    }

    private void InitAttacks()
    {
        foreach (Transform candyTransform in redCandiesParent)
        {
            candyTransform.GetComponent<AttackFromEnemy>().enemySource = enemySource.gameObject;
        }
        foreach (Transform candyTransform in purpleCandiesParent)
        {
            candyTransform.GetComponent<AttackFromEnemy>().enemySource = enemySource.gameObject;
        }
    }
    
    /// <summary>
    /// 根据红色糖果的象限和RedIsSafe的值，确定紫色糖果的象限。
    /// </summary>
    private void InitZoneData()
    {
        if (RedZone < 0)
        {
            RedZone = Random.Range(1, 5);
        }
        
        // 根据红色糖果的象限和RedIsSafe的值，确定紫色糖果的象限
        // 已知红色糖果的象限，若RedIsSafe为true，则紫色糖果一定和红色糖果在屏幕的左/右同侧，否则在屏幕的上/下同侧。
        switch (RedZone)
        {
            case 1:
                PurpleZone = RedIsSafe ? 4 : 2;
                break;
            case 2:
                PurpleZone = RedIsSafe ? 3 : 1;
                break;
            case 3:
                PurpleZone = RedIsSafe ? 2 : 4;
                break;
            case 4:
                PurpleZone = RedIsSafe ? 1 : 3;
                break;
        }
        
        
        
    }

    /// <summary>
    /// 初始化红色和紫色糖果的位置。
    /// </summary>
    private void InitStartPositions()
    {
        
        
        
        Vector2 firstQuadrant = new Vector2(1, 1);
        Vector2 secondQuadrant = new Vector2(-1, 1);
        Vector2 thirdQuadrant = new Vector2(-1, -1);
        Vector2 fourthQuadrant = new Vector2(1, -1);

        Vector2 redCandiesQuadrant;
        Vector2 purpleCandiesQuadrant;
        
        switch (RedZone)
        {
            case 1:
                redCandiesQuadrant = firstQuadrant;
                break;
            case 2:
                redCandiesQuadrant = secondQuadrant;
                break;
            case 3:
                redCandiesQuadrant = thirdQuadrant;
                break;
            case 4:
                redCandiesQuadrant = fourthQuadrant;
                break;
            default:
                redCandiesQuadrant = firstQuadrant;
                break;
        }
        
        switch (PurpleZone)
        {
            case 1:
                purpleCandiesQuadrant = firstQuadrant;
                break;
            case 2:
                purpleCandiesQuadrant = secondQuadrant;
                break;
            case 3:
                purpleCandiesQuadrant = thirdQuadrant;
                break;
            case 4:
                purpleCandiesQuadrant = fourthQuadrant;
                break;
            default:
                purpleCandiesQuadrant = firstQuadrant;
                break;
        }
        
        
        //根据糖果的象限，对各个糖果的初始位置进行调整，注意1,2象限是引用FirstUpper，3,4象限是引用FirstLower
        redCandiesParent.GetChild(0).position =
            new Vector2(redCandiesQuadrant.x * _startPositionDictionary["FirstUpper"].x,
                redCandiesQuadrant.y > 0? _startPositionDictionary["FirstUpper"].y : _startPositionDictionary["FirstLower"].y);
        
        redCandiesParent.GetChild(1).position =
            new Vector2(redCandiesQuadrant.x * _startPositionDictionary["SecondUpper"].x,
                redCandiesQuadrant.y > 0? _startPositionDictionary["SecondUpper"].y : _startPositionDictionary["SecondLower"].y);
        
        redCandiesParent.GetChild(2).position =
            new Vector2(redCandiesQuadrant.x * _startPositionDictionary["ThirdUpper"].x,
                redCandiesQuadrant.y > 0? _startPositionDictionary["ThirdUpper"].y : _startPositionDictionary["ThirdLower"].y);
        
        purpleCandiesParent.GetChild(0).position =
            new Vector2(purpleCandiesQuadrant.x * _startPositionDictionary["FirstUpper"].x,
                purpleCandiesQuadrant.y > 0? _startPositionDictionary["FirstUpper"].y : _startPositionDictionary["FirstLower"].y);
        
        purpleCandiesParent.GetChild(1).position =
            new Vector2(purpleCandiesQuadrant.x * _startPositionDictionary["SecondUpper"].x,
                purpleCandiesQuadrant.y > 0? _startPositionDictionary["SecondUpper"].y : _startPositionDictionary["SecondLower"].y);
        
        purpleCandiesParent.GetChild(2).position = 
            new Vector2(purpleCandiesQuadrant.x * _startPositionDictionary["ThirdUpper"].x,
                purpleCandiesQuadrant.y > 0? _startPositionDictionary["ThirdUpper"].y : _startPositionDictionary["ThirdLower"].y);


    }

    /// <summary>
    /// 初始化红色和紫色糖果的终点位置。
    /// </summary>
    private void InitEndPositions()
    {
        // 翻转的象限即为终点位置的象限
        Vector2 firstQuadrant = new Vector2(-1, -1);
        Vector2 secondQuadrant = new Vector2(1, -1);
        Vector2 thirdQuadrant = new Vector2(1, 1);
        Vector2 fourthQuadrant = new Vector2(-1, 1);

        Vector2 redCandiesQuadrant;
        Vector2 purpleCandiesQuadrant;

        switch (RedZone)
        {
            case 1:
                redCandiesQuadrant = firstQuadrant;
                break;
            case 2:
                redCandiesQuadrant = secondQuadrant;
                break;
            case 3:
                redCandiesQuadrant = thirdQuadrant;
                break;
            case 4:
                redCandiesQuadrant = fourthQuadrant;
                break;
            default:
                redCandiesQuadrant = firstQuadrant;
                break;
        }

        switch (PurpleZone)
        {
            case 1:
                purpleCandiesQuadrant = firstQuadrant;
                break;
            case 2:
                purpleCandiesQuadrant = secondQuadrant;
                break;
            case 3:
                purpleCandiesQuadrant = thirdQuadrant;
                break;
            case 4:
                purpleCandiesQuadrant = fourthQuadrant;
                break;
            default:
                purpleCandiesQuadrant = firstQuadrant;
                break;
        }
        
        _redCandiesEndPositionList.Clear();
        _purpleCandiesEndPositionList.Clear();

        //根据糖果的象限，对各个糖果的终点位置进行调整
        _redCandiesEndPositionList.Add
        (new Vector2(_endPositionDictionary["UpperVerticalUp"].x * redCandiesQuadrant.x,
            redCandiesQuadrant.y > 0? _endPositionDictionary["UpperVerticalUp"].y : _endPositionDictionary["LowerVerticalUp"].y));
        
        _redCandiesEndPositionList.Add
            (new Vector2(_endPositionDictionary["UpperVerticalCenter"].x * redCandiesQuadrant.x,
                redCandiesQuadrant.y > 0? _endPositionDictionary["UpperVerticalCenter"].y : _endPositionDictionary["LowerVerticalCenter"].y));
        
        _redCandiesEndPositionList.Add
            (new Vector2(_endPositionDictionary["UpperVerticalDown"].x * redCandiesQuadrant.x,
                redCandiesQuadrant.y > 0? _endPositionDictionary["UpperVerticalDown"].y : _endPositionDictionary["LowerVerticalDown"].y));
        
        _purpleCandiesEndPositionList.Add
            (new Vector2(_endPositionDictionary["UpperHorizontalLeft"].x * purpleCandiesQuadrant.x,
                purpleCandiesQuadrant.y > 0? _endPositionDictionary["UpperHorizontalLeft"].y : _endPositionDictionary["LowerHorizontalLeft"].y));
        
        _purpleCandiesEndPositionList.Add
            (new Vector2(_endPositionDictionary["UpperHorizontalCenter"].x * purpleCandiesQuadrant.x,
                purpleCandiesQuadrant.y > 0? _endPositionDictionary["UpperHorizontalCenter"].y : _endPositionDictionary["LowerHorizontalCenter"].y));
        
        _purpleCandiesEndPositionList.Add
            (new Vector2(_endPositionDictionary["UpperHorizontalRight"].x * purpleCandiesQuadrant.x,
                purpleCandiesQuadrant.y > 0? _endPositionDictionary["UpperHorizontalRight"].y : _endPositionDictionary["LowerHorizontalRight"].y));
        
        redCandiesParent.gameObject.SetActive(true);
        purpleCandiesParent.gameObject.SetActive(true);
       
        
        
        
        
    }

    private void LaunchCandies(float hideTime)
    {
        // redCandiesParent.gameObject.SetActive(true);
        // purpleCandiesParent.gameObject.SetActive(true);
        // 红色糖果
        for (int i = 0; i < redCandiesParent.childCount; i++)
        {
            redCandiesParent.GetChild(i).DOMove(_redCandiesEndPositionList[i], 1.5f).SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed);
        }

        // 紫色糖果
        for (int i = 0; i < purpleCandiesParent.childCount; i++)
        {
            purpleCandiesParent.GetChild(i).DOMove(_purpleCandiesEndPositionList[i], 1.5f).SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed);
        }
        
        Invoke("HideAllCandies",hideTime);
    }
    
    private void HideAllCandies()
    {
        redCandiesParent.gameObject.SetActive(false);
        purpleCandiesParent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 根据RedIsSafe和RedZone综合判断，对激光的安全区域进行调整。
    /// </summary>
    private void LaserAttack()
    {
        int safeQuadrant;
        if (RedIsSafe)
        {
            safeQuadrant = RedZone;
        }
        else
        {
            safeQuadrant = PurpleZone;
        }
        
        print(PurpleZone);
        print(RedZone);

        Vector3 redLaserPosition = Vector3.zero;
        Vector3 purpleLaserPosition = Vector3.zero;;
        
        switch (RedZone)
        {
            case 1:
                redLaserPosition = _laserPositionList[2];
                break;
            case 2:
                redLaserPosition = _laserPositionList[3];
                break;
            case 3:
                redLaserPosition = _laserPositionList[0];
                break;
            case 4:
                redLaserPosition = _laserPositionList[1];
                break;
        }

        switch (PurpleZone)
        {
            case 1:
                purpleLaserPosition = _laserPositionList[2];
                break;
            case 2:
                purpleLaserPosition = _laserPositionList[3];
                break;
            case 3:
                purpleLaserPosition = _laserPositionList[0];
                break;
            case 4:
                purpleLaserPosition = _laserPositionList[1];
                break;
        }
        
        
        
        Instantiate(laserAttackPrefabHorizontal, redLaserPosition,
            Quaternion.Euler(0, 0, 90),transform);
        
        Instantiate(laserAttackPrefabVertical, purpleLaserPosition,
            Quaternion.identity,transform);

        var laserPrefab = laserAttackColliderPrefab[safeQuadrant-1];
        
        this.InstantiateRangedObject(laserPrefab, Vector3.zero,
            BattleStageManager.Instance.GetNewRangedContainer(), 1,1,enemySource);


    }



}
