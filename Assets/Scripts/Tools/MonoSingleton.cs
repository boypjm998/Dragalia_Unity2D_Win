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

    
    protected virtual void OnDestroy()
    {
        if (instance == this) 
        {
            instance = null; // 销毁时置空
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
