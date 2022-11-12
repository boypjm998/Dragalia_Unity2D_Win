using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRangedFromPlayer : AttackFromPlayer
{
    // Start is called before the first frame update
    void Awake()
    {
        hitFlags = SearchEnemyList();
        attackCollider = GetComponent<Collider2D>();
    }
    protected override void Start()
    {
        base.Start();
    }
  

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        Transform enemyTrans;
        //Debug.Log(collision.gameObject.GetInstanceID());
        print("Ranged stay!!!!");
        
        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            //print(collision.name);

            hitFlags.Remove(collision.transform.parent.GetInstanceID());

            enemyTrans = collision.transform.parent;

            int dmg = battleStageManager.PlayerHit(collision.gameObject, this);
            
            

            GameObject eff = Instantiate(hitConnectEffect, new Vector2(collision.transform.position.x,transform.position.y), Quaternion.identity);
            eff.name = "HitEffect1";
            CineMachineOperator.Instance.CamaraShake(hitShakeIntensity, .1f);
            collision.gameObject.GetComponent<Enemy>().TakeDamage();
            
            AttackContainer container = gameObject.GetComponentInParent<AttackContainer>();
            container.AttackOneHit();
            if (container.NeedTotalDisplay() && dmg > 0)
                container.AddTotalDamage(dmg);
            
            
            
            

        }
        
    }
    
    
    
}
