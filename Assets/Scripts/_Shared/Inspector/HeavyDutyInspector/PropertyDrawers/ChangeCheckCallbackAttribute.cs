//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;

namespace BW.Inspector
{

	public class ChangeCheckCallbackAttribute : PropertyAttribute {

		public string callback
		{
			get;
			private set;
		}

		/// <summary>
		/// Calls a function in your script when the value of the variable changes.
		/// </summary>
		/// <param name="callbackName">The name of the function to call.</param>
		public ChangeCheckCallbackAttribute(string callbackName)
		{
			callback = callbackName;
		}
	}

}
