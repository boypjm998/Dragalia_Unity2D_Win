using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    public int chara_id;
    public int skill_id = 0;
    
    public BasicCalculation.AttackType attackType;
    
    [SerializeField] protected float[] dmgModifier;
    [SerializeField] protected List<float> nextDmgModifier;
    [SerializeField] protected List<float> nextKnockbackPower;
    [SerializeField] protected List<float> nextKnockbackForce;
    [SerializeField] protected List<float> nextKnockbackTime;

    [SerializeField] protected AudioClip hitSoundEffect;
    protected BattleEffectManager _effectManager;
    protected void DestroyContainer()
    {
        var container = GetComponentInParent<AttackContainer>();
        Destroy(container.gameObject);
        print("DestroyContainer");
    }



}
