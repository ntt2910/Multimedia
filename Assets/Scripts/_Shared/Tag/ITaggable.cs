namespace BW.Taggers
{
    public interface ITaggable
    {
        ITagger Tagger { get; }
    }
}