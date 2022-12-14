using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerOnewayPlatformEffector : MonoBehaviour
{
    public delegate void OnGetContactGround(GameObject ground);
    public static OnGetContactGround ContactGroundEvent;
    
    
    [SerializeField]
    public GameObject currentOnewayPlatform;

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
        playerCollider = transform.Find("GroundSensor").GetComponentInChildren<BoxCollider2D>();
        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        anim = rigid.GetComponentInChildren<Animator>();

        //For Manacaster:
        attackStates = new string[]
            { "combo","crunch", "recover","airdash","dash","s1","s2","s3","s4","s1_boost","s2_boost","s3_boost" };
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
            ContactGroundEvent?.Invoke(currentOnewayPlatform);
        }else if (collision.gameObject.CompareTag("Ground"))
        {
            currentGround = collision.gameObject;
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
    private IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentOnewayPlatform.GetComponent<Collider2D>();
        
        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider,false);
        
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


}
