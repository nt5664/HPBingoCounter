using HPBingoCounter.Core.Types;

namespace HPBingoCounter.Coverters
{
    internal class CardTypeDisplayConverter : EnumDisplayConverterBase<HPBingoCardTypes>
    {
        public static CardTypeDisplayConverter Instance { get; }

        static CardTypeDisplayConverter()
        {
            Instance = new CardTypeDisplayConverter();

            Pairs = new Dictionary<HPBingoCardTypes, string>
            {
                { HPBingoCardTypes.Normal, "Normal" },
                { HPBingoCardTypes.Short, "Short" },
                { HPBingoCardTypes.Blackout, "Blackout" }
            };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
