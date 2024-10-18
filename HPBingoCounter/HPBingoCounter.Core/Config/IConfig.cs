namespace HPBingoCounter.Core.Config
{
    public interface IConfig
    {
        string? VersionUrl { get; }
        string? GeneratorFuncUrl { get; }
        string? GeneratorUrl { get; }
        string? GoalsUrl { get; }
    }
}
