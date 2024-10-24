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

        public override bool Equals(object? obj)
        {
            return GetType().Equals(obj?.GetType()) && GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ RequiredAmount.GetHashCode();
        }
    }
}
