//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace BW.Inspector
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagDrawer : IllogikaDrawer
    {
        TagAttribute tagAttribute
        {
            get { return ((TagAttribute) attribute); }
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label);
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            if (prop.propertyType != SerializedPropertyType.String)
            {
                Debug.LogError("The TagAttribute is designed to be applied to strings only");
                WrongVariableTypeWarning("Tag", "strings");
                return;
            }

            int originalIndentLevel = EditorGUI.indentLevel;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel = 0;

            if (string.IsNullOrEmpty(prop.stringValue))
                prop.stringValue = "Untagged";

            if (prop.hasMultipleDifferentValues)
            {
                EditorGUI.BeginChangeCheck();

                string temp = EditorGUI.TagField(position, "-");

                if (EditorGUI.EndChangeCheck())
                    prop.stringValue = temp;
            }
            else
            {
                string[] splits = prop.stringValue.Split('/');

                if (splits.Length == 2)
                {
                    prop.stringValue = splits[1];
                }
                else
                {
                    prop.stringValue = splits[0];
                }

                prop.stringValue = EditorGUI.TagField(position, prop.stringValue);
            }

            EditorGUI.indentLevel = originalIndentLevel;
            EditorGUI.EndProperty();
        }
    }
}