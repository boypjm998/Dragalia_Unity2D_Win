using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneController_HB04 : CutSceneController
{
    [SerializeField] private SkinnedMeshRenderer _model;
    
    [SerializeField] private Color originColor = new Color(0.45f, 0.45f, 0.45f, 0.5f);
    
    [SerializeField] private Color nightColor = new Color(0f, 0.2f, 0.4f, 0.5f);
    private void Start()
    {
        _director = GetComponent<PlayableDirector>();
        skyMat.SetColor("_Tint", nightColor);skyMat.SetFloat("_Exposure", 0.8f);
    }

    private void OnDestroy()
    {
        skyMat.SetColor("_Tint", nightColor);skyMat.SetFloat("_Exposure", 0.8f);
    }

    public override void Replay()
    {
        base.Replay();
        skyMat.SetColor("_Tint", nightColor);
        skyMat.SetFloat("_Exposure", 0.8f);
    }

    public void DoSkyBoxTween()
    {
        var Sequence = DOTween.Sequence();

        Sequence.Insert(0,
            DOTween.To(() => skyMat.GetColor("_Tint"),
            x => skyMat.SetColor("_Tint",x),
            originColor, 1.5f)
            );
        
        Sequence.Insert(
            0,
            DOTween.To(() => skyMat.GetFloat("_Exposure"),
            x => skyMat.SetFloat("_Exposure",x),
            1.5f, 0.75f)
            );
        
        Sequence.Append(
            DOTween.To(() => skyMat.GetFloat("_Exposure"),
            x => skyMat.SetFloat("_Exposure",x),
            1.1f, 1.25f)
            );

        Sequence.Play();

    }
    public void ChangeExpressionEye(int expressionId)
    {
        //MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        int col = expressionId % 4;
        int row = expressionId / 4;
        
        Vector2 offset = new Vector2(col * 0.25f, -row * 0.25f);
        

        // 设置纹理偏移量
        //block.SetVector("_MainTex_ST", new Vector4(offset.x, offset.y, 0, 0));
        // 将 MaterialPropertyBlock 应用到渲染器上
        
        //_model.SetPropertyBlock(block, 2);
        
        _model.materials[2].mainTextureOffset = offset;
        
    }

    public void ChangeExpressionMouth(int expressionId)
    {
        //MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        int col = expressionId % 4;
        int row = expressionId / 4;
        
        Vector2 offset = new Vector2(col * 0.25f, -row * 0.25f);
        
        print(offset.x + " " + offset.y);
        
        _model.materials[1].mainTextureOffset = offset;
        

        // 设置纹理偏移量
        //block.SetVector("_MainTex_ST", new Vector4(offset.x, offset.y, 0, 0));
        // 将 MaterialPropertyBlock 应用到渲染器上
        
        //_model.SetPropertyBlock(block, 2);
        
        
    }
    public void ResetFaceExpression()
    {
        _model.materials[1].mainTextureOffset = new Vector2(0, 0);
        _model.materials[2].mainTextureOffset = new Vector2(0, 0);
    }
}
