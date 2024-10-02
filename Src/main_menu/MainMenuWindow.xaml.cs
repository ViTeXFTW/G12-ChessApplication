﻿using System;
using System.Collections.Generic;
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

namespace G12_ChessApplication
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        public event EventHandler<OptionSelectedEventArgs> OptionSelected;
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void PlayChessBtn_Click(object sender, RoutedEventArgs e)
        {
            // Trigger the OptionSelected event for "Play Chess"
            OptionSelected?.Invoke(this, new OptionSelectedEventArgs { SelectedOption = "PlayChess" });
            //Close(); // Close main menu if needed
        }
    }

    public class OptionSelectedEventArgs : EventArgs
    {
        public string SelectedOption { get; set; }
    }
}