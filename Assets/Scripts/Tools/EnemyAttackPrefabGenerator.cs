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

    private static GameObject circBarPrefab;
    private static GameObject rectBarPrefab;
    
    /// <summary>
    /// 生成一个矩形的敌人攻击提示条，包含背景、动态填充条、边框装饰，并可配置警告时长、颜色主题、填充方向、闪光特效等属性。
    /// </summary>
    /// <param name="actor">关联的敌人角色基类。</param>
    /// <param name="position">提示条根物体的世界坐标位置。</param>
    /// <param name="parent">提示条根物体的父级Transform。</param>
    /// <param name="size">提示条背景的整体尺寸。</param>
    /// <param name="offset">提示条内部元素相对于根物体的本地坐标偏移。
    /// 用于微调提示条内部的布局，仅影响子物体位置，不改变提示条根物体的世界坐标。</param>
    /// <param name="avoidable">该攻击是否可躲避。true 显示红色，false 显示紫色。</param>
    /// <param name="fillAxis">填充动画的轴向。0 为水平（X轴）填充，1 为垂直（Y轴）填充。</param>
    /// <param name="fillTime">警告填充的持续时间（即提示条从出现到攻击生效的时间）。</param>
    /// <param name="rotateAngle">提示条根物体的 Z 轴旋转角度（度）。</param>
    /// <param name="atkLastTime">提示条充满后继续存在的时间，默认为 0.5f。</param>
    /// <param name="autoDestroy">攻击结束后是否自动销毁提示条，默认为 true。</param>
    /// <param name="interupptable">提示条是否可被敌人的状态中断（如异常状态、Break状态或死亡），默认为 true。</param>
    /// <param name="addShine">是否添加闪光效果组件，默认为 true。</param>
    /// <param name="shineTime">闪光效果的持续时间，当前代码未完全启用。</param>
    /// <param name="overlay">是否将提示条渲染在 "BattleHintsOverlay" 层级（覆盖层），默认为 false。</param>
    /// <returns>生成的提示条 GameObject 实例。</returns>
    public static GameObject GenerateRectEnemyHintBar(ActorBase actor, Vector3 position, Transform parent, Vector2 size, Vector2 offset, bool avoidable,
        int fillAxis, float fillTime, float rotateAngle, float atkLastTime = 0.5f,bool autoDestroy = true,
        bool interupptable = true, bool addShine = true, float shineTime = 0.15f, bool overlay = false)
    {
        if (rectBarPrefab == null)
        {
            rectBarPrefab = Resources.Load<GameObject>("UI/InBattle/BattleHint/RectTemplate");
        }
        // GameObject instance = Instantiate(Resources.Load<GameObject>
        //     ("UI/InBattle/BattleHint/RectTemplate"),position,Quaternion.identity,parent);
        
        
        GameObject instance = Instantiate(rectBarPrefab,position,Quaternion.identity,parent);
        
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

        if (overlay)
        {
            back.sortingLayerName = "BattleHintsOverlay";
            fill.sortingLayerName = "BattleHintsOverlay";
            borderB.sortingLayerName = "BattleHintsOverlay";
            borderT.sortingLayerName = "BattleHintsOverlay";
            borderL.sortingLayerName = "BattleHintsOverlay";
            borderR.sortingLayerName = "BattleHintsOverlay";
        }
        
        return instance;

    }

    
    /// <summary>
    /// 生成一个圆形的敌人攻击提示条，包含背景、动态填充圆、边框装饰，并可配置警告时长、颜色主题、缩放动画、闪光特效等属性。
    /// </summary>
    /// <param name="actor">关联的敌人角色基类。</param>
    /// <param name="position">提示条根物体的世界坐标位置。</param>
    /// <param name="parent">提示条根物体的父级Transform。</param>
    /// <param name="radius">提示条的半径，决定背景和边框的整体大小。</param>
    /// <param name="offset">提示条内部元素（背景、填充条、边框父物体）相对于根物体的本地坐标偏移。
    /// 用于微调提示条内部的布局，仅影响子物体位置，不改变提示条根物体的世界坐标。</param>
    /// <param name="avoidable">该攻击是否可躲避。true 显示红色，false 显示紫色。</param>
    /// <param name="doscale">是否启用初始缩放动画（从极小放大到目标大小）。</param>
    /// <param name="fillTime">警告填充的持续时间（即提示条从出现到攻击生效的时间）。</param>
    /// <param name="edgeWidth">圆形边框的宽度，范围限制在 0.05f 到 0.5f 之间。</param>
    /// <param name="atkLastTime">提示条充满后继续存在的时间，默认为 0.5f。</param>
    /// <param name="autoDestroy">攻击结束后是否自动销毁提示条，默认为 true。</param>
    /// <param name="interupptable">提示条是否可被敌人的状态中断（如异常状态、Break状态或死亡），默认为 true。</param>
    /// <param name="addShine">是否添加闪光效果组件，默认为 true。</param>
    /// <param name="shineTime">闪光效果的持续时间，当前代码未完全启用。</param>
    /// <param name="overlay">是否将提示条渲染在 "BattleHintsOverlay" 层级（覆盖层），默认为 false。</param>
    /// <returns>生成的提示条 GameObject 实例。</returns>
    public static GameObject GenerateCircEnemyHintBar(ActorBase actor, Vector3 position, Transform parent, float radius,
        Vector2 offset, bool avoidable, bool doscale,
        float fillTime, float edgeWidth = 0.1f, float atkLastTime = 0.5f, bool autoDestroy = true,
        bool interupptable = true, bool addShine = true, float shineTime = 0.15f, bool overlay = false)
    {

        if (circBarPrefab == null)
        {
            circBarPrefab = Resources.Load<GameObject>
                ("UI/InBattle/BattleHint/CircleTemplate");
        }
        
        
        edgeWidth = Mathf.Clamp(edgeWidth, 0.05f, 0.5f);
        
        GameObject instance = Instantiate(circBarPrefab,position,Quaternion.identity,parent);
        
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

        if (overlay)
        {
            back.sortingLayerName = "BattleHintsOverlay";
            fill.sortingLayerName = "BattleHintsOverlay";
            borderRenderer.sortingLayerName = "BattleHintsOverlay";
            
            instance.GetComponentInChildren<SpriteMask>().frontSortingLayerID = SortingLayer.NameToID("BattleHintsOverlay");
            
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
