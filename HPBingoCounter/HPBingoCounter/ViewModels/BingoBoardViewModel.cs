using System.Collections.ObjectModel;
using System.ComponentModel;
using HPBingoCounter.Commands;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Core.Types;
using HPBingoCounter.ViewModels.Types;

namespace HPBingoCounter.ViewModels
{
    internal class BingoBoardViewModel : ViewModelBase
    {
        private readonly Dictionary<int, int> _savedState;

        public BingoBoardViewModel()
        {
            _savedState = new Dictionary<int, int>();
            Goals = new ObservableCollection<BingoGoalViewModel>();
            BoardVersion = "No board loaded";
            ComparisonMode = BoardComparisonModes.Single;
            CompletedGoals = 0;

            SaveStateCommand = new DelegateCommand(_ => ProcessGoals(g => _savedState[g.GetGoalHashCode()] = g.Count), CanExecuteCommand);
            LoadSavedStateCommand = new DelegateCommand(_ => ProcessGoals(g => g.Count = _savedState[g.GetGoalHashCode()]), CanExecuteCommand);
            ClearSavedStateCommand = new DelegateCommand(_ => ClearSavedState(), CanExecuteCommand);
            ResetBoardCommand = new DelegateCommand(_ =>
            {
                ClearSavedState();
                LoadSavedStateCommand.Execute(null);
            }, CanExecuteCommand);
        }

        public ObservableCollection<BingoGoalViewModel> Goals { get; }

        public DelegateCommand SaveStateCommand { get; }

        public DelegateCommand LoadSavedStateCommand { get; }

        public DelegateCommand ClearSavedStateCommand { get; }

        public DelegateCommand ResetBoardCommand { get; }

        private string? _boardVersion;
        public string? BoardVersion
        {
            get => _boardVersion;
            private set => SetValue(ref _boardVersion, value);
        }

        private string? _seed;
        public string? Seed
        {
            get => _seed;
            private set => SetValue(ref _seed, value);
        }

        private HPBingoCardTypes? _cardType;
        public HPBingoCardTypes? CardType
        {
            get => _cardType;
            private set => SetValue(ref _cardType, value);
        }

        public bool IsBoardEmpty => Goals.Count == 0;

        public bool IsBoardCompleted => CompletedGoals >= RequiredGoals;

        private BoardComparisonModes _comparisonMode;
        public BoardComparisonModes ComparisonMode
        {
            get => _comparisonMode;
            set => SetValue(ref _comparisonMode, value, nameof(ComparisonMode), nameof(RequiredGoals), nameof(IsBoardCompleted));
        }

        public int RequiredGoals
        {
            get
            {
                return ComparisonMode switch
                {
                    BoardComparisonModes.Single => 5,
                    BoardComparisonModes.Double => 10,
                    BoardComparisonModes.Triple => 15,
                    BoardComparisonModes.Elimination => 13,
                    _ => 0,
                };
            }
        }

        private int _completedGoals;
        public int CompletedGoals
        {
            get => _completedGoals;
            private set => SetValue(ref _completedGoals, value, nameof(CompletedGoals), nameof(IsBoardCompleted));
        }

        public void LoadBoard(HPBingoBoardDto board)
        {
            _savedState.Clear();

            int idx = board.Goals is null ? 0 : -1;
            if (board.Goals is not null)
            {
                foreach (var goal in board.Goals)
                {
                    if (goal is null)
                        continue;

                    if (++idx >= Goals.Count)
                    {
                        var newVm = new BingoGoalViewModel();
                        newVm.PropertyChanged += OnGoalPropertyChanged;
                        Goals.Add(newVm);
                    }

                    BingoGoalViewModel goalVm = Goals[idx];
                    goalVm.SetGoal(goal);

                    _savedState.Add(goalVm.GetGoalHashCode(), 0);
                }
            }

            while (idx++ < Goals.Count - 1)
            {
                var goalToDelete = Goals[idx];
                goalToDelete.PropertyChanged -= OnGoalPropertyChanged;
                Goals.Remove(goalToDelete);
            }

            BoardVersion = board.Version;
            Seed = board.Seed;
            CardType = board.CardType;
            CompletedGoals = 0;
            RaisePropertyChanged(nameof(IsBoardEmpty), nameof(IsBoardCompleted));
            SaveStateCommand.RaiseCanExecuteChanged();
            LoadSavedStateCommand.RaiseCanExecuteChanged();
            ClearSavedStateCommand.RaiseCanExecuteChanged();
            ResetBoardCommand.RaiseCanExecuteChanged();
        }

        public override void Dispose()
        {
            _savedState.Clear();
            foreach (var goal in Goals)
            {
                goal.PropertyChanged -= OnGoalPropertyChanged;
                goal.Dispose();
            }

            Goals.Clear();
            base.Dispose();
        }

        private bool CanExecuteCommand(object? _)
        {
            return Goals.Count > 0;
        }

        private void ProcessGoals(Action<BingoGoalViewModel> processAction)
        {
            ArgumentNullException.ThrowIfNull(processAction);

            foreach (var goal in Goals)
            {
                processAction(goal);
            }
        }

        private void ClearSavedState()
        {
            foreach (var key in _savedState.Keys)
            {
                _savedState[key] = 0;
            }
        }

        private void OnGoalPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not BingoGoalViewModel vm || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (e.PropertyName.Equals(nameof(BingoGoalViewModel.IsCompleted)))
                CompletedGoals += vm.IsCompleted ? 1 : -1;
        }
    }
}
