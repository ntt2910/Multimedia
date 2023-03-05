//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;

namespace BW.Inspector
{

	public class KeywordsConfig : ScriptableObject
	{

		public List<KeywordCategory> keyWordCategories = new List<KeywordCategory>();

	}

	[Serializable]
	public class KeywordCategory : System.Object
	{
		public string name;

		[NonSerialized]
		public bool expanded;

		public List<string> keywords = new List<string>();

		public KeywordCategory()
		{
			this.name = "";
		}

		public KeywordCategory(string name)
		{
			this.name = name;
		}
	}

}
