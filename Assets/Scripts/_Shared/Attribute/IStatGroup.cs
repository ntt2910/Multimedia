using System;
using System.Collections.Generic;
using BW.Stats;
using BW.Inspector;

public interface IStatGroup
{
    IEnumerable<string> StatNames { get; }

    void AddStat(string statName, float baseValue, float min = 0f, float max = float.MaxValue);
    void AddListener(string statName, Action<float> callback);
    void RemoveListener(string statName, Action<float> callback);

    IStatGroup SetBaseValue(string statName, float value, bool callUpdater = true);
    IStatGroup SetMinValue(string statName, float min);
    IStatGroup SetMaxValue(string statName, float max);
    float GetBaseValue(string statName, float defaultValue = 0f);
    float GetValue(string statName, float defaultValue = 0f);
    float GetLastValue(string statName, float defaultValue = 0f);
    float GetMinConstraint(string statName);
    float GetMaxConstraint(string statName);
    IEnumerable<StatModifier> GetModifiers(string statName);

    void AddModifier(string statName, StatModifier mod, object source);
    void RemoveModifier(string statName, StatModifier mod);
    void RemoveModifiersFromSource(object source);
    void ClearAllModifiers();
    int CountModifierFromSource(string statName, object source);

    bool HasStat(string statName);
    bool HasModifier(string statName, StatModifier modifier);

    void Copy(IStatGroup statGroup, float percentage = 1f);
    void Copy(IStatGroup statGroup, Keyword[] _ignoreKeys, float percentage = 1f);
    void CalculateStats();

#if UNITY_EDITOR
    void OnValidate();
#endif
}