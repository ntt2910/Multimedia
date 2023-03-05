using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using BW.Inspector;

namespace BW.Taggers
{
    [CreateAssetMenu(menuName = "Tag Database", fileName = "ScriptableObjects/TagDatabase")]
    public class TagDatabase : ScriptableObject
    {
        [SerializeField, InlineProperty] private TagCategory[] _tagCategories;

#if UNITY_EDITOR

        [Sirenix.OdinInspector.Button("Build Tags")]
        private void SaveTags()
        {
            var tags = new List<string>();

            foreach (var category in this._tagCategories)
            {
                category.Sort();
                tags.AddRange(category.GetTags());
            }

            TagEditor.ClearTags();

            foreach (var tag in tags)
            {
                TagEditor.AddTag(tag);
            }

            TagsLayersScenesBuilder.RebuildTags();
        }
#endif
    }
}