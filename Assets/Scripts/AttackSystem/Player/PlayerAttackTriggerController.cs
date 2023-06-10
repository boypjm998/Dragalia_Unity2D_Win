using UnityEngine;

public class PlayerAttackTriggerController : MonoBehaviour
{
    [SerializeField] private float[] attackAwakeTime;
    [SerializeField] private float[] attackSleepTime;

    [SerializeField] private float[] nextAttackTime;
    [SerializeField] private float[] nextConditionTime;

    [SerializeField] private Collider2D targetCollider;
    [SerializeField] private float destroyTime = 1;

    [SerializeField] private bool sleepAfterAnimStopped;
    private Animation anim;

    private AttackFromPlayer _attackFromPlayer;


    private void Awake()
    {
        
        
        if (targetCollider == null)
        {
            targetCollider = GetComponent<Collider2D>();
        }

        _attackFromPlayer = GetComponent<AttackFromPlayer>();
        
        
        if (nextAttackTime.Length > 0)
        {
            foreach (var time in nextAttackTime)
            {
                Invoke("NextAttack",time);
            }
        }

        if (attackAwakeTime.Length > 0)
        {
            foreach (var time in attackAwakeTime)
            {
                Invoke("AttackAwake",time);
            }
        }

        if (attackSleepTime.Length > 0)
        {
            foreach (var time in attackSleepTime)
            {
                Invoke("AttackSleep",time);
            }
        }
        
        if (nextConditionTime.Length > 0)
        {
            foreach (var time in nextConditionTime)
            {
                Invoke("NextCondition",time);
            }
        }
        
        if(destroyTime>0)
            Destroy(gameObject,destroyTime);
        //Destroy(gameObject,destroyTime);

    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    void NextAttack()
    {
        _attackFromPlayer.NextAttack();
    }
    
    void AttackAwake()
    {
        targetCollider.enabled = true;
        if (_attackFromPlayer.forcedShake)
        {
            CineMachineOperator.Instance.CamaraShake(_attackFromPlayer.hitShakeIntensity,0.1f);
        }
    }
    void AttackSleep()
    {
        targetCollider.enabled = false;
    }

    void NextCondition()
    {
        _attackFromPlayer.ResetWithConditionFlags();
    }

    public void SetNextWithConditionTime(float[] times)
    {
        nextConditionTime = times;
    }
}
