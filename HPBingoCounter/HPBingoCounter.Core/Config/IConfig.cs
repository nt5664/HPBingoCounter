﻿namespace HPBingoCounter.Core.Config
{
    public interface IConfig
    {
        string? VersionUrl { get; }
        string? GeneratorFile { get; }
        string? GoalsUrl { get; }
        bool UseLocalVersions { get; }
        bool UseLocalGoals { get; }
        bool UseLocalGenerator { get; }
        string? FileName { get; }
    }
}
