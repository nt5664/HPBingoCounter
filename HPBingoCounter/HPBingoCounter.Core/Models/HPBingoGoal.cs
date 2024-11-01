using Newtonsoft.Json;

namespace HPBingoCounter.Core.Models
{
    public class HPBingoGoal
    {
        [JsonConstructor]
        public HPBingoGoal(string id, string name, int amount, int uniqueAmount, IEnumerable<string>? triggers)
        {
            Id = id;
            Name = name;
            UniqueAmount = uniqueAmount;
            RequiredAmount = amount;
            Triggers = triggers;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("uniqueAmount")]
        public int UniqueAmount { get; }

        [JsonProperty("amount")]
        public int RequiredAmount { get; }

        [JsonProperty("triggers")]
        public IEnumerable<string>? Triggers { get; }

        public override bool Equals(object? obj)
        {
            return GetType().Equals(obj?.GetType()) && GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return 
                Id.GetHashCode() ^ 
                Name.GetHashCode() ^ 
                RequiredAmount.GetHashCode() ^ 
                UniqueAmount.GetHashCode();
        }
    }
}
