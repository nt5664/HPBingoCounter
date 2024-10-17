using HPBingoCounter.Commands;
using HPBingoCounter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPBingoCounter.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly HPBingoService _service;

        public MainViewModel()
        {
            _service = new HPBingoService();
            NewBoardViewModel = new NewBoardDetailsViewModel(_service);
            NewBoardViewModel.CancelCommand = new DelegateCommand(_ => SelectNewBoard = false);

            SelectNewBoard = false;
            ShowNewBoardDetailsCommand = new DelegateCommand(_ => 
            { 
                NewBoardViewModel.ResetState();
                SelectNewBoard = true;
            });
        }

        public DelegateCommand ShowNewBoardDetailsCommand { get; }

        private bool _selectNewBoard;
        public bool SelectNewBoard
        {
            get => _selectNewBoard;
            set => SetValue(ref _selectNewBoard, value);
        }

        public NewBoardDetailsViewModel NewBoardViewModel { get; set; }
    }
}
