using UnityEngine;

public class CellBackgroundPool : MonoBehaviour
{
    private ObjectPool<Cell> m_pool;

    public void Initialize(int initialSize)
    {
        Cell prefab = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND).GetComponent<Cell>();
        m_pool = new ObjectPool<Cell>(prefab, this.transform, initialSize);
    }

    public Cell Get()
    {
        return m_pool.Get();
    }

    public void Return(Cell cell)
    {
        m_pool.Return(cell);
    }

    public void Clear()
    {
        m_pool.Clear();
    }
}
