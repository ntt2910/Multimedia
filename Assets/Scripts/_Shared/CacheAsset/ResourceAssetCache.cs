using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace BW.Assets
{
    public class ResourceAssetCache : IAssetCache
    {
        private readonly Dictionary<string, object> _cache;
        private readonly List<string> _tempCacheIds;

        public ResourceAssetCache()
        {
            this._cache = new Dictionary<string, object>();
            this._tempCacheIds = new List<string>(100);
        }

        public void CacheAsset<T>(string id, T asset, bool permanent = false)
        {
            if (this._cache.ContainsKey(id))
            {
                Debug.LogError("Duplicate caching id " + id);
                return;
            }

            this._cache.Add(id, asset);

            if (!permanent)
            {
                this._tempCacheIds.Add(id);
            }
        }

        public void CacheAssetAsync<T>(string id, Action<T> setter, bool permanent = false) where T : Object
        {
            if (this._cache.ContainsKey(id))
            {
                Debug.LogError("Duplicate caching id " + id);
                return;
            }

            Addressables.LoadAssetAsync<T>(id).Completed += operation =>
            {
                CacheAsset(id, operation.Result);
                setter.Invoke(operation.Result);

                if (!permanent)
                {
                    this._tempCacheIds.Add(id);
                }
            };
        }

        public bool TryGetCache<T>(string id, out T asset)
        {
            asset = default(T);

            if (!this._cache.ContainsKey(id))
            {
                Debug.LogError("Duplicate caching id " + id);
                return false;
            }

            var cachedAsset = this._cache[id];
            var cachedType = cachedAsset.GetType();
            var castType = typeof(T);

            if (cachedType.IsInstanceOfType(castType))
            {
                Debug.LogError("Cached asset id " + id + " expects type of " + cachedAsset.GetType() +
                               " but being casted to " + typeof(T));
                return false;
            }

            asset = (T) this._cache[id];
            return true;
        }

        public T GetCache<T>(string id)
        {
            var asset = default(T);

            if (!this._cache.ContainsKey(id))
            {
                Debug.LogError("Duplicate caching id " + id);
                return asset;
            }

            var cachedAsset = this._cache[id];

            if (cachedAsset == null)
            {
                Debug.LogError("Cached asset does not exist " + id);
                return asset;
            }

            var cachedType = cachedAsset.GetType();
            var castType = typeof(T);

            if (cachedType.IsInstanceOfType(castType))
            {
                Debug.LogError("Cached asset id " + id + " expects type of " + cachedAsset.GetType() +
                               " but being casted to " + typeof(T));
                return asset;
            }

            asset = (T) this._cache[id];
            return asset;
        }

        public bool HasCache(string id)
        {
            return this._cache.ContainsKey(id);
        }

        public void Clear()
        {
            foreach (var id in this._tempCacheIds)
            {
                this._cache.Remove(id);
            }

            this._tempCacheIds.Clear();
        }
    }
}