using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float leftEdge;
    [SerializeField] private float rightEdge;
    [SerializeField] private float speed;

    [SerializeField] private int direction;
    
    //private Rigidbody2D _rigidbody2D;

    private List<Rigidbody2D> _contactsList = new();

    private void Awake()
    {
        //_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var rigid = other.rigidbody;
        print(other + " Enter");
        
        if (rigid != null && !_contactsList.Contains(rigid))
        {
            
            _contactsList.Add(rigid);
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        var rigid = other.rigidbody;
        if (rigid != null)
        {
            _contactsList.Remove(rigid);
        }
    }

    private void FixedUpdate()
    {
        var spd = GetPlatformSpeed();
        transform.position += new Vector3(GetPlatformSpeed(), 0);
        
        for (int i = 0; i < _contactsList.Count; i++)
        {
            _contactsList[i].position += new Vector2(GetPlatformSpeed(), 0);
        }
    }

    private float GetPlatformSpeed()
    {
        var spd = speed * Time.fixedDeltaTime;

        if (direction < 0 && transform.position.x > leftEdge)
        {
            return -spd;
        }
        else if(direction < 0 &&transform.position.x <= leftEdge)
        {
            direction = 1;
            return spd;
        }else if (direction > 0 && transform.position.x < rightEdge)
        {
            return spd;
        }else if (direction > 0 && transform.position.x >= rightEdge)
        {
            direction = -1;
            return -spd;
        }
        else return spd;
        
        

    }
}
