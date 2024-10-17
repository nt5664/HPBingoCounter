using HPBingoCounter.Commands;
using HPBingoCounter.Core;
using HPBingoCounter.Core.Types;
using System.Collections.ObjectModel;

namespace HPBingoCounter.ViewModels
{
    internal class NewBoardDetailsViewModel : ViewModelBase
    {
        private const string DEFAULT_SEED = "000000";
        private const HPBingoCardTypes DEFAULT_CARD_TYPE = HPBingoCardTypes.Normal;

        private readonly HPBingoService _bingoService;

        public NewBoardDetailsViewModel(HPBingoService bingoService)
        {
            _bingoService = bingoService;
            AvailableVersions = new ObservableCollection<string>();



            RefreshAvailableVersions();
        }

        public DelegateCommand CancelCommand { get; set; }

        public DelegateCommand RequestNewBoardCommand { get; set; }

        private string? _seed;
        public string? Seed
        {
            get => _seed;
            set => SetValue(ref _seed, value);
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
            set => SetValue(ref _selectedVersion, value);
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
