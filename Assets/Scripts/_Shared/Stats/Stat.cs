using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BW.Stats
{
    [Serializable, InlineProperty, HideLabel]
    public class Stat
    {
        [SerializeField] private float _baseValue;

        public float BaseValue
        {
            set
            {
                value = Mathf.Clamp(value, this._minValue, this._maxValue);

                if (Math.Abs(this._baseValue - value) > Mathf.Epsilon)
                {
                    this._baseValue = value;
                    RecalculateValue();
                }
            }
            get { return this._baseValue; }
        }

        [SerializeField, ReadOnly] private float _value;
        [SerializeField] private float _minValue = 0f;
        [SerializeField] private float _maxValue = float.MaxValue;

//        private bool isDirty = true;
//        private float lastBaseValue;

        public float LastValue { private set; get; }
        public float Value => this._value;
        public float ConstraintMin => this._minValue;
        public float ConstraintMax => this._maxValue;

        public IEnumerable<StatModifier> Modifiers => this.attributeModifiers;

        [SerializeField] private List<StatModifier> attributeModifiers;

        //protected readonly List<AttributeModifier> attributeModifiers;
        //public readonly ReadOnlyCollection<AttributeModifier> AttributeModifiers;

        private float _lastValue;
        private readonly List<Action<float>> _listeners;

        public Stat()
        {
            //AttributeModifiers = attributeModifiers.AsReadOnly();
            this.attributeModifiers = new List<StatModifier>();
            this._listeners = new List<Action<float>>();
        }

        public Stat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public Stat(float baseValue, float min, float max) : this(baseValue)
        {
            SetConstraintMin(min);
            SetConstraintMax(max);
        }

        public void SetConstraintMin(float min)
        {
            if (min > this._maxValue)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("Min value " + min + " cannot be greater than Max value  " + this._maxValue);
                return;
            }

            this._minValue = min;
        }

        public void SetConstraintMax(float max)
        {
            if (max < this._minValue)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("Max value " + max + " cannot be smaller than Min value  " + this._minValue);
                return;
            }

            this._maxValue = max;
        }

        public void AddListener(Action<float> callback)
        {
            this._listeners.Add(callback);
            callback(Value);
        }

        public void RemoveListener(Action<float> callback)
        {
            this._listeners.Remove(callback);
        }

        public void ClearAllListeners()
        {
            this._listeners.Clear();
        }

        public virtual StatModifier GetModifier(int index)
        {
            return this.attributeModifiers[index];
        }

        public virtual void AddModifier(StatModifier mod)
        {
            mod.SetStat(this);
            this.attributeModifiers.Add(mod);
            this.attributeModifiers.Sort(CompareModifierOrder);
            RecalculateValue();
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (!this.attributeModifiers.Remove(mod)) return false;
            mod.SetStat(null);
            RecalculateValue();
            return true;
        }

        public bool HasModifier(StatModifier modifier)
        {
            return this.attributeModifiers.Contains(modifier);
        }

        public void ClearModifiers()
        {
            this.attributeModifiers.Clear();
            RecalculateValue();
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            var didRemove = false;

            for (int i = this.attributeModifiers.Count - 1; i >= 0; i--)
            {
                if (this.attributeModifiers[i].Source != source) continue;
                this.attributeModifiers[i].SetStat(null);
                didRemove = true;
                this.attributeModifiers.RemoveAt(i);
            }

            RecalculateValue();
            return didRemove;
        }

        public virtual int CountModifiersFromSource(object source)
        {
            var count = 0;
            for (int i = this.attributeModifiers.Count - 1; i >= 0; i--)
            {
                if (this.attributeModifiers[i].Source == source) count++;
            }

            return count;
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order) return 1;
            return 0; //if (a.Order == b.Order)
        }

        /// <summary>
        /// Follow formula: Value = Sum(Flat) x (1 + Sum(Increase) - Sum(Reduce)) x Product(1 + More) x Product(1 - Less)
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected virtual float CalculateFinalValue(float baseValue)
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < this.attributeModifiers.Count; i++)
            {
                StatModifier mod = this.attributeModifiers[i];

                switch (mod.Type)
                {
                    case StatModType.Flat:
                        finalValue += mod.Value;
                        break;
                    case StatModType.PercentAdd:
                    {
                        sumPercentAdd += mod.Value;

                        if (i + 1 >= this.attributeModifiers.Count || this.attributeModifiers[i + 1].Type != StatModType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }

                        break;
                    }

                    case StatModType.PercentMul:
                        finalValue *= 1 + mod.Value;
                        break;
                }
            }

            // Workaround for float calculation errors, like displaying 12.00002 instead of 12
            return (float) Math.Round(finalValue, 2);
        }

        /// <summary>
        /// Follow formula: Value = Sum(Flat) x (1 + Sum(Increase) - Sum(Reduce)) x Sum(1 + More) x Sum(1 - Less)
        /// </summary>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        protected virtual float CalculateFinalValuePOE(float baseValue)
        {
            var finalValue = baseValue;
            var sumPercentAdd = 0f;
            var sumPercentMulMore = 0f;
            var sumPercentMulLess = 0f;

            for (int i = 0; i < this.attributeModifiers.Count; i++)
            {
                StatModifier mod = this.attributeModifiers[i];

                switch (mod.Type)
                {
                    case StatModType.Flat:
                        finalValue += mod.Value;
                        break;
                    case StatModType.PercentAdd:
                    {
                        sumPercentAdd += mod.Value;
                        break;
                    }

                    case StatModType.PercentMul:

                        // More
                        if (mod.Value >= 0f)
                        {
                            sumPercentMulMore += mod.Value;
                        }
                        // Less
                        else
                        {
                            sumPercentMulLess += mod.Value;
                        }

                        break;
                }
            }

            // Percent Add (Increase, Decrease)
            finalValue *= 1f + sumPercentAdd;

            // Percent Mul (More, Less)
            finalValue *= 1f + sumPercentMulMore;
            finalValue *= 1f + sumPercentMulLess;

            // Workaround for float calculation errors, like displaying 12.00002 instead of 12
            return finalValue;
//            return (float) System.Math.Round(finalValue, 4);
        }

        public void RecalculateValue()
        {
//            lastBaseValue = _baseValue;
            LastValue = this._value;
            float baseValue = this._baseValue;
            this._value = this.attributeModifiers != null ? CalculateFinalValuePOE(baseValue) : baseValue;
            this._value = Mathf.Clamp(this._value, this._minValue, this._maxValue);

            if (Math.Abs(this._value - this._lastValue) > 1e-4)
            {
                this._lastValue = this._value;
                InvokeListeners();
            }
        }

        public float CalculateTemporaryValue(float baseValue)
        {
            float value = CalculateFinalValuePOE(baseValue);
            value = Mathf.Clamp(value, this._minValue, this._maxValue);
            return value;
        }

        private void InvokeListeners()
        {
            foreach (var listener in this._listeners)
            {
                listener?.Invoke(this._value);
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(this._baseValue)}: {this._baseValue}, {nameof(this._value)}: {this._value}, {nameof(this._minValue)}: {this._minValue}, {nameof(this._maxValue)}: {this._maxValue}, {nameof(this.attributeModifiers)} count: {this.attributeModifiers.Count}";
        }
    }
}