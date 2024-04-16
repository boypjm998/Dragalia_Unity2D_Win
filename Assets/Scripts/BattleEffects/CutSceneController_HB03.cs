using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneController_HB03 : CutSceneController
{
    private CutSceneController_HB04 _cutSceneControllerHb04;
    
    [SerializeField] private SkinnedMeshRenderer _model;
    
    private void Start()
    {
        _director = GetComponent<PlayableDirector>();
        
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
