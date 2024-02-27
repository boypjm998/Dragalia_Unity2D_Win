using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{

    private ActorController ac;
    public BoxCollider2D box;
    public PlayerOnewayPlatformEffector pltEffector;
    [SerializeField]
    private Animator anim;
    public Rigidbody2D rigid;
    private Vector2 boxsize;
    private Vector3 center;
    //public Transform GroundCheck;
    //private bool isGround;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponentInParent<Rigidbody2D>();
        anim = rigid.GetComponent<Animator>();
        ac = rigid.GetComponent<ActorController>();

        boxsize = box.size;
        pltEffector = GetComponentInParent<PlayerOnewayPlatformEffector>();


        //print(center);


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        center = transform.position;
        center.y += (box.offset.y - 0.05f);
        Collider2D outputCol = Physics2D.OverlapBox(center, boxsize + new Vector2(-0.15f,0),0.0f,LayerMask.GetMask("Ground"));
        //Collider2D outputCol2 = Physics2D.OverlapBox(center, boxsize, 0.0f, LayerMask.GetMask("Obstacles"));
        Collider2D pointColliderA = Physics2D.OverlapPoint(new Vector2(center.x - 0.5f * boxsize.x, center.y - 0.5f * boxsize.y), LayerMask.GetMask("Platforms"));
        Collider2D pointColliderB = Physics2D.OverlapPoint(new Vector2(center.x + 0.5f * boxsize.x, center.y - 0.5f * boxsize.y), LayerMask.GetMask("Platforms"));
        //PlayerOnewayPlatformEffector platformEffector = GetComponentInParent<PlayerOnewayPlatformEffector>();
        //RayCast
        if (outputCol != null || (pointColliderA != null || pointColliderB != null) && rigid.velocity.y==0 ) //|| center.y + 0.5f*boxsize.y >= platformEffector.currentOnewayPlatform.transform.position.y)
        {
            
            ac.IsGround();
            // if(outputCol!=null)
            //     print("OnGroundSensor: OutputCol is not null");
            //SendMessageUpwards("IsGround");
        } else {
            ac.isNotGround();
            //SendMessageUpwards("isNotGround");
        }
        //print("PlayerPos:"+transform.parent.position.y);
        
    }

    public GameObject GetContactGround()
    {
        return pltEffector.GetCurrentAttachedGroundInfo();
    }


}
