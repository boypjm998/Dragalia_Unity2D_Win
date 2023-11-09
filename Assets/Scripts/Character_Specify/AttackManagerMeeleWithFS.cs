using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackManagerMeeleWithFS : AttackManager
{
    [SerializeField] protected GameObject forceStrikeIndicatorPrefab;
    
    protected UI_ForceStrikeAimerMeele forceStrikeIndicator;
    
    [SerializeField] protected GameObject[] dashFX;
    [SerializeField] protected GameObject[] forceFX;
    [SerializeField] protected GameObject[] comboFX;
    [SerializeField] protected GameObject[] skillFX;


    public override void DashAttack()
    {
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(dashFX[0],transform.position,container);
    }

    private ActorController_c003 c;

    

    public virtual void ForceStrikeRelease(int currentFSLV){
        
        if(currentFSLV <= 0)
            return;
        
        ac.OnAttackInterrupt?.Invoke();

        ac.SetFaceDir(forceStrikeIndicator.forceDirection);
        
        var container = Instantiate(attackContainer,transform.position, Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateMeele(forceFX[0],transform.position,container);
    
    }
    
    public void OnForceStart()
    {
        
        
        
        if (forceStrikeIndicator == null)
        {
            var prefabIndicator = Instantiate(forceStrikeIndicatorPrefab, transform.position, Quaternion.identity,
                BuffFXLayer.gameObject.transform);
            forceStrikeIndicator = prefabIndicator.GetComponent<UI_ForceStrikeAimerMeele>();
            prefabIndicator.name = "ForceStrikeIndicator";
            forceStrikeIndicator.SetActorController(ac as ActorControllerMeeleWithFS);
            forceStrikeIndicator.SetMaxForceInfo(new float[] {(ac as ActorControllerMeeleWithFS).forcingRequireTime}.ToList());
        }
        else
        {
            if(forceStrikeIndicator.gameObject.activeSelf)
                return;
            forceStrikeIndicator.gameObject.SetActive(true);
        }
        forceStrikeIndicator.SetForceDirection(ac.facedir);
        Debug.Log("Called OnForceStart");

    }
    
    
    
    protected void OnForcingUpdate()
    {
        var ac = this.ac as ActorController;
        
        if(ac.pi.buttonLeft.IsPressing && !ac.pi.buttonRight.IsPressing)
            forceStrikeIndicator.SetForceDirection(-1);
        
        else if(ac.pi.buttonRight.IsPressing && !ac.pi.buttonLeft.IsPressing)
            forceStrikeIndicator.SetForceDirection(1);
        
        
    }

    public void Skill4()
    {
        _statusManager.HPRegenImmediately(0,10,true);
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
        //Instantiate(healbuff, transform.position, Quaternion.identity, BuffFXLayer.transform);
        _statusManager.ObtainHealOverTimeBuff(10,15,true);
        
    
    }
}
