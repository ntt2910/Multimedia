//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using System.Linq;

namespace BW.Inspector
{

	[System.Serializable]
	public class Scene : System.Object
	{

		[SerializeField]
		[HideInInspector]
		private string _name;

		public Scene()
		{
			this._name = "";
		}

		private Scene(string name)
		{
			this._name = name;
		}

		public static implicit operator string(Scene scene)
		{
			return scene._name.Split('/').Last().Replace(".unity", "");
		}

		public static implicit operator Scene(string path)
		{
			return new Scene(path);
		}

		public string fullPath
		{
			get { return this._name; }
		}
	}

}
