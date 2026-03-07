using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneController_HB05 : CutSceneController
{
    [SerializeField] private SkinnedMeshRenderer _model;
    private Color _normalColor = new Color(0f,0.9f,0.9f);
    private List<Color> _hellColors = new List<Color>
    {
        new Color(0,0.5f,0.8f),
        new Color(0.5f,0.5f,0.5f),
        new Color(0.8f,0.6f,1),
        new Color(1,0.5f,1),
        new Color(1,0.3f,1),
        new Color(1,0.35f,0.45f),
        new Color(1,0.25f,0.3f),
        new Color(1,0.15f,0.15f),
        new Color(1,0,0)
    };
    private EnemyMoveController_HB05_Legend _enemyMoveController;
    [SerializeField] private GameObject fogFX1;
    [SerializeField] private GameObject fogFX2;
    
    private void Start()
    {
        _director = GetComponent<PlayableDirector>();
        skyMat.SetColor("_Tint", _normalColor);
    }

    public override void Replay()
    {
        base.Replay();
        skyMat.SetColor("_Tint", GetHellColor());
        print(GetHellColor());
        ResetFaceExpression();

        if (!Projectile_C007_2_Boss.Instance.FogStarted)
        {
            fogFX1.SetActive(false);
            fogFX2.SetActive(false);
        }else
        {
            fogFX1.SetActive(true);
            fogFX2.SetActive(true);
        }
        
        
    }

    public void SetController(EnemyMoveController_HB05_Legend controller)
    {
        _enemyMoveController = controller;
    }

    private Color GetHellColor()
    {
        if(_enemyMoveController == null)
            return _normalColor;
        
        if(_enemyMoveController.HellLevel == 0)
            return _normalColor;
        else
        {
            return _hellColors[_enemyMoveController.HellLevel - 1];
        }
        
        
    }
    
    public void ChangeExpressionEye(int expressionId)
    {

        int col = expressionId % 4;
        int row = expressionId / 4;
        
        Vector2 offset = new Vector2(col * 0.25f, -row * 0.25f);
      
        _model.materials[2].mainTextureOffset = offset;
        
    }

    public void ChangeExpressionMouth(int expressionId)
    {
      
        
        int col = expressionId % 4;
        int row = expressionId / 4;
        
        Vector2 offset = new Vector2(col * 0.25f, -row * 0.25f);
        
        print(offset.x + " " + offset.y);
        
        _model.materials[1].mainTextureOffset = offset;

    }
    public void ResetFaceExpression()
    {
        _model.materials[1].mainTextureOffset = new Vector2(0, 0);
        _model.materials[2].mainTextureOffset = new Vector2(0, 0);
    }
}
