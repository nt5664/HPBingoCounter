using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Config
{
    public sealed class HPBingoVersions : IBingoVersions
    {
        [JsonProperty("default_version")]
        public string DefaultVersion { get; set; } = string.Empty;

        [JsonProperty("versions")]
        public IDictionary<string, string> VersionDictionary { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public bool IsValid => VersionDictionary.Any() && !string.IsNullOrEmpty(DefaultVersion);
    }
}
