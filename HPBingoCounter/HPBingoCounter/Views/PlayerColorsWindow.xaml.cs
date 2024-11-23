using System.Windows;
using HPBingoCounter.Types;
using HPBingoCounter.ViewModels;

namespace HPBingoCounter.Views
{
    /// <summary>
    /// Interaction logic for PlayerColorsWindow.xaml
    /// </summary>
    public partial class PlayerColorsWindow : Window
    {
        private readonly PlayerColorSelectorViewModel _vm;

        private bool _optionSelected = false;

        public PlayerColorsWindow()
        {
            InitializeComponent();

            _vm = new PlayerColorSelectorViewModel();
            DataContext = _vm;

            _vm.SelectedPlayerColor = App.PlayerColor;
            _vm.SelectedOpponentColor = App.OpponentColor;
        }

        public PlayerColors PlayerColor => _vm.SelectedPlayerColor;

        public PlayerColors OpponentColor => _vm.SelectedOpponentColor;

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            App.SetPlayerColor(PlayerColor);
            App.SetClaimedColor(OpponentColor);
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
