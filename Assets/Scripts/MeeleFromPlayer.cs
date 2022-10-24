using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleFromPlayer : AttackFromPlayer
{
    //AirDash For Most Characters

    Coroutine ConnectCoroutine;
    //private float defaultGravity = 4;

    // Start is called before the first frame update
    private void Awake()
    {

        hitFlags = SearchEnemyList();

       
        attackCollider = GetComponent<Collider2D>();
        playerpos = GameObject.Find("PlayerHandle").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        //defaultGravity = rigid.gravityScale;
        self = gameObject;
    }

    public void InstantDestroySelf()
    {
        hitFlags.Clear();
        RecoverFromMeeleTimeStop(4);
        Destroy(self);

    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform enemyTrans;
        //Debug.Log(collision.gameObject.GetInstanceID());
        
        
        if (collision.CompareTag("Enemy") && hitFlags.Contains(collision.transform.parent.GetInstanceID()))
        {

            //print(collision.name);

            hitFlags.Remove(collision.transform.parent.GetInstanceID());

            enemyTrans = collision.transform.parent;

            //´ýÓÅ»¯
            GameObject damageManager = GameObject.Find("DamageManager");
            DamageNumberManager dnm = damageManager.GetComponent<DamageNumberManager>();
            if (Random.Range(0, 100) < 12)
            {
                dnm.DamagePopEnemy(enemyTrans, (int)(0.75f * 1.7f* 998 * Random.Range(0.95f, 1.05f)),2);//crit
            }
            else {
                dnm.DamagePopEnemy(enemyTrans, (int)(0.75f * 998 * Random.Range(0.95f, 1.05f)),1);
            }
            

            GameObject eff = Instantiate(hitConnectEffect, new Vector2(collision.transform.position.x,transform.position.y), Quaternion.identity);
            eff.name = "HitEffect1";
            CineMachineOperator.Instance.CamaraShake(5f, .1f);
            collision.gameObject.GetComponent<Enemy>().TakeDamage();
            if(ConnectCoroutine==null)
            { 
                ConnectCoroutine = StartCoroutine(MeeleTimeStop(0.1f));
            }
            

        }
        
    }
    public override IEnumerator MeeleTimeStop(float time)
    {

        Animator animAttack = GetComponentInParent<Animator>();
        Rigidbody2D rigid = playerpos.gameObject.GetComponentInParent<Rigidbody2D>();
        Animator anim = playerpos.gameObject.GetComponentInParent<Animator>();
        //Debug.Log(parent);
        animAttack.speed = 0.5f;
        anim.speed = 0;
        float gravity = rigid.gravityScale;
        //print(gravity);
        rigid.gravityScale = 0;
        yield return new WaitForSeconds(time);

        RecoverFromMeeleTimeStop(4);

       
    }

    private void OnDestroy()
    {
        RecoverFromMeeleTimeStop(4);
    }


}
