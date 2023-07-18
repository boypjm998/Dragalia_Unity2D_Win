using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BOSS召唤的影子分身（梅奈）
/// </summary>
public class EnemyMoveController_DB01_M1 : EnemyMoveManager
{
    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerFlyingHigh>();
        
    }

    public IEnumerator DB01_M1_Action01()
    {
        ac.OnAttackEnter();
        
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("combo1");

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.33f);
        Combo1();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        anim.Play("combo2");
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f);
        Combo2();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        anim.Play("idle");
        
        QuitAttack();

    }
    
    public IEnumerator DB01_M1_Action02()
    {
        ac.OnAttackEnter();
        
        ac.TurnMove(_behavior.targetPlayer);
        
        ButterFlyFade();
        
        yield return new WaitForSeconds(10f);
        
        QuitAttack();
        //Destroy(gameObject);

    }

    private void Combo1()
    {
        //TODO: 普攻Combo1
        var container = Instantiate(attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var attack = 
            InstantiateRanged(projectile1,
                transform.position + new Vector3(ac.facedir * 1.5f, 0, 0),
            container,ac.facedir,0);
        attack.GetComponent<HomingAttack>().target = _behavior.targetPlayer.transform;
        attack.GetComponent<HomingAttack>().firedir = ac.facedir;

    }
    private void Combo2()
    {
        //TODO: 普攻Combo2
        var container = Instantiate(attackContainer,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var attack1 = 
            InstantiateRanged(projectile1,
                transform.position + new Vector3(ac.facedir * 1f, 0.5f, 0),
                container,ac.facedir,0);
        attack1.GetComponent<HomingAttack>().target = _behavior.targetPlayer.transform;
        var attack2 = 
            InstantiateRanged(projectile1,
                transform.position + new Vector3(ac.facedir * 1f, -0.5f, 0),
                container,ac.facedir,0);
        attack2.GetComponent<HomingAttack>().target = _behavior.targetPlayer.transform;
        attack1.GetComponent<HomingAttack>().firedir = ac.facedir;
        attack2.GetComponent<HomingAttack>().firedir = ac.facedir;
    }

    private void ButterFlyFade()
    {
        var container = Instantiate(projectile2,
            transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        transform.Find("Model").gameObject.SetActive(false);
        transform.Find("Shine").gameObject.SetActive(false);
        ac.minimapIcon.SetActive(false);
        container.GetComponent<IEnemySealedContainer>().SetEnemySource(gameObject);
    }

}
