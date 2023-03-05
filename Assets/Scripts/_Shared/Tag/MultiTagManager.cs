using System.Collections.Generic;
using BW.Util;

namespace BW.Taggers
{
    public class MultiTagManager : SingletonMonoBehaviour<MultiTagManager>
    {
        private Dictionary<int, ITagger> _multiTagLookup;

        protected override void Awake()
        {
            base.Awake();
            this._multiTagLookup = new Dictionary<int, ITagger>();
        }

        public void AddMultiTag(ITagger tagger)
        {
            this._multiTagLookup.Add(tagger.Id, tagger);
        }

        public void RemoveMultiTag(ITagger tagger)
        {
            this._multiTagLookup.Remove(tagger.Id);
        }

        public bool HasTag(int id, string tag)
        {
            return this._multiTagLookup.ContainsKey(id) && this._multiTagLookup[id].HasTag(tag);
        }

        public bool HasTags(int id, IEnumerable<string> tags)
        {
            return this._multiTagLookup.ContainsKey(id) && this._multiTagLookup[id].HasTags(tags);
        }
    }
}