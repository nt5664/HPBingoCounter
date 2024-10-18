using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Config
{
    public sealed class HPBingoConfig : IConfig
    {
        [JsonProperty("versions")]
        public string? VersionUrl { get; set; }

        [JsonProperty("generatorApi")]
        public string? GeneratorFuncUrl { get; set; }

        [JsonProperty("generatorUrl")]
        public string? GeneratorUrl { get; set; }

        [JsonProperty("goalsUrl")]
        public string? GoalsUrl { get; set; }
    }
}
