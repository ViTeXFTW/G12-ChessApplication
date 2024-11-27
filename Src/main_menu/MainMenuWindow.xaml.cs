﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        private string username;
        public event EventHandler<OptionSelectedEventArgs> OptionSelected;
        public MainMenuWindow(string username)
        {
            InitializeComponent();
            this.username = username;
            UsernameLabel.Content = this.username;
        }

        private void PlayChessBtn_Click(object sender, RoutedEventArgs e)
        {
            // Trigger the OptionSelected event for "Play Chess"
            OptionSelected?.Invoke(this, new OptionSelectedEventArgs { SelectedOption = "PlayChess" });
            //Close(); // Close main menu if needed
        }

        private void JoinGameBtn_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeBox.Text;
            if(code == string.Empty)
            {
                code += "127.0.0.1";
                //MessageBox.Show("Enter code to join");
                //return;
            }
            OptionSelected?.Invoke(this, new OptionSelectedEventArgs { SelectedOption = "JoinGame", Code = code });
        }

        private void PuzzlesBtn_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(this, new OptionSelectedEventArgs { SelectedOption = "Puzzles" });
        }

        private void AnalysisBtn_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(this, new OptionSelectedEventArgs { SelectedOption = "Analysis" });
        }

        private void OpenLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            Leaderboard leaderboard = new Leaderboard(username);
            leaderboard.Show();
            this.Close();
        }
    }

    public class OptionSelectedEventArgs : EventArgs
    {
        public string SelectedOption { get; set; }
        public string Code { get; set; }

    }
}
