using System.Linq;

namespace BW.Strings
{
    public class FirstCharUpperModifier : BaseStringModifier
    {
        public override string Processing(string input)
        {
            string lowerInvariant = input.ToLowerInvariant();
            return $"{lowerInvariant.First().ToString().ToUpper()}{lowerInvariant.Substring(1)}";
        }
    }
}