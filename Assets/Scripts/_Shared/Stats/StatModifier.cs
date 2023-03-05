using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BW.Stats
{
    [Serializable]
    public class StatModifier
    {
        [SerializeField, HideInInspector] private float value;

        [SerializeField] private StatModType type;

        [ShowInInspector]
        public float Value
        {
            set
            {
                this.value = value;
                this.stat?.RecalculateValue();
            }
            get => this.value;
        }

        public StatModType Type => this.type;

        [HideInInspector] public int Order;
        [HideInInspector] public object Source;

        [NonSerialized] private Stat stat;

        public StatModifier(float value, StatModType type, int order)
        {
            Value = value;
            this.type = type;
            this.Order = order;
        }

        public StatModifier(float value, StatModType type, int order, object source) : this(value, type, order)
        {
            this.Source = source;
        }

        public StatModifier(float value, StatModType type, object source) : this(value, type, (int) type, source)
        {
        }

        public void SetStat(Stat stat)
        {
            this.stat = stat;
        }

        public static StatModType StringToType(string modType)
        {
            switch (modType)
            {
                case "Flat": return StatModType.Flat;
                case "PercentAdd": return StatModType.PercentAdd;
                case "PercentMult": return StatModType.PercentMul;
            }

            return StatModType.Flat;
        }

        public override string ToString()
        {
            return
                $"{nameof(this.value)}: {this.value}, {nameof(this.type)}: {this.type}, {nameof(this.Order)}: {this.Order}, {nameof(this.Source)}: {this.Source}, {nameof(this.stat)}: {this.stat}";
        }

        public StatModifier Clone()
        {
            return new StatModifier(this.value, this.type, this.Order, this.Source);
        }
    }
}