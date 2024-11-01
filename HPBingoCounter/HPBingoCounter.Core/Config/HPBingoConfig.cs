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

        [JsonProperty("apiFile")]
        public string? ApiFile { get; set; }

        [JsonProperty("useLocalVersions")]
        public bool UseLocalVersions { get; set; }

        [JsonProperty("useLocalGoals")]
        public bool UseLocalGoals { get; set; }

        [JsonProperty("useLocalGenerator")]
        public bool UseLocalGenerator { get; set; }

        [JsonProperty("useLocalApi")]
        public bool UseLocalApi { get; set; }

        [JsonIgnore]
        public string? FileName { get; set; }
    }
}
