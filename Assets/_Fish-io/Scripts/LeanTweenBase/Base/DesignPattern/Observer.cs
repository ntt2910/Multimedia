﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityBase.DesignPattern;

namespace LTABase.DesignPattern
{
	public class Observer : Singleton<Observer>
	{
		public delegate void CallBackObserver(object data);

		Dictionary<string, HashSet<CallBackObserver>> dictObserver = new Dictionary<string, HashSet<CallBackObserver>>();
		// Use this for initialization
		public void AddObserver(string topicName, CallBackObserver callbackObserver)
		{
			HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
			listObserver.Add(callbackObserver);
		}

		public void RemoveObserver(string topicName, CallBackObserver callbackObserver)
		{
			HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
			listObserver.Remove(callbackObserver);
		}

		public void Notify<OData>(string topicName, OData Data)
		{
			HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
			foreach (CallBackObserver observer in listObserver)
			{
				observer(Data);
			}
		}

		public void Notify(string topicName)
		{
			HashSet<CallBackObserver> listObserver = CreateListObserverForTopic(topicName);
			foreach (CallBackObserver observer in listObserver)
			{
				observer(null);
			}
		}

		protected HashSet<CallBackObserver> CreateListObserverForTopic(string topicName)
		{
			if (!dictObserver.ContainsKey(topicName))
				dictObserver.Add(topicName, new HashSet<CallBackObserver>());
			return dictObserver[topicName];
		}
	}
}
