using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancePropertyUI : MonoBehaviour
{
    private int instanceFlag;
    private Transform generateParent;

    private void Awake()
    {
        
    }
    public Transform GetGenerateParent()
    {
        return generateParent;
    }
    public Transform SetGenerateParent(Transform value)
    {
        generateParent = value;
        return value;
    }
    public int GetInstanceFlag()
    {
        return instanceFlag;
    }



}
