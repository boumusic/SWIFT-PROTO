using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    [Tooltip("The prefab to instantiate.")] public GameObject prefab;
    [Range(0, 1000), Tooltip("The amount of gameObject to instantiate in the pool")] public int amount = 20;
    public List<Entity> entities = new List<Entity>();
    public Transform parent;
    public bool foldout = true;
    public float usage = 0f;
    public Entity Entity => prefab.GetComponent<Entity>();
}

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance { get { if (!instance) instance = FindObjectOfType<PoolManager>(); return instance; } }

    public bool instantiateOnAwake = false;
    public List<Pool> pools = new List<Pool>();

    private void Awake()
    {
        if(instantiateOnAwake)
        {
            InstantiatePools();
        }
    }

    private void Update()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            UpdateCurrentlyActive(i);
        }
    }

    public T GetEntity<T>() where T : class
    {
        for (int p = 0; p < pools.Count; p++)
        {
            Pool poolable = pools[p];
            if (poolable.entities.Count > 0)
            {
                if (poolable.entities[0] is T)
                {
                    for (int i = 0; i < poolable.entities.Count; i++)
                    {
                        Entity entity = poolable.entities[i];
                        if (!entity.gameObject.activeInHierarchy)
                        {
                            return entity as T;
                        }
                    }
                }
            }
        }

        Debug.LogError("Error : not enough entities of types " + typeof(T).ToString() + " in pool.");
        return null;
    }
    
    public Entity GetEntityOfType(System.Type type)
    {
        if (type == null) return null;
        for (int p = 0; p < pools.Count; p++)
        {
            Pool pool = pools[p];
            if (pool.entities.Count > 0)
            {
                if (pool.entities[0].GetType() == type)
                {
                    for (int i = 0; i < pool.entities.Count; i++)
                    {
                        Entity entity = pool.entities[i];
                        if (!entity.gameObject.activeInHierarchy)
                        {
                            return entity;
                        }
                    }
                }
            }
        }

        Debug.LogError("Error : not enough entities of types " + type.ToString() + " in pool.");
        return null;
    }

    public int UpdateCurrentlyActive(int i)
    {
        int currentlyActive = 0;
        pools[i].usage = 0;
        if (pools[i].entities.Count > 0)
        {
            for (int e = 0; e < pools[i].entities.Count; e++)
            {
                Entity entity = pools[i].entities[e];
                if (entity)
                {
                    if (entity.gameObject.activeInHierarchy)
                    {
                        currentlyActive++;
                        pools[i].usage += (1f / (pools[i].entities.Count));
                    }
                }
            }
        }

        return currentlyActive;
    }

    public bool ShouldRefresh()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            Pool pool = pools[i];
            if (pool.prefab == null)
            {
                return false;
            }

            else if (!pool.prefab.GetComponentInChildren<Entity>())
            {
                return false;
            }

            if (pool.entities.Count != pool.amount)
            {
                return true;
            }

            else
            {
                if (pool.entities.Count > 0)
                {
                    for (int e = 0; e < pool.entities.Count; e++)
                    {
                        if (pool.entities[e] == null)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    
    public Pool GetLevelElementPoolAtIndex(int index)
    {
        if (index < pools.Count)
        {
            return pools[index];
        }

        else return null;
    }    
    
    private void InstantiatePools()
    {
        DestroyPools();
        if (pools.Count > 0)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                Pool p = pools[i];
                if (p.prefab)
                {
                    if (!p.parent)
                    {
                        GameObject newParentGo = new GameObject(p.prefab.name + "_Pool");
                        newParentGo.transform.parent = transform;
                        p.parent = newParentGo.transform;
                    }

                    for (int amnt = 0; amnt < p.amount; amnt++)
                    {
                        GameObject newGo = Instantiate(p.prefab, p.parent);
                        //GameObject newGo = PrefabUtility.InstantiatePrefab(p.prefab, p.parent) as GameObject;
                        newGo.name = p.prefab.name + "_" + amnt;
                        newGo.SetActive(false);
                        Entity e = newGo.GetComponentInChildren<Entity>();
                        p.entities.Add(e);
                    }
                }
            }
        }
    }

    private void DestroyPools()
    {
        if (pools.Count > 0)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].entities.Count > 0)
                {
                    for (int g = 0; g < pools[i].entities.Count; g++)
                    {
                        if (pools[i].entities[g])
                        {
                            GameObject go = pools[i].entities[g].gameObject;
                            if (go)
                            {
                                DestroyImmediate(go);
                            }
                        }
                    }

                    pools[i].entities.Clear();
                }
            }
        }
    }
}
