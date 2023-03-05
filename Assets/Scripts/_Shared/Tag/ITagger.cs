using System.Collections.Generic;

namespace BW.Taggers
{
    public interface ITagger
    {
        int Id { get; }
        IList<string> Tags { get; }
        void AddTag(string tag);
        void AddTags(IEnumerable<string> tags);
        void RemoveTag(string tag);
        bool HasTag(string tag);
        bool HasTags(IEnumerable<string> tags);
        bool HasAnyOfTags(IEnumerable<string> tags);
    }
}