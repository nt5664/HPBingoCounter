using HPBingoCounter.Core.Types;

namespace HPBingoCounter.Core.Models
{
    public class HPBingoBoardDto
    {
        public HPBingoBoardDto(string version, string seed, HPBingoCardTypes cardType, IEnumerable<HPBingoGoal>? goals)
        {
            Version = version;
            Seed = seed;
            CardType = cardType;
            Goals = goals;
        }

        public string Version { get; }
        public string Seed { get; }
        public HPBingoCardTypes CardType { get; }
        public IEnumerable<HPBingoGoal>? Goals { get; }
    }
}
