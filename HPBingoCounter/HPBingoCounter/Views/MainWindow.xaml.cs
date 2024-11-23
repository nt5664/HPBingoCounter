using System.Windows;
using HPBingoCounter.Types;

namespace HPBingoCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            App.SetPlayerColor(PlayerColors.Orange);
            App.SetClaimedColor(PlayerColors.Orange);
        }
    }
}