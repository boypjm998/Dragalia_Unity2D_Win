using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class DamageNumberManager : MonoBehaviour
{
    public int fontSize = 12;
    public GameObject normDamageNumPrefab;
    public GameObject critDamageNumPrefab;
    public GameObject critPrefab;
    [SerializeField]
    private GameObject totalDamagePrefab;

    public void DamagePopEnemy(Transform enemyPos,int dmg, int dmgType)
    {
        
        //Vector3 newPosition = new Vector3(enemyPos.position.x + Random.Range(-1f, 1f), enemyPos.position.y + Random.Range(-0.5f, 0.5f) + 3f, enemyPos.position.z);

        int dmgNumPos = SelectDamageNumberPosition(enemyPos); 
        //默认是0，代表伤害数字产生的位置（子物体）的ID

        Transform dmgNumParent = GetComponent<Transform>().GetChild(dmgNumPos);
        //搜寻上一步找到的子物体的Transform。

        Vector3 newDmgNumPosVec = PositionSelectByChild(enemyPos, dmgNumPos);
        //确定伤害数字实例化的位置基准

        if (dmgType == 1)
        {
            GenerateNormalDamageNumber(dmg, newDmgNumPosVec, enemyPos, dmgNumParent);
        }
        else if (dmgType == 2)
        {
            GenerateCriticalDamageNumber(dmg, newDmgNumPosVec, enemyPos, dmgNumParent);
        }
        



        /*
        if (dmgType == 1)
        {
            num =
            Instantiate(normDamageNumPrefab,
            new Vector3(newDmgNumPosVec.x + Random.Range(-1f, 1f), newDmgNumPosVec.y + Random.Range(-0.5f, 0.5f) + 3f, newDmgNumPosVec.z),
            Quaternion.identity,
            dmgNumParent);
        }
        if (dmgType == 2)
        {
            num =
            Instantiate(critDamageNumPrefab,
            new Vector3(newDmgNumPosVec.x + Random.Range(-1f, 1f), newDmgNumPosVec.y + Random.Range(-0.5f, 0.5f) + 3f, newDmgNumPosVec.z),
            Quaternion.identity,
            dmgNumParent);

            GameObject crit = null;

            crit = 
            Instantiate(critPrefab,
            new Vector3(num.transform.position.x, num.transform.position.y-1.4f, num.transform.position.z),
            Quaternion.identity,
            dmgNumParent);
            TextMeshPro critText = crit.GetComponentInChildren<TextMeshPro>();
            critText.fontSize = fontSize*0.9f;
        }


        TextMeshPro dmgText = num.transform.GetChild(0).GetComponent<TextMeshPro>();
        //dmgText.text = "<sprite=9><sprite=9><sprite=8>";
        dmgText.text = Text2SpriteAssetNumber(dmg,dmgType);
        if (dmgType == 1)
            dmgText.fontSize = this.fontSize;
        else
        {
            dmgText.fontSize = 1.2f*this.fontSize;
        }
        InstancePropertyUI dmgProperty = num.GetComponent<InstancePropertyUI>();
        dmgProperty.SetGenerateParent(enemyPos);
        */
    }

    //伤害数字位置检测，防止一个区域伤害数字叠的太多导致重复遮挡
    
    private int SelectDamageNumberPosition(Transform oldPosition)
    {
        int temp = SearchEmptyPositionToInstantiateDmgNum(GetComponent<Transform>().GetChild(0), oldPosition);
        if (temp==0 || temp>5)
        {
            //Debug.Log(0);
            return 0;
        }
        else {
            int rand = (Random.Range(0, 8));
            //Debug.Log(rand);
            for (int i = 0; i < 8; i++)
            {
                if (SearchEmptyPositionToInstantiateDmgNum(GetComponent<Transform>().GetChild(rand+1), oldPosition) == 0)
                {
                    return rand;
                }
                rand = (rand + 1) % 8;
            }
            return rand;
        }


        
    }

    private string Text2SpriteAssetNumber(int num,int dmgType)
    {
        //伤害数字实例化
        string str = num.ToString();
        int len = str.Length;

        StringBuilder sb = new StringBuilder(10);
        if (dmgType == 1)

        {
            for (int i = 0; i < len; i++)
            {
                sb.Append($"<sprite={str[i]}>");
            }
            return sb.ToString();

        }
        else {
            for (int i = 0; i < len; i++)
            {
                sb.Append($"<sprite={str[i]}>");
            }
            //sb.Append("\n              <sprite=10>");
            return sb.ToString();
        }
        





    }

    private int SearchEmptyPositionToInstantiateDmgNum(Transform _parent, Transform target)
    {
        int count = 0;
        foreach(Transform t in _parent)
        {
            if (t.GetComponentInChildren<InstancePropertyUI>().GetGenerateParent() == target)
            {
                count++;
            }
        }
        return count;
    }

    private Vector3 PositionSelectByChild(Transform oldPos, int childID)
    {
        //返回一个向量：新位置
        float offsetX=0;
        float offsetY=0;
        switch (childID)
        {
            case 0:
                break;
            case 1://Left
                offsetX = -1;
                offsetY = 0;
                break;
            case 2://Right
                offsetX = 1;
                offsetY = 0;
                break;
            case 3:
                offsetX = 0;
                offsetY = 1;
                break;
            case 4:
                offsetX = 0;
                offsetY = -1;
                break;
            case 5:
                offsetX = -1;
                offsetY = -1;
                break;
            case 6:
                offsetX = 1;
                offsetY = -1;
                break;
            case 7:
                offsetX = -1;
                offsetY = 1;
                break;
            case 8:
                offsetX = 1;
                offsetY = 1;
                break;
            default:
                break;

        }
        return new Vector3(oldPos.position.x+offsetX, oldPos.position.y+offsetY,oldPos.position.z);
    }

    private void GenerateNormalDamageNumber(int dmg, Vector3 newDmgNumPosVec, Transform enemyPos, Transform dmgNumParent)
    {
        GameObject num = null;
        num =
            Instantiate(normDamageNumPrefab,
            new Vector3(newDmgNumPosVec.x + Random.Range(-0.5f, 0.5f), newDmgNumPosVec.y + Random.Range(-0.5f, 0.5f) + 3f, newDmgNumPosVec.z),
            Quaternion.identity,
            dmgNumParent);


        TextMeshPro dmgText = num.transform.GetChild(0).GetComponent<TextMeshPro>();
        
        dmgText.text = Text2SpriteAssetNumber(dmg, 1);
        
        dmgText.fontSize = this.fontSize;
       
        InstancePropertyUI dmgProperty = num.GetComponent<InstancePropertyUI>();
        dmgProperty.SetGenerateParent(enemyPos);

    }

    private void GenerateCriticalDamageNumber(int dmg, Vector3 newDmgNumPosVec, Transform enemyPos, Transform dmgNumParent)
    {
        

        GameObject num =
            Instantiate(critDamageNumPrefab,
            new Vector3(newDmgNumPosVec.x + Random.Range(-0.5f, 0.5f), newDmgNumPosVec.y + Random.Range(-0.5f, 0.5f) + 3f, newDmgNumPosVec.z),
            Quaternion.identity,
            dmgNumParent);

        

        GameObject crit =
            Instantiate(critPrefab,
            new Vector3(num.transform.position.x, num.transform.position.y - 1.4f, num.transform.position.z),
            Quaternion.identity,
            dmgNumParent);
            TextMeshPro critText = crit.GetComponentInChildren<TextMeshPro>();
            critText.fontSize = fontSize * 0.9f;

        TextMeshPro dmgText = num.transform.GetChild(0).GetComponent<TextMeshPro>();
        //dmgText.text = "<sprite=9><sprite=9><sprite=8>";
        dmgText.text = Text2SpriteAssetNumber(dmg,2);
        
        dmgText.fontSize = 1.2f * this.fontSize;
        
        InstancePropertyUI dmgProperty = num.GetComponent<InstancePropertyUI>();
        dmgProperty.SetGenerateParent(enemyPos);




    }

    public void SpawnTotalDamage(int damage)
    {
        

        Transform displaypos = GameObject.Find("PlayerHandle").transform;

        Transform postrans = GameObject.Find("TotalDamageDisplayer").transform;

        if (postrans.childCount != 0)
        {
            for (int i = 0; i < postrans.childCount; i++)
            {
                Destroy(postrans.GetChild(i).gameObject);
            }
        }

        GameObject camera = GameObject.Find("Main Camera");

        GameObject num =
            Instantiate(totalDamagePrefab,
            new Vector3(displaypos.position.x + Random.Range(-1f, 1f), displaypos.position.y + Random.Range(-0.5f, 0.5f) + 4f, 0),
            Quaternion.identity,
            postrans);

        TextMeshPro tmp = num.GetComponentInChildren<TextMeshPro>();
        tmp.text = Text2SpriteAssetNumber(damage, 1);

        if (Mathf.Abs(num.transform.position.y - camera.transform.position.y) > 3)
        {
            num.transform.position = new Vector2(num.transform.position.x, camera.transform.position.y);
        }
        if (Mathf.Abs(num.transform.position.x - camera.transform.position.x) > 5)
        {
            num.transform.position = new Vector2(camera.transform.position.x, num.transform.position.y);
        }


    }



}
