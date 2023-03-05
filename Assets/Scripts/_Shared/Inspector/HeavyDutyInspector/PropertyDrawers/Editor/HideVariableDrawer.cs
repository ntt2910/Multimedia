//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;

namespace BW.Inspector
{

	[CustomPropertyDrawer(typeof(HideVariableAttribute))]
	public class HideVariableDrawer : IllogikaDrawer {
			
		HideVariableAttribute hideVariableAttribute { get { return ((HideVariableAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return 0;
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			// Do nothing
		}
	}

}
