using UnityEngine;

namespace BW.Strings
{
    public class AppendModifier : BaseStringModifier
    {
        [SerializeField] private string _text;

        public override string Processing(string str)
        {
            return str + this._text;
        }
    }
}