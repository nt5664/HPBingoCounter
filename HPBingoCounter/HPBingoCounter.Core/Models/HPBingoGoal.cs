using Newtonsoft.Json;

namespace HPBingoCounter.Core.Models
{
    public class HPBingoGoal
    {
        [JsonConstructor]
        public HPBingoGoal(string name, int amount)
        {
            Name = name;
            RequiredAmount = amount;
        }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("amount")]
        public int RequiredAmount { get; }
    }
}
