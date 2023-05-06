using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class MyShadowCaster : MonoBehaviour
{
    protected Collider2D _groundCol;

    private void Start()
    {
        _groundCol = transform.parent.GetComponentInChildren<IGroundSensable>().GetSelfCollider();
    }

    // Update is called once per frame
    void Update()
    {
        var distance = BasicCalculation.GetRaycastedPlatformY(transform.parent.gameObject);
        transform.position =
            new Vector3(transform.position.x, distance, transform.position.z);
    }
}
