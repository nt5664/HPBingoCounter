using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HPBingoCounter.Commands;
using HPBingoCounter.Core.Models;
using HPBingoCounter.ViewModels.Types;

namespace HPBingoCounter.ViewModels
{
    internal class BingoGoalViewModel : ViewModelBase
    {
        private HPBingoGoal? _goal;

        public BingoGoalViewModel()
        {
            IncrementCommand = new DelegateCommand(_ => ++Count, _ => _goal is not null && !GoalState.Equals(GoalStates.Completed));
            ReductCommand = new DelegateCommand(_ => --Count, _ => _goal is not null && Count > 0);
        }

        public DelegateCommand IncrementCommand { get; }

        public DelegateCommand ReductCommand { get; }

        public string Name => _goal?.Name ?? "UNKNOWN";

        public bool CollectMultiple => _goal?.CollectMultiple ?? false;

        public int RequiredAmount => _goal?.RequiredAmount ?? -1;

        private int _count;
        public int Count
        {
            get => _count;
            set 
            {
                if (!SetValue(ref _count, value, nameof(Count), nameof(GoalState)))
                    return;

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

                if (Count == 0)
                    return GoalStates.Default;

                if (!CollectMultiple || Count == RequiredAmount)
                    return GoalStates.Completed;

                return GoalStates.Active;
            }
        }

        public void SetGoal(HPBingoGoal goal)
        {
            _goal = goal;
            Count = 0;
            RaisePropertyChanged(nameof(Name), nameof(CollectMultiple), nameof(RequiredAmount));
            IncrementCommand.RaiseCanExecuteChanged();
            ReductCommand.RaiseCanExecuteChanged();
        }

        public int GetGoalHashCode()
        {
            return _goal is null ? int.MinValue : _goal.Name.GetHashCode() ^ _goal.RequiredAmount.GetHashCode();
        }
    }
}
