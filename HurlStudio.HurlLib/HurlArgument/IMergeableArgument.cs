namespace HurlStudio.HurlLib.HurlArgument
{
    public interface IMergeableArgument<out T>
    {
        IEnumerable<T> GetMergeableValues();
    }
}