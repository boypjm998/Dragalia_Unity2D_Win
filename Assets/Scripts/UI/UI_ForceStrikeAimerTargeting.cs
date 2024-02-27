using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class UI_ForceStrikeAimerTargeting : MonoBehaviour
{
    private List<float> forceTimeRequired = new();
    private float currentForceTime = 0;
    int currentFSLV = 0;

    private Transform fillMask;
    private Vector2 maxFillVector;
    private MuzzleSESender _SEsender;

    public Vector2 aimSize = new Vector2(20,8);
    public ActorControllerRangedWithFS ac;
    protected AttackManagerRanged _attackManagerRanged;
    public AudioClip forceSE;
    protected SpriteRenderer innerGaugeSprite;
    protected Color originColor;

    private Transform fx;
    private TargetAimer ta;
    
    private bool manualControlable = false;
    private bool startOnSelf = true;
    private float movingSpeed;
    
    
    
    
    private void Awake()
    {
        fillMask = transform.Find("Back/gauge");
        maxFillVector = fillMask.localScale;
        _SEsender = GetComponent<MuzzleSESender>();
        fx = transform.Find("fx");
        innerGaugeSprite = transform.Find("Back").GetComponent<SpriteRenderer>();
        originColor = innerGaugeSprite.color;
    }

    private void OnEnable()
    {
        if(startOnSelf)
            transform.position = ac.gameObject.RaycastedPosition();
        
        fx.GetChild(0).gameObject.SetActive(true);
        fx.GetChild(1).gameObject.SetActive(false);
        innerGaugeSprite.color = originColor;
    }

    private void OnDisable()
    {
        fx.GetChild(1).gameObject.SetActive(false);
        fx.GetChild(0).gameObject.SetActive(false);
        if (ac.hurt)
            currentFSLV = 0;
        _attackManagerRanged.ForceStrikeRelease(currentFSLV);
    }
    
    private void Update()
    {
        if (GlobalController.currentGameState != GlobalController.GameState.Inbattle)
        {
            gameObject.SetActive(false);
            ac.anim.SetInteger("force_level",-1);
            return;
        }
    
    
        if (ac.forceLevel < 0)
        {
            gameObject.SetActive(false);
            //Destroy(GetComponentInParent<AttackContainer>().gameObject);
            return;
        }


        if (manualControlable)
        {
            AimTargetManually();
        }
        else
        {
            AimTargetAutomatically();
        }
        
            
        if (ac.forceLevel < ac.maxForceLevel)
        {
            fillMask.transform.localScale =
                new Vector3((ac.forcingTime / forceTimeRequired[ac.forceLevel]),
                    (ac.forcingTime / forceTimeRequired[ac.forceLevel]),1);
            innerGaugeSprite.enabled = true;
        }
        else
        {
            //innerGaugeSprite.enabled = false;
            fillMask.transform.localScale = Vector3.one;
        }
            
        if (currentFSLV < ac.forceLevel)
        {
            _SEsender.SendVoiceToPlay(forceSE);
            if (ac.forceLevel == ac.maxForceLevel)
            {
                fx.GetChild(0).gameObject.SetActive(false);
                fx.GetChild(1).gameObject.SetActive(true);
                innerGaugeSprite.color = Color.green;
            }
        }
        
        currentFSLV = ac.forceLevel;
    }
    
    public void SetActorController(ActorControllerRangedWithFS ac)
    {
        this.ac = ac;
        ta = ac.GetComponentInChildren<TargetAimer>();
        _attackManagerRanged = ac.GetComponent<AttackManagerRanged>();
    }

    public void SetMaxForceInfo(List<float> info)
    {
        forceTimeRequired = info;
        
    }

    public void SetTargetingType(bool appearOnSelf, bool manualControlable, float movingSpeed = 8)
    {
        startOnSelf = appearOnSelf;
        this.manualControlable = manualControlable;
        this.movingSpeed = movingSpeed;
    }
    
    private void AimTargetAutomatically()
    {
        var targetTransform = ta.GetNearestTargetInRangeDirection(ac.facedir, aimSize.x, aimSize.y,
            LayerMask.GetMask("Enemies"));
            
        var reversedTargetTransform = ta.GetNearestTargetInRangeDirection
        (-ac.facedir, aimSize.x,  aimSize.y,
            LayerMask.GetMask("Enemies"));

        if(targetTransform == null)
            targetTransform = reversedTargetTransform;
        else if(reversedTargetTransform!=null)
        {
            if(ta.HasMarking(reversedTargetTransform) && !ta.HasMarking(targetTransform))
                targetTransform = reversedTargetTransform;
        }





        if (targetTransform == null)
        {
            targetTransform = ac.transform;
        }

        var targetCol = BasicCalculation.CheckRaycastedPlatform(targetTransform.gameObject);

        var position = new Vector3(targetTransform.position.x, targetCol.bounds.max.y, 0);
            
        transform.position = position;
            
        //ac.forceAttackPosition = position;
    }

    private void AimTargetManually()
    {
        var leftBorder = Mathf.Max
            (BattleStageManager.Instance.mapBorderL, ac.transform.position.x - aimSize.x);

        var rightBorder = Mathf.Min
            (BattleStageManager.Instance.mapBorderR, ac.transform.position.x + aimSize.x);

        Vector2 aimingPosition = new Vector2(transform.position.x, ac.transform.position.y);

        if (!ac.pi.buttonLeft.IsPressing && ac.pi.buttonRight.IsPressing)
        {
            aimingPosition.x += Time.deltaTime * movingSpeed;
            aimingPosition.x = Mathf.Clamp(aimingPosition.x, leftBorder, rightBorder);
            transform.position = new Vector3(aimingPosition.x,BasicCalculation.GetRaycastedPlatformY(aimingPosition));

        }else if (ac.pi.buttonLeft.IsPressing && !ac.pi.buttonRight.IsPressing)
        {
            aimingPosition.x -= Time.deltaTime * movingSpeed;
            aimingPosition.x = Mathf.Clamp(aimingPosition.x, leftBorder, rightBorder);
            transform.position = new Vector3(aimingPosition.x,BasicCalculation.GetRaycastedPlatformY(aimingPosition));
        }
        
        
        
    }
    
    
}
