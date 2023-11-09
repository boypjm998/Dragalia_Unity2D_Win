using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class DragonAttackManager_D011_NPC : AttackManager
{
    public List<GameObject> projectileList = new();

    private ActorControllerSpecial ac_sp;

    private VoiceControllerPlayer voice;
    
    
    protected override void Awake()
    {
        ac_sp = GetComponentInParent<ActorControllerSpecial>();
        _effectManager = BattleEffectManager.Instance;
        MeeleAttackFXLayer = ac_sp.transform.Find("MeeleAttackFX").gameObject;
        BuffFXLayer = ac_sp.transform.Find("BuffLayer").gameObject;
        _statusManager = ac_sp.GetComponent<StatusManager>();
        voice = ac_sp.GetComponentInChildren<VoiceControllerPlayer>();
    }


    private void Combo1()
    {
        var proj = InstantiateMeele(projectileList[0],
            transform.position + new Vector3(ac_sp.facedir * 1.5f, 3.5f, 0),
            InitContainer(true));
        proj.GetComponent<AttackFromPlayer>().playerpos = ac_sp.transform;
        voice?.PlayAttackVoice(1);
    }
    
    private void Combo2()
    {
        var proj = InstantiateRanged(projectileList[1],
            gameObject.RaycastedPosition(),
            InitContainer(false),1);
        proj.GetComponent<AttackFromPlayer>().playerpos = ac_sp.transform;
        voice?.PlayAttackVoice(2);
    }
    
    private void Combo3()
    {
        var proj = Instantiate(projectileList[2],
            transform.position + new Vector3(0,1.5f,0),
            Quaternion.identity,RangedAttackFXLayer.transform);
        
        //voice?.PlayAttackVoice(3);

        //proj.GetComponent<AttackFromPlayer>().playerpos = ac_sp.transform;

        var list = DragaliaEnemyBehavior.GetPlayerList();

        foreach (var plr in list)
        {
            var targetSM = plr.GetComponent<StatusManager>();
            print(25f.NormalizedHealPotencyToPercentage(_statusManager,targetSM));
            targetSM.HPRegenImmediately(0,
                25f.NormalizedHealPotencyToPercentage(
                    _statusManager,targetSM),false);
            Instantiate(projectileList[3], plr.gameObject.RaycastedPosition(), Quaternion.identity,
                RangedAttackFXLayer.transform);
            targetSM.ObtainTimerBuff((int)BasicCalculation.BattleCondition.FrostbitePunisher,20,10,
                1,201101);
        }
    }

}
