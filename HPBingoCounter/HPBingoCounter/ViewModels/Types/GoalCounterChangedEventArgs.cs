namespace HPBingoCounter.ViewModels.Types
{
    internal class GoalCounterChangedEventArgs
    {
        public GoalCounterChangedEventArgs(IEnumerable<string> triggers, int diff)
        {
            Triggers = triggers;
            Diff = diff;
        }

        public IEnumerable<string> Triggers { get; }

        public int Diff { get; }
    }
}
