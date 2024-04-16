using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileControllerTest))]
public class ProjectileClamp : MonoBehaviour
{
    private ProjectileControllerTest _projectileControllerTest;
    [SerializeField] private float leftClamp;
    [SerializeField] private float rightClamp;
    [SerializeField] private float topClamp;
    [SerializeField] private float bottomClamp;
    
    [SerializeField] private bool clampX = false;
    [SerializeField] private bool clampY = false;
    
    private void Awake()
    {
        _projectileControllerTest = GetComponent<ProjectileControllerTest>();
    }

    private void FixedUpdate()
    {
        if (clampX)
        {
            if (transform.position.x < leftClamp)
            {
                _projectileControllerTest.SetVelocity(new Vector2(Mathf.Abs(_projectileControllerTest.velocity.x), _projectileControllerTest.velocity.y));
            }
            else if (transform.position.x > rightClamp)
            {
                _projectileControllerTest.SetVelocity(new Vector2(-Mathf.Abs(_projectileControllerTest.velocity.x), _projectileControllerTest.velocity.y));
            }
        }

        if (clampY)
        {
            if (transform.position.y < bottomClamp)
            {
                _projectileControllerTest.SetVelocity(new Vector2(_projectileControllerTest.velocity.x, Mathf.Abs(_projectileControllerTest.velocity.y)));
            }
            else if (transform.position.y > topClamp)
            {
                _projectileControllerTest.SetVelocity(new Vector2(_projectileControllerTest.velocity.x, -Mathf.Abs(_projectileControllerTest.velocity.y)));
            }
        }
    }
}
