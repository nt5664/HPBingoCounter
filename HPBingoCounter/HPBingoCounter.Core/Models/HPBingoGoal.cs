using Newtonsoft.Json;

namespace HPBingoCounter.Core.Models
{
    public class HPBingoGoal : IGoal
    {
        [JsonConstructor]
        public HPBingoGoal(string name)
        {
            Name = name;
            RequiredAmount = 0;
        }

        public string Name { get; }

        [JsonIgnore]
        public bool CollectMultiple => RequiredAmount > 0;

        public int RequiredAmount { get; }
    }
}
