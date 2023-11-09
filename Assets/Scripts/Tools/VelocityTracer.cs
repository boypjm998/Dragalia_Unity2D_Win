using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class VelocityTracer : MonoBehaviour
{
    public ActorBase actor;

    private Dictionary<ActorBase, Vector2> moveInfoDictionary = new();

    public float angleDeg;
    
    public Quaternion Rotation => Quaternion.Euler(0,0,angleDeg);

    public Vector2 velocity { private set; get; } = Vector2.zero;

    public bool rotate = false;


    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>actor!=null);
        
        if(!moveInfoDictionary.ContainsKey(actor))
            moveInfoDictionary.Add(actor,actor.transform.position);
    }
    
    

    private void FixedUpdate()
    {
        if(!actor)
            return;
        
        if(!moveInfoDictionary.ContainsKey(actor))
            moveInfoDictionary.Add(actor,actor.transform.position);
        

        Vector2 diff = (Vector2)actor.transform.position - moveInfoDictionary[actor];

        velocity = diff;

        float angleRad = Mathf.Atan2(diff.y, diff.x);
        
        //float angleDeg;
        
        if (diff.x != 0 || diff.y != 0)
        {
            angleDeg = (180 / Mathf.PI) * angleRad;
            //transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
        else
        {
            angleDeg = actor.facedir == 1 ? 0 : 180;
            //transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }

        moveInfoDictionary[actor] = actor.transform.position;
        
        //print(angleDeg);


        if(rotate)
            transform.rotation = Quaternion.Euler(0,0,angleDeg);
    }
    
    
}
