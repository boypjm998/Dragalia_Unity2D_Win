using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController_E2002 : EnemyMoveManager
{
    public bool touched = false;
    // Start is called before the first frame update
    
    
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlying>();
    }
    public IEnumerator E2002_Action01()
    {
        ac.OnAttackEnter();
        while (!touched)
        {
            ac.SetMove(1);
            yield return new WaitForFixedUpdate();
        }
        ac.SetMove(0);
        
        ac.SetHitSensor(false);
        
        var burst = Instantiate(projectile1, transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        var atk = burst.GetComponentInChildren<ForcedAttackFromEnemy>();
        atk.enemySource = gameObject;
        atk.target = _behavior.targetPlayer;
        foreach (var plr in _behavior.playerList)
        {
            if (plr != _behavior.targetPlayer)
                atk.extraTargets.Add(plr);
        }

        yield return new WaitForSeconds(0.5f);
        QuitAttack();
        _statusManager.currentHp = 0;
    }


}
