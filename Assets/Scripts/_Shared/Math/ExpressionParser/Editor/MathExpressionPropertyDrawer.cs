using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InspectorMathExpressions
{
    [CustomPropertyDrawer(typeof(MathExpressionSerialized), true)]
    public class MathExpressionPropertyDrawer : PropertyDrawer
    {
        static HashSet<string> expandedFields = new HashSet<string>();
        string error;
        List<string> parameters;
        List<string> parameterWarnigns;


        Type ReflectionSearch(SerializedProperty property)
        {
            var path = property.propertyPath;
            while (path.Contains(".Array.data"))
            {
                var i = path.IndexOf(".Array.data");
                path = path.Remove(i, ".Array.data[0]".Length);
            }

            string[] parts = path.Split('.');
            Type currentType = property.serializedObject.targetObject.GetType();
            for (int i = 0; i < parts.Length; i++)
            {
                var field = currentType.GetField(parts[i],
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance);
                if (field == null)
                    return null;
                currentType = field.FieldType;
                if (currentType.IsArray)
                {
                    currentType = currentType.GetElementType();
                }
            }

            return currentType;
        }

        bool TryParseExpression(SerializedProperty prop)
        {
            var type = ReflectionSearch(prop);
            if (type == null)
            {
                return false;
            }

            var expression = prop.FindPropertyRelative("expression").stringValue;
            var instance = Activator.CreateInstance(type);
            type.GetField("expression", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(instance, expression);
            error = type.GetMethod("TryParseExpression").Invoke(instance, null) as string;
            parameters = type.GetProperty("ParameterNames").GetValue(instance, null) as List<string>;
            parameterWarnigns = type.GetProperty("ParameterWarnings").GetValue(instance, null) as List<string>;
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (!expandedFields.Contains(label.text))
                return 18;
            if (TryParseExpression(prop))
            {
                return 36 + 18 + parameters.Count * 12 + (error == null ? 0 : 30);
            }
            else
            {
                return 36;
            }
        }

        public override void OnGUI(Rect position,
            SerializedProperty prop,
            GUIContent label)
        {
            var fPos = position;
            fPos.height = 18;

            bool expandField = EditorGUI.Foldout(fPos, expandedFields.Contains(label.text), label);
            position.y += 18;
            if (expandField)
            {
                expandedFields.Add(label.text);
                Rect textFieldPosition = position;
                textFieldPosition.height = 18;
                var expression = prop.FindPropertyRelative("expression");
                DrawTextField(textFieldPosition, expression);
                if (TryParseExpression(prop))
                {
                    Rect helpPosition = EditorGUI.IndentedRect(position);
                    helpPosition.y += 18;
                    helpPosition.height -= 36;
                    DrawParameters(helpPosition);
                }
            }
            else
            {
                expandedFields.Remove(label.text);
            }
        }

        void DrawTextField(Rect position, SerializedProperty prop)
        {
            EditorGUI.BeginChangeCheck();
            string value = EditorGUI.TextField(position, prop.stringValue);
            if (EditorGUI.EndChangeCheck())
                prop.stringValue = value;
        }


        void DrawParameters(Rect position)
        {
            var message = "Available Parameters:\n";
            foreach (var parameterName in parameters)
            {
                message += parameterName;
                if (parameterWarnigns.Contains(parameterName))
                {
                    message += " (can't use directly)";
                }

                message += "\n";
            }

            if (error != null)
            {
                message += "Error:\n" + error;
            }

            EditorGUI.HelpBox(position, message, error != null ? MessageType.Error : MessageType.Info);
        }
    }
}