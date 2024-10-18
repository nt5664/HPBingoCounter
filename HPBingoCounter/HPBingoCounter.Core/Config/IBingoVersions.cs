namespace HPBingoCounter.Core.Config
{
    public interface IBingoVersions
    {
        string DefaultVersion { get; }
        IDictionary<string, string> VersionDictionary { get; }
        bool IsValid { get; }
    }
}
