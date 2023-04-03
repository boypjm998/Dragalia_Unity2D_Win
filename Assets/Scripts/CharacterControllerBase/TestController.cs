using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics;

public class TestController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool findPathTrigger = false;
    public List<Platform> mapInfo;
    public GameObject target;
    void Start()
    {
        mapInfo = InitMapInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (findPathTrigger)
        {
            findPathTrigger = false;
            //FindPath();
            TestMatrix();
        }
    }

    void FindPath()
    {
        AStar aStar = new AStar(4,20,7,2);
        var pathList = aStar.Execute(mapInfo, transform, target.transform);
        //找到mapInfo里面叫Platform2和Platform6的两个平台
        
        //TestMatrix();
        
    }

    void TestMatrix()
    {
        Platform GroundPic = mapInfo.Find(x => x.collider.name == "GroundPic");
        Platform platform1 = mapInfo.Find(x => x.collider.name == "Platform1");
        Platform platform2 = mapInfo.Find(x => x.collider.name == "Platform2");
        Platform platform3 = mapInfo.Find(x => x.collider.name == "Platform3");
        Platform platform4 = mapInfo.Find(x => x.collider.name == "Platform4");
        Platform platform5 = mapInfo.Find(x => x.collider.name == "Platform5");
        Platform platform6 = mapInfo.Find(x => x.collider.name == "Platform6");
        
        
        //输出6*6的邻接矩阵
        print("platform1 to platform2: "+Platform.CanReach(platform1, platform2,4,20,7));
        print("platform1 to platform3: "+Platform.CanReach(platform1, platform3,4,20,7));
        print("platform1 to platform4: "+Platform.CanReach(platform1, platform4,4,20,7));
        print("platform1 to platform5: "+Platform.CanReach(platform1, platform5,4,20,7));
        print("platform1 to platform6: "+Platform.CanReach(platform1, platform6,4,20,7));
        print("platform1 to GroundPic: "+Platform.CanReach(platform1, GroundPic,4,20,7));
        print("platform2 to platform1: "+Platform.CanReach(platform2, platform1,4,20,7));
        print("platform2 to platform3: "+Platform.CanReach(platform2, platform3,4,20,7));
        print("platform2 to platform4: "+Platform.CanReach(platform2, platform4,4,20,7));
        print("platform2 to platform5: "+Platform.CanReach(platform2, platform5,4,20,7));
        print("platform2 to platform6: "+Platform.CanReach(platform2, platform6,4,20,7));
        print("platform2 to GroundPic: "+Platform.CanReach(platform2, GroundPic,4,20,7));
        print("platform3 to platform1: "+Platform.CanReach(platform3, platform1,4,20,7));
        print("platform3 to platform2: "+Platform.CanReach(platform3, platform2,4,20,7));
        print("platform3 to platform4: "+Platform.CanReach(platform3, platform4,4,20,7));
        print("platform3 to platform5: "+Platform.CanReach(platform3, platform5,4,20,7));
        print("platform3 to platform6: "+Platform.CanReach(platform3, platform6,4,20,7));
        print("platform3 to GroundPic: "+Platform.CanReach(platform3, GroundPic,4,20,7));
        print("platform4 to platform1: "+Platform.CanReach(platform4, platform1,4,20,7));
        print("platform4 to platform2: "+Platform.CanReach(platform4, platform2,4,20,7));
        print("platform4 to platform3: "+Platform.CanReach(platform4, platform3,4,20,7));
        print("platform4 to platform5: "+Platform.CanReach(platform4, platform5,4,20,7));
        print("platform4 to platform6: "+Platform.CanReach(platform4, platform6,4,20,7));
        print("platform4 to GroundPic: "+Platform.CanReach(platform4, GroundPic,4,20,7));
        print("platform5 to platform1: "+Platform.CanReach(platform5, platform1,4,20,7));
        print("platform5 to platform2: "+Platform.CanReach(platform5, platform2,4,20,7));
        print("platform5 to platform3: "+Platform.CanReach(platform5, platform3,4,20,7));
        print("platform5 to platform4: "+Platform.CanReach(platform5, platform4,4,20,7));
        print("platform5 to platform6: "+Platform.CanReach(platform5, platform6,4,20,7));
        print("platform5 to GroundPic: "+Platform.CanReach(platform5, GroundPic,4,20,7));
        print("platform6 to platform1: "+Platform.CanReach(platform6, platform1,4,20,7));
        print("platform6 to platform2: "+Platform.CanReach(platform6, platform2,4,20,7));
        print("platform6 to platform3: "+Platform.CanReach(platform6, platform3,4,20,7));
        print("platform6 to platform4: "+Platform.CanReach(platform6, platform4,4,20,7));
        print("platform6 to platform5: "+Platform.CanReach(platform6, platform5,4,20,7));
        print("platform6 to GroundPic: "+Platform.CanReach(platform6, GroundPic,4,20,7));
        print("GroundPic to platform1: "+Platform.CanReach(GroundPic, platform1,4,20,7));
        print("GroundPic to platform2: "+Platform.CanReach(GroundPic, platform2,4,20,7));
        print("GroundPic to platform3: "+Platform.CanReach(GroundPic, platform3,4,20,7));
        print("GroundPic to platform4: "+Platform.CanReach(GroundPic, platform4,4,20,7));
        print("GroundPic to platform5: "+Platform.CanReach(GroundPic, platform5,4,20,7));
        print("GroundPic to platform6: "+Platform.CanReach(GroundPic, platform6,4,20,7));


    }

    List<Platform> InitMapInfo()
    {
        //获取场景上所有tag为platform或Ground的物体和其碰撞体
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        GameObject[] all = new GameObject[platforms.Length + grounds.Length];
        platforms.CopyTo(all, 0);
        grounds.CopyTo(all, platforms.Length);
        print(all.Length);
        //将all中所有碰撞体存入mapInfo列表。
        List<Platform> platformsInfo = new List<Platform>();
        foreach (GameObject go in all)
        {
            print(go.name);
            var platform = new Platform(go);
            platformsInfo.Add(platform);
        }

        

        return platformsInfo;
    }


}
