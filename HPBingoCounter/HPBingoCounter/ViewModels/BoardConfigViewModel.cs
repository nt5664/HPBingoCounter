using HPBingoCounter.Commands;
using HPBingoCounter.Core;
using HPBingoCounter.Core.Types;
using System.Collections.ObjectModel;

namespace HPBingoCounter.ViewModels
{
    internal class BoardConfigViewModel : ViewModelBase
    {
        private const string DEFAULT_SEED = "000000";
        private const HPBingoCardTypes DEFAULT_CARD_TYPE = HPBingoCardTypes.Normal;

        private readonly HPBingoService _bingoService;

        public BoardConfigViewModel(HPBingoService bingoService, DelegateCommand newBoardCommand, DelegateCommand cancelCommand)
        {
            _bingoService = bingoService ?? throw new ArgumentNullException(nameof(bingoService));
            CancelCommand = cancelCommand ?? throw new ArgumentNullException(nameof(cancelCommand));
            RequestNewBoardCommand = newBoardCommand ?? throw new ArgumentNullException(nameof(newBoardCommand));
            AvailableVersions = new ObservableCollection<string>();
            
            RefreshAvailableVersions();
        }

        public DelegateCommand CancelCommand { get; }

        public DelegateCommand RequestNewBoardCommand { get; }

        private string? _seed;
        public string? Seed
        {
            get => _seed;
            set
            {
                if (SetValue(ref _seed, value))
                    RequestNewBoardCommand.RaiseCanExecuteChanged();
            }
        }

        public Array CardTypes => Enum.GetValues(typeof(HPBingoCardTypes));

        private HPBingoCardTypes _selectedCardType;
        public HPBingoCardTypes SelectedCardType
        {
            get => _selectedCardType;
            set => SetValue(ref _selectedCardType, value);
        }

        public ObservableCollection<string> AvailableVersions { get; }

        private string? _selectedVersion;
        public string? SelectedVersion
        {
            get => _selectedVersion;
            set 
            {
                if (SetValue(ref _selectedVersion, value))
                    RequestNewBoardCommand.RaiseCanExecuteChanged();
            }
        }

        public void RefreshAvailableVersions()
        {
            AvailableVersions.Clear();
            if (_bingoService.Versions is null)
                return;

            foreach (string version in _bingoService.Versions.VersionDictionary.Values)
            {
                AvailableVersions.Add(version);
            }

            ResetState();
        }

        public void ResetState()
        {
            if (_bingoService.Versions is null)
                return;

            Seed = DEFAULT_SEED;
            SelectedCardType = DEFAULT_CARD_TYPE;
            SelectedVersion = _bingoService.Versions.DefaultVersion;
        }
    }
}
