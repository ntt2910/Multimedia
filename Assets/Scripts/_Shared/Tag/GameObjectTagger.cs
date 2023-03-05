using System.Collections.Generic;
using BW.Inspector;
using UnityEngine;

namespace BW.Taggers
{
    public class GameObjectTagger : MonoBehaviour, ITagger
    {
        [SerializeField, TagList] private string[] _tags;

        public int Id
        {
            get => gameObject.GetInstanceID();
        }

        public IList<string> Tags
        {
            get => this._tags;
        }

        public void AddTag(string tag)
        {
        }

        public void AddTags(IEnumerable<string> tags)
        {
        }

        public void RemoveTag(string tag)
        {
        }

        private readonly HashSet<string> _hashTags = new HashSet<string>();

        private void OnEnable()
        {
            MultiTagManager.Instance.AddMultiTag(this);
        }

        private void OnDisable()
        {
            if (MultiTagManager.Instance != null)
                MultiTagManager.Instance.RemoveMultiTag(this);
        }

        public bool HasTags(IEnumerable<string> tags)
        {
            return this._hashTags.IsSupersetOf(tags);
        }

        public bool HasAnyOfTags(IEnumerable<string> tags)
        {
            return false;
        }

        public bool HasTag(string tag)
        {
            return this._hashTags.Contains(tag);
        }
    }
}