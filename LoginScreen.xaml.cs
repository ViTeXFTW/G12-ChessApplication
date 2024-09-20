using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_screen
{
    public partial class LoginScreen : Window
    {
        private string userName = "";
        private string passWord = "";

        public LoginScreen()
        {
            InitializeComponent();
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

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        // Checks if the user has entered a username and password, and if so, stores them in the userName and passWord variables.
        {
            userName = userNameInput.Text;
            passWord = passWordInput.Password;

            if (userName == "" || passWord == "") 
            {
                warningMessage.Content = "Please enter both a username and a password to proceed.";
            }
            // This is where we should send a request to a DB to check if the user exists and if the password is correct.


        }
    }
}