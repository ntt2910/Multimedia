using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class _2dxFX_Shiny_Reflect2 : _2dxFX_Shiny_Reflect {

	protected override Texture2D GetMainTexture ()
	{
		return Resources.Load ("_2dxFX_Gradient2") as Texture2D;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_Shiny_Reflect2)),CanEditMultipleObjects]
public class _2dxFX_Shiny_Reflect2_Editor : _2dxFX_Shiny_Reflect_Editor
{
	private SerializedObject m_object;

	public void OnEnable()
	{

		m_object = new SerializedObject(targets);
	}

	public override void OnInspectorGUI()
	{
		m_object.Update();
		DrawDefaultInspector();

		_2dxFX_Shiny_Reflect2 _2dxScript = (_2dxFX_Shiny_Reflect2)target;

		Texture2D icon = Resources.Load ("2dxfxinspector-anim") as Texture2D;
		if (icon)
		{
			Rect r;
			float ih=icon.height;
			float iw=icon.width;
			float result=ih/iw;
			float w=Screen.width;
			result=result*w;
			r = GUILayoutUtility.GetRect(ih, result);
			EditorGUI.DrawTextureTransparent(r,icon);
		}

		EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));

		if (_2dxScript.ForceMaterial == null)
		{
			_2dxScript.ActiveChange = true;
		}
		else
		{
			if(GUILayout.Button("Remove Shared Material"))
			{
				_2dxScript.ForceMaterial= null;
				_2dxScript.ShaderChange = 1;
				_2dxScript.ActiveChange = true;
				_2dxScript.CallUpdate();
			}

			EditorGUILayout.PropertyField (m_object.FindProperty ("ActiveChange"), new GUIContent ("Change Material Property", "Change The Material Property"));
		}

		if (_2dxScript.ActiveChange)
		{

			EditorGUILayout.BeginVertical("Box");

			Texture2D icone = Resources.Load ("2dxfx-icon-color") as Texture2D;
			EditorGUILayout.PropertyField (m_object.FindProperty ("UseShinyCurve"), new GUIContent ("Use Shiny Curve", "Change The Material Property"));

			if (_2dxScript.UseShinyCurve)
			{
				EditorGUILayout.PropertyField(m_object.FindProperty("ShinyLightCurve"), new GUIContent("Shiny Light Curve", icone, "Use Curve"));		
				icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
				EditorGUILayout.PropertyField(m_object.FindProperty("AnimationSpeedReduction"), new GUIContent("Animation Speed Reduction", icone, "Change the speed of the animation based on the curve timeline"));
			} 
			else
			{
				EditorGUILayout.PropertyField(m_object.FindProperty("Light"), new GUIContent("Shiny Light", icone, "Position of the Shine Light!"));
			}

			icone = Resources.Load ("2dxfx-icon-color") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("LightSize"), new GUIContent("Shiny Light Size", icone, "Size of the Shine Light!"));
			EditorGUILayout.PropertyField(m_object.FindProperty("Intensity"), new GUIContent("Light Intensity", icone, "Intensity of the light"));
			EditorGUILayout.PropertyField(m_object.FindProperty("OnlyLight"), new GUIContent("Only Show Light", icone, "the value between the sprite and no sprite to show only the light"));
			EditorGUILayout.PropertyField(m_object.FindProperty("LightBump"), new GUIContent("Light Bump Intensity", icone, "the intensity of the light bump"));

			EditorGUILayout.BeginVertical("Box");

			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));


			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();

		}

		m_object.ApplyModifiedProperties();

	}
}
#endif
