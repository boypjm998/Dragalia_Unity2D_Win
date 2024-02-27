using System;
using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>,new()
{
    protected static T instance;
    
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this as T;
        
    }
}
