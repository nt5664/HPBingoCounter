using HPBingoCounter.ViewModels.Types;

namespace HPBingoCounter.Coverters
{
    internal class ComparisonModeDisplayConverter : EnumDisplayConverterBase<BoardComparisonModes>
    {
        public static ComparisonModeDisplayConverter Instance { get; }

        static ComparisonModeDisplayConverter()
        {
            Instance = new ComparisonModeDisplayConverter();

            Pairs = new Dictionary<BoardComparisonModes, string>
            {
                { BoardComparisonModes.Single, "Single" },
                { BoardComparisonModes.Double, "Double" },
                { BoardComparisonModes.Triple, "Triple" },
                { BoardComparisonModes.Blackout, "Blackout (Singleplayer)" },
                { BoardComparisonModes.Elimination, "Blackout (Race)" }
            };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
