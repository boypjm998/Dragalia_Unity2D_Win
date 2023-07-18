using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardGroundSensor : MonoBehaviour , IGroundSensable
{
    public BoxCollider2D box;
    private Rigidbody2D rigid;
    private Vector2 center;

    public delegate void GroundDelegator(bool ground);
    public delegate void GroundTrigger();

    public event GroundDelegator IsGround;
    public event GroundTrigger OnGroundEnter;
    
    
    
    public GameObject currentPlatform;
    public GameObject currentGround;
    private GameObject lastContanctGround;


    private void Awake()
    {
        
        
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponentInParent<Rigidbody2D>();
        if(box==null)
            box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        center = transform.position;
        center.y += (box.offset.y - 0.05f);
        Collider2D outputCol = Physics2D.OverlapBox(center, box.size,0.0f,LayerMask.GetMask("Ground"));
        //Collider2D outputCol2 = Physics2D.OverlapBox(center, boxsize, 0.0f, LayerMask.GetMask("Obstacles"));
        Collider2D pointColliderA = Physics2D.OverlapPoint(new Vector2(center.x - 0.5f * box.size.x, center.y - 0.5f * box.size.y), LayerMask.GetMask("Platforms"));
        Collider2D pointColliderB = Physics2D.OverlapPoint(new Vector2(center.x + 0.5f * box.size.x, center.y - 0.5f * box.size.y), LayerMask.GetMask("Platforms"));
        //PlayerOnewayPlatformEffector platformEffector = GetComponentInParent<PlayerOnewayPlatformEffector>();
        //RayCast
        if (outputCol != null || (pointColliderA!=null || pointColliderB!=null)&&rigid.velocity.y==0 ) //|| center.y + 0.5f*boxsize.y >= platformEffector.currentOnewayPlatform.transform.position.y)
        {
            IsGround?.Invoke(true);
        } else {
            IsGround?.Invoke(false);
        }
        //print("EnemyPos:"+transform.parent.position.y);
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
        {
            lastContanctGround = currentPlatform;
            currentPlatform = collision.gameObject;
            OnGroundEnter?.Invoke();
        }else if (collision.gameObject.CompareTag("Ground"))
        {
            lastContanctGround = currentPlatform;
            currentGround = collision.gameObject;
            OnGroundEnter?.Invoke();
        }


    }
    private void OnCollisionExit2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("platform"))
            currentPlatform = null;
        else if (collision.gameObject.CompareTag("Ground"))
        {
            currentGround = null;
        }

    }
    
    public IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentPlatform?.GetComponent<Collider2D>();
        if (!platformCollider)
        {
            print("No platform");
            yield break;
        }

        
        print("Disable Collision With Platform"+platformCollider.name);
        Physics2D.IgnoreCollision(box, platformCollider);
        yield return new WaitForSeconds(0.15f);
        Physics2D.IgnoreCollision(box, platformCollider,false);
        
    }

    public GameObject GetCurrentAttachedGroundInfo()
    {
        if(currentPlatform!=null)
            return currentPlatform;
        else if (currentGround!=null)
            return currentGround;
        else
            return null;
    }

    public Collider2D GetCurrentAttachedGroundCol()
    {
        if (GetCurrentAttachedGroundInfo() == null)
            return null;
        
        return GetCurrentAttachedGroundInfo().GetComponent<Collider2D>();
    }

    public GameObject GetLastAttachedGroundInfo()
    {
        return lastContanctGround;
    }

    public Collider2D GetSelfCollider()
    {
        return box;
    }
}
