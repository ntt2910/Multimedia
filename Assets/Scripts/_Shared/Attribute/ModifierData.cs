using System;
using BW.Stats;
using BW.Inspector;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BW.Attributes
{
    [Serializable]
    public class ModifierData
    {
        [SerializeField, Tag] private string attributeName;

        [SerializeField, InlineProperty, HideLabel]
        private StatModifier modifier;

        public string AttributeName => this.attributeName;
        public StatModifier Modifier => this.modifier;
    }
}