﻿using HPBingoCounter.Commands;
using HPBingoCounter.Core.Models;
using HPBingoCounter.Types;
using HPBingoCounter.ViewModels.Types;

namespace HPBingoCounter.ViewModels
{
    internal class BingoGoalViewModel : ViewModelBase
    {
        public event EventHandler<GoalCounterChangedEventArgs>? CounterSet;

        private HPBingoGoal? _goal;

        public BingoGoalViewModel()
        {
            IncrementCommand = new DelegateCommand(_ => 
            {
                ++Count;
                if (_goal?.Triggers is null)
                    return;

                CounterSet?.Invoke(this, new GoalCounterChangedEventArgs(_goal.Triggers, 1));
            }, _ => _goal is not null && !IsClaimed && !IsCompleted);
            ReductCommand = new DelegateCommand(_ => 
            { 
                --Count;
                if (_goal?.Triggers is null)
                    return;

                CounterSet?.Invoke(this, new GoalCounterChangedEventArgs(_goal.Triggers, -1));
            }, _ => _goal is not null && !IsClaimed && Count > 0);
            TogglePinCommand = new DelegateCommand(_ => IsPinned = !IsPinned);
            ClaimGoalCommand = new DelegateCommand(_ => IsClaimed = !IsClaimed, _ => IsClaimEnabled);

            IsPinned = false;
            IsClaimed = false;
            IsClaimEnabled = false;
        }

        public DelegateCommand IncrementCommand { get; }

        public DelegateCommand ReductCommand { get; }

        public DelegateCommand TogglePinCommand { get; }

        public DelegateCommand ClaimGoalCommand { get; }

        public string? Id => _goal?.Id;

        public string Name => _goal?.Name ?? "NULL";

        public PlayerColors PlayerColor => App.PlayerColor;

        public int RequiredAmount => _goal?.RequiredAmount ?? -1;

        public int UniqueAmount => _goal?.UniqueAmount ?? -1;

        public bool IsCompleted => GoalState.Equals(GoalStates.Completed) && !IsClaimed;

        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set => SetValue(ref _isPinned, value);
        }

        private bool _isClaimEnabled;
        public bool IsClaimEnabled
        {
            get => _isClaimEnabled;
            set
            {
                if (!SetValue(ref _isClaimEnabled, value))
                    return;

                if (!value)
                    IsClaimed = false;

                ClaimGoalCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isClaimed;
        public bool IsClaimed
        {
            get => _isClaimed;
            set 
            {
                bool isCompleted = Count == RequiredAmount;
                if (!SetValue(ref _isClaimed, value, nameof(IsClaimed), nameof(GoalState)))
                    return;

                if (isCompleted)
                    RaisePropertyChanged(nameof(IsCompleted));

                IncrementCommand.RaiseCanExecuteChanged();
                ReductCommand.RaiseCanExecuteChanged();
            }
        }

        private int _count;
        public int Count
        {
            get => _count;
            set 
            {
                GoalStates prevState = GoalState;
                if (!SetValue(ref _count, value, nameof(Count), nameof(GoalState)))
                    return;

                if (GoalState != prevState && (GoalState.Equals(GoalStates.Completed) || prevState.Equals(GoalStates.Completed)))
                    RaisePropertyChanged(nameof(IsCompleted));

                IncrementCommand.RaiseCanExecuteChanged();
                ReductCommand.RaiseCanExecuteChanged();
            }
        }

        public GoalStates GoalState
        {
            get
            {
                if (_goal is null)
                    return GoalStates.None;

                if (IsClaimed)
                    return GoalStates.Claimed;

                if (Count == 0)
                    return GoalStates.Default;

                if (Count == RequiredAmount)
                    return GoalStates.Completed;

                return GoalStates.Active;
            }
        }

        public void SetGoal(HPBingoGoal goal, bool enableClaim)
        {
            _goal = goal;
            Count = 0;
            IsPinned = false;
            IsClaimEnabled = enableClaim;
            IsClaimed = false;
            RaisePropertyChanged(nameof(Id), nameof(Name), nameof(RequiredAmount), nameof(UniqueAmount));
            IncrementCommand.RaiseCanExecuteChanged();
            ReductCommand.RaiseCanExecuteChanged();
            ClaimGoalCommand.RaiseCanExecuteChanged();
        }

        public int GetGoalHashCode()
        {
            return _goal is null ? int.MinValue : _goal.Name.GetHashCode() ^ _goal.RequiredAmount.GetHashCode();
        }

        public void RefreshPlayerColor()
        {
            RaisePropertyChanged(nameof(PlayerColor));
        }
    }
}
