//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;

namespace BW.Inspector
{

	public class SceneSelectionAttributeAttribute : PropertyAttribute {

		public string BasePath
		{
			get;
			private set;
		}

		public SceneSelectionAttributeAttribute(string basePath)
		{
			BasePath = basePath;
		}
	}
	
}
	