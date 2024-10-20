﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HPBingoCounter.Views
{
    /// <summary>
    /// Interaction logic for BugReportWindow.xaml
    /// </summary>
    public partial class BugReportWindow : Window
    {
        public BugReportWindow()
        {
            InitializeComponent();
        }

        private void HyperlinkOpenUrl(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/nt5664/HPBingoCounter/issues") { UseShellExecute = true });
        }
    }
}
