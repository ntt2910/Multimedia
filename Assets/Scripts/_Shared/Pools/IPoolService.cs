using BW.Services;
using UnityEngine;

namespace BW.Pools
{
    public interface IPoolService : IService
    {
        void Preload(GameObject prefab, int amount);
        GameObject Spawn(GameObject prefab);
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation);
        T Spawn<T>(GameObject prefab) where T : Component;
        T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component;
        void Despawn(GameObject go);
        void Reset();
    }
}