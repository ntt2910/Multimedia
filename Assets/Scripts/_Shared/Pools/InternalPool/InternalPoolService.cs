using BW.Pools;
using UnityEngine;

namespace BW.Pooling
{
    public class InternalPoolService : IPoolService
    {
        public void Preload(GameObject prefab, int amount)
        {
            PoolManager.WarmPool(prefab, amount);
        }

        public GameObject Spawn(GameObject prefab)
        {
            return PoolManager.SpawnObject(prefab);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return PoolManager.SpawnObject(prefab, position, rotation);
        }

        public T Spawn<T>(GameObject prefab) where T : Component
        {
            return Spawn(prefab).GetComponent<T>();
        }

        public T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab, position, rotation).GetComponent<T>();
        }

        public void Despawn(GameObject go)
        {
            PoolManager.ReleaseObject(go);
        }

        public void Reset()
        {
            PoolManager.ResetPool();
        }
    }
}