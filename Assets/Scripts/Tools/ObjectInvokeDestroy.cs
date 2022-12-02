using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectInvokeDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,destroyTime);
    }

    // Update is called once per frame

}
