//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;

namespace BW.Inspector
{

	public class HideVariableAttribute : PropertyAttribute {

		/// <summary>
		/// Works like HideInInspector but doesn't prevent DecoratorDrawers from being displayed
		/// </summary>
		public HideVariableAttribute()
		{
			
		}
	}
	
}
	