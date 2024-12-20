﻿using System.Collections.ObjectModel;
using HPBingoCounter.Commands;
using HPBingoCounter.Core;
using HPBingoCounter.Core.Types;

namespace HPBingoCounter.ViewModels
{
    internal class BoardConfigViewModel : ViewModelBase
    {
        private const string FALLBACK_VERSION = "NONE";
        private const string DEFAULT_SEED = "000000";
        private const HPBingoCardTypes DEFAULT_CARD_TYPE = HPBingoCardTypes.Normal;

        private static readonly Random Rnd = new();

        private readonly IBingoService _bingoService;

        public BoardConfigViewModel(IBingoService bingoService, DelegateCommand newBoardCommand, DelegateCommand cancelCommand)
        {
            _bingoService = bingoService ?? throw new ArgumentNullException(nameof(bingoService));
            CancelCommand = cancelCommand ?? throw new ArgumentNullException(nameof(cancelCommand));
            RequestNewBoardCommand = newBoardCommand ?? throw new ArgumentNullException(nameof(newBoardCommand));
            AvailableVersions = new ObservableCollection<string>();
            RandomSeedCommand = new DelegateCommand(_ => Seed = (Rnd.NextDouble() * 999999d).ToString("F0"));
            
            RefreshAvailableVersions();
        }

        public DelegateCommand CancelCommand { get; }

        public DelegateCommand RequestNewBoardCommand { get; }

        public DelegateCommand RandomSeedCommand { get; }

        private string? _seed;
        public string Seed
        {
            get => _seed ?? DEFAULT_SEED;
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
        public string SelectedVersion
        {
            get => _selectedVersion ?? FALLBACK_VERSION;
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
            Seed = DEFAULT_SEED;
            SelectedCardType = DEFAULT_CARD_TYPE;
            SelectedVersion = _bingoService.Versions?.DefaultVersion ?? FALLBACK_VERSION;
        }
    }
}
