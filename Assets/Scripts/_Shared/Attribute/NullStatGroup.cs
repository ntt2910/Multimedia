using System;
using System.Collections.Generic;
using System.Linq;
using BW.Stats;
using BW.Inspector;

public class NullStatGroup : IStatGroup
{
    public NullStatGroup()
    {
    }

    public IEnumerable<string> StatNames
    {
        get { return Enumerable.Empty<string>(); }
    }

    public void AddStat(string name, float baseValue, float min = 0, float max = float.MaxValue)
    {
    }

    public void AddListener(string name, Action<float> callback)
    {
    }

    public void AddModifier(string name, StatModifier mod, object source)
    {
    }

    public bool HasModifier(string statName, StatModifier modifier)
    {
        return false;
    }

    public void Copy(IStatGroup statGroup, float percentage = 1)
    {
    }

    public void Copy(IStatGroup statGroup, Keyword[] _ignoreKeys, float percentage = 1)
    {
    }

    public void CalculateStats()
    {
    }

    public void ClearAllModifiers()
    {
    }

    public int CountModifierFromSource(string name, object source)
    {
        return 0;
    }

    public float GetBaseValue(string name, float defaultValue = 0)
    {
        return defaultValue;
    }

    public float GetValue(string name, float defaultValue = 0)
    {
        return defaultValue;
    }

    public float GetLastValue(string statName, float defaultValue = 0)
    {
        return 0f;
    }

    public float GetMaxConstraint(string name)
    {
        return float.MaxValue;
    }

    public float GetMinConstraint(string name)
    {
        return 0f;
    }

    public IEnumerable<StatModifier> GetModifiers(string name)
    {
        return Enumerable.Empty<StatModifier>();
    }

    public bool HasStat(string name)
    {
        return false;
    }

    public void OnValidate()
    {
    }

    public void RemoveListener(string name, Action<float> callback)
    {
    }

    public void RemoveModifier(string name, StatModifier mod)
    {
    }

    public void RemoveModifiersFromSource(object source)
    {
    }

    public IStatGroup SetMaxValue(string name, float max)
    {
        return this;
    }

    public IStatGroup SetMinValue(string name, float min)
    {
        return this;
    }

    public IStatGroup SetBaseValue(string name, float value, bool callUpdater = true)
    {
        return this;
    }
}