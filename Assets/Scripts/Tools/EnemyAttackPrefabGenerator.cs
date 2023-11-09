using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameMechanics;


public class EnemyAttackPrefabGenerator : MonoBehaviour
{
    private AttackPrefabInfo info;
    
    private void ParseAndOutput()
    {
        
        





    }

    private void ParseColliderInfo()
    {
        Collider2D collider2D = GetComponent<Collider2D>();

        if (collider2D == null)
        {
            info.colliderType = AttackPrefabInfo.ColliderType.None;
            return;
        }

        if (collider2D is BoxCollider2D)
        {
            var boxCollider2D = collider2D as BoxCollider2D;
            info.colliderType = AttackPrefabInfo.ColliderType.Box;
            info.SetBoxColliderInfo(boxCollider2D.offset.x,
                boxCollider2D.offset.y,boxCollider2D.size.x,
                boxCollider2D.size.y);
        }
        else if(collider2D is CircleCollider2D)
        {
            var circleCollider2D = collider2D as CircleCollider2D;
            info.colliderType = AttackPrefabInfo.ColliderType.Circle;
            info.SetCircleColliderInfo(circleCollider2D.offset.x,
                circleCollider2D.offset.y,circleCollider2D.radius);
        }
        else
        {
            var polygonCollider2D = collider2D as PolygonCollider2D;
            info.colliderType = AttackPrefabInfo.ColliderType.Polygon;
            info.SetPolygonColliderInfo(polygonCollider2D.GetTotalPointCount(),
                polygonCollider2D.points.ToList());
        }



    }

    private void ParseAttackValueInfo()
    {
        var attackValue = GetComponent<AttackFromEnemy>();
        if (attackValue == null)
        {
            info.attackType = AttackPrefabInfo.AttackType.None;
            return;
        }

        info.attackInfos = attackValue.attackInfo;
        info.attackProperty = attackValue.attackType;
        info.interruptable = attackValue.isMeele;
        info.AvoidableProperty = attackValue.GetAvoidableProperty();
        var attackInfo = attackValue.attackInfo;
        
        
        if (attackValue is CustomMeeleFromEnemy)
        {
            info.attackType = AttackPrefabInfo.AttackType.Meele;
        }else if (attackValue is CustomRangedFromEnemy)
        {
            info.attackType = AttackPrefabInfo.AttackType.Ranged;
        }
        else if(attackValue is BulletFromEnemy)
        {
            info.attackType = AttackPrefabInfo.AttackType.Bullet;
        }
        else if(attackValue is ForcedAttackFromEnemy)
        {
            info.attackType = AttackPrefabInfo.AttackType.Forced;
            var forcedAttack = attackValue as ForcedAttackFromEnemy;
            info.isAOE = forcedAttack.isAoE;
            info.awakeTimes.Add(forcedAttack.triggerTime);
        }
        else
        {
            info.attackType = AttackPrefabInfo.AttackType.None;
        }


    }

    private void ParseTriggerInfo()
    {
        EnemyAttackTriggerController triggerController = GetComponent<EnemyAttackTriggerController>();
        if (triggerController == null)
        {
            ObjectInvokeDestroy invokeDestroy = GetComponent<ObjectInvokeDestroy>();
            if (invokeDestroy != null)
            {
                return;
            }

            info.invokeDestroyTime = invokeDestroy.destroyTime;
            return;
        }
        else
        {
            info.invokeDestroyTime = triggerController.DestroyTime;
        }
        //TODO:Unfinished
        

    }

}

public class AttackPrefabInfo
{
    public enum AttackType
    {
        None,
        Meele,
        Ranged,
        Bullet,
        Forced
    }
    
    public enum ColliderType
    {
        None,
        Box,
        Circle,
        Polygon
    }
    
    public AttackType attackType = AttackType.None;
    public ColliderType colliderType = ColliderType.None;
    public List<float> colliderInfo = new();


    private int polygonColliderPointCount = -1;
    private List<Vector2> poloynColliderPoints = new();
    private Vector4 _boxColliderInfo = Vector4.zero;
    private Vector3 _circleColliderInfo = Vector3.zero;
    
    public int shakeIntensity = 0;
    public float invokeDestroyTime = 1f;
    public List<float> awakeTimes = new();
    public List<float> sleepTimes = new();

    public AttackFromEnemy.AvoidableProperty AvoidableProperty = AttackFromEnemy.AvoidableProperty.Red;
    public bool isAOE = false;
    public BasicCalculation.AttackType attackProperty = BasicCalculation.AttackType.STANDARD;
    public bool interruptable = false;
    public List<AttackInfo> attackInfos = new();
    

    public void SetBoxColliderInfo(float offsetX, float offsetY, float sizeX, float sizeY)
    {
        _boxColliderInfo = new Vector4(offsetX,offsetY,sizeX,sizeY);
        colliderInfo.Clear();
        colliderInfo.Add(offsetX);
        colliderInfo.Add(offsetY);
        colliderInfo.Add(sizeX);
    }

    public void SetCircleColliderInfo(float offsetX, float offsetY, float radius)
    {
        _circleColliderInfo = new Vector3(offsetX,offsetY,radius);
        colliderInfo.Clear();
        colliderInfo.Add(radius);
        colliderInfo.Add(offsetX);
        colliderInfo.Add(offsetY);
        
    }
    
    public void SetPolygonColliderInfo(int pointCount, List<Vector2> points)
    {
        polygonColliderPointCount = pointCount;
        poloynColliderPoints = points;
        colliderInfo.Clear();
        colliderInfo.Add(pointCount);
        foreach (var point in points)
        {
            colliderInfo.Add(point.x);
            colliderInfo.Add(point.y);
        }
    }


}
