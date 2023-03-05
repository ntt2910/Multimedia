using UnityEditor;

namespace InspectorMathExpressions {

	[CustomEditor(typeof(MathExpression), true)]
	public class MathExpressionEditor : UnityEditor.Editor
	{
		string parseError;
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			MathExpression mathExpression = ((MathExpression)target);
			mathExpression.ParseExpression(out parseError);
			
			if (mathExpression.IsValid)
			{
				EditorGUILayout.LabelField("Available Parameters:");
				++EditorGUI.indentLevel;
				foreach (var parameterName in mathExpression.ParameterNames)
				{
					var caption = parameterName;
					if(mathExpression.ParameterWarnings.Contains(parameterName)) {
						caption += " (can't use directly)";
					}
					EditorGUILayout.LabelField(caption);
				}
				--EditorGUI.indentLevel;
			}
			else
			{
				EditorGUILayout.HelpBox("MathExpression type is invalid. Check console for errors.", MessageType.Error);
			}
			/*
		if (GUILayout.Button("Validate MathExpression"))
		{
			mathExpression.ParseExpression(out parseError);
		}*/
			
			if (parseError != null) {
				EditorGUILayout.HelpBox("MathExpression invalid: " + parseError, MessageType.Error);
			}
		}
		
	}

}
