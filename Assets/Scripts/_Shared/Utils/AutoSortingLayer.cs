using BW.Inspector;
using UnityEngine;

namespace BW.Utils
{
    /// <summary>
    /// Change drawing order based on y position.
    /// </summary>
    public class AutoSortingLayer : MonoBehaviour
    {
        [SerializeField] private bool _static;

        [SerializeField, Range(0f, 10f), HideConditional("_static", false)]
        private float updatePeriod = 0.1f;

        [SerializeField] private float offsetZ = 0;

        [SerializeField] private bool followParrent;

        [SerializeField, HideConditional("_followParrent", false)]
        private Transform followTrans;

        private Transform trans;
        private float lastTimeUpdate;

        public bool FollowParent
        {
            set { this.followParrent = value; }
            get { return this.followParrent; }
        }

        public float OffsetZ
        {
            set { this.offsetZ = value; }
            get { return this.offsetZ; }
        }

        public void Follow()
        {
            this.followParrent = true;
        }

        private void OnEnable()
        {
            if (_static)
            {
                Refresh();
            }
            else
            {
                if (SortingLayerUpdater.Instance != null)
                    SortingLayerUpdater.Add(this);
            }
        }

        private void OnDisable()
        {
            if (!_static)
            {
                if (SortingLayerUpdater.Instance != null)
                    SortingLayerUpdater.Remove(this);
            }
        }

        public void Tick(float time)
        {
            this.lastTimeUpdate = time;
            Refresh();
            if (time - this.lastTimeUpdate >= this.updatePeriod)
            {
                this.lastTimeUpdate = time;
                Refresh();
            }
        }

        [ContextMenu("Update")]
        public void Refresh()
        {
            if (this.trans == null)
                this.trans = transform;

            if (this.followParrent)
                this.followTrans = this.trans.parent;

            Vector3 pos = this.trans.position;

            if (this.followTrans != null)
                pos.z = this.followTrans.position.y - this.offsetZ;
            else
                pos.z = this.trans.position.y - this.offsetZ;
            this.trans.position = pos;
        }
    }
}