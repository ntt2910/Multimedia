namespace BW.Strings
{
    public class TitleCaseModifier : BaseStringModifier
    {
        public override string Processing(string str)
        {
            return str.ToTitleCase();
        }
    }
}