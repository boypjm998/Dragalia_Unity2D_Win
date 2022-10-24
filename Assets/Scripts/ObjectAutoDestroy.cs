using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAutoDestroy : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            Destroy(gameObject);
        }    
    }
}
