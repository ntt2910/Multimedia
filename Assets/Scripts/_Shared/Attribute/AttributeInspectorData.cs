using System;
using BW.Stats;
using BW.Inspector;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BW.Attributes
{
    [Serializable]
    public class AttributeInspectorData
    {
        public static readonly AttributeInspectorData NullAttribute = new AttributeInspectorData();

        [HideInInspector, SerializeField] private string _editorName;


        [SerializeField, ReadOnly, Background(ColorEnum.Red), HideLabel]
        private string _name;

        [SerializeField, Background(ColorEnum.Green)]
        private Stat stat;

        public string Name
        {
            get { return this._name; }
        }

        public Stat Stat
        {
            get { return this.stat; }
        }

        public AttributeInspectorData()
        {
        }

        public AttributeInspectorData(string name, Stat stat)
        {
            this._name = name;
            this._editorName = name;
            this.stat = stat;
        }
    }
}