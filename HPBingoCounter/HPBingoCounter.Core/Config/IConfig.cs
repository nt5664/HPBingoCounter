namespace HPBingoCounter.Core.Config
{
    public interface IConfig
    {
        string? VersionUrl { get; }
        string? GeneratorFile { get; }
        string? GoalsUrl { get; }
        string? FilePath { get; }
    }
}
