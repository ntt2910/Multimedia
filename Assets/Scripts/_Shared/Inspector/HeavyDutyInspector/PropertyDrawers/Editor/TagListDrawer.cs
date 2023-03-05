//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------


using System.Collections;
using UnityEngine;
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEditor;

namespace BW.Inspector
{
    [CustomPropertyDrawer(typeof(TagListAttribute))]
    public class TagListDrawer : IllogikaDrawer
    {
        TagListAttribute tagListAttribute
        {
            get { return ((TagListAttribute) attribute); }
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (prop.serializedObject.targetObjects.Length > 1)
            {
                if (int.Parse(prop.propertyPath.Split('[')[1].Split(']')[0]) != 0)
                    return -2.0f;
                else
                    return base.GetPropertyHeight(prop, label) * 2;
            }

            return base.GetPropertyHeight(prop, label);
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            int index = int.Parse(prop.propertyPath.Split('[')[1].Split(']')[0]);

            IList list = null;
            try
            {
                list = (prop.serializedObject.targetObject as MonoBehaviour).GetType().GetField(prop.propertyPath.Split('.')[0])
                    .GetValue(prop.serializedObject.targetObject) as IList;
            }
            catch
            {
                try
                {
                    var obj = (prop.serializedObject.targetObject as ScriptableObject).GetType().GetField(prop.propertyPath.Split('.')[0])
                                                                                        .GetValue(prop.serializedObject.targetObject);
                    if (obj is IList iL) 
                        list = iL;
                }
                catch
                {
                    Debug.LogWarning(string.Format("The script has no property named {0} or {0} is not an IList", prop.propertyPath.Split('.')[0]));
                }
            }

            if (prop.serializedObject.targetObjects.Length > 1)
            {
                if (index == 0)
                {
                    position.height = base.GetPropertyHeight(prop, label) * 2;
                    EditorGUI.indentLevel = 1;
                    position = EditorGUI.IndentedRect(position);
                    EditorGUI.HelpBox(position, "Multi object editing is not supported.", MessageType.Warning);
                }

                return;
            }

            int originalIndentLevel = EditorGUI.indentLevel;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel = 0;

            if (tagListAttribute.canDeleteFirstElement || index != 0)
                position.width -= 18;

            if (string.IsNullOrEmpty(prop.stringValue))
                prop.stringValue = "Untagged";

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

            position.x += position.width;
            position.width = 16;

            if ((tagListAttribute.canDeleteFirstElement || index != 0) && GUI.Button(position, "", "OL Minus"))
            {
                list.RemoveAt(index);
            }


            EditorGUI.indentLevel = originalIndentLevel;
            EditorGUI.EndProperty();
        }
    }
}
#endif