using System;
using System.Collections.Generic;
using System.Linq;
using BW.Stats;
using BW.Inspector;
using UnityEngine;

namespace BW.Attributes
{
    [Serializable]
    public class StatGroup : IStatGroup
    {
#if UNITY_EDITOR
        [SerializeField] private List<AttributeInspectorData> _attributeDebugger;

        public void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (this._attributeDebugger == null || this._attrDict == null) return;
            foreach (var data in this._attributeDebugger)
            {
                data.Stat.RecalculateValue();
            }
        }
#endif

        private readonly Dictionary<string, Stat> _attrDict;

        public int TotalStat => this._attrDict.Count;

        public IEnumerable<string> StatNames => this._attrDict.Keys;

        public StatGroup()
        {
            this._attrDict = new Dictionary<string, Stat>();
        }

        public IStatGroup SetMinValue(string name, float min)
        {
            if (this._attrDict.ContainsKey(name)) this._attrDict[name].SetConstraintMin(min);
            return this;
        }

        public IStatGroup SetMaxValue(string name, float max)
        {
            if (this._attrDict.ContainsKey(name)) this._attrDict[name].SetConstraintMax(max);
            return this;
        }

        public void AddStat(string name, float baseValue, float min = float.MinValue, float max = float.MaxValue)
        {
            if (this._attrDict.ContainsKey(name))
            {
//                Debug.LogError("Duplicated attribute " + name);
            }
            else
            {
                Stat stat = new Stat {BaseValue = baseValue};
                stat.SetConstraintMin(min);
                stat.SetConstraintMax(max);

                this._attrDict.Add(name, stat);

#if UNITY_EDITOR
                if (this._attributeDebugger == null) this._attributeDebugger = new List<AttributeInspectorData>();

                var data = new AttributeInspectorData(name, stat);
                this._attributeDebugger.Add(data);
#endif
            }
        }

        public Stat CreateStat(string name, float baseValue, float min = float.MinValue, float max = float.MaxValue)
        {
            if (this._attrDict.ContainsKey(name)) return null;
            Stat stat = new Stat {BaseValue = baseValue};
            stat.SetConstraintMin(min);
            stat.SetConstraintMax(max);

            this._attrDict.Add(name, stat);

#if UNITY_EDITOR
            if (this._attributeDebugger == null) this._attributeDebugger = new List<AttributeInspectorData>();
            var data = new AttributeInspectorData(name, stat);
            this._attributeDebugger.Add(data);
#endif
            return stat;
        }

        public void AddListener(string name, Action<float> callback)
        {
            if (!this._attrDict.ContainsKey(name)) return;
            Stat stat = this._attrDict[name];
            stat.AddListener(callback);
        }

        public void RemoveListener(string name, Action<float> callback)
        {
            if (!this._attrDict.ContainsKey(name)) return;
            this._attrDict[name].RemoveListener(callback);
        }

        public bool HasStat(string name)
        {
            return this._attrDict.ContainsKey(name);
        }

        public bool HasModifier(string statName, StatModifier modifier)
        {
            if (!HasStat(statName)) return false;
            return this._attrDict[statName].HasModifier(modifier);
        }

        public float GetTemporaryValue(string statName, float baseValue)
        {
            return !this._attrDict.ContainsKey(statName)
                ? baseValue
                : this._attrDict[statName].CalculateTemporaryValue(baseValue);
        }

        /// <summary>
        /// Set base value for attribute. Must set callUpdater=false when called in UpdateAttributes method of updaters to prevent stack overflow.
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Base value</param>
        /// <param name="callUpdater">Decide if attribute updaters are notified by this change</param>
        public IStatGroup SetBaseValue(string name, float value, bool callUpdater = true)
        {
            if (this._attrDict != null && this._attrDict.ContainsKey(name))
            {
                this._attrDict[name].BaseValue = value;
            }
            else
            {
                //Debug.Log(gameObject.name + " " + name + " is not in attribute dictionary");
            }

            return this;
        }

        public float GetBaseValue(string name, float defaultValue = 0f)
        {
            if (this._attrDict == null) return 0f;

            if (this._attrDict.ContainsKey(name)) return this._attrDict[name].BaseValue;

//            Debug.LogError(name + " is not in attribute dictionary");
            return defaultValue;
        }

