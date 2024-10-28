using HPBingoCounter.Commands;
using HPBingoCounter.Core;
using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Models;
using HPBingoCounter.ViewModels.Types;
using HPBingoCounter.Views;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace HPBingoCounter.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly IBingoService _service;
        private readonly IDisposable _newBoardSubscription;

        public MainViewModel(IBingoService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _newBoardSubscription = _service.NewBoardObservable.Subscribe(HandleNewBoard);
            NewBoardConfigViewModel = new BoardConfigViewModel(
                _service,
                new DelegateCommand(_ =>
                {
                    ShowLoadingScreen = true;
                    SelectNewBoard = false;
                    SafeInvoke(
                        () => _service.RequestNewBoardAsync(NewBoardConfigViewModel.SelectedVersion, NewBoardConfigViewModel.SelectedCardType, NewBoardConfigViewModel.Seed),
                        () => ShowLoadingScreen = false
                    );
                }, _ => !string.IsNullOrWhiteSpace(NewBoardConfigViewModel.SelectedVersion) && !string.IsNullOrWhiteSpace(NewBoardConfigViewModel.Seed)),
                new DelegateCommand(_ => SelectNewBoard = false));

            BoardViewModel = new BingoBoardViewModel();

            SelectNewBoard = false;
            ShowNewBoardConfigCommand = new DelegateCommand(_ => 
            { 
                NewBoardConfigViewModel.ResetState();
                SelectNewBoard = true;
            });

            RefreshAvailableConfigCommand = new DelegateCommand(_ => 
            {
                RaisePropertyChanged(nameof(AvailableConfigFiles));
            });

            ReloadConfigCommand = new DelegateCommand(o => 
            {
                string? configFile = o?.ToString();

                ShowLoadingScreen = true;
                SafeInvoke(() => 
                {
                    HPBingoConfigManager.ReloadConfig(configFile);
                    _service.ForceReloadVersions();
                    NewBoardConfigViewModel.RefreshAvailableVersions();
                    MessageBox.Show($"Config file ({configFile}) successfully loaded", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                ShowLoadingScreen = false;
                RaisePropertyChanged(nameof(ActiveConfigFile), nameof(WindowTitle));
            });

            SetComparisonModeCommand = new DelegateCommand(o =>
            {
                if (o is not BoardComparisonModes newMode)
                    return;

                BoardViewModel.ComparisonMode = newMode;
            });

            OpenBingoWebsiteCommand = new DelegateCommand(_ => Process.Start(new ProcessStartInfo("https://hpbingo.github.io/") { UseShellExecute = true }));
            BugReportCommand = new DelegateCommand(_ => new BugReportWindow().ShowDialog());
        }

        public Array ComparisonModes => Enum.GetValues(typeof(BoardComparisonModes));

        public IEnumerable<string> AvailableConfigFiles => HPBingoConfigManager.AvailableConfigFiles;

        public string? ActiveConfigFile => HPBingoConfigManager.Current?.FileName;

        public DelegateCommand ShowNewBoardConfigCommand { get; }

        public DelegateCommand SetComparisonModeCommand { get; }

        public DelegateCommand OpenBingoWebsiteCommand { get; }

        public DelegateCommand RefreshAvailableConfigCommand { get; }

        public DelegateCommand ReloadConfigCommand { get; }

        public DelegateCommand BugReportCommand { get; }

        public string WindowTitle => $"HP Bingo Counter [v{App.AppVersion}] [Config: {ActiveConfigFile ?? "NO CONFIG LOADED"}]";

        private bool _selectNewBoard;
        public bool SelectNewBoard
        {
            get => _selectNewBoard;
            set => SetValue(ref _selectNewBoard, value);
        }

        private bool _showLoadingScreen;
        public bool ShowLoadingScreen
        {
            get => _showLoadingScreen;
            private set => SetValue(ref _showLoadingScreen, value);
        }

        public BoardConfigViewModel NewBoardConfigViewModel { get; }

        public BingoBoardViewModel BoardViewModel { get; }

        private void HandleNewBoard(HPBingoBoardDto newGoals)
        {
            if (newGoals is null)
                return;

            BoardViewModel.LoadBoard(newGoals);
            ShowLoadingScreen = false;
        }

        public override void Dispose()
        {
            NewBoardConfigViewModel.Dispose();
            BoardViewModel.Dispose();
            _newBoardSubscription.Dispose();
            _service.Dispose();

            base.Dispose();
        }
    }
}
