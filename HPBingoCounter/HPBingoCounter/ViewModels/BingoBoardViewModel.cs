using HPBingoCounter.Commands;
using HPBingoCounter.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.ViewModels
{
    internal class BingoBoardViewModel : ViewModelBase
    {
        private readonly Dictionary<int, int> _savedState;

        public BingoBoardViewModel()
        {
            _savedState = new Dictionary<int, int>();
            Goals = new ObservableCollection<BingoGoalViewModel>();

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

        public void LoadBoard(IEnumerable<HPBingoGoal> goals)
        {
            _savedState.Clear();
            int idx = -1;
            foreach (var goal in goals)
            {
                if (goal is null)
                    continue;

                if (++idx >= Goals.Count)
                    Goals.Add(new BingoGoalViewModel());

                BingoGoalViewModel goalVm = Goals[idx];
                goalVm.SetGoal(goal);

                _savedState.Add(goalVm.GetGoalHashCode(), 0);
            }

            while (idx++ < Goals.Count - 1)
            {
                Goals.RemoveAt(idx);
            }
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
    }
}
