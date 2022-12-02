using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstantiator : MonoBehaviour
{

    [SerializeField] private GameObject prefab;

    private void MyInstantiate(string type)
    {
        switch (type)
        {
            case "parent":
                Instantiate(prefab, transform.position, Quaternion.identity,transform.parent);
                break;
            default:
                Instantiate(prefab, transform.position, Quaternion.identity);
                break;
            
        }
        
    }
}
