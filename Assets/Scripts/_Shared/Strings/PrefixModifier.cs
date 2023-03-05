using UnityEngine;

namespace BW.Strings
{
    public class PrefixModifier : BaseStringModifier
    {
        [SerializeField] private string _prefix;

        public override string Processing(string str)
        {
            return this._prefix + str;
        }
    }
}