using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using UnityEngine;

public class Projectile_C003_3_Boss : MonoBehaviour
{
    [Range(0.1f,1f)]public float throwInterval = 0.15f;
    private GameObject myGameObject;
    
    private List<Transform> targetList = new();
    private List<Projectile_C003_2_Boss> projectileList = new();
    private List<Transform> projectiles = new();
    private int thrownCount = 0;
    public int firedir = 1;
    private void Start()
    {
        // if(myGameObject == null)
        //     myGameObject = GameObject.Find("PlayerHandle").gameObject;
        
        targetList = SearchEnemyList();
        for(int i = 0 ; i < transform.childCount ; i++)
        {
            var projectile = transform.GetChild(i).GetComponent<Projectile_C003_2_Boss>();
            projectile.SetFiredir(firedir);
            projectileList.Add(projectile);
            projectiles.Add(transform.GetChild(i));
        }
        //将targetList按照和myGameObject的距离排序
        if (targetList.Count > 0)
        {
            targetList.Sort((a, b) =>
            {
                var aDistance = Vector2.Distance(a.position, myGameObject.transform.position);
                var bDistance = Vector2.Distance(b.position, myGameObject.transform.position);
                return aDistance.CompareTo(bDistance);
            });

        }
        else
        {
            targetList.Add(myGameObject.transform);
        }
        DistrubuteProjectileTargets(targetList);
        Invoke("ThrowProjectile", 0);
        Invoke("ThrowProjectile", throwInterval);
        Invoke("ThrowProjectile", throwInterval * 2);
        Invoke("ThrowProjectile", throwInterval * 3);
        Invoke("ThrowProjectile", throwInterval * 4);
    }
    
    public virtual List<Transform> SearchEnemyList()
    {
        var hitFlags = new List<Transform>();

        var enemyLayer = GameObject.Find("Player");
        for (var i = 0; i < enemyLayer.transform.childCount; i++)
            if (enemyLayer.transform.GetChild(i).gameObject.activeSelf)
                hitFlags.Add(enemyLayer.transform.GetChild(i));
        return hitFlags;
    }

    void DistrubuteProjectileTargets(List<Transform> targets)
    {
        for (int i = 0; i < 5; i++)
        {
            projectileList[i].SetContactTarget(targets[i % targets.Count].gameObject);
        }
    }

    void ThrowProjectile()
    {
        thrownCount++;
        projectiles[thrownCount-1].gameObject.SetActive(true);
        //transform.GetChild(thrownCount - 1).gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
