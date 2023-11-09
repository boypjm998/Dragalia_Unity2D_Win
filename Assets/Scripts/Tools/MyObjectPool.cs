using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjectPool : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    protected Queue<GameObject> _pooledInstanceQueue = new();
    public int capacity = 50;
    public int startSize = 25;


    private void Start()
    {
        InitializePool(prefab, startSize);
    }
    
    private void InitializePool(GameObject prefab, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            _pooledInstanceQueue.Enqueue(instance);
        }
    }
    
    

    public GameObject GetInstance(Transform parent = null) 
    {
        if (_pooledInstanceQueue.Count > 0) 
        {
            GameObject instanceToReuse = _pooledInstanceQueue.Dequeue();
            instanceToReuse.SetActive(true);
            
            if(parent != null)
                instanceToReuse.transform.SetParent(parent);
            
            return instanceToReuse;
        }

        if (parent == null)
        {
            parent = transform;
        }

        return Instantiate(prefab, parent);
    }

    public void ReturnInstance(GameObject gameObjectToPool) 
    {
        _pooledInstanceQueue.Enqueue(gameObjectToPool);
        gameObjectToPool.SetActive(false);
        gameObjectToPool.transform.SetParent(gameObject.transform);
    }
}
