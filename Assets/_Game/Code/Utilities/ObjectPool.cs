using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T m_prefab;
    private Transform m_parent;
    private Queue<T> m_pool = new Queue<T>();

    public ObjectPool(T prefab, Transform parent, int initialSize = 0)
    {
        m_prefab = prefab;
        m_parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateInstance();
            obj.gameObject.SetActive(false);
            m_pool.Enqueue(obj);
        }
    }

    private T CreateInstance()
    {
        return GameObject.Instantiate(m_prefab, m_parent);
    }

    public T Get()
    {
        T obj;
        if (m_pool.Count > 0)
        {
            obj = m_pool.Dequeue();
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = CreateInstance();
        }
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        m_pool.Enqueue(obj);
    }

    public void Clear()
    {
        while (m_pool.Count > 0)
        {
            T obj = m_pool.Dequeue();
            if (obj != null)
            {
                GameObject.Destroy(obj.gameObject);
            }
        }
    }

    public int CountInactive => m_pool.Count;
}
