using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class Projectile_H004_1 : MonoBehaviour
{
    public const int Active = 10;
    public const int Disable = 0;
    public enum MelodyType
    {
        Hell,
        Heaven
    }
    [SerializeField] private MelodyType type;
    private Collider2D _collider2D;
    [SerializeField]
    private Collider2D _otherCollider2D;
    [SerializeField]
    private float delayTime = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        BattleStageManager.Instance.specialEventTriggered += SetHitBox;
    }

    private void SetHitBox(int msg)
    {
        if (msg >= 9)
        {
            _collider2D.enabled = true;
            _otherCollider2D.enabled = true;
            _collider2D.gameObject.SetActive(true);
            _otherCollider2D.gameObject.SetActive(true);
        }
        else if(msg <= 0)
        {
            _collider2D.enabled = false;
            _otherCollider2D.enabled = false;
            _collider2D.gameObject.SetActive(false);
            _otherCollider2D.gameObject.SetActive(false);
        }
        
        
    }

    private void OnDestroy()
    {
        BattleStageManager.Instance.specialEventTriggered -= SetHitBox;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        var atk = other.GetComponent<AttackFromPlayer>();
        
        if(atk == null)
            return;
        if(atk.attackType != BasicCalculation.AttackType.DASH)
            return;

        _collider2D.enabled = false;
        _otherCollider2D.enabled = false;
        _collider2D.gameObject.SetActive(false);
        _otherCollider2D.gameObject.SetActive(false);

        var hitEff = atk.hitConnectEffect;

        if (hitEff != null)
        {
            Instantiate(hitEff, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
        }
        
        CineMachineOperator.Instance.CamaraShake(atk.hitShakeIntensity, .1f);
        BattleStageManager.Instance.TriggerSpecialEvent(type == MelodyType.Hell ? 
            1 : 
            2
        );

        DOVirtual.DelayedCall(delayTime, () =>
        {
            _collider2D.enabled = true;
            _otherCollider2D.enabled = true;
            _collider2D.gameObject.SetActive(true);
            _otherCollider2D.gameObject.SetActive(true);
        },false);
    }
}
