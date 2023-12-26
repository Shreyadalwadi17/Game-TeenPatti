using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPoolController : MonoBehaviour
{
    public CardDisplay prefab;
    public int poolSize = 10;

    private ObjectPool<CardDisplay> objectPool;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        objectPool = new ObjectPool<CardDisplay>(prefab, poolSize, transform);
    }

    public CardDisplay SpawnObject()
    {
        CardDisplay obj = objectPool.GetObject();
        // Use the object as needed
        return obj;

    }

    public void ReturnObject(CardDisplay cardDisplay)
    {
        objectPool.ReturnObject(cardDisplay);
    }
}

public class ObjectPool<T> where T : MonoBehaviour
{
    private T prefab;
    private int poolSize;
    private List<T> pool = new List<T>();
    private Transform poolParent;

    public ObjectPool(T prefab, int poolSize, Transform poolParent)
    {

        this.prefab = prefab;
        this.poolSize = poolSize;
        this.poolParent = poolParent;
        CreatePool();
    }

    private void CreatePool()
    {
        pool = new List<T>();
        // Instantiate the objects and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            T obj = GameObject.Instantiate(prefab,  poolParent);//Vector3.zero, Quaternion.identity,
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public T GetObject()
    {
        // Search for an inactive object in the pool
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        // If all objects are active, create a new one and add it to the pool
        T newObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, poolParent);
        newObj.gameObject.SetActive(true);
        pool.Add(newObj);

        return newObj;
    }

    public void ReturnObject(T obj)
    {
        // Reset the object and set it inactive, then add it back to the pool
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(poolParent);
        obj.transform.localPosition = Vector3.zero; // Optional: reset the position
        obj.transform.localRotation = Quaternion.identity; // Optional: reset the position
        obj.transform.localScale = Vector3.one; // Optional: reset the position
        pool.Add(obj);
    }
}


