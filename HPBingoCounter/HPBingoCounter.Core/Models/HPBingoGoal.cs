using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.Core.Models
{
    public class HPBingoGoal : IGoal
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonIgnore]
        public bool CollectMultiple => RequiredAmount > 0;

        public int RequiredAmount { get; set; }
    }
}
