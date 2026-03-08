using System.Collections.Generic;
using UnityEngine;

public class ItemViewPool : MonoBehaviour
{
    private Dictionary<string, Queue<Transform>> m_pools = new Dictionary<string, Queue<Transform>>();
    private Dictionary<string, GameObject> m_prefabCache = new Dictionary<string, GameObject>();
    private Transform m_root;

    public void Initialize(int initialSizePerType)
    {
        m_root = this.transform;

        string[] prefabKeys = new string[]
        {
            Constants.PREFAB_NORMAL_TYPE_ONE,
            Constants.PREFAB_NORMAL_TYPE_TWO,
            Constants.PREFAB_NORMAL_TYPE_THREE,
            Constants.PREFAB_NORMAL_TYPE_FOUR,
            Constants.PREFAB_NORMAL_TYPE_FIVE,
            Constants.PREFAB_NORMAL_TYPE_SIX,
            Constants.PREFAB_NORMAL_TYPE_SEVEN,
            Constants.PREFAB_BONUS_HORIZONTAL,
            Constants.PREFAB_BONUS_VERTICAL,
            Constants.PREFAB_BONUS_BOMB
        };

        foreach (string key in prefabKeys)
        {
            GameObject prefab = Resources.Load<GameObject>(key);
            if (prefab == null) continue;

            m_prefabCache[key] = prefab;
            m_pools[key] = new Queue<Transform>();

            for (int i = 0; i < initialSizePerType; i++)
            {
                Transform instance = Instantiate(prefab, m_root).transform;
                instance.gameObject.SetActive(false);
                m_pools[key].Enqueue(instance);
            }
        }
    }

    public Transform Get(string prefabKey)
    {
        if (!m_pools.ContainsKey(prefabKey))
        {
            m_pools[prefabKey] = new Queue<Transform>();
        }

        if (m_pools[prefabKey].Count > 0)
        {
            Transform obj = m_pools[prefabKey].Dequeue();
            obj.localScale = Vector3.one;
            obj.gameObject.SetActive(true);
            return obj;
        }

        if (!m_prefabCache.ContainsKey(prefabKey))
        {
            GameObject prefab = Resources.Load<GameObject>(prefabKey);
            if (prefab == null) return null;
            m_prefabCache[prefabKey] = prefab;
        }

        return Instantiate(m_prefabCache[prefabKey], m_root).transform;
    }

    public void Return(string prefabKey, Transform obj)
    {
        if (obj == null) return;

        DG.Tweening.DOTween.Kill(obj);
        obj.gameObject.SetActive(false);
        obj.SetParent(m_root);

        if (!m_pools.ContainsKey(prefabKey))
        {
            m_pools[prefabKey] = new Queue<Transform>();
        }

        m_pools[prefabKey].Enqueue(obj);
    }

    public void Clear()
    {
        foreach (var kvp in m_pools)
        {
            while (kvp.Value.Count > 0)
            {
                Transform obj = kvp.Value.Dequeue();
                if (obj != null) Destroy(obj.gameObject);
            }
        }
        m_pools.Clear();
        m_prefabCache.Clear();
    }
}
