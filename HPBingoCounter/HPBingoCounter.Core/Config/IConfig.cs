namespace HPBingoCounter.Core.Config
{
    public interface IConfig
    {
        string? VersionUrl { get; }
        string? GeneratorFile { get; }
        string? GoalsUrl { get; }
        string? ApiFile { get; }
        bool UseLocalVersions { get; }
        bool UseLocalGoals { get; }
        bool UseLocalGenerator { get; }
        bool UseLocalApi { get; }
        string? FileName { get; }
    }
}
