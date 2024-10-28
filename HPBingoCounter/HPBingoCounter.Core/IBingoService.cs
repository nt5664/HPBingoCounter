using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Core.Types;

namespace HPBingoCounter.Core
{
    public interface IBingoService : IDisposable
    {
        IObservable<HPBingoBoardDto> NewBoardObservable { get; }
        IBingoVersions? Versions { get; }

        void ForceReloadVersions();
        Task RequestNewBoardAsync(string version, HPBingoCardTypes cardType, string seed);
    }
}
