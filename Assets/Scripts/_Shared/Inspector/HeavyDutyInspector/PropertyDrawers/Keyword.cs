﻿using UnityEngine;
using System.Linq;

namespace BW.Inspector
{

	[System.Serializable]
	public class Keyword : System.Object
	{

		[SerializeField]
		[HideInInspector]
		protected string _key;

		public Keyword()
		{
			this._key = "";
		}

		protected Keyword(string key)
		{
			this._key = key;
		}

		public static implicit operator string(Keyword word)
		{
			return word._key.Split('/').Last();
		}

		public static implicit operator Keyword(string key)
		{
			return new Keyword(key);
		}

		public string category
		{
			get
			{
				if (this._key.LastIndexOf('/') < 0)
					return "";
				else
					return this._key.Substring(0, this._key.LastIndexOf('/'));
			}
		}

		public string fullPath
		{
			get { return this._key; }
		}
	}

	public class Keywords : System.Object
	{

		private static KeywordsConfig _config;
		public static KeywordsConfig Config
		{
			get
			{
				if (_config == null)
				{
					_config = Resources.Load("Config/KeywordsConfig") as KeywordsConfig;
				}
				return _config;
			}
		}
	}

}
