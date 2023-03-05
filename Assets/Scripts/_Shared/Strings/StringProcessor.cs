using System;
using UnityEngine;

namespace BW.Strings
{
    [Serializable]
    public class StringProcessor
    {
        [SerializeField] private BaseStringModifier[] _modifiers;

        public string Process(string str)
        {
            foreach (var modifier in this._modifiers)
            {
                if (modifier != null)
                    str = modifier.Processing(str);
            }

            return str;
        }
    }
}