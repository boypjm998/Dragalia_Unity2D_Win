using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_C001_3 : MonoBehaviour
{
    //Otherworld Gate Controller for Ilia.
    private CircleCollider2D blackholeDragger;
    private float lifetime = 5.01f;
    private float lefttime;
    private float resetCD = 0.1f;
    private AttackFromPlayer _attackFromPlayer;
    private Coroutine timerRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,lifetime);
        lefttime = lifetime;
        InvokeRepeating("ResetFlag",0.1f,0.1f);
        blackholeDragger = transform.GetChild(0).GetComponent<CircleCollider2D>();
        _attackFromPlayer = GetComponent<AttackFromPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void ResetFlag()
    {
        _attackFromPlayer.ResetFlags();
    }





}
