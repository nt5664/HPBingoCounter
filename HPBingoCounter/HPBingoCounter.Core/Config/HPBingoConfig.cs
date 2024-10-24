using Newtonsoft.Json;

namespace HPBingoCounter.Core.Config
{
    public sealed class HPBingoConfig : IConfig
    {
        [JsonProperty("versions")]
        public string? VersionUrl { get; set; }

        [JsonProperty("generatorFile")]
        public string? GeneratorFile { get; set; }

        [JsonProperty("goalsUrl")]
        public string? GoalsUrl { get; set; }

        [JsonIgnore]
        public string? FilePath { get; set; }
    }
}
