using System.Windows;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;


namespace G12_ChessApplication
{
    public partial class LoginScreen : Window
    {
        private string userName = "";
        private string passWord = "";

        // In-memory user list for simplicity. Later, you can replace this with a DB.
        private SQLConnector.User currentUser;

        // Database context
        private SQLConnector dbConnector;

        
        public LoginScreen()
        {
            InitializeComponent();
            dbConnector = new SQLConnector();
            dbConnector.InitializeDatabase(); // Ensure the database is created and ready
        }

        private void UserNameInput_TextChanged(object sender, RoutedEventArgs e)
        // Hides the placeholder text when the user starts typing in the username field.
        {
            if (userNameInput.Text.Length > 0)
            {
                tbUsername.Visibility = Visibility.Collapsed;
            }
            else 
            {
                tbUsername.Visibility = Visibility.Visible;
            }
        }

        private void PassWordInput_PasswordChanged(object sender, RoutedEventArgs e)
        // Hides the placeholder text when the user starts typing in the password field.
        {
            if (passWordInput.Password.Length > 0)
            {
                tbPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbPassword.Visibility = Visibility.Visible;
            }
        }

        private async void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            userName = userNameInput.Text;
            passWord = passWordInput.Password;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                warningMessage.Content = "Please enter both a username and a password to proceed.";
                return;
            }
    
            // Authenticate the user asynchronously
            currentUser = await dbConnector.AuthenticateUser(userName, passWord);

            if (currentUser != null)
            {
                Console.WriteLine("Login successful");
                Console.WriteLine($"Logged in as: {currentUser.Username}");
                // Handle admin vs. user logic
            }
            else
            {
                warningMessage.Content = "Invalid username or password.";
            }
        }
        
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            userName = userNameInput.Text;
            passWord = passWordInput.Password;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                warningMessage.Content = "Please enter both a username and a password to create an account.";
                warningMessage.Foreground = new SolidColorBrush(Colors.Red); // Set the color to red for error
                return;
            }

            // Create user through SQLConnector
            var result = dbConnector.CreateUser(userName, passWord);

            if (result == "Username already exists. Please choose a different username.")
            {
                // Set the message and change color to red for an existing user error
                warningMessage.Content = result;
                warningMessage.Foreground = new SolidColorBrush(Colors.Red); // Red for errors
            }
            else
            {
                // Set the message and change color to green for successful account creation
                warningMessage.Content = result;
                warningMessage.Foreground = new SolidColorBrush(Colors.Green); // Green for success
            }

            // Optionally clear the input fields after creation
            userNameInput.Clear();
            passWordInput.Clear();
        }

    }
}