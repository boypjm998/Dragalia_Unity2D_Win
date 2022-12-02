using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParticleAnimReciever : MonoBehaviour
{
    protected enum AttackSource
    {
        Player = 1,
        Enemy = 2
    }

    [SerializeField]protected float atkAwakeTime;

    protected ParticleSystem _particleSystem;
    [SerializeField] protected List<float> normailzedNextAttackTime;
    protected AttackFromEnemy _attackFromEnemy;
    protected AttackFromPlayer _attackFromPlayer;
    [SerializeField] protected AttackSource _attackSource;
    [SerializeField] protected float totalTime;
    
    protected virtual void OnParticleSystemStopped()
    {
        GetComponentInParent<Collider2D>().enabled = false;
        CancelInvoke();
        Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (_attackSource == AttackSource.Enemy)
        {
            _attackFromEnemy = GetComponentInParent<AttackFromEnemy>();
            _attackFromPlayer = null;
        }
        else if (_attackSource == AttackSource.Player)
        {
            _attackFromEnemy = null;
            _attackFromPlayer = GetComponentInParent<AttackFromPlayer>();
        }
        else throw new NotSupportedException();

        //totalTime = BasicCalculation.ParticleSystemLength(_particleSystem);
        
        if (normailzedNextAttackTime.Count > 0)
        {
            StartCoroutine(NextAttackRoutine());
        }
        Invoke("AttackAwake",atkAwakeTime);
        Invoke("AttackSleep",totalTime);

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    protected void NextAttack()
    {
        if (_attackSource == AttackSource.Player)
        {
            _attackFromPlayer.NextAttack();
        }
        if (_attackSource == AttackSource.Enemy)
        {
            _attackFromEnemy.NextAttack();
        }
    }

    protected IEnumerator NextAttackRoutine()
    {
        normailzedNextAttackTime.Sort();
        int i = 0;
        while (normailzedNextAttackTime.Count>0)
        {
            if ((_particleSystem.time/totalTime) >= normailzedNextAttackTime[i])
            {
                switch (_attackSource)
                {
                    case AttackSource.Enemy:
                        _attackFromEnemy.NextAttack();
                        break;
                    case AttackSource.Player:
                        _attackFromPlayer.NextAttack();
                        break;
                }
            }

            yield return null;

        }
        
    }

    void AttackAwake()
    {
        GetComponentInParent<Collider2D>().enabled = true;
    }
    void AttackSleep()
    {
        GetComponentInParent<Collider2D>().enabled = false;
    }

}
