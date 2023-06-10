using System.Collections;
using CharacterSpecificProjectiles;
using UnityEngine;


public class EnemyMoveController_E3001 : EnemyMoveManager
{
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerTherian>();
    }

    public IEnumerator E3001_Action01()
    {
        ac.OnAttackEnter(999);
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position + new Vector3(0,3,0));

        anim.Play("force_enter");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("force_loop");
        
        var attackHint = Instantiate(projectile1,
            transform.position - new Vector3(0,2,0),Quaternion.identity,
            MeeleAttackFXLayer.transform);

        var forcetime = attackHint.GetComponent<Projectile_E3001_1>().forcingTime;

        yield return new WaitForSeconds(forcetime);
        
        anim.Play("force_exit");

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f);
        
        var container = Instantiate(attackContainer,Vector3.zero,Quaternion.identity,
            RangedAttackFXLayer.transform);
        var atk = InstantiateRanged(projectile2,Vector3.zero,container,1);
        atk.GetComponent<ForcedAttackFromEnemy>().target = _behavior.targetPlayer;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();

    }



}
