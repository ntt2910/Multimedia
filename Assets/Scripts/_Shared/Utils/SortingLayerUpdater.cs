using System.Collections.Generic;
using BW.Util;
using UnityEngine;

namespace BW.Utils
{
    public class SortingLayerUpdater : SingletonMonoBehaviour<SortingLayerUpdater>
    {
        private List<AutoSortingLayer> sortingLayers;
        private HashSet<int> lookup;

        protected override void Awake()
        {
            base.Awake();
            this.sortingLayers = new List<AutoSortingLayer>(50);
            this.lookup = new HashSet<int>();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < this.sortingLayers.Count; ++i)
            {
                var sorter = this.sortingLayers[i];

                if (sorter != null)
                {
                    sorter.Tick(Time.time);
                }
            }
        }

        public void add(AutoSortingLayer autoSortingLayer)
        {
            if (this.lookup != null && !this.lookup.Contains(autoSortingLayer.GetInstanceID()))
            {
                this.lookup.Add(autoSortingLayer.GetInstanceID());
                this.sortingLayers.Add(autoSortingLayer);
            }
        }

        public void remove(AutoSortingLayer autoSortingLayer)
        {
            if (this.lookup != null && this.lookup.Contains(autoSortingLayer.GetInstanceID()))
            {
                this.lookup.Remove(autoSortingLayer.GetInstanceID());
                this.sortingLayers.Remove(autoSortingLayer);
            }
        }

        public static void Add(AutoSortingLayer autoSortingLayer)
        {
            Instance.add(autoSortingLayer);
        }

        public static void Remove(AutoSortingLayer autoSortingLayer)
        {
            Instance.remove(autoSortingLayer);
        }
    }
}