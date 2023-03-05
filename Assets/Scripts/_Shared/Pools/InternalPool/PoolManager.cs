using System;
using System.Collections.Generic;
using BW.Util;
using UnityEngine;

namespace BW.Pooling
{
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        public Transform root;

        private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
        private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

        private bool dirty = false;

        public delegate void WarmingDelegate(GameObject go);

        public static event WarmingDelegate OnWarmingEvent;

        public delegate void SpawnDelegate(GameObject go);

        public static event SpawnDelegate OnSpawnEvent;

        public delegate void DespawnDelegate(GameObject go);

        public static event DespawnDelegate OnDespawnEvent;

        protected override void Awake()
        {
            base.Awake();
            this.prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
            this.instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        }

#if UNITY_EDITOR
        public bool logStatus;

        void Update()
        {
            if (this.logStatus && this.dirty)
            {
                PrintStatus();
                this.dirty = false;
            }
        }
#endif

        public void warmPool(GameObject prefab, int size)
        {
            if (this.prefabLookup.ContainsKey(prefab))
            {
#if UNITY_EDITOR
                //Debug.Log("Pool for prefab " + prefab.name + " has already been created");
#endif
            }
            else
            {
                prefab.SetActive(false);
                var pool = new ObjectPool<GameObject>(() => InstantiatePrefab(prefab), size);
                this.prefabLookup[prefab] = pool;
                this.dirty = true;
            }
        }

        public void warmPool(GameObject prefab, int size, Func<GameObject> warmFunc)
        {
            if (this.prefabLookup.ContainsKey(prefab))
            {
#if UNITY_EDITOR
                //Debug.Log("Pool for prefab " + prefab.name + " has already been created");
#endif
            }
            else
            {
                prefab.SetActive(false);
                var pool = new ObjectPool<GameObject>(warmFunc, size);
                this.prefabLookup[prefab] = pool;
                this.dirty = true;
            }
        }

        public void Reset()
        {
            this.prefabLookup.Clear();
            this.instanceLookup.Clear();
        }

        public GameObject spawnObject(GameObject prefab)
        {
            return spawnObject(prefab, null, Vector3.zero, Quaternion.identity);
        }

        public GameObject spawnObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation,
            bool autoWarmPool = false, int warmAmount = 5)
        {
            GameObject clone = null;
            if (!this.prefabLookup.ContainsKey(prefab))
            {
                if (autoWarmPool)
                {
                    WarmPool(prefab, warmAmount);

                    var pool = this.prefabLookup[prefab];

                    clone = pool.GetItem();

                    if (clone != null)
                    {
                        var cloneTrans = clone.transform;
                        cloneTrans.position = position;
                        cloneTrans.rotation = rotation;
                        clone.SetActive(true);

                        this.instanceLookup.Add(clone, pool);
                        this.dirty = true;
                    }
                    else
                    {
                        clone = parent == null
                            ? Instantiate(prefab, position, rotation)
                            : Instantiate(prefab, position, rotation, parent);
                        clone.SetActive(true);
                    }

                    OnSpawnEvent?.Invoke(clone);
                }
                else
                {
                    clone = parent == null
                        ? Instantiate(prefab, position, rotation)
                        : Instantiate(prefab, position, rotation, parent);
                    clone.SetActive(true);

                    OnSpawnEvent?.Invoke(clone);
                }
            }
            else
            {
                var pool = this.prefabLookup[prefab];

                clone = pool.GetItem();

                if (clone != null)
                {
                    clone.transform.position = position;
                    clone.transform.rotation = rotation;
                    clone.SetActive(true);

                    this.instanceLookup.Add(clone, pool);
                    this.dirty = true;
                }
                else
                {
                    clone = parent == null
                        ? Instantiate(prefab, position, rotation)
                        : Instantiate(prefab, position, rotation, parent);
                    clone.SetActive(true);
                }

                OnSpawnEvent?.Invoke(clone);
            }

            return clone;
        }

        public void releaseObject(GameObject clone)
        {
            //clone.SetActive(false);

            if (this.instanceLookup.ContainsKey(clone))
            {
                OnDespawnEvent?.Invoke(clone);

                clone.SetActive(false);

                clone.transform.SetParent(this.root);

                this.instanceLookup[clone].ReleaseItem(clone);
                this.instanceLookup.Remove(clone);
                this.dirty = true;
            }
            else
            {
                Destroy(clone);
#if UNITY_EDITOR
                // Debug.LogWarning("No pool contains the object: " + clone.name);
#endif
            }
        }


        private GameObject InstantiatePrefab(GameObject prefab)
        {
            prefab.SetActive(true);
            var go = Instantiate(prefab) as GameObject;

            OnWarmingEvent?.Invoke(go);

            if (this.root != null) go.transform.SetParent(this.root);

            go.SetActive(false);
//            prefab.SetActive(false);
            return go;
        }

        public void PrintStatus()
        {
            foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in this.prefabLookup)
            {
                Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name,
                    keyVal.Value.CountUsedItems, keyVal.Value.Count));
            }
        }

        #region Static API

        public static void WarmPool(GameObject prefab, int size)
        {
            Instance.warmPool(prefab, size);
        }

        public static void WarmPool(GameObject prefab, int size, Func<GameObject> warmFunc)
        {
            Instance.warmPool(prefab, size, warmFunc);
        }

        public static GameObject SpawnObject(GameObject prefab)
        {
            return Instance.spawnObject(prefab);
        }

        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instance.spawnObject(prefab, null, position, rotation);
        }

        public static GameObject SpawnObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            return Instance.spawnObject(prefab, parent, position, rotation);
        }

        public static void ReleaseObject(GameObject clone)
        {
            Instance.releaseObject(clone);
        }

        public static void ResetPool()
        {
            Instance.Reset();
        }

        #endregion
    }
}