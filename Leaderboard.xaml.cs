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

        public static List<LeaderboardEntry> GetLeaderboardEntries()
        {
            return new List<LeaderboardEntry>(new LeaderboardEntry[4] {
                new LeaderboardEntry("User1", 10, 5, 2),
                new LeaderboardEntry("User2", 8, 7, 2),
                new LeaderboardEntry("User3", 6, 9, 2),
                new LeaderboardEntry("User4", 4, 11, 2)
            });
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
            LeaderBoardGrid.ItemsSource = LeaderboardEntry.GetLeaderboardEntries();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await dbConnector.AddMatchResult("John", "Bent" , 1);
        }
        
        private async void LoadLeaderboard()
        {
            var leaderboardEntries = await dbConnector.GetLeaderboard();
            LeaderBoardGrid.ItemsSource = leaderboardEntries;
        }
    }
}
