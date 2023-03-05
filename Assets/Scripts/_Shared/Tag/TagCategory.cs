using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.Utilities;

#endif

namespace BW.Taggers
{
    [Serializable]
    public class TagCategory
    {
        [SerializeField, TitleGroup("$_categoryName")]
        private string _categoryName;

        [SerializeField] private string[] _tags;

        public string CategoryName => this._categoryName;

        public IEnumerable<string> GetTags()
        {
            var tags = new string[this._tags.Length];

            for (int i = 0; i < this._tags.Length; i++)
            {
                tags[i] = this._categoryName + "/" + this._tags[i];
            }

            return tags;
        }

        public bool HasTag(string tag)
        {
            foreach (var categoryTag in this._tags)
            {
                if (tag != categoryTag) continue;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public void Sort()
        {
            this._tags.Sort();
        }
#endif
    }
}