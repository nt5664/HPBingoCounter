using Newtonsoft.Json;

namespace HPBingoCounter.Core.Config
{
    public sealed class HPBingoVersions : IBingoVersions
    {
        [JsonProperty("default_version")]
        public string DefaultVersion { get; set; } = string.Empty;

        [JsonProperty("versions")]
        public IDictionary<string, string> VersionDictionary { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public bool IsValid => (VersionDictionary?.Any() ?? false) && !string.IsNullOrWhiteSpace(DefaultVersion);

        public override bool Equals(object? obj)
        {
            return GetType() == obj?.GetType() && GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return DefaultVersion.GetHashCode() ^ VersionDictionary.GetHashCode();
        }
    }
}
