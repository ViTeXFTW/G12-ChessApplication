using System;
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
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    /// 

    public class LeaderboardEntry
    {
        public string Username { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int TotalGames { get; set; }

        
        private SQLConnector dbConnector;

        public LeaderboardEntry(string username, int wins, int losses, int draws)
        {
            this.Username = username;
            this.Wins = wins;
            this.Losses = losses;
            this.Draws = draws;
            this.TotalGames = wins + losses + draws;
        }
    }

    public partial class Leaderboard : Window
    {
        private SQLConnector dbConnector;
        public Leaderboard()
        {
            InitializeComponent();
            dbConnector = new SQLConnector();
            LoadLeaderboard();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await dbConnector.AddOrUpdateMatchResult("Magnus69", "makki12", 1);
                LoadLeaderboard();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private async void LoadLeaderboard()
        {
            var leaderboardEntries = await dbConnector.GetLeaderboard();
            var matchhistory = await dbConnector.GetMatchHistory("Magnus69");
            LeaderBoardGrid.ItemsSource = leaderboardEntries;
            MatchHistoryGrid.ItemsSource = matchhistory;
        }
    }
}
