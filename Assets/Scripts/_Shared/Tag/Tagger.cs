using System;
using System.Collections.Generic;
using BW.Inspector;
using Sirenix.OdinInspector;
using UniLinq;
using UnityEngine;

namespace BW.Taggers
{
    [Serializable, InlineProperty, HideLabel]
    public class Tagger : ITagger, ISerializationCallbackReceiver
    {
        [SerializeField, TagList] private List<string> _tags;

        public int Id { get; }

        public IList<string> Tags
        {
            get => this._tags;
        }

        private HashSet<string> _hashTags;

        public Tagger()
        {
            this._tags = Enumerable.Empty<string>().ToList();
            this._hashTags = new HashSet<string>();

            foreach (var tag in this._tags)
            {
                this._hashTags.Add(tag);
            }
        }

        public Tagger(IEnumerable<string> tags)
        {
            this._tags = tags.ToList();
            this._hashTags = new HashSet<string>();

            foreach (var tag in this._tags)
            {
                this._hashTags.Add(tag);
            }
        }

        public void AddTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
        }

        public void AddTag(string tag)
        {
            if (this._hashTags.Contains(tag)) return;
            this._tags.Add(tag);
            this._hashTags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            if (!this._hashTags.Contains(tag)) return;
            this._tags.Remove(tag);
            this._hashTags.Remove(tag);
        }

        public bool HasTags(IEnumerable<string> tags)
        {
            return this._hashTags.IsSupersetOf(tags);
        }

        public bool HasAnyOfTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (this._hashTags.Contains(tag)) return true;
            }

            return false;
        }

        public bool HasTag(string tag)
        {
            return this._hashTags.Contains(tag);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (this._tags == null) return;

            if (this._hashTags == null) this._hashTags = new HashSet<string>();

            foreach (var tag in this._tags)
            {
                this._hashTags.Add(tag);
            }
        }
    }
}