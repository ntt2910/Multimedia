using System;
using System.Collections.Generic;
using UnityEngine;

namespace BW.Pooling
{
    public class ObjectPool<T>
    {
        private List<ObjectPoolContainer<T>> list;
        private Dictionary<T, ObjectPoolContainer<T>> lookup;
        private Func<T> factoryFunc;
        private int lastIndex = 0;

        public ObjectPool(Func<T> factoryFunc, int initialSize)
        {
            this.factoryFunc = factoryFunc;

            this.list = new List<ObjectPoolContainer<T>>(initialSize);
            this.lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

            Warm(initialSize);
        }

        private void Warm(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                CreateConatiner();
            }
        }

        private ObjectPoolContainer<T> CreateConatiner()
        {
            var container = new ObjectPoolContainer<T>();
            container.Item = this.factoryFunc();
            this.list.Add(container);
            return container;
        }

        public T GetItem()
        {
            ObjectPoolContainer<T> container = null;
            for (int i = 0; i < this.list.Count; i++)
            {
                this.lastIndex++;
                if (this.lastIndex > this.list.Count - 1) this.lastIndex = 0;

                if (this.list[this.lastIndex].Used)
                {
                    continue;
                }
                else
                {
                    container = this.list[this.lastIndex];
                    break;
                }
            }

            if (container == null)
            {
                container = CreateConatiner();
            }

            container.Consume();
            this.lookup.Add(container.Item, container);
            return container.Item;
        }

        public void ReleaseItem(object item)
        {
            ReleaseItem((T)item);
        }

        public void ReleaseItem(T item)
        {
            if (this.lookup.ContainsKey(item))
            {
                var container = this.lookup[item];
                container.Release();
                this.lookup.Remove(item);
            }
            else
            {
                Debug.LogWarning("This object pool does not contain the item provided: " + item);
            }
        }

        public int Capacity => this.list.Capacity;

        public int Count => this.list.Count;

        public int CountUsedItems => this.lookup.Count;
    }
}
