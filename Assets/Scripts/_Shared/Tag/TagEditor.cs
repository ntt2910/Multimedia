using UnityEditor;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace BW.Taggers
{
    public static class TagEditor
    {
        private static readonly string TagAssetPath = "ProjectSettings/TagManager.asset";

        public static bool AddTag(string tag)
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath(TagAssetPath);

            if ((asset == null) || (asset.Length <= 0)) return false;

            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    return false; // Tag already present, nothing to do.
                }
            }

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
            return true;
        }

        public static bool AddTags(IEnumerable<string> tags)
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath(TagAssetPath);

            if ((asset == null) || (asset.Length <= 0)) return false;

            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty editorTags = so.FindProperty("tags");

            foreach (var tag in tags)
            {
                for (int i = 0; i < editorTags.arraySize; ++i)
                {
                    if (editorTags.GetArrayElementAtIndex(i).stringValue == tag)
                    {
                        continue; // Tag already present, nothing to do.
                    }

                    editorTags.InsertArrayElementAtIndex(editorTags.arraySize);
                    editorTags.GetArrayElementAtIndex(editorTags.arraySize - 1).stringValue = tag;
                }
            }

            so.ApplyModifiedProperties();
            so.Update();
            return true;
        }

        public static void ClearTags()
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath(TagAssetPath);

            if ((asset == null) || (asset.Length <= 0)) return;

            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            tags.ClearArray();
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
}
#endif