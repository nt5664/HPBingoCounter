using HPBingoCounter.Commands;
using HPBingoCounter.Core;
using HPBingoCounter.Core.Config;
using HPBingoCounter.Core.Models;
using System.Text;
using System.Windows;

namespace HPBingoCounter.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly HPBingoService _service;
        private readonly IDisposable _newBoardSubscription;

        public MainViewModel()
        {
            _service = new HPBingoService();
            _newBoardSubscription = _service.NewBoardObservable.Subscribe(HandleNewBoard);
            NewBoardConfigViewModel = new BoardConfigViewModel(
                _service,
                new DelegateCommand(_ =>
                {
                    ShowLoadingScreen = true;
                    SelectNewBoard = false;
                    SafeInvoke(
                        () => _service.RequestNewBoard(NewBoardConfigViewModel.SelectedVersion, NewBoardConfigViewModel.SelectedCardType, NewBoardConfigViewModel.Seed),
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

            ReloadConfigCommand = new DelegateCommand(_ => 
            {
                ShowLoadingScreen = true;
                SafeInvoke(() => HPBingoConfigManager.ReloadConfig());
                ShowLoadingScreen = false;
            });
        }

        public DelegateCommand ShowNewBoardConfigCommand { get; }

        public DelegateCommand ReloadConfigCommand { get; }

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

        private void HandleNewBoard(IEnumerable<HPBingoGoal>? newGoals)
        {
            if (newGoals is null)
                return;

            BoardViewModel.LoadBoard(newGoals);
            ShowLoadingScreen = false;
        }

        public override void Dispose()
        {
            NewBoardConfigViewModel.Dispose();
            _newBoardSubscription.Dispose();
            _service.Dispose();

            base.Dispose();
        }
    }
}
