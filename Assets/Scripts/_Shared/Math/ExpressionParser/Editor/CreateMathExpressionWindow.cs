using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using BW.Utils;
using UnityEditor;
using UnityEngine;

namespace InspectorMathExpressions
{
    public class CreateMathExpressionWindow : EditorWindow
    {
        [MenuItem("Window/Inspector Math Expressions/Create expression template")]
        static void CreateMathExpression()
        {
            var w = GetWindow<CreateMathExpressionWindow>();
            w.name = "Create expression template";
            w.path = AssetDatabaseUtils.GetSelectionObjectPath();
            w.Show();
        }

        string caption;
        MathExpressionType type;
        ParamType returnType;
        List<Param> parameters = new List<Param>();
        string path;

        enum MathExpressionType
        {
            @ScriptableObject,
            Field,
        }


        enum ParamType
        {
            @float,
            @int,
            @double,
        }

        class Param
        {
            public ParamType type;
            public string name;
        }

        List<string> Check()
        {
            var errors = new List<string>();
            if (!CodeGenerator.IsValidLanguageIndependentIdentifier(caption))
            {
                errors.Add("Invalid class name");
            }

            if (parameters.Count == 0)
            {
                errors.Add("Need at least one parameter\n");
            }

            var index = 0;
            foreach (var p in parameters)
            {
                index++;
                if (!CodeGenerator.IsValidLanguageIndependentIdentifier(p.name))
                {
                    errors.Add("Parameter " + index + ": invalid name - " + p.name);
                }
            }

            return errors;
        }

        string fieldTemplate
        {
            get
            {
                return
                    "using InspectorMathExpressions;\n\n[System.Serializable]\npublic class #name# : MathExpressionSerialized<#name#>\n" +
                    "{\n\t[MathExpressionEvaluator]\n\t" +
                    "public #return# Get(#params#) {\n\t\t return (#return#)EvaluateMathExpression(#paramsPass#);\n\t}\n}";
            }
        }

        string objectTemplate
        {
            get
            {
                return "using InspectorMathExpressions;\n\npublic class #name# : MathExpression<#name#>\n" +
                       "{\n\t[MathExpressionEvaluator]\n\t" +
                       "public #return# Get(#params#) {\n\t\t return (#return#)EvaluateMathExpression(#paramsPass#);\n\t}\n" +
                       "#if UNITY_EDITOR\n\tpublic static partial class ScriptableObjectCreators\n\t" +
                       "{\n\t\t[UnityEditor.MenuItem(\"Assets/Create/Math expression/#name#\")]" +
                       "\n\t\tpublic static void Create#name#() { ScriptableObjectUtility.CreateAsset<#name#>(); }" +
                       "\n\t}\n#endif\n}";
            }
        }

        string paramString(bool pass)
        {
            var str = "";
            foreach (var p in parameters)
            {
                str += (pass ? "" : (p.type.ToString() + " ")) + p.name + ", ";
            }

            str = str.TrimEnd(',', ' ');
            return str;
        }

        void Create()
        {
            var template = type == MathExpressionType.ScriptableObject ? objectTemplate : fieldTemplate;
            template = template.Replace("#name#", caption);
            template = template.Replace("#params#", paramString(false));
            template = template.Replace("#paramsPass#", paramString(true));
            template = template.Replace("#return#", returnType.ToString());
            var absolutePath = Application.dataPath;
            absolutePath = absolutePath.Remove(absolutePath.Length - 6);
            File.WriteAllText(Path.Combine(absolutePath, Path.Combine(path, caption) + ".cs"), template);
            AssetDatabase.ImportAsset(Path.Combine(path, caption) + ".cs");
            AssetDatabase.Refresh();
        }

        void OnSelectionChange()
        {
            path = AssetDatabaseUtils.GetSelectionObjectPath();
            Repaint();
        }

        void OnGUI()
        {
            caption = EditorGUILayout.TextField("Name", caption);
            type = (MathExpressionType) EditorGUILayout.EnumPopup("Type", type);
            Param toRemove = null;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Parameters:");
            for (int i = 0; i < parameters.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var p = parameters[i];
                p.type = (ParamType) EditorGUILayout.EnumPopup(p.type, GUILayout.Width(80));
                p.name = EditorGUILayout.TextField(p.name);
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    toRemove = p;
                }

                EditorGUILayout.EndHorizontal();
            }

            parameters.Remove(toRemove);
            if (GUILayout.Button("Add parameter"))
            {
                parameters.Add(new Param {name = "p" + parameters.Count});
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Return type:");
            returnType = (ParamType) EditorGUILayout.EnumPopup(returnType);
            EditorGUILayout.EndHorizontal();
            var errors = Check();
            if (errors.Count == 0)
            {
                EditorGUILayout.LabelField("Will be saved at Project->Selection path: " + Path.Combine(path, caption) +
                                           ".cs");
                if (GUILayout.Button("Create"))
                {
                    Create();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Fix errors before creation:");
                foreach (var e in errors)
                {
                    EditorGUILayout.LabelField(e);
                }
            }
        }
    }
}