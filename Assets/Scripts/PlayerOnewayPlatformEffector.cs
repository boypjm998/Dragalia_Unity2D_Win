using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerOnewayPlatformEffector : MonoBehaviour
{
    
    [SerializeField]
    public GameObject currentOnewayPlatform;
    private Rigidbody2D rigid;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private BoxCollider2D playerCollider;
    //private EdgeCollider2D playerColliderFoot;
    public PlayerInput pi;
    
    string[] attackStates;
    

    void Awake()
    {
        playerCollider = GetComponentInChildren<BoxCollider2D>();
        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        anim = rigid.GetComponent<Animator>();

        //For Manacaster:
        attackStates = new string[9]{ "combo","crunch", "recover","airdash","dash","s1","s2","s3","s4" };
    }

    void Update()
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
        }


        }
    private void OnCollisionExit2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
            currentOnewayPlatform = null;

    }
    private IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentOnewayPlatform.GetComponent<Collider2D>();
        
        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider,false);
        
    }


}
