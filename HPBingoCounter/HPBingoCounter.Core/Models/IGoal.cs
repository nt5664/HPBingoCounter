namespace HPBingoCounter.Core.Models
{
    public interface IGoal
    {
        string? Name { get; }
        bool CollectMultiple { get; }
        int RequiredAmount { get; }
    }
}
