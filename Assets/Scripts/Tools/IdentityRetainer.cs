using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityRetainer : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
