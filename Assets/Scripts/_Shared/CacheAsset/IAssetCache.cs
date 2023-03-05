using System;
using Object = UnityEngine.Object;

namespace BW.Assets
{
    public interface IAssetCache
    {
        void CacheAsset<T>(string id, T asset, bool permanent = false);
        void CacheAssetAsync<T>(string id, Action<T> getter, bool permanent = false) where T : Object;
        bool TryGetCache<T>(string id, out T asset);
        T GetCache<T>(string id);
        bool HasCache(string id);
        void Clear();
    }
}