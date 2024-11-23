using HPBingoCounter.Commands;
using HPBingoCounter.Types;

namespace HPBingoCounter.ViewModels
{
    internal class PlayerColorSelectorViewModel : ViewModelBase
    {
        public PlayerColorSelectorViewModel()
        {
            SetPlayerColorCommand = new DelegateCommand(c =>
            {
                if (c is not PlayerColors pc)
                    return;

                SelectedPlayerColor = pc;
            });

            SetOpponentColorCommand = new DelegateCommand(c => 
            {
                if (c is not PlayerColors pc)
                    return;

                SelectedOpponentColor = pc;
            });
        }

        public DelegateCommand SetPlayerColorCommand { get; }

        public DelegateCommand SetOpponentColorCommand { get; }

        private PlayerColors _selectedPlayerColor;
        public PlayerColors SelectedPlayerColor 
        {
            get => _selectedPlayerColor;
            set => SetValue(ref _selectedPlayerColor, value);
        }

        private PlayerColors _selectedOpponentColor;
        public PlayerColors SelectedOpponentColor
        {
            get => _selectedOpponentColor;
            set => SetValue(ref _selectedOpponentColor, value);
        }
    }
}
