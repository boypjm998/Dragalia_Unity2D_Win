using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerOnewayPlatformEffector : MonoBehaviour, IGroundSensable
{
    public delegate void OnGetContactGround(GameObject ground);
    public static OnGetContactGround ContactGroundEvent;
    
    
    [SerializeField]
    public GameObject currentOnewayPlatform;
    private GameObject lastContanctGround;

    [SerializeField] protected GameObject currentGround;
    private Rigidbody2D rigid;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private BoxCollider2D playerCollider;
    //private EdgeCollider2D playerColliderFoot;
    public PlayerInput pi;
    
    string[] attackStates;
    

    protected virtual void Awake()
    {
        if(playerCollider== null)
            playerCollider = transform.Find("Platform Sensor").GetComponentInChildren<BoxCollider2D>();
        //playerCollider = transform.Find("GroundSensor").GetComponentInChildren<BoxCollider2D>();
        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        anim = rigid.GetComponentInChildren<Animator>();

        //For Manacaster:
        attackStates = new string[]
        {
            "combo","crunch", "recover","airdash","dash","s1","s2","s3","s4","s1_boost","s2_boost","s3_boost","combo1",
            "combo2","combo3","combo4","combo5","combo6","combo7","combo8","combo9","force_attack"
        };
    }

    protected virtual void Update()
    {
        if (!pi.buttonAttack.IsPressing && pi.buttonDown.IsPressing && pi.CheckCharacterClipState(anim,0,attackStates) == false)
        {

            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            
            if (currentOnewayPlatform!=null)
                StartCoroutine(DisableCollision());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
        {
            currentOnewayPlatform = collision.gameObject;
            lastContanctGround = currentOnewayPlatform;
            ContactGroundEvent?.Invoke(currentOnewayPlatform);
        }else if (collision.gameObject.CompareTag("Ground"))
        {
            
            //2023.12.8: Add a condition 
            if(collision.collider.bounds.min.y > transform.position.y)
                return;
            
            
            currentGround = collision.gameObject;
            lastContanctGround = currentGround;
            ContactGroundEvent?.Invoke(currentGround);
        }

        

    }
    private void OnCollisionExit2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
            currentOnewayPlatform = null;
        else if (collision.gameObject.CompareTag("Ground"))
        {
            currentGround = null;
        }

    }
    public IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentOnewayPlatform.GetComponent<Collider2D>();
        
        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        pi.quicklandingEnabled = false;
        //rigid.velocity = new Vector2(rigid.velocity.x, 0);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider,false);
        pi.quicklandingEnabled = true;
        
    }

    public GameObject GetCurrentAttachedGroundInfo()
    {
        if (currentGround)
            return currentGround;
        else if (currentOnewayPlatform)
            return currentOnewayPlatform;

        return null;
        
        //return currentOnewayPlatform;
    }
    
    public Collider2D GetCurrentAttachedGroundCol()
    {
        if (GetCurrentAttachedGroundInfo() == null)
        {
            return null;
        }

        return GetCurrentAttachedGroundInfo().GetComponent<Collider2D>();
    }

    public GameObject GetLastAttachedGroundInfo()
    {
        return lastContanctGround;
    }
    
    public Collider2D GetSelfCollider()
    {
        return playerCollider;
    }


}
