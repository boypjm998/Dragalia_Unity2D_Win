using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController_HB01 : CutSceneController
{
    
    public Color originColor = new Color(0.45f, 0.45f, 0.45f, 0.5f); 
    protected Color worldColor1 = new Color(1f, 0.95f, 0.4f, 0.5f);
    protected Color worldColor2 = new Color(1f, 0.6f, 0.4f, 0.5f);
    
    [SerializeField] SkinnedMeshRenderer _model;

    private Animator anim;
    void Start()
    {
        
    }

    public void ChangeMouseExpression(float offset)
    {
        _model.materials[2].mainTextureOffset = new Vector2(offset, 0);
    }

    public void ChangeFaceExpression(float offset)
    {
        _model.materials[1].mainTextureOffset = new Vector2(offset, 0);
        
    }

    public void SetSkyBoxDirection(int dir)
    {
        print("SetSkyBoxDirection"+dir);
        if (dir == 1)
        {
            skyMat.SetFloat("_Rotation", (250f));
            //将天空盒的旋转角度设置为0
            //RenderSettings.skybox.SetFloat("_Rotation", (250f));
        }
        else
        {
            skyMat.SetFloat("_Rotation", (90f));
            //将天空盒的旋转角度设置为90
            //RenderSettings.skybox.SetFloat("_Rotation", (90f));
        }
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

    public void TurnToSide()
    {
        return;
    }

    public void TurnToMiddle()
    {
        return;
    }
    
    public void SetSkyBoxColor(int colorID)
    {
        if (colorID == 1)
        {
            skyMat.SetColor("_Tint", worldColor1);
        }
        else if(colorID == 2)
        {
            skyMat.SetColor("_Tint", worldColor2);
        }
        else if(colorID == 0)
        {
            skyMat.SetColor("_Tint", originColor);
        }
    }


}
