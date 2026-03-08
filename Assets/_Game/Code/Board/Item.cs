using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Item
{
    public Cell Cell { get; private set; }

    public Transform View { get; private set; }

    private SpriteRenderer m_spriteRenderer;

    protected ItemViewPool m_viewPool;

    public void SetViewPool(ItemViewPool pool)
    {
        m_viewPool = pool;
    }

    public virtual void SetView()
    {
        string prefabname = GetPrefabName();

        if (!string.IsNullOrEmpty(prefabname))
        {
            if (m_viewPool != null)
            {
                View = m_viewPool.Get(prefabname);
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>(prefabname);
                if (prefab)
                {
                    View = GameObject.Instantiate(prefab).transform;
                }
            }

            if (View != null)
            {
                m_spriteRenderer = View.GetComponent<SpriteRenderer>();
            }
        }
    }

    protected virtual string GetPrefabName() { return string.Empty; }

    public virtual void SetCell(Cell cell)
    {
        Cell = cell;
    }

    internal void AnimationMoveToPosition()
    {
        if (View == null) return;

        View.DOMove(Cell.transform.position, 0.2f);
    }

    public void SetViewPosition(Vector3 pos)
    {
        if (View)
        {
            View.position = pos;
        }
    }

    public void SetViewRoot(Transform root)
    {
        if (View)
        {
            View.SetParent(root);
        }
    }

    public void SetSortingLayerHigher()
    {
        if (m_spriteRenderer != null)
        {
            m_spriteRenderer.sortingOrder = 1;
        }
    }


    public void SetSortingLayerLower()
    {
        if (m_spriteRenderer != null)
        {
            m_spriteRenderer.sortingOrder = 0;
        }
    }

    internal void ShowAppearAnimation()
    {
        if (View == null) return;

        Vector3 scale = View.localScale;
        View.localScale = Vector3.one * 0.1f;
        View.DOScale(scale, 0.1f);
    }

    internal virtual bool IsSameType(Item other)
    {
        return false;
    }

    internal virtual void ExplodeView()
    {
        if (View)
        {
            string prefabname = GetPrefabName();
            Transform viewRef = View;
            View = null;
            viewRef.DOScale(0.1f, 0.1f).OnComplete(
                () =>
                {
                    if (m_viewPool != null)
                    {
                        m_viewPool.Return(prefabname, viewRef);
                    }
                    else
                    {
                        GameObject.Destroy(viewRef.gameObject);
                    }
                }
                );
        }
    }



    internal void AnimateForHint()
    {
        if (View)
        {
            View.DOPunchScale(View.localScale * 0.1f, 0.1f).SetLoops(-1);
        }
    }

    internal void StopAnimateForHint()
    {
        if (View)
        {
            View.DOKill();
        }
    }

    internal void Clear()
    {
        if (View)
        {
            if (m_viewPool != null)
            {
                m_viewPool.Return(GetPrefabName(), View);
            }
            else
            {
                GameObject.Destroy(View.gameObject);
            }
            View = null;
        }

        Cell = null;
    }
}
