using UnityEngine;

namespace BW.Strings
{
    public class UpperCaseModifier : BaseStringModifier
    {
        [SerializeField] private bool _invariant;

        public override string Processing(string str)
        {
            return this._invariant ? str.ToUpperInvariant() : str.ToUpper();
        }
    }
}