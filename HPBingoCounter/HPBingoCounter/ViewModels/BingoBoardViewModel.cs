﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using HPBingoCounter.Commands;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Core.Types;
using HPBingoCounter.ViewModels.Types;

namespace HPBingoCounter.ViewModels
{
    internal class BingoBoardViewModel : ViewModelBase
    {
        private class BingoState
        {
            #region BingoState instance management
            private static int InstanceCtr = 0;
            private static readonly List<BingoState> InstancePool = [];

            public static BingoState GetInstance()
            {
                if (InstanceCtr == InstancePool.Count)
                    InstancePool.Add(new BingoState());

                return InstancePool[InstanceCtr++];
            }

            public static List<BingoState> GetInstances(int num)
            {
                if (num < 1)
                    return Enumerable.Empty<BingoState>().ToList();

                List<BingoState> instances = new List<BingoState>();
                for (int i = 0; i < num; ++i)
                {
                    instances.Add(GetInstance());
                }

                return instances;
            }

            public static void BoardCleared()
            {
                InstanceCtr = 0;
            }

            public static void RemoveUnusedInstances()
            {
                while (InstancePool.Count > InstanceCtr)
                {
                    InstancePool.RemoveAt(InstanceCtr);
                }
            }
            #endregion

            private readonly Dictionary<int, bool> _goals = [];

            private bool _reevaluateBingos = false;
            private bool _isBingo = false;

            private BingoState()
            { }

            public bool IsBingo
            {
                get
                {
                    if (_reevaluateBingos)
                    { 
                        _isBingo = _isBingo = _goals.Count > 0 && _goals.Values.All(x => x);
                        _reevaluateBingos = false;
                    }

                    return _isBingo;
                }
            }

            public bool Contains(int goalHash)
            {
                return _goals.ContainsKey(goalHash);
            }

            public void AddGoal(int goalHash, bool initialState)
            {
                if (Contains(goalHash))
                    return;

                _goals.Add(goalHash, initialState);
                _reevaluateBingos = true;
            }

            public void SetState(int goalHash, bool newState)
            {
                if (!Contains(goalHash))
                    return;

                bool prevState = _goals[goalHash];
                _goals[goalHash] = newState;
                _reevaluateBingos = prevState != newState;
            }

            public void Reset()
            {
                _goals.Clear();
                _reevaluateBingos = true;
            }
        }

        private readonly Dictionary<int, int> _savedState;
        private readonly List<BingoState> _bingos;

        private int _boardDimension;

        public BingoBoardViewModel()
        {
            _savedState = new Dictionary<int, int>();
            _bingos = new List<BingoState>();
            _boardDimension = 0;
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
            set 
            {
                if (!SetValue(ref _comparisonMode, value, nameof(ComparisonMode), nameof(RequiredGoals), nameof(IsBoardCompleted)))
                    return;

                CompletedGoals = value.Equals(BoardComparisonModes.Elimination) ?
                    Goals.Count(x => x.IsCompleted) :
                    _bingos.Count(x => x.IsBingo);
            }
        }

        public int RequiredGoals
        {
            get
            {
                return ComparisonMode switch
                {
                    BoardComparisonModes.Single => 1,
                    BoardComparisonModes.Double => 2,
                    BoardComparisonModes.Triple => 3,
                    BoardComparisonModes.Elimination => (int)Math.Ceiling(Goals.Count / 2d),
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
            _bingos.Clear();
            BingoState.BoardCleared();

            int idx = board.Goals is null ? 0 : -1;
            if (board.Goals is not null)
            {
                List<HPBingoGoal> bingoGoals = board.Goals.ToList();
                double sqrt = Math.Sqrt(bingoGoals.Count(x => x != null));
                if (!double.IsInteger(sqrt))
                    throw new InvalidOperationException("Board must be NxN to calculate progress");

                _boardDimension = (int)sqrt;

                BingoState diagonalTB = BingoState.GetInstance();
                BingoState diagonalBT = BingoState.GetInstance();
                List<BingoState> vertical = BingoState.GetInstances(_boardDimension);
                List<BingoState> horizontal = BingoState.GetInstances(_boardDimension);

                foreach (var goal in bingoGoals)
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

                    int goalHash = goalVm.GetGoalHashCode();
                    _savedState.Add(goalHash, 0);

                    int row = idx / _boardDimension;
                    int col = idx % _boardDimension;
                    if (row == col)
                        diagonalTB.AddGoal(goalHash, false);

                    if (row + col == _boardDimension - 1 && idx < bingoGoals.Count - 1)
                        diagonalBT.AddGoal(goalHash, false);

                    horizontal[row].AddGoal(goalHash, false);
                    vertical[col].AddGoal(goalHash, false);
                }

                _bingos.Add(diagonalBT);
                _bingos.Add(diagonalTB);
                _bingos.AddRange(vertical);
                _bingos.AddRange(horizontal);
            }

            while (idx++ < Goals.Count - 1)
            {
                var goalToDelete = Goals[idx];
                goalToDelete.PropertyChanged -= OnGoalPropertyChanged;
                Goals.Remove(goalToDelete);
            }

            BingoState.RemoveUnusedInstances();
            BoardVersion = board.Version;
            Seed = board.Seed;
            CardType = board.CardType;
            CompletedGoals = 0;

            RaisePropertyChanged(nameof(IsBoardEmpty), nameof(IsBoardCompleted), nameof(RequiredGoals));
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
            {
                bool isElmimination = ComparisonMode.Equals(BoardComparisonModes.Elimination);
                int diff = 0;

                // in elimination mode, the objective is to get 50% + 1 goals faster than the opponent
                if (isElmimination)
                { 
                    diff += vm.IsCompleted ? 1 : -1;
                }

                // in "regular" bingo, the objective is to complete 1/2/3 line(s) of goals vertically, horizontally or diagonally
                List<BingoState> bingos = _bingos.Where(x => x.Contains(vm.GetGoalHashCode())).ToList();
                foreach (var bingo in bingos)
                {
                    bool prevState = bingo.IsBingo;
                    bingo.SetState(vm.GetGoalHashCode(), vm.IsCompleted);
                    if (isElmimination)
                        continue;

                    diff += bingo.IsBingo switch
                    {
                        true when !prevState => 1,
                        false when prevState => -1,
                        _ => 0
                    };
                }

                CompletedGoals += diff;
            }
        }
    }
}
