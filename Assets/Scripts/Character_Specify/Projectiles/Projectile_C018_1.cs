using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_C018_1 : MonoBehaviour
{
    public static Projectile_C018_1 Instance { get; private set; }

    private List<GameObject> inList = new();

    private void Awake()
    {
        var others = FindObjectsOfType<Projectile_C018_1>();

        for (int i = others.Length - 1; i >= 0; i--)
        {
            if(others[i]!=this)
                Destroy(others[i].gameObject);
        }





    }

    private void Start()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public bool TargetInRange()
    {
        if (inList.Count > 0)
            return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inList.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inList.Remove(other.gameObject);
            //inList.Remove(other.transform.parent.gameObject);
        }
    }
}
