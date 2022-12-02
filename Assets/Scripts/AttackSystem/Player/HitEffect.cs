using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public GameObject effect;
    public GameObject self;
   
    [SerializeField]
    private GameObject hitConnectEffect;
    [SerializeField]
    private Collider2D attackCollider;


    // Start is called before the first frame update
    void Awake()
    {
        //attackCollider = GetComponent<Collider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        self = gameObject;
    }

    private void FixedUpdate()
    {
        //attackCollider = GetComponent<Collider2D>();
    }

    public void InstantDestroy()
    {
        Destroy(effect);
        
    }
    public void InstantDestroySelf()
    {
        Destroy(self);

    }
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hitFlag==false)
        {
            hitFlag = true;
            GameObject eff = Instantiate(hitConnectEffect, collision.gameObject.transform.position, Quaternion.identity);
            eff.name = "HitEffect1";
            CineMachineOperator.Instance.CamaraShake(5f, .1f);
        }
        
    }*/
}
