using G12_ChessApplication;
using System.Configuration;
using System.Data;
using System.Windows;

namespace G12_ChessApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Start with the login window, Hej
            init_login();
        }

        private void init_login()
        {
            var loginWindow = new LoginWindow();
            // Subscribe to the login success event
            loginWindow.LoginSuccess += OnLoginSuccess;
            loginWindow.Show();
        }

        private void OnLoginSuccess(object sender, EventArgs e)
        {
            // Close the login window
            if (sender is Window loginWindow)
            {
                loginWindow.Hide();
            }

            // Show the main menu window
            init_mainmenu();

            // Close the login window
            if (sender is Window loginWindowToClose)
            {
                loginWindowToClose.Close();
            }
        }

        private void init_mainmenu()
        {

            try
            {
                var mainMenuWindow = new MainMenuWindow();
                mainMenuWindow.OptionSelected += OnOptionSelected;
                mainMenuWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Main Menu: {ex.Message}");
            }
        }

        private void OnOptionSelected(object sender, OptionSelectedEventArgs e)
        {
            // Close the login window
            if (sender is Window mainMenuWindow)
            {
                mainMenuWindow.Hide();
            }
            // Handle option selection
            switch (e.SelectedOption)
            {
                case "PlayChess":
                    init_game("Host", "play");
                    break;
                case "JoinGame":
                    init_game(e.Code, "play");
                    break;
                case "Puzzles":
                    init_game("Host", "puzzles");
                    break;
                case "Analysis":
                    init_game("Host", "Analysis");
                    break;
                case "Settings":
                    break;
                default:
                    break;
            }

            // Close the login window
            if (sender is Window mainMenuWindowToClose)
            {
                mainMenuWindowToClose.Close();
            }
        }

        private void init_game(string code, string gameType)
        {
            var gameWindow = new MainWindow(gameType, code);
            gameWindow.goBack += BackFromGame;
            gameWindow.Show();
        }

        private void BackFromGame(object sender, EventArgs e)
        {
            // Close the game window
            if (sender is Window gameWindow)
            {
                gameWindow.Hide();
            }

            // Show the main menu window
            init_mainmenu();

            // Close the game window
            if (sender is Window gameWindowToClose)
            {
                gameWindowToClose.Close();
            }
        }
    }

}
