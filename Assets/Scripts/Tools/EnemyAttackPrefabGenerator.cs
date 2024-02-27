using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameMechanics;


public class EnemyAttackPrefabGenerator : MonoBehaviour
{
    private AttackPrefabInfo info;
    private static Color borderRed = new Color(0.5f, 0f, 0f, 1f);
    private static Color borderPurple = new Color(0.35f, 0f, 0.5f, 1f);
    
    public static GameObject GenerateRectEnemyHintBar(ActorBase actor, Vector3 position, Transform parent, Vector2 size, Vector2 offset, bool avoidable,
        int fillAxis, float fillTime, float rotateAngle, float atkLastTime = 0.5f,bool autoDestroy = true,
        bool interupptable = true, bool addShine = true, float shineTime = 0.15f)
    {
        GameObject instance = Instantiate(Resources.Load<GameObject>
            ("UI/InBattle/BattleHint/RectTemplate"),position,Quaternion.identity,parent);
        
        instance.transform.localRotation = Quaternion.Euler(0, 0, rotateAngle);
        
        SpriteRenderer back = instance.transform.Find("Back").GetComponent<SpriteRenderer>();
        SpriteRenderer fill = instance.transform.Find("Fill").GetComponent<SpriteRenderer>();
        
        SpriteRenderer borderL = instance.transform.Find("Borders/L").GetComponent<SpriteRenderer>();
        SpriteRenderer borderR = instance.transform.Find("Borders/R").GetComponent<SpriteRenderer>();
        SpriteRenderer borderT = instance.transform.Find("Borders/T").GetComponent<SpriteRenderer>();
        SpriteRenderer borderB = instance.transform.Find("Borders/B").GetComponent<SpriteRenderer>();
        
        var hintBar = instance.AddComponent<EnemyAttackHintBarRect2D>();

        hintBar.warningTime = fillTime;
        hintBar.interruptable = interupptable;
        hintBar.AutoDestruct = autoDestroy;
        hintBar.attackLastTime = atkLastTime;
        hintBar.SetAc(actor as EnemyController);

        if (addShine)
        {
            var shineBar = instance.AddComponent<EnemyAttackHintBarShine>();
            shineBar.warningTime = fillTime;
            shineBar.interruptable = interupptable;
            shineBar.AutoDestruct = autoDestroy;
            shineBar.attackLastTime = atkLastTime;
            //shineBar.shineTime = shineTime;
            shineBar.SetAc(actor as EnemyController);
        }

        hintBar.SetFillAxis(fillAxis);



        back.size = size;
        if (fillAxis == 0)
        {
            fill.size = new Vector2(.1f, size.y);
        }
        else
        {
            fill.size = new Vector2(size.x, .1f);
        }
        
        back.transform.localPosition = offset;
        fill.transform.localPosition = offset;
        borderB.transform.parent.localPosition = offset;
        
        borderB.size = new Vector2(size.x, 0.1f);
        borderT.size = new Vector2(size.x, 0.1f);
        borderL.size = new Vector2(0.1f, size.y);
        borderR.size = new Vector2(0.1f, size.y);
        
        //左对齐的
        borderL.transform.localPosition = new Vector3(0.05f, 0, 0);
        borderR.transform.localPosition = new Vector3(size.x - 0.05f, 0, 0);
        borderT.transform.localPosition = new Vector3(size.x / 2, size.y / 2, 0);
        borderB.transform.localPosition = new Vector3(size.x / 2, -size.y / 2, 0);
        
        

        if (avoidable)
        {
            back.color = new Color(1, 0, 0,back.color.a);
            fill.color = new Color(1, 0, 0,fill.color.a);
            borderB.color = borderRed;
            borderT.color = borderRed;
            borderL.color = borderRed;
            borderR.color = borderRed;
        }else
        {
            back.color = new Color(0.4f, 0.2f, 0.6f,back.color.a);
            fill.color = new Color(0.4f, 0.2f, 0.6f,fill.color.a);
            borderB.color = borderPurple;
            borderT.color = borderPurple;
            borderL.color = borderPurple;
            borderR.color = borderPurple;
        }
        
        return instance;

    }

    public static GameObject GenerateCircEnemyHintBar(ActorBase actor, Vector3 position, Transform parent, float radius,
        Vector2 offset, bool avoidable, bool doscale,
        float fillTime, float edgeWidth = 0.1f, float atkLastTime = 0.5f, bool autoDestroy = true,
        bool interupptable = true, bool addShine = true, float shineTime = 0.15f)
    {
        
        edgeWidth = Mathf.Clamp(edgeWidth, 0.05f, 0.5f);
        
        GameObject instance = Instantiate(Resources.Load<GameObject>
            ("UI/InBattle/BattleHint/CircleTemplate"),position,Quaternion.identity,parent);
        
        SpriteRenderer back = instance.transform.Find("Back").GetComponent<SpriteRenderer>();
        SpriteRenderer fill = instance.transform.Find("Fill").GetComponent<SpriteRenderer>();
        Transform mask = instance.transform.Find("Borders/Mask");
        
        var hintBar = instance.AddComponent<EnemyAttackHintBarCircle>();
        SpriteRenderer borderRenderer = instance.transform.Find("Borders/Renderer").
            GetComponent<SpriteRenderer>();
        
        
        hintBar.warningTime = fillTime;
        hintBar.interruptable = interupptable;
        hintBar.AutoDestruct = autoDestroy;
        hintBar.attackLastTime = atkLastTime;
        hintBar.SetAc(actor as EnemyController);
        
        if (addShine)
        {
            var shineBar = instance.AddComponent<EnemyAttackHintBarShine>();
            shineBar.warningTime = fillTime;
            shineBar.interruptable = interupptable;
            shineBar.AutoDestruct = autoDestroy;
            shineBar.attackLastTime = atkLastTime;
            //shineBar.shineTime = shineTime;
            shineBar.SetAc(actor as EnemyController);
        }
        
        fill.size = new Vector2(0.1f, 0.1f);
        back.size = new Vector2(radius * 2, radius * 2);
        borderRenderer.size = new Vector2(radius * 2, radius * 2);

        var maskScaleFactor = (radius * 2 / 3) - edgeWidth;
        mask.localScale = maskScaleFactor * Vector3.one;
        
        back.transform.localPosition = offset;
        fill.transform.localPosition = offset;
        mask.transform.parent.localPosition = offset;

        if (doscale)
        {
            instance.transform.localScale = Vector3.one * 0.1f;
            hintBar.SetDoScale(true);
        }

        if (avoidable)
        {
            back.color = new Color(1, 0, 0,back.color.a);
            fill.color = new Color(1, 0, 0,fill.color.a);
            borderRenderer.color = borderRed;
            
        }else
        {
            back.color = new Color(0.4f, 0.2f, 0.6f,back.color.a);
            fill.color = new Color(0.4f, 0.2f, 0.6f,fill.color.a);
            borderRenderer.color = borderPurple;
        }
        
        return instance;


    }






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
                polygonCollider2D.points);
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
    
    public void SetPolygonColliderInfo(int pointCount, params Vector2[] points)
    {
        polygonColliderPointCount = pointCount;
        poloynColliderPoints = points.ToList();
        colliderInfo.Clear();
        colliderInfo.Add(pointCount);
        foreach (var point in points)
        {
            colliderInfo.Add(point.x);
            colliderInfo.Add(point.y);
        }
    }


}