        public float GetValue(string name, float defaultValue = 0f)
        {
            if (this._attrDict == null) return defaultValue;

            return this._attrDict.ContainsKey(name) ? this._attrDict[name].Value : defaultValue;
        }

        public float GetLastValue(string statName, float defaultValue = 0f)
        {
            if (this._attrDict == null) return defaultValue;

            return this._attrDict.ContainsKey(statName) ? this._attrDict[statName].LastValue : defaultValue;
        }

        public IEnumerable<StatModifier> GetModifiers(string statName)
        {
            if (this._attrDict == null || !this._attrDict.ContainsKey(statName)) return Enumerable.Empty<StatModifier>();

            return this._attrDict[statName].Modifiers;
        }

        public float GetMinConstraint(string name)
        {
            if (this._attrDict == null) return 0f;

            return this._attrDict.ContainsKey(name) ? this._attrDict[name].ConstraintMin : 0f;
        }

        public float GetMaxConstraint(string name)
        {
            if (this._attrDict == null) return float.MaxValue;

            return this._attrDict.ContainsKey(name) ? this._attrDict[name].ConstraintMax : float.MaxValue;
        }

        public void AddModifier(string statName, StatModifier mod, object source)
        {
            if (string.IsNullOrEmpty(statName) || !this._attrDict.ContainsKey(statName))
            {
                //Debug.Log(gameObject.name + " Attribute key does not exist: " + name);
            }
            else
            {
                Stat attr = this._attrDict[statName];
                mod.Source = source;
                attr.AddModifier(mod);
            }
        }

        public void RemoveModifier(string name, StatModifier mod)
        {
            if (this._attrDict.ContainsKey(name))
            {
                Stat attr = this._attrDict[name];
                attr.RemoveModifier(mod);
            }

//            else
//            {
            //Debug.Log(gameObject.name + " Attribute key does not exist: " + name);
//            }
        }

        public void RemoveModifiersFromSource(object source)
        {
            foreach (var key in this._attrDict.Keys)
            {
                this._attrDict[key].RemoveAllModifiersFromSource(source);
            }
        }

        public void ClearAllModifiers()
        {
            foreach (var stat in this._attrDict.Values)
            {
                stat.ClearModifiers();
            }
        }

        public int CountModifierFromSource(string name, object source)
        {
            return !this._attrDict.ContainsKey(name) ? 0 : this._attrDict[name].CountModifiersFromSource(source);
        }

        public void Copy(IStatGroup statGroup, float percentage = 1f)
        {
            foreach (var attributeName in statGroup.StatNames)
            {
                if (HasStat(attributeName))
                {
                    SetBaseValue(attributeName, statGroup.GetBaseValue(attributeName) * percentage);
                }
                else
                {
                    AddStat(attributeName, statGroup.GetBaseValue(attributeName),
                        statGroup.GetMinConstraint(attributeName),
                        statGroup.GetMaxConstraint(attributeName));
                }

                foreach (var mod in statGroup.GetModifiers(attributeName))
                {
                    AddModifier(attributeName, mod, mod.Source);
                }
            }
        }

        public void Copy(IStatGroup statGroup, Keyword[] _ignoreKeys, float percentage = 1f)
        {
            foreach (var attributeName in statGroup.StatNames)
            {
                var ignore = false;

                foreach (var key in _ignoreKeys)
                {
                    if (attributeName != key) continue;
                    ignore = true;
                    break;
                }

                if (HasStat(attributeName) && !ignore)
                {
                    SetBaseValue(attributeName, statGroup.GetBaseValue(attributeName) * percentage);
                }
            }
        }

        public void CalculateStats()
        {
            foreach (var stat in this._attrDict.Values)
            {
                stat.RecalculateValue();
            }
        }

        public Dictionary<string, float> ConvertToDictionary()
        {
            var dict = new Dictionary<string, float>();
            foreach (var pair in this._attrDict)
            {
                dict.Add(pair.Key, pair.Value.Value);
            }

            return dict;
        }

        public override string ToString()
        {
            return $"StatGroup: {nameof(TotalStat)}: {TotalStat}";
        }
    }
}